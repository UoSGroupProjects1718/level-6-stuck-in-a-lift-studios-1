using GameState;
using Player.SyncedData;
using Player.Tracking;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Level {
	class LevelStart : NetworkBehaviour {

		public int delayTimer = 3;

		private bool ready;
		private int playersFromLobbyCount;
		private int playersInSceneCount;

		void Start () {
			ready = false;
			StartCoroutine(this.DelayStart());
		}

		void Update(){
			if (!ready){
				return;
			}
			CheckIfPlayersReady();
		}

		[Server]
		private IEnumerator DelayStart(){
			while (delayTimer > 0){
				yield return new WaitForSeconds(1);
				delayTimer --;
			}
			ready = true;
		}

		[Server]
		private void CheckIfPlayersReady(){
			if (State.GetInstance().Level() == State.LEVEL_PLAYING || State.GetInstance().Level() == State.LEVEL_READY){
				return;
			}
			playersFromLobbyCount = State.GetInstance().GetPlayerCount();
			playersInSceneCount = GameObject.FindGameObjectsWithTag("Player").Length;
			if (playersFromLobbyCount == playersInSceneCount){
				AssignTeams();
				StartCoroutine(WaitForClientSync());
			}
		}

		private IEnumerator WaitForClientSync(){
			//This adds a delay to allow the clients to sync scores before starting the game
			yield return new WaitForSeconds(0.5f);
			RpcStartTheGame();
		}

		[ClientRpc]
		private void RpcStartTheGame(){
			State.GetInstance().Level(State.LEVEL_READY).Publish();
		}

		[Server]
		private void AssignTeams(){
			int teamCount = 0;
			GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
			foreach (GameObject player in players){
				teamCount ++;
				PlayerDataForClients playerData = player.GetComponent<PlayerDataForClients>();
				switch(teamCount){
					case 1:
						playerData.SetTeam(PlayerDataForClients.PLAYER_A);
						playerData.SetCanMoveFlag(true);
						break;
					case 2:
						playerData.SetTeam(PlayerDataForClients.PLAYER_B);
						playerData.SetCanMoveFlag(true);
						break;
					case 3:
						playerData.SetTeam(PlayerDataForClients.PLAYER_C);
						playerData.SetCanMoveFlag(true);
						break;
					case 4:
						playerData.SetTeam(PlayerDataForClients.PLAYER_D);
						playerData.SetCanMoveFlag(true);
						break;
					case 5:
						playerData.SetTeam(PlayerDataForClients.PLAYER_E);
						playerData.SetCanMoveFlag(true);
						break;
				}
			}
		}
	}
}
