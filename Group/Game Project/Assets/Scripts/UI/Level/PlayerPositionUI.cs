using GameState;
using Player.SyncedData;
using Player.Tracking;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UI.Level {
	public class PlayerPositionUI : NetworkBehaviour {

		public Sprite withNut;
		public Sprite withoutNut;
		public Color[] playerColours = new Color[5];

		private PlayerDataForClients playerData;
		private Image positionImage;
		private Text positionText;

		void Start () {
			if (!isLocalPlayer){
				return;
			}

			playerData = transform.gameObject.GetComponent<PlayerDataForClients>();

			GameObject positionImageObj = GameObject.FindGameObjectWithTag("PositionImage");
			if (positionImageObj != null){
				positionImage = positionImageObj.GetComponent<Image>();
			}
			GameObject positionTextObj = GameObject.FindGameObjectWithTag("PositionText");
			if (positionTextObj != null){
				positionText = positionTextObj.GetComponent<Text>();
			}
		}

		void Update () {
			if (!isLocalPlayer || State.GetInstance().Level() != State.LEVEL_PLAYING){
				return;
			}
			if (playerData.GetHasNutFlag()){
				positionImage.sprite = withNut;
			} else {
				positionImage.sprite = withoutNut;
			}
			SetTeamColour();
			SetRank();
		}

		private void SetRank(){
			int rank = playerData.GetRank() + 1;
			string rankString = rank.ToString();
			switch (rank){
				case 1:
					rankString += "st";
					break;
				case 2:
					rankString += "nd";
					break;
				case 3:
					rankString += "rd";
					break;
				case 4:
				case 5:
					rankString += "th";
					break;
				default:
					rankString = "Error";
					break;
			}
			positionText.text = rankString;
		}

		private void SetTeamColour(){
			int team = playerData.GetTeam();
			Debug.Log("Team = " + team);
			switch (team){
				case PlayerDataForClients.PLAYER_A:
					Debug.Log("PLAYER A Colour");
					positionImage.color = playerColours[0];
					break;
				case PlayerDataForClients.PLAYER_B:
					positionImage.color = playerColours[1];
					break;
				case PlayerDataForClients.PLAYER_C:
					positionImage.color = playerColours[2];
					break;
				case PlayerDataForClients.PLAYER_D:
					positionImage.color = playerColours[3];
					break;
				case PlayerDataForClients.PLAYER_E:
					positionImage.color = playerColours[4];
					break;
				default:
					Debug.Log("DEFAULT Colour");
					positionImage.color = Color.white;
					break;
			}
		}
	}
}
