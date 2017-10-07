using Player.SyncedData;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Lobby.Player {
	public class LocalEntryUI : MonoBehaviour, EntryInterface {

		public InputField nameInputField;
		public GameObject alphaButton;
		public GameObject betaButton;

		public GameObject readyBackground;
		public Text nameText;
		public Text teamText;

		private PlayerDataForClients settings;

		public void SetPlayerObject(GameObject player){
			settings = player.GetComponent<PlayerDataForClients>();

			UpdateNameFromSettings(player, settings.GetName());
			UpdateTeamWithSettings(player, settings.GetTeam());
			UpdateReadyFlagFromSettings(player, settings.GetIsReadyFlag());

			settings.OnNameUpdated += UpdateNameFromSettings;
			settings.OnTeamUpdated += UpdateTeamWithSettings;
			settings.OnIsReadyFlagUpdated += UpdateReadyFlagFromSettings;
		}

		public void SendNameToSettings(InputField nameText){
			settings.SetName(nameText.text);
		}

		public void SendTeamToSettings(int teamID){
			settings.SetTeam(teamID);
		}

		public void UpdateNameFromSettings(GameObject player, string name){
			nameInputField.text = name;
			nameText.text = nameInputField.text;
		}

		private void UpdateTeamWithSettings(GameObject player, int teamID){
			if (teamID == PlayerDataForClients.PLAYER_ALPHA){
				if (alphaButton) alphaButton.SetActive(true);
				if (betaButton) betaButton.SetActive(false);
				teamText.text = "ALPHA";
			}
			if (teamID == PlayerDataForClients.PLAYER_BETA){
				if (alphaButton) alphaButton.SetActive(false);
				if (betaButton) betaButton.SetActive(true);
				teamText.text = "BETA";
			}
		}

		public void UpdateReadyFlagFromSettings(GameObject player, bool isReady){
			readyBackground.SetActive(isReady);
			nameText.gameObject.SetActive(isReady);
			teamText.gameObject.SetActive(isReady);
			nameInputField.gameObject.SetActive(!isReady);

			if (isReady) {
				alphaButton.SetActive(false);
				betaButton.SetActive(false);
			} else {
				UpdateTeamWithSettings(player, settings.GetTeam());
			}
		}
	}
}
