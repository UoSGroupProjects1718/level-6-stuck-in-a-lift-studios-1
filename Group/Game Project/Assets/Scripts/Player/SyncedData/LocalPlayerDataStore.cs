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

		private LocalPlayerDataStore(){ }

		public static LocalPlayerDataStore GetInstance(){
			if (instance == null){
				instance = new LocalPlayerDataStore();
			}
			return instance;
		}
	}
}
