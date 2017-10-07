using UnityEngine;
using UnityEngine.Networking;

namespace GameState {
	public class StateManager : MonoBehaviour {

		private NetworkManager network;
		private State state;

		public void Start(){
			state = State.GetInstance();
			state.Game(State.GAME_OFFLINE).Publish();
		}

		public void Update(){
			if (network == null){
				network = NetworkManager.singleton;
			}
			SetNetworkState();
			if (SetConnectionState()){
				state.Publish();
			}
		}

		private void SetNetworkState(){
			bool isServer = NetworkServer.active;
			bool isClient = NetworkClient.active && !isServer;

			if (isServer && state.Network() != State.NETWORK_SERVER){
				state.Network(State.NETWORK_SERVER);
			}
			if (isClient && state.Network() != State.NETWORK_CLIENT){
				state.Network(State.NETWORK_CLIENT);
			}
		}

		private bool SetConnectionState(){
			bool isOffline = !network.IsClientConnected() && !NetworkServer.active && network.matchMaker == null;
			bool hasConnection = network.client != null && network.client.connection != null && network.client.connection.connectionId != -1;

			if (isOffline && !hasConnection && state.Game() != State.GAME_OFFLINE){
				state.Game(State.GAME_OFFLINE);
				return true;
			}
			if (isOffline && hasConnection && state.Game() != State.GAME_CONNECTING){
				state.Game(State.GAME_CONNECTING);
				return true;
			}
			if (!isOffline && state.Game() != State.GAME_ONLINE){
				state.Game(State.GAME_ONLINE);
				return true;
			}

			return false;
		}
	}
}
