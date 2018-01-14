using GameState;
using Player.SyncedData;
using UnityEngine;
using UnityEngine.Networking;

namespace Player {
	public class PlayerPosition : NetworkBehaviour {

		private float weight;
		private float position;
		private float maxDistance;
		private float checkpointDistance;
		private float treeLeftDistance;
		private float treeRightDistance;
		private PlayerDataForClients playerData;
		private GameObject checkpoint;
		private GameObject spawn;
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
			GameObject spawnObj = GameObject.FindGameObjectWithTag("MaxDistanceSpawn");
			if (spawnObj != null){
				spawn = spawnObj;
				treeLeftDistance = Vector3.Distance(spawn.transform.position, tree_left.transform.position);
				treeRightDistance = Vector3.Distance(spawn.transform.position, tree_right.transform.position);
				if (treeLeftDistance < treeRightDistance){
					maxDistance = treeLeftDistance;
				} else {
					maxDistance = treeRightDistance;
				}
			}
		}

		void Update () {
			if (!isLocalPlayer || State.GetInstance().Level() != State.LEVEL_PLAYING){
				return;
			}

			treeLeftDistance = Vector3.Distance(transform.position, tree_left.transform.position);
			treeRightDistance = Vector3.Distance(transform.position, tree_right.transform.position);

			weight = 10000;
			weight += playerData.GetScore() * 10000;
			if (playerData.GetHasNutFlag()){
				weight += 5000;
				checkpointDistance = Vector3.Distance(transform.position, checkpoint.transform.position);
				weight -= checkpointDistance;
			} else {
				if (treeLeftDistance < treeRightDistance){
					weight -= treeLeftDistance;
				} else {
					weight -= treeRightDistance;
				}
			}
			if (treeLeftDistance < treeRightDistance){
				position = treeLeftDistance/maxDistance;
			} else {
				position = treeRightDistance/maxDistance;
			}

			playerData.CmdSetWeight(weight);
			playerData.CmdSetPosition(position);
		}
	}
}
