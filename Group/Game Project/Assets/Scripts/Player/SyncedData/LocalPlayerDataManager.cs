/**
 * This is based on the tutorial series found here: https://ditt.to/game-dev/multiplayer-1
 **/
using GameState;
using UnityEngine;
using UnityEngine.Networking;

namespace Player.SyncedData {
	public class LocalPlayerDataManager : NetworkBehaviour {

		public PlayerDataForClients clientData;

		private string[] names = new string[] {"Rebecca", "Max", "Joe", "Elliot", "Zak", "Will"};
		private int[] teams = new int[] {
			PlayerDataForClients.PLAYER_A,
			PlayerDataForClients.PLAYER_B,
			PlayerDataForClients.PLAYER_C,
			PlayerDataForClients.PLAYER_D,
			PlayerDataForClients.PLAYER_E};

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
						clientData.CmdSetHasNutFlag(false);
						clientData.SetCanMoveFlag(true);
					} else {
						store.playerName = "";
						store.team = 0;
						store.isReady = false;
						store.isServer = false;
						store.hasNut = false;
						store.canMove = true;
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
			clientData.SetCanMoveFlag(store.canMove);

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
			store.playerName = names[Random.Range(0, names.Length)];
			store.team = teams[Random.Range(0, teams.Length)];

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
