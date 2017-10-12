using GameState;
using Player.SyncedData;
using UnityEngine;
using UnityEngine.Networking;

public class Checkpoint : NetworkBehaviour {

	public int maxNuts = 3;

	void OnCollisionEnter(Collision col){
		if (col.gameObject.tag == "player"){
			PlayerDataForClients playerData = col.gameObject.GetComponent<PlayerDataForClients>();
			if (playerData.GetHasNutFlag()){
				playerData.SetHasNutFlag(false);
				playerData.ServerIncrementScore();
				if (playerData.GetScore() >= maxNuts){
					RpcEndTheGame();
				}
			}
		}
	}

	[ClientRpc]
		private void RpcEndTheGame(){
			State.GetInstance().Level(State.LEVEL_COMPLETE).Publish();
		}
}
