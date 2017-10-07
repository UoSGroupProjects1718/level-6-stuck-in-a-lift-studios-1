using Player.SyncedData;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Lobby.Player {
	public class RemoteEntryUI : MonoBehaviour, EntryInterface {

		public GameObject isReadyBackground;
		public GameObject isServerBackground;
		public Text nameText;
		public Text teamText;

		private PlayerDataForClients settings;

		public void SetPlayerObject(GameObject player){
			settings = player.GetComponent<PlayerDataForClients>();

			UpdateNameFromSettings(player, settings.GetName());
			UpdateTeamFromSettings(player, settings.GetTeam());
			UpdateReadyFlagFromSettings(player, settings.GetIsReadyFlag());
			UpdateServerFlagFromSettings(player, settings.GetIsServerFlag());

			settings.OnNameUpdated += UpdateNameFromSettings;
			settings.OnTeamUpdated += UpdateTeamFromSettings;
			settings.OnIsReadyFlagUpdated += UpdateReadyFlagFromSettings;
			settings.OnIsServerFlagUpdated += UpdateServerFlagFromSettings;
		}

		public void UpdateNameFromSettings(GameObject player, string name){
			nameText.text = name;
		}

		public void UpdateTeamFromSettings(GameObject player, int teamID){
			if (teamID == PlayerDataForClients.PLAYER_ALPHA){
				teamText.text = "ALPHA";
			}
			if (teamID == PlayerDataForClients.PLAYER_BETA){
				teamText.text = "BETA";
			}
		}

		public void UpdateReadyFlagFromSettings(GameObject player, bool isReady){
			isReadyBackground.SetActive(isReady);
		}

		public void UpdateServerFlagFromSettings(GameObject player, bool isServer){
			isServerBackground.SetActive(isServer);
		}
	}
}
