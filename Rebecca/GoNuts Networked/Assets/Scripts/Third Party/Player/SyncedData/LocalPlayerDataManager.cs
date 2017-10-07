﻿using GameState;
using UnityEngine;
using UnityEngine.Networking;

namespace Player.SyncedData {
	public class LocalPlayerDataManager : NetworkBehaviour {

		public PlayerDataForClients clientData;

		private string[] names = new string[] {"Twilight Sparkle", "Pinkie Pie", "Rarity", "Fluttershy", "Rainbow Dash", "Applejack"};
		private int[] teams = new int[] {PlayerDataForClients.PLAYER_ALPHA, PlayerDataForClients.PLAYER_BETA};

		public override void OnStartLocalPlayer(){
			LocalPlayerDataStore store = LocalPlayerDataStore.GetInstance();

			State.GetInstance().Subscribe(
				new StateOption().GameState(State.GAME_OFFLINE),
				() => {
					if (clientData != null){
						clientData.SetName("");
						clientData.SetTeam(0);
						clientData.SetIsReadyFlag(false);
						clientData.SetIsServerFlag(false);
					} else {
						store.playerName = "";
						store.team = 0;
						store.isReady = false;
						store.isServer = false;
					}
				}
			);

			State.GetInstance().Subscribe(
				new StateOption().LevelState(State.LEVEL_IN_LOBBY),
				() => {
					if (clientData != null){
						clientData.SetIsReadyFlag(false);
						clientData.SetIsServerFlag(State.GetInstance().Network() == State.NETWORK_SERVER);
					} else {
						store.isReady = false;
						store.isServer = State.GetInstance().Network() == State.NETWORK_SERVER;
					}
				}
			);

			CreateDefaultValues();

			clientData.SetName(store.playerName);
			clientData.SetTeam(store.team);
			clientData.SetIsReadyFlag(store.isReady);
			clientData.SetIsServerFlag(store.isServer);

			clientData.OnNameUpdated += OnNameUpdated;
			clientData.OnTeamUpdated += OnTeamUpdated;
			clientData.OnIsReadyFlagUpdated += OnIsReadyFlagUpdated;
			clientData.OnIsServerFlagUpdated += OnIsServerFlagUpdated;
		}

		private void CreateDefaultValues(){
			LocalPlayerDataStore store = LocalPlayerDataStore.GetInstance();
			if (store.playerName != "" || store.team != 0 | store.isServer != false || store.isReady != false){
				return;
			}
			store.playerName = names[Random.Range(0, 6)];
			store.team = teams[Random.Range(0, 2)];

			if (State.GetInstance().Network() == State.NETWORK_SERVER){
				store.isServer = true;
			}
		}

		public void OnNameUpdated (GameObject player, string newName){
			LocalPlayerDataStore.GetInstance().playerName = newName;
		}

		public void OnTeamUpdated (GameObject player, int newTeam){
			LocalPlayerDataStore.GetInstance().team = newTeam;
		}

		public void OnIsReadyFlagUpdated (GameObject player, bool isReady){
			LocalPlayerDataStore.GetInstance().isReady = isReady;
		}

		public void OnIsServerFlagUpdated (GameObject player, bool isServer){
			LocalPlayerDataStore.GetInstance().isServer = isServer;
		}
	}
}