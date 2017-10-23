using GameState;
using Player.SyncedData;
using Player.Tracking;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Level {
	public class ScoreKeeper : NetworkBehaviour {

		public int maxNuts = 2;
		public Text scoreText;

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

		[Server]
		private void ResetPlayerScore(GameObject player){
			player.GetComponent<PlayerDataForClients>().ResetScore();
		}

		[Server]
		private void SubscribeToServerPlaying(){
			State.GetInstance().Subscribe(new StateOption().LevelState(State.LEVEL_PLAYING),() => { keepScoring = true; } );
			State.GetInstance().Subscribe(new StateOption().LevelState(State.LEVEL_COMPLETE), () => { keepScoring = false; } );
		}

		[Server]
		public void CheckScore(GameObject player, int score){
			if (!keepScoring){
//				return;
			}
			Debug.LogError("Someone Scored! Checking for win");
			UpdateScoreUI(player, score);
			if (score >= maxNuts){
				RpcEndTheGame();
			}
		}

		[Client]
		private void UpdateScoreUI(GameObject player, int score){
			scoreText.text = player.GetComponent<PlayerDataForClients>().GetName()+" has scored "+score;
		}

		[ClientRpc]
		private void RpcEndTheGame(){
			State.GetInstance().Level(State.LEVEL_COMPLETE).Publish();
		}
	}
}
