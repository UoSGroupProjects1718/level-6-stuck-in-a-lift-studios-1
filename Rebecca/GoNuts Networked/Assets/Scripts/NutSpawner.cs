using UnityEngine;
using UnityEngine.Networking;

public class NutSpawner : NetworkBehaviour {

	public GameObject nutPrefab;

	public override void OnStartServer () {
		GameObject[] spawnLocations = GameObject.FindGameObjectsWithTag("NutSpawn");
		foreach (GameObject spawnPoint in spawnLocations) {
			GameObject nut = (GameObject)Instantiate(nutPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
			NetworkServer.Spawn(nut);
		}
	}

}
