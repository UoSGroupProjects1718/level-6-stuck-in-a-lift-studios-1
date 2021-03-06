/**
 * This is based on the tutorial series found here: https://ditt.to/game-dev/multiplayer-1
 **/
using UnityEngine;

namespace Player.SyncedData {
	public class LocalPlayerDataStore {

		private static LocalPlayerDataStore instance;

		public string playerName = "";
		public int team = 0;
		public int score = 0;
		public bool isReady = false;
		public bool isServer = false;
		public bool hasNut = false;
		public bool canMove = true;

		private LocalPlayerDataStore(){ }

		public static LocalPlayerDataStore GetInstance(){
			if (instance == null){
				instance = new LocalPlayerDataStore();
			}
			return instance;
		}
	}
}
