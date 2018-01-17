using GameState;
using Player.SyncedData;
using Player.Tracking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UI.Level {
	public class PlayerPositionUI : NetworkBehaviour {

		public Sprite withNut;
		public Sprite withoutNut;
		public Sprite withNutIcon;
		public Sprite withoutNutIcon;
		public Color[] playerColours = new Color[5];

		private PlayerDataForClients playerData;
		private List<GameObject> playerList;
		private GameObject[] playerIconListObjs;
		private Image positionImage;
		private Text positionText;
		private RectTransform spawnIcon;
		private RectTransform treeIcon;

        void Start () {
			if (!isLocalPlayer){
                Debug.Log("Not Local Player PlayerPositionUI");
				return;
			}
            Debug.Log("PlayerPositionUI is Local Player");
			playerData = transform.gameObject.GetComponent<PlayerDataForClients>();
			playerList = PlayerTracker.GetInstance().GetPlayers();

			GameObject positionImageObj = GameObject.FindGameObjectWithTag("PositionImage");
			if (positionImageObj != null){
				positionImage = positionImageObj.GetComponent<Image>();
			}
			GameObject positionTextObj = GameObject.FindGameObjectWithTag("PositionText");
			if (positionTextObj != null){
				positionText = positionTextObj.GetComponent<Text>();
			}

			playerIconListObjs = GameObject.FindGameObjectsWithTag("PlayerIcon");
			if (playerIconListObjs != null){
				if (playerList != null){
					if (playerIconListObjs.Length > 0){
						int i = 0;
						for (int j=0; j<playerIconListObjs.Length; j++){
							Debug.Log("Setting Icons Inactive");
							playerIconListObjs[i].SetActive(false);
						}
						foreach (GameObject player in playerList){
							Debug.Log("Setting Icons Active");
							playerIconListObjs[i].SetActive(true);
							i++;
						}
					}
				}
			}

			GameObject spawnIconObj = GameObject.FindGameObjectWithTag("FlagIcon");
			if (spawnIconObj != null){
				spawnIcon = spawnIconObj.GetComponent<RectTransform>();
			}
			GameObject treeIconObj = GameObject.FindGameObjectWithTag("TreeIcon");
			if (treeIconObj != null){
				treeIcon = treeIconObj.GetComponent<RectTransform>();
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
			SetTeamColour(playerData.GetTeam(), positionImage.gameObject);
			SetRank();
			
			int i = 0;
			foreach (GameObject player in playerList){
				PlayerDataForClients pData = player.GetComponent<PlayerDataForClients>();
				SetUIPosition(playerIconListObjs[i], pData.GetPosition());
				SetSprite(playerIconListObjs[i], pData.GetHasNutFlag());
				SetTeamColour(pData.GetTeam(), playerIconListObjs[i]);
				i++;
			}
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

		private void SetTeamColour(int team, GameObject image){
			switch (team){
				case PlayerDataForClients.PLAYER_A:
					image.GetComponent<Image>().color = playerColours[0];
					break;
				case PlayerDataForClients.PLAYER_B:
					image.GetComponent<Image>().color = playerColours[1];
					break;
				case PlayerDataForClients.PLAYER_C:
					image.GetComponent<Image>().color = playerColours[2];
					break;
				case PlayerDataForClients.PLAYER_D:
					image.GetComponent<Image>().color = playerColours[3];
					break;
				case PlayerDataForClients.PLAYER_E:
					image.GetComponent<Image>().color = playerColours[4];
					break;
				default:
					//Set invisible
					var tempColour = image.GetComponent<Image>().color;
					tempColour.a = 0f;
					image.GetComponent<Image>().color = tempColour;
					break;
			}
		}
		
		private void SetSprite(GameObject image, bool hasNut){
			if (hasNut){
				image.GetComponent<Image>().sprite = withNutIcon;
			} else {
				image.GetComponent<Image>().sprite = withoutNutIcon;
			}
		}
		
		private void SetUIPosition(GameObject image, float percentage){
			//HORRIBLE HACK FIX LATER
			var maxDistance = (spawnIcon.anchoredPosition.y - treeIcon.anchoredPosition.y);
			var distance = maxDistance * (1f - percentage);
			Vector2 newPosition = new Vector3(spawnIcon.anchoredPosition.x, spawnIcon.anchoredPosition.y - distance);
			image.GetComponent<Image>().gameObject.GetComponent<RectTransform>().anchoredPosition = newPosition;
		}
	}
}
