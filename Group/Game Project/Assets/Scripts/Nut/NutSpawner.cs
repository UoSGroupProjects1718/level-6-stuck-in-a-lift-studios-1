using Player.Tracking;
using UnityEngine;
using UnityEngine.Networking;

namespace Nut {
	public class NutSpawner : NetworkBehaviour {

		public GameObject nutPrefab;
		public string nutSpawnTag = "NutSpawn";

		private bool hasSpawned = false;

		public override void OnStartServer () {
			if (!hasSpawned){
				hasSpawned = true;
				GameObject[] spawnLocations = GameObject.FindGameObjectsWithTag(nutSpawnTag);
				for (int i = 0; i < PlayerTracker.GetPlayers().Count; i++){
					GameObject nut = (GameObject)Instantiate(nutPrefab, spawnLocations[i].transform.position, spawnLocations[i].transform.rotation);
					NetworkServer.Spawn(nut);
				}
			}
		}
	}
}
