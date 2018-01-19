using GameState;
using Player.SyncedData;
using Player.Tracking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Level {
	public class ScoreKeeper : NetworkBehaviour {

		public int maxNuts = 2;

		private bool keepScoring = false;

		public void Awake(){
			if (State.GetInstance().Network() == State.NETWORK_SERVER){
				SubscribeToServerPlaying();

				foreach (GameObject player in PlayerTracker.GetInstance().GetPlayers()){
					ResetPlayerScore(player);
				}
				PlayerTracker.GetInstance().OnPlayerAdded += ResetPlayerScore;
			}
		}

		public void Start(){
			foreach (GameObject player in PlayerTracker.GetInstance().GetPlayers()){
				player.GetComponent<PlayerDataForClients>().OnScoreUpdated += CheckScore;
			}
		}

		public void Update(){
			List<GameObject> playerList = PlayerTracker.GetInstance().GetPlayers();
			RankPlayers(playerList);
			//This is horrible. But it works. Horrible horrible hack. Sorry Chris.
			foreach (GameObject player in playerList){
				CheckScore(player, player.GetComponent<PlayerDataForClients>().GetScore());
			}

		}

		[Server]
		private void ResetPlayerScore(GameObject player){
			player.GetComponent<PlayerDataForClients>().ResetScore();
		}

		[Server]
		private void SubscribeToServerPlaying(){
			State.GetInstance().Subscribe(new StateOption().LevelState(State.LEVEL_PLAYING), () => { keepScoring = true; } );
			State.GetInstance().Subscribe(new StateOption().LevelState(State.LEVEL_COMPLETE), () => { keepScoring = false; } );
		}

		[Server]
		public void CheckScore(GameObject player, int score){
			if (!keepScoring){
				return;
			}
			UpdateScoreUI(player, score);
			if (score >= maxNuts){
				keepScoring = false;
				StartCoroutine(WaitForClientSync());
			}
		}

		private int SortByWeight(GameObject p1, GameObject p2){
			var weightA = p1.GetComponent<PlayerDataForClients>().GetWeight();
			var weightB = p2.GetComponent<PlayerDataForClients>().GetWeight();

			if (weightA < weightB){
				return 1;
			}
			if (weightA > weightB){
				return -1;
			}
			return 0;
		}

		[Server]
		public void RankPlayers(List<GameObject> playerList){
			playerList.Sort(SortByWeight);
			for (int i=0; i<playerList.Count; i++){
				playerList[i].GetComponent<PlayerDataForClients>().SetRank(i);
				int rank = i + 1;
			}
		}

		private IEnumerator WaitForClientSync(){
			//This adds a delay to allow the clients to sync scores before displaying end screen
			yield return new WaitForSeconds(0.1f);
			RpcEndTheGame();
		}

		[Client]
		private void UpdateScoreUI(GameObject player, int score){
			//TODO Change score UI
		}

		[ClientRpc]
		private void RpcEndTheGame(){
			State.GetInstance().Level(State.LEVEL_COMPLETE).Publish();
		}
	}
}
