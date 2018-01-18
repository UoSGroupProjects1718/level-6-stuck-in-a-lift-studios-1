using Player.SyncedData;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Eagle {
	public class EagleController : NetworkBehaviour {
		
		public float attackSpeed;
		
		private GameObject[] playersInScene;
		
		private int points_playerA;
		private int points_playerB;
		private int points_playerC;
		private int points_playerD;
		private int points_playerE;
		
		void Start(){
			
		}
		
		void Update(){
			
		}
	}
}
