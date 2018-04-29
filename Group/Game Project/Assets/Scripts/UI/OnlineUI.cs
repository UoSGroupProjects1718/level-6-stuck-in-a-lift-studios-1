/**
 * This is based on the tutorial series found here: https://ditt.to/game-dev/multiplayer-1
 **/
using GameState;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;

namespace UI {
	public class OnlineUI : MonoBehaviour {

		public AudioMixer audioMixer;
		public GameObject client;
		public GameObject server;

		private bool menuVisible = false;

		public void Update(){
			if (Input.GetKey(KeyCode.Escape)){
				menuVisible = !menuVisible;
			}
			if (menuVisible){
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
			else {
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
			if (State.GetInstance().Network() == State.NETWORK_CLIENT){
				client.SetActive(menuVisible);
			}
			if (State.GetInstance().Network() == State.NETWORK_SERVER){
				server.SetActive(menuVisible);
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

		public void CloseMenu(){
			menuVisible = false;
		}

		public void SetVolume (float volume){
			if  (volume < -19.5f){
				volume = -80;
			}
			audioMixer.SetFloat("volume", volume);
		}
	}
}
