using GameState;
using Player.SyncedData;
using System.Collections;
using System.Collections.Generic;
using UI.Level;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Level {
	public class EagleController : NetworkBehaviour {

		public float moveSpeed;
		public float attackSpeed;
		public float attackIntervalMinSec = 30.0f;
		public float attackIntervalMaxSec = 60.0f;
		public float liftSpeed = 1.0f;
		public float maxLiftHeight = 50.0f;
		public float playerStunDuration = 3.0f;
		public GameObject[] waypoints;
		public AudioSource screechAudioSource;

		private bool isAttacking;
		private int playerPoints;
		private int waypointIndex;
		private GameObject[] playersInScene;
		private GameObject targetPlayer;
		private GameObject targetWaypoint;
		private List<GameObject> playerList;

		void Start(){
			ServerStartThings();
		}

		void Update(){
			if (State.GetInstance().Level() != State.LEVEL_PLAYING){
				return;
			}
			if (!isAttacking){
				CalculateTargetablePlayers();
				MoveEagle();
			} else {
				RpcAttackPlayer(targetPlayer);
			}
		}

		[Server]
		private void ServerStartThings(){
			isAttacking = false;
			playerPoints = 0;
			waypointIndex = 0;
			targetWaypoint = waypoints[waypointIndex];
			StartCoroutine(this.AttackTimer(Random.Range(attackIntervalMinSec, attackIntervalMaxSec)));
		}

		[Server]
		private void CalculateTargetablePlayers(){
			playersInScene = GameObject.FindGameObjectsWithTag("Player");
			playerList = new List<GameObject>();
			foreach (GameObject player in playersInScene){
				PlayerDataForClients playerData = player.GetComponent<PlayerDataForClients>();
				var rank = playerData.GetRank();
				playerPoints = playerData.GetEagleTarget();
				if (IsPlayerVisible(player)){
					if (rank == playersInScene.Length){
						//Never target the player in last place
						playerPoints = 0;
					} else {
						//Higher ranked players gain points faster
						playerPoints += (5 - rank) * 10;
					}
				} else if (playerPoints > 0){
					//Higher ranked players lose points slower
					playerPoints -= (rank + 1) * 5;
				}
				playerData.SetEagleTarget(playerPoints);
				playerList.Add(player);
			}
		}

		[Server]
		private int SortByPoints(GameObject p1, GameObject p2){
			var pointsA = p1.GetComponent<PlayerDataForClients>().GetEagleTarget();
			var pointsB = p2.GetComponent<PlayerDataForClients>().GetEagleTarget();

			if (pointsA < pointsB){
				return -1;
			}
			if (pointsA > pointsB){
				return 1;
			}
			return 0;
		}

		[Server]
		private bool IsPlayerVisible(GameObject player){
			RaycastHit checkLineOfSight;
			Ray ray = new Ray(transform.position, player.transform.position - transform.position);
			Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.green);
			if (Physics.Raycast(ray, out checkLineOfSight, 10000f)){
				if (checkLineOfSight.collider.gameObject.CompareTag("Player")){
					return true;
				}
			}
			return false;
		}

		[Server]
		private GameObject FindTargetPlayer(){
			playerList.Sort(SortByPoints);
			//Target visible player with the highest score, ignoring last place
			if (playerList.Count > 1){
				for (int i=0; i < playerList.Count-1; i++){
					Debug.Log("Searching for valid target");
					if (i == playersInScene.Length){
						Debug.Log("Can't target player in last place");
						return null;
					}
					if (IsPlayerVisible(playerList[i])){
						screechAudioSource.Play();
						return playerList[i];
					}
					Debug.Log("Player not visible. Keep looking");
				}
			} else {
				if (IsPlayerVisible(playerList[0])){
					if (!screechAudioSource.isPlaying){
						screechAudioSource.Play();
					}
					return playerList[0];
				}
			}
			return null;
		}

		[Server]
		private IEnumerator AttackTimer(float attackInterval){
			Debug.Log("Starting AttackTimer");
			while (attackInterval > 0) {
				yield return new WaitForSeconds(1);
				if (State.GetInstance().Level() == State.LEVEL_PLAYING){
					attackInterval --;
				}
			}
			GameObject player = FindTargetPlayer();
			if (player != null){
				Debug.Log("Target Player Found! Attacking!");
				targetPlayer = player;
				isAttacking = true;
				foreach (GameObject p in playersInScene){
					p.GetComponent<Hint>().ShowHintEagle(true);
				}
			} else {
				Debug.Log("No Target Found :(");
				StartCoroutine(this.AttackTimer(Random.Range(attackIntervalMinSec, attackIntervalMaxSec)));
			}
		}

		[Server]
		private IEnumerator PlayerStun(float stunDuration, GameObject player){
			while (stunDuration > 0){
				yield return new WaitForSeconds(1);
				stunDuration --;
			}
			RpcUpdatePlayerMoveState(player, true);
			player.GetComponent<PlayerDataForClients>().SetCanMoveFlag(true);
		}

		private void MoveEagle(){
			if (Vector3.Distance(transform.position, targetWaypoint.transform.position) <= 1f){
				if (waypointIndex < waypoints.Length){
					targetWaypoint = waypoints[waypointIndex ++];
				} else {
					waypointIndex = 0;
					targetWaypoint = waypoints[waypointIndex];
				}
			} else {
				transform.LookAt(targetWaypoint.transform);
				float step = moveSpeed * Time.deltaTime;
				transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.transform.position, step);
			}
		}

		[ClientRpc]
		private void RpcAttackPlayer(GameObject player){
			var finishedAttack = false;
			if (Vector3.Distance(transform.position, player.transform.position) <= 1.5f){
				RpcUpdatePlayerMoveState(player, false);
				player.GetComponent<PlayerDataForClients>().SetCanMoveFlag(false);
					if (player.transform.position.y >= maxLiftHeight){
						StartCoroutine(this.PlayerStun(playerStunDuration, player));
						finishedAttack = true;
					} else {
						var newEaglePos = new Vector3(transform.position.x, transform.position.y + liftSpeed, transform.position.z);
						var newPlayerPos = new Vector3(player.transform.position.x, player.transform.position.y + liftSpeed, player.transform.position.z);
						transform.position = newEaglePos;
						player.transform.position = newPlayerPos;
					}
			} else {
				transform.LookAt(player.transform);
				float step = attackSpeed * Time.deltaTime;
				transform.position = Vector3.MoveTowards(transform.position, player.transform.position, step);
			}
			if (finishedAttack){
				finishedAttack = false;
				targetPlayer = null;
				//reset all player's eagle target scores
				playersInScene = GameObject.FindGameObjectsWithTag("Player");
				ResetPlayers();
				StartCoroutine(this.AttackTimer(Random.Range(attackIntervalMinSec, attackIntervalMaxSec)));
				isAttacking = false;
			}
		}

