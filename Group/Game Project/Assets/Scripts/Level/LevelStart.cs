using Player.SyncedData;
using Player.Tracking;
using GameState;
using UnityEngine;
using UnityEngine.Networking;

namespace Level {
	class LevelStart : NetworkBehaviour {

		private bool ready;
		private int playersFromLobbyCount;
		private int playersInSceneCount;

		void Start () {
		}

		void Update(){
			CheckIfPlayersReady();
		}

		[Server]
		private void CheckIfPlayersReady(){
			if (State.GetInstance().Level() == State.LEVEL_PLAYING){
				return;
			}
			playersFromLobbyCount = State.GetInstance().GetPlayerCount();
			playersInSceneCount = GameObject.FindGameObjectsWithTag("Player").Length;
			Debug.Log("Players from lobby = " + playersFromLobbyCount + ", players in scene = " + playersInSceneCount);
			if (playersFromLobbyCount == playersInSceneCount){
				Debug.Log("Starting Game!");
				AssignTeams();
				State.GetInstance().Level(State.LEVEL_READY).Publish();
			} else {
				Debug.Log("Waiting on player(s)");
			}
		}

		[Server]
		private void AssignTeams(){
			Debug.Log("Assigning Teams");
			int teamCount = 0;
			GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
			foreach (GameObject player in players){
				teamCount ++;
				Debug.Log("Team Assignment teamcount = " + teamCount);
				PlayerDataForClients playerData = player.GetComponent<PlayerDataForClients>();
				switch(teamCount){
					case 1:
						playerData.SetTeam(PlayerDataForClients.PLAYER_A);
						playerData.SetCanMoveFlag(true);
						Debug.Log("Assigning Player A");
						break;
					case 2:
						playerData.SetTeam(PlayerDataForClients.PLAYER_B);
						playerData.SetCanMoveFlag(true);
						Debug.Log("Assigning Player B");
						break;
					case 3:
						playerData.SetTeam(PlayerDataForClients.PLAYER_C);
						playerData.SetCanMoveFlag(true);
						Debug.Log("Assigning Player C");
						break;
					case 4:
						playerData.SetTeam(PlayerDataForClients.PLAYER_D);
						playerData.SetCanMoveFlag(true);
						Debug.Log("Assigning Player D");
						break;
					case 5:
						playerData.SetTeam(PlayerDataForClients.PLAYER_E);
						playerData.SetCanMoveFlag(true);
						Debug.Log("Assigning Player E");
						break;
				}
			}
		}
	}
}
