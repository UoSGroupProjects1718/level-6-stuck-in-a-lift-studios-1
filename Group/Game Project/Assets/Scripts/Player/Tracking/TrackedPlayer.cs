/**
 * This is based on the tutorial series found here: https://ditt.to/game-dev/multiplayer-1
 **/
using Player.SyncedData;
using UnityEngine;
using UnityEngine.Networking;

namespace Player.Tracking{
    class TrackedPlayer : NetworkBehaviour {

		public override void OnStartClient (){
			PlayerTracker.GetInstance().AddPlayer(gameObject);
			gameObject.GetComponent<PlayerDataForClients>().OnIsServerFlagUpdated += UpdatePlayerIsServer;
		}
		private void UpdatePlayerIsServer (GameObject player, bool isServer){
			PlayerTracker.GetInstance().SetServerPlayer(gameObject);
		}

		public override void OnStartLocalPlayer (){
			PlayerTracker.GetInstance().SetLocalPlayer(gameObject);
		}

        public void OnDestroy() {
            Debug.Log("Player tracker being destroyed!!!");
            PlayerTracker.GetInstance().RemovePlayer(gameObject);
		}
	}
}
