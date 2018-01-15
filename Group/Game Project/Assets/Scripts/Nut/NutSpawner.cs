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
			if (hasSpawned){
				return;
			}
			hasSpawned = true;

			List<Transform> spawnLocations = new List<Transform>();

			foreach (Transform child in transform){
				foreach (Transform grandchild in child){
					Debug.Log("Found A Nut!");
					spawnLocations.Add(grandchild);
				}
			}

			for (int i = 0; i <= PlayerTracker.GetInstance().GetPlayers().Count; i++){
				GameObject nut = (GameObject)Instantiate(nutPrefab, spawnLocations[i].position, spawnLocations[i].rotation);
				NetworkServer.Spawn(nut);
			}
		}
	}
}
