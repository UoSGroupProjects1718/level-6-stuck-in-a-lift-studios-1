using Player.Tracking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Nut {
	public class NutSpawner : NetworkBehaviour {

		public GameObject nutPrefab;

		private bool hasSpawned = false;

		public override void OnStartServer () {
			if (!hasSpawned){
				hasSpawned = true;
				
				List<Transform> spawnLocations = new List<Transform>();
				
				foreach (Transform nutSpawn in transform){
					spawnLocations.Add(nutSpawn);
				}

				for (int i = 0; i < PlayerTracker.GetPlayers().Count; i++){
					GameObject nut = (GameObject)Instantiate(nutPrefab, spawnLocations[i].position, spawnLocations[i].rotation);
					NetworkServer.Spawn(nut);
				}
			}
		}
	}
}
