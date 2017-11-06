using GameState;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UI.Level {
	public class TimerUI : NetworkBehaviour {

		[SyncVar]
		public int timer = 5;

		public Text timerText;

		private int minutes;
		private int seconds;
		private string niceTime;

		public void Awake(){
//			timer = LevelData.GetInstance().levelTime;
			SubscribeToServerReady();
		}

		public void Update (){
			if (timer > 0){
				minutes = Mathf.FloorToInt(timer/60F);
				seconds = Mathf.FloorToInt(timer - minutes * 60);
				niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);
				timerText.text = niceTime;
			}
		}

		public void OnDestroy(){
			StopAllCoroutines();
		}

		[ServerCallback]
		private void SubscribeToServerReady(){
			State.GetInstance().Subscribe(new StateOption().LevelState(State.LEVEL_READY), StartTimer);
		}

		[Server]
		private void StartTimer(){
			if (this != null) {
				StartCoroutine(this.WaitForTimerToEnd());
				RpcStartTheGame();
			}
		}
//		[Server]
		private IEnumerator WaitForTimerToEnd(){
			while (timer > 0) {
				yield return new WaitForSeconds(1);
				timer --;
			}
			timerText.text = "Go Nuts!";
			RpcStartTheGame();
		}

		[ClientRpc]
		private void RpcStartTheGame(){
			State.GetInstance().Level(State.LEVEL_PLAYING).Publish();
		}
	}
}