//		[Server]
		private void EagleBonk(Collision col){
			//if eagle has collided with anything that isn't the player
			if (col.gameObject.tag != "Player"){
				//break off attack and start looking to attack again
				isAttacking = false;
				ResetPlayers();
				targetPlayer.GetComponent<PlayerDataForClients>().SetCanMoveFlag(true);
				RpcUpdatePlayerMoveState(targetPlayer, true);
				StartCoroutine(this.AttackTimer(Random.Range(attackIntervalMinSec, attackIntervalMaxSec)));
			}
		}

		[ClientRpc]
		private void RpcWarnPlayers(GameObject player){
			player.GetComponent<Hint>().ShowHintEagle(false);
		}

		[ClientRpc]
		private void RpcUpdatePlayerMoveState(GameObject player, bool canMove){
			player.GetComponent<PlayerDataForClients>().SetCanMoveFlag(canMove);
		}

		private void ResetPlayers(){
			playersInScene = GameObject.FindGameObjectsWithTag("Player");
			foreach (GameObject p in playersInScene){
				PlayerDataForClients playerData = p.GetComponent<PlayerDataForClients>();
				playerData.SetEagleTarget(0);
				p.GetComponent<Hint>().ShowHintEagle(false);
				RpcWarnPlayers(p);
			}
		}

		void OnCollisionEnter(Collision col){
			EagleBonk(col);
		}
	}
}
