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

		public void OnDestroy (){
			PlayerTracker.GetInstance().RemovePlayer(gameObject);
		}
	}
}
