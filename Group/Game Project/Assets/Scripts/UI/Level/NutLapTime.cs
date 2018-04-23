using GameState;
using Player.SyncedData;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UI.Level {
	public class NutLapTime : NetworkBehaviour {

		public int fontSizeLarge = 24;
		public int fontSizeSmall = 18;

		private PlayerDataForClients playerData;
		private Text lapTimeText1;
		private Text lapTimeText2;
		private TimerUI timerUI;
		private int nut1Time = 0;
		private int nut2Time = 0;
		private bool firstLapDone = false;
		private bool secondLapDone = false;

		void Start () {
			if (!isLocalPlayer){
				return;
			}
			playerData = transform.gameObject.GetComponent<PlayerDataForClients>();

			GameObject onlineHudObj = GameObject.FindGameObjectWithTag("OnlineHUD");
			if (onlineHudObj != null){
				timerUI = onlineHudObj.GetComponent<TimerUI>();
			}

			GameObject nut1TimeObj = GameObject.FindGameObjectWithTag("NutTime1");
			if (nut1TimeObj != null){
				lapTimeText1 = nut1TimeObj.GetComponent<Text>();
			}
			GameObject nut2TimeObj = GameObject.FindGameObjectWithTag("NutTime2");
			if (nut2TimeObj != null){
				lapTimeText2 = nut2TimeObj.GetComponent<Text>();
			}
		}

		void Update () {
			if (!isLocalPlayer || State.GetInstance().Level() != State.LEVEL_PLAYING){
				return;
			}

			if (playerData.GetScore() == 0){
				nut1Time = timerUI.GetTime() - 1;
				lapTimeText1.text = "Nut 1: " + TimeToNiceTime(nut1Time);
				lapTimeText2.text = "Nut 2: --:--";
				lapTimeText1.fontSize = fontSizeLarge;
				lapTimeText2.fontSize = fontSizeSmall;
			} else {
				if (!firstLapDone){
					playerData.CmdSetNutTime(nut1Time);
					firstLapDone = true;
				} else {
					if (!secondLapDone){
						int totalNutTime = nut1Time + nut2Time;
						playerData.CmdSetTotalNutTime(totalNutTime);
						secondLapDone = true;
						return;
					}
				}
				nut2Time = timerUI.GetTime() - (nut1Time + 1);
				lapTimeText1.text = "Nut 1: " + TimeToNiceTime(nut1Time);
				lapTimeText2.text = "Nut 2: " + TimeToNiceTime(nut2Time);
				lapTimeText1.fontSize = fontSizeSmall;
				lapTimeText2.fontSize = fontSizeLarge;
			}
		}

		private string TimeToNiceTime(int time){
			int minutes = Mathf.FloorToInt(time/60F);
			int seconds = Mathf.FloorToInt(time - minutes * 60);
			return string.Format("{00:00}:{1:00}", minutes, seconds);
		}
	}
}
