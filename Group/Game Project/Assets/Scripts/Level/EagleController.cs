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
		public float maxLiftHeight = 100.0f;

		private GameObject[] playersInScene;
		private GameObject targetPlayer;
		private List<GameObject> playerList;
		private bool isAttacking;
		private int playerPoints;

		void Start(){
			playerPoints = 0;
			isAttacking = false;
			StartCoroutine(this.AttackTimer(Random.Range(attackIntervalMinSec, attackIntervalMaxSec)));
		}

		void Update(){
			if (!isAttacking){
				CalculateTargetablePlayers();
				MoveEagle();
			} else {
				AttackPlayer(targetPlayer);
			}
		}

		[Server]
		private void CalculateTargetablePlayers(){
			playersInScene = GameObject.FindGameObjectsWithTag("Player");
			playerList = null;
			foreach (GameObject player in playersInScene){
				PlayerDataForClients playerData = player.GetComponent<PlayerDataForClients>();

				if (IsPlayerVisible(player)){
					if (playerData.GetRank() == playersInScene.Length){
						//Never target the player in last place
						playerPoints = 0;
					} else {
						//Higher ranked players gain points faster
						playerPoints += 50 - playerData.GetRank();
					}
				} else if (playerPoints > 0){
					//Higher ranked players lose points slower
					playerPoints -= playerData.GetRank();
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
			return false;
		}

		[Server]
		private GameObject FindTargetPlayer(){
			playerList.Sort(SortByPoints);
			//Target visible player with the highest score, ignoring last place
			for (int i=0; i<playerList.Count-1; i++){
				if (IsPlayerVisible(playerList[i])){
					return playerList[i];
				}
			}
			return null;
		}

		[Server]
		private IEnumerator AttackTimer(float attackInterval){
			while (attackInterval > 0) {
				yield return new WaitForSeconds(1);
				if (State.GetInstance().Game() == State.LEVEL_PLAYING){
					attackInterval --;
				}
			}
			GameObject player = FindTargetPlayer();
			if (player != null){
				targetPlayer = player;
				isAttacking = true;
			}
		}

		[Server]
		private void MoveEagle(){
			//Standard movement for the eagle
		}

		[Server]
		private void AttackPlayer(GameObject player){
			var finishedAttack = false;
			Debug.Log("Attacking Player!");
			//If eagle has reached player
				//if eagle has not collided with anything
					//disable player's input
					//lift both eagle and player up to maxheight
					//if maxheight reached
						//drop the player
						//start a timer that will allow player to move again once expired
						//set finishedAttack = true;
				//else set finishedAttack = true;
			// else
				//Move eagle towards player at attack speed
			if (finishedAttack){
				finishedAttack = false;
				targetPlayer = null;
				StartCoroutine(this.AttackTimer(Random.Range(attackIntervalMinSec, attackIntervalMaxSec)));
				isAttacking = false;
			}
		}
	}
}
