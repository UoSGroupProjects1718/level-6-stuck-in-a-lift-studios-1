using UnityEngine;
using UnityEngine.Networking;

public class NutSpawner : NetworkBehaviour {

	public GameObject nutPrefab;
	public Transform[] spawnLocations;

	public override void OnStartServer () {
		foreach (Transform spawnPoint in spawnLocations) {
			GameObject nut = (GameObject)Instantiate(nutPrefab, spawnPoint.position, spawnPoint.rotation);
			NetworkServer.Spawn(nut);
		}
	}

}
