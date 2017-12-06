using Player.SyncedData;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Lobby.Player {
	public class LocalEntryUI : MonoBehaviour, EntryInterface {

		public InputField nameInputField;
		public GameObject readyBackground;
		public Text nameText;

		private PlayerDataForClients settings;

		public void SetPlayerObject(GameObject player){
			settings = player.GetComponent<PlayerDataForClients>();

			UpdateNameFromSettings(player, settings.GetName());
			UpdateReadyFlagFromSettings(player, settings.GetIsReadyFlag());

			settings.OnNameUpdated += UpdateNameFromSettings;
			settings.OnIsReadyFlagUpdated += UpdateReadyFlagFromSettings;
		}

		public void SendNameToSettings(InputField nameText){
			settings.SetName(nameText.text);
		}

		public void UpdateNameFromSettings(GameObject player, string name){
			nameInputField.text = name;
			nameText.text = nameInputField.text;
		}

		public void UpdateReadyFlagFromSettings(GameObject player, bool isReady){
			readyBackground.SetActive(isReady);
			nameText.gameObject.SetActive(isReady);
			nameInputField.gameObject.SetActive(!isReady);
		}
	}
}
