using GameState;
using Player.SyncedData;
using System.Collections;
using System.Collections.Generic;
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

		private bool isAttacking;
		private int playerPoints;
		private int waypointIndex;
		private GameObject[] playersInScene;
		private GameObject targetPlayer;
		private GameObject targetWaypoint;
		private List<GameObject> playerList;

		void Start(){
			isAttacking = false;
			playerPoints = 0;
			waypointIndex = 0;
			targetWaypoint = waypoints[waypointIndex];
			StartCoroutine(this.AttackTimer(Random.Range(attackIntervalMinSec, attackIntervalMaxSec)));
		}

		void Update(){
			if (State.GetInstance().Level() != State.LEVEL_PLAYING){
				Debug.Log("game not ready");
				return;
			}
			if (!isAttacking){
				Debug.Log("Not Attacking");
				CalculateTargetablePlayers();
				MoveEagle();
			} else {
				AttackPlayer(targetPlayer);
			}
		}

		[Server]
		private void CalculateTargetablePlayers(){
			playersInScene = GameObject.FindGameObjectsWithTag("Player");
			playerList = new List<GameObject>();
			foreach (GameObject player in playersInScene){
				PlayerDataForClients playerData = player.GetComponent<PlayerDataForClients>();
				var rank = playerData.GetRank();
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

		private int SortByPoints(GameObject p1, GameObject p2){
			var pointsA = p1.GetComponent<PlayerDataForClients>().GetEagleTarget();
			var pointsB = p2.GetComponent<PlayerDataForClients>().GetEagleTarget();

			if (pointsA < pointsB){
				return 1;
			}
			if (pointsA > pointsB){
				return -1;
			}
			return 0;
		}

		[Server]
		private bool IsPlayerVisible(GameObject player){
			RaycastHit checkLineOfSight;
			Ray ray = new Ray(transform.position, player.transform.position - transform.position);
			Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.green);
			if (Physics.Raycast(ray, out checkLineOfSight, 1000f)){
				if (checkLineOfSight.collider.gameObject.CompareTag("Player")){
					Debug.Log("player is visible to the eagle");
					return true;
				}
			}
			return false;
		}

		[Server]
		private GameObject FindTargetPlayer(){
			playerList.Sort(SortByPoints);
			//Target visible player with the highest score, ignoring last place
			for (int i=0; i<playerList.Count; i++){
				Debug.Log("Searching for valid target");
				if (IsPlayerVisible(playerList[i])){
					return playerList[i];
				}
				Debug.Log("Player not visible. Keep looking");
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
					Debug.Log("Time to attack: " + attackInterval);
				}
			}
			GameObject player = FindTargetPlayer();
			if (player != null){
				Debug.Log("Target Player Found! Attacking!");
				targetPlayer = player;
				isAttacking = true;
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
			player.GetComponent<PlayerDataForClients>().SetCanMoveFlag(true);
		}

		[Server]
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

		[Server]
		private void AttackPlayer(GameObject player){
			var finishedAttack = false;
			Debug.Log("Attacking Player!");
			if (Vector3.Distance(transform.position, player.transform.position) <= 2.0f){
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
				StartCoroutine(this.AttackTimer(Random.Range(attackIntervalMinSec, attackIntervalMaxSec)));
				isAttacking = false;
			}
		}

		void OnCollisionEnter(Collision col){
			//if eagle has collided with anything that isn't the player
			if (col.gameObject.tag != "Player"){
				//break off attack
				isAttacking = false;
			}
		}
	}
}
