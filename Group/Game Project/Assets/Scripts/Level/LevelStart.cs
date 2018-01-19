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
			playersFromLobbyCount = State.GetInstance().GetPlayerCount();
			playersInSceneCount = GameObject.FindGameObjectsWithTag("Player").Length;
			Debug.Log("Players from lobby = " + playersFromLobbyCount + ", players in scene = " + playersInSceneCount);
			if (playersFromLobbyCount == playersInSceneCount){
				Debug.Log("Starting Game!");
				State.GetInstance().Level(State.LEVEL_READY).Publish();
				ready = true;
			}
		}
	}
}
