using GameState;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UI.Level {
	public class TimerUI : NetworkBehaviour {

		[SyncVar]
		public int time = 0;
		[SyncVar]
		public int countdown = 3;
		[SyncVar]
		public string niceTime;
		[SyncVar]
		public bool goNuts = false;

		public Text timerText;
		public Image trafficLights;
		public Sprite redLight;
		public Sprite amberLight;
		public Sprite greenLight;

		private int minutes;
		private int seconds;

		public void Awake(){
			SubscribeToServerReady();
		}

		public void Update (){
			ServerKeepTime();
			UpdateTrafficLights();
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
			}
		}

		[Server]
		private IEnumerator WaitForTimerToEnd(){
			while (countdown > 0) {
				yield return new WaitForSeconds(1);
				countdown --;
			}
			RpcStartTheGame();
			StartCoroutine(this.GameClock());
		}

		[Server]
		private IEnumerator GameClock(){
			while (true) {
				yield return new WaitForSeconds(1);
				if (State.GetInstance().Level() == State.LEVEL_PLAYING){
					time++;
				}
			}
		}

		[Server]
		private void ServerKeepTime(){
			minutes = Mathf.FloorToInt(time/60F);
			seconds = Mathf.FloorToInt(time - minutes * 60);
			niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);
		}

		private void UpdateTrafficLights(){
			if (State.GetInstance().Level() == State.LEVEL_READY){
//			State.GetInstance().Subscribe(new StateOption().LevelState(State.LEVEL_READY), () => {
				trafficLights.gameObject.SetActive(true);
				switch (countdown){
					case 3:
						trafficLights.sprite = redLight;
					break;
					case 2:
						trafficLights.sprite = amberLight;
					break;
					case 1:
						trafficLights.sprite = greenLight;
					break;
					default:
					break;
				}
//			} );
			} else if (State.GetInstance().Level() == State.LEVEL_PLAYING){
//			State.GetInstance().Subscribe(new StateOption().LevelState(State.LEVEL_PLAYING), () => {
				trafficLights.gameObject.SetActive(false);
				if (goNuts){
					timerText.text = "Go Nuts!";
				} else {
					Debug.Log(niceTime);
					timerText.text = niceTime;
				}
//			} );
			}
		}

		private IEnumerator NutTime(int nutTimer){
			Debug.Log("Starting Nut Time");
			while (nutTimer > 0) {
				yield return new WaitForSeconds(1);
				nutTimer --;
			}
			goNuts = false;
		}

		[ClientRpc]
		private void RpcStartTheGame(){
			goNuts = true;
			timerText.text = "Go Nuts!";
			Debug.Log("RpcStartTheGame");
			StartCoroutine(this.NutTime(3));
			State.GetInstance().Level(State.LEVEL_PLAYING).Publish();
		}
	}
}
