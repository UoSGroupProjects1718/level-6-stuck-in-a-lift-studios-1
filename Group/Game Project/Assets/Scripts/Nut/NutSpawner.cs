using UnityEngine;
using UnityEngine.Networking;

namespace Nut {
	public class NutSpawner : NetworkBehaviour {

		public GameObject nutPrefab;

		private bool hasSpawned = false;

		public override void OnStartServer () {
			if (!hasSpawned){
				hasSpawned = true;
				GameObject[] spawnLocations = GameObject.FindGameObjectsWithTag("NutSpawn");
				foreach (GameObject spawnPoint in spawnLocations) {
					GameObject nut = (GameObject)Instantiate(nutPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
					NetworkServer.Spawn(nut);
				}
			}
		}

	}
}
