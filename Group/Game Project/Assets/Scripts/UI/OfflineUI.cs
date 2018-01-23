using GameState;
using Modal;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UI {
	public class OfflineUI : MonoBehaviour {

		public int maxConnections = 4;

		public void Start(){
			State.GetInstance().Subscribe(
				new StateOption()
					.NetworkState(State.NETWORK_CLIENT)
					.PreviousGameState(State.GAME_ONLINE)
					.GameState(State.GAME_OFFLINE),
				() => {
					ModalManager.GetInstance().Show(
						"Lost connection to server",
						"OK",
						() => { ModalManager.GetInstance().Hide(); }
					);
				}
			);

			State.GetInstance().Subscribe(
				new StateOption()
					.NetworkState(State.NETWORK_CLIENT)
					.PreviousGameState(State.GAME_CONNECTING)
					.GameState(State.GAME_OFFLINE),
				() => {
					ModalManager.GetInstance().Show(
						"Cannot establish connection to server",
						"OK", 
						() => { ModalManager.GetInstance().Hide(); }
					);
				}
			);
		}

		public void StartServer(){
			NetworkManager.singleton.networkAddress = "localhost";

			var config = new ConnectionConfig();
			config.AddChannel(QosType.ReliableSequenced);
			config.AddChannel(QosType.Unreliable);
			if (NetworkManager.singleton.StartHost(config, maxConnections) == null){
				ModalManager.GetInstance().Show(
					"You already have a server running on this machine",
					"Oh, ok",
					() => { ModalManager.GetInstance().Hide(); }
				);
			}
		}

		public void JoinGame(Text ipAddressText){
			if (ipAddressText.text == ""){
				ModalManager.GetInstance().Show(
					"You need to enter an IP address to connect to",
					"Try Again",
					() => { ModalManager.GetInstance().Hide(); }
				);
				return;
			}

			NetworkManager.singleton.networkAddress = ipAddressText.text;
			if (NetworkManager.singleton.StartClient() == null){
				ModalManager.GetInstance().Show(
					"Connection not attempted to " + ipAddressText.text,
					"OK",
					() => { ModalManager.GetInstance().Hide(); }
				);
				return;
			}

			ModalManager.GetInstance().Show(
				"Attempting to join " + ipAddressText.text,
				"Cancel attempt",
				() => {
					NetworkManager.singleton.StopClient();
					ModalManager.GetInstance().Hide();
				}
			);
		}
	}
}
