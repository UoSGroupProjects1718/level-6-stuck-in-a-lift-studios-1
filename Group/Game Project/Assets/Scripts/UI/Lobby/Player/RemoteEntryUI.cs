/**
 * This is based on the tutorial series found here: https://ditt.to/game-dev/multiplayer-1
 **/
using Player.SyncedData;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Lobby.Player {
	public class RemoteEntryUI : MonoBehaviour, EntryInterface {

		public GameObject isReadyBackground;
		public GameObject isServerBackground;
		public Text nameText;

		private PlayerDataForClients settings;

		public void SetPlayerObject(GameObject player){
			settings = player.GetComponent<PlayerDataForClients>();

			UpdateNameFromSettings(player, settings.GetName());
			UpdateReadyFlagFromSettings(player, settings.GetIsReadyFlag());
			UpdateServerFlagFromSettings(player, settings.GetIsServerFlag());

			settings.OnNameUpdated += UpdateNameFromSettings;
			settings.OnIsReadyFlagUpdated += UpdateReadyFlagFromSettings;
			settings.OnIsServerFlagUpdated += UpdateServerFlagFromSettings;
		}

		public void UpdateNameFromSettings(GameObject player, string name){
			nameText.text = name;
		}

		public void UpdateReadyFlagFromSettings(GameObject player, bool isReady){
			isReadyBackground.SetActive(isReady);
		}

		public void UpdateServerFlagFromSettings(GameObject player, bool isServer){
			isServerBackground.SetActive(isServer);
		}
	}
}
