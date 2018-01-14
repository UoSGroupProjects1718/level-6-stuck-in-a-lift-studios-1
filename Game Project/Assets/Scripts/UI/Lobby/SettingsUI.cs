using GameState;
using Modal;
using Player.SyncedData;
using Player.Tracking;
using UI.Level;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UI.Lobby {
	public class SettingsUI : NetworkBehaviour {

		public GameObject clientUI;
		public GameObject serverUI;

		public Text serverIpAddress;

		public GameObject readyWaitingButton;
		public GameObject readyNowButton;

		public GameObject startGameButton;
		public Text startGameButtonText;

		private bool allowServerStart = false;

		public void Awake(){
			State.GetInstance().Level(State.LEVEL_IN_LOBBY).Publish();
		}

		public void Start(){
			if (State.GetInstance().Network() == State.NETWORK_CLIENT){
				clientUI.SetActive(true);
			} else {
				serverUI.SetActive(true);
				SetIpAddress();
			}
		}

		public void OnGUI(){
			UpdateServerStartButton();
		}

		[Server]
		public void SetIpAddress(){
			serverIpAddress.text = Network.player.ipAddress;
		}

		[Client]
		public void SetReadyState(bool isReady){
			PlayerTracker.GetInstance().GetLocalPlayer().GetComponent<PlayerDataForClients>().SetIsReadyFlag(isReady);
			if (readyWaitingButton == null || readyNowButton == null) {
				return;
			}
			readyWaitingButton.SetActive(!isReady);
			readyNowButton.SetActive(isReady);
		}

		public void LeaveLobby(){
			if (State.GetInstance().Network() == State.NETWORK_CLIENT){
				NetworkManager.singleton.StopClient();
			}
			if (State.GetInstance().Network() == State.NETWORK_SERVER){
				NetworkManager.singleton.StopHost();
			}
			State.GetInstance().Game(State.GAME_DISCONNECTING);
		}

		public void StartGame(){
			if (!allowServerStart){
				return;
			}

			ModalManager.GetInstance().Show(
				"Ready to start the game?",
				"Yes!",
				"Not yet...",
				() => {
					RpcUpdateClientStateOnStart();
					AssignTeams();
					NetworkManager.singleton.ServerChangeScene("Level A");
				},
				() => {
					ModalManager.GetInstance().Hide();
					}
			);
		}

		[ClientRpc]
		private void RpcUpdateClientStateOnStart(){
			State.GetInstance().Level(State.LEVEL_NOT_READY).Publish();
		}

		[ServerCallback]
		private void UpdateServerStartButton(){
			bool allReady = true;
			foreach (GameObject player in PlayerTracker.GetInstance().GetPlayers()){
				if (!player){
					continue;
				}
				PlayerDataForClients settings = player.GetComponent<PlayerDataForClients>();
				if (!settings.GetIsReadyFlag() && !settings.GetIsServerFlag()){
					allReady = false;
				}
			}
			startGameButtonText.text = allReady ? "Start Game" : "Waiting on Ready";
			allowServerStart = allReady;
		}

		private void AssignTeams(){
			Debug.Log("Assigning Teams");
			int teamCount = 0;
			foreach (GameObject player in PlayerTracker.GetInstance().GetPlayers()){
				teamCount ++;
				switch(teamCount){
					case 1:
						player.GetComponent<PlayerDataForClients>().SetTeam(PlayerDataForClients.PLAYER_A);
						Debug.Log("Assigning Player A");
						break;
					case 2:
						player.GetComponent<PlayerDataForClients>().SetTeam(PlayerDataForClients.PLAYER_B);
						break;
					case 3:
						player.GetComponent<PlayerDataForClients>().SetTeam(PlayerDataForClients.PLAYER_C);
						break;
					case 4:
						player.GetComponent<PlayerDataForClients>().SetTeam(PlayerDataForClients.PLAYER_D);
						break;
					case 5:
						player.GetComponent<PlayerDataForClients>().SetTeam(PlayerDataForClients.PLAYER_E);
						break;
				}
			}
		}
	}
}
