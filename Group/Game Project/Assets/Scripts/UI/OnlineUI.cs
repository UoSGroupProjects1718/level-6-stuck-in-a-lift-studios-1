using GameState;
using UnityEngine;
using UnityEngine.Networking;

namespace UI {
	public class OnlineUI : MonoBehaviour {

		public GameObject client;
		public GameObject server;

		public void Awake(){
			if (State.GetInstance().Network() == State.NETWORK_CLIENT){
				client.SetActive(true);
			}
			if (State.GetInstance().Network() == State.NETWORK_SERVER){
				server.SetActive(true);
			}
		}

		public void Disconnect(){
			if (State.GetInstance().Network() == State.NETWORK_CLIENT){
				NetworkManager.singleton.StopClient();
				State.GetInstance().Game(State.GAME_DISCONNECTING);
			}
			if (State.GetInstance().Network() == State.NETWORK_SERVER){
				NetworkManager.singleton.StopHost();
				State.GetInstance().Game(State.GAME_DISCONNECTING);
			}
		}
	}
}
