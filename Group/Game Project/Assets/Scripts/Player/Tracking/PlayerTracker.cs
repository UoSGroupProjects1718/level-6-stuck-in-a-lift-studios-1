using System.Collections.Generic;
using UnityEngine;

namespace Player.Tracking {
	public class PlayerTracker {

		private static PlayerTracker instance;

		public delegate void LocalPlayerUpdated(GameObject localPlayer);
		public event LocalPlayerUpdated OnLocalPlayerUpdated;

		public delegate void ServerPlayerUpdated(GameObject serverPlayer);
		public event ServerPlayerUpdated OnServerPlayerUpdated;

		public delegate void PlayerAdded(GameObject newPlayer);
		public event PlayerAdded OnPlayerAdded;

		public delegate void PlayerRemoved(GameObject oldPlayer);
		public event PlayerRemoved OnPlayerRemoved;

		private List<GameObject> players = new List<GameObject>();
		private GameObject localPlayer;
		private GameObject serverPlayer;

		private PlayerTracker(){}

		public static PlayerTracker GetInstance(){
			if (instance == null){
				instance = new PlayerTracker();
			}
			return instance;
		}

		public void AddPlayer(GameObject obj){
			players.Add(obj);
			if (this.OnPlayerAdded != null){
				this.OnPlayerAdded(obj);
			}
		}

		public void RemovePlayer(GameObject obj){
            Debug.Log("Player removed!");
			players.Remove(obj);
			if (this.OnPlayerRemoved != null){
				this.OnPlayerRemoved(obj);
			}
			if (localPlayer == obj){
				SetLocalPlayer(obj);
			}
			if (serverPlayer == obj){
				SetServerPlayer(obj);
			}
		}

		public List<GameObject> GetPlayers(){
			return players;
		}

		public int GetPlayerCount(){
			return players.Count;
		}

		public void SetLocalPlayer(GameObject obj){
			localPlayer = obj;
			if (this.OnLocalPlayerUpdated != null){
				this.OnLocalPlayerUpdated(obj);
			}
		}
		public GameObject GetLocalPlayer(){
			return localPlayer;
		}

		public void SetServerPlayer(GameObject obj){
			serverPlayer = obj;
			if (this.OnServerPlayerUpdated != null){
				this.OnServerPlayerUpdated(obj);
			}
		}
		public GameObject GetServerPlayer(){
			return serverPlayer;
		}
	}
}
