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
			ready = false;
		}

		void Update(){
			if (ready){
				return;
			}
			CheckIfPlayersReady();
		}

		[Server]
		private void CheckIfPlayersReady(){
			playersFromLobbyCount = State.GetInstance().GetPlayerCount();
			playersInSceneCount = GameObject.FindGameObjectsWithTag("Player").Length;
			Debug.Log("Players from lobby = " + playersFromLobbyCount + ", players in scene = " + playersInSceneCount);
			if (playersFromLobbyCount == playersInSceneCount){
				Debug.Log("Starting Game!");
				AssignTeams();
				State.GetInstance().Level(State.LEVEL_READY).Publish();
				ready = true;
			}
		}

		[Server]
		private void AssignTeams(){
			Debug.Log("Assigning Teams");
			int teamCount = 0;
			foreach (GameObject player in PlayerTracker.GetInstance().GetPlayers()){
				teamCount ++;
				Debug.Log("Team Assignment teamcount = " + teamCount);
				switch(teamCount){
					case 1:
						player.GetComponent<PlayerDataForClients>().SetTeam(PlayerDataForClients.PLAYER_A);
						Debug.Log("Assigning Player A");
						break;
					case 2:
						player.GetComponent<PlayerDataForClients>().SetTeam(PlayerDataForClients.PLAYER_B);
						Debug.Log("Assigning Player B");
						break;
					case 3:
						player.GetComponent<PlayerDataForClients>().SetTeam(PlayerDataForClients.PLAYER_C);
						Debug.Log("Assigning Player C");
						break;
					case 4:
						player.GetComponent<PlayerDataForClients>().SetTeam(PlayerDataForClients.PLAYER_D);
						Debug.Log("Assigning Player D");
						break;
					case 5:
						player.GetComponent<PlayerDataForClients>().SetTeam(PlayerDataForClients.PLAYER_E);
						Debug.Log("Assigning Player E");
						break;
				}
			}
		}
	}
}
