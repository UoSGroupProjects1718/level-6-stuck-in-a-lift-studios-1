using GameState;
using Player.SyncedData;
using UnityEngine;
using UnityEngine.Networking;

namespace Player {
	public class PlayerPosition : NetworkBehaviour {

		private float weight;
		private float checkpointDistance;
		private float treeLeftDistance;
		private float treeRightDistance;
		private PlayerDataForClients playerData;
		private GameObject checkpoint;
		private GameObject tree_left;
		private GameObject tree_right;

		void Start () {
			if (!isLocalPlayer){
				return;
			}
			playerData = transform.gameObject.GetComponent<PlayerDataForClients>();

			weight = 0;

			GameObject checkpointObj = GameObject.FindGameObjectWithTag("Checkpoint");
			if (checkpointObj != null){
				checkpoint = checkpointObj;
			}
			GameObject treeLeftObj = GameObject.FindGameObjectWithTag("TreeLeft");
			if (treeLeftObj != null){
				tree_left = treeLeftObj;
			}
			GameObject treeRightObj = GameObject.FindGameObjectWithTag("TreeRight");
			if (treeRightObj != null){
				tree_right = treeRightObj;
			}
		}

		void Update () {
			if (!isLocalPlayer || State.GetInstance().Level() != State.LEVEL_PLAYING){
				return;
			}

			weight = 10000;
			weight += playerData.GetScore() * 10000;
			if (playerData.GetHasNutFlag()){
				weight += 5000;
				checkpointDistance = Vector3.Distance(transform.position, checkpoint.transform.position);
				weight -= checkpointDistance;
			} else {
				treeLeftDistance = Vector3.Distance(transform.position, tree_left.transform.position);
				treeRightDistance = Vector3.Distance(transform.position, tree_right.transform.position);
				if (treeLeftDistance < treeRightDistance){
					weight -= treeLeftDistance;
				} else {
					weight -= treeRightDistance;
				}
			}
			playerData.CmdSetWeight(weight);
		}
	}
}
