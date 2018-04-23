using GameState;
using Player.Tracking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Nut {
	public class NutSpawner : NetworkBehaviour {

		public GameObject nutPrefab;
		public GameObject treeBeam;
		public GameObject beamBase;

		private bool hasSpawned = false;
		private List<GameObject> nutList;
		private int initialNutCount = 0;
		private int inactiveNutCount = 0;
		private Material beamMat;
		private Color maxColor;
		private float percent = 1;
		private float newGreen = 0.3f;
		private Vector3 finalBeamScale;

		public void Start(){
			RpcNutStart();
		}

		public void Update () {
			RpcNutUpdate();
		}

		[ClientRpc]
		private void RpcNutStart(){
			nutList = new List<GameObject>();
			beamMat = treeBeam.GetComponent<Renderer>().material;
			beamMat.EnableKeyword("_EMISSION");
			maxColor = beamMat.GetColor("_EmissionColor");
			finalBeamScale = new Vector3(0, 0, 0);
		}

		[ClientRpc]
		private void RpcNutUpdate(){
			if (hasSpawned){
				if (inactiveNutCount < initialNutCount){
					foreach(GameObject nut in nutList){
						if (nut == null){
							inactiveNutCount ++;
						}
					}
				}
				if (inactiveNutCount == initialNutCount){
					percent = 0f;
					newGreen = 0f;
					beamBase.transform.localScale = Vector3.Lerp(beamBase.transform.localScale, finalBeamScale, 0.5f * Time.deltaTime);
				} else {
					percent = (1f - ((float)inactiveNutCount/(float)initialNutCount));
					newGreen = percent * maxColor.g;
				}

				float green = Mathf.Lerp(maxColor.g, newGreen, Time.deltaTime);

				Color fadedColor = new Color(maxColor.r, green, maxColor.b, maxColor.a);
				beamMat.SetColor("_EmissionColor", fadedColor);
			}
			if (hasSpawned || State.GetInstance().Level() != State.LEVEL_PLAYING){
				return;
			}

			List<Transform> spawnLocations = new List<Transform>();

			foreach (Transform child in transform){
				foreach (Transform grandchild in child){
					spawnLocations.Add(grandchild);
				}
			}

			for (int i = 0; i < PlayerTracker.GetInstance().GetPlayerCount(); i++){
				GameObject nut = (GameObject)Instantiate(nutPrefab, spawnLocations[i].position, spawnLocations[i].rotation);
				NetworkServer.Spawn(nut);
				nutList.Add(nut);
			}
			initialNutCount = nutList.Count;
			hasSpawned = true;
		}
	}
}
