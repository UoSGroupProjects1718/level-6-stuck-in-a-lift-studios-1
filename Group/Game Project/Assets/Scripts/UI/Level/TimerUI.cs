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
		[SyncVar]
		public bool trafficLightsVisible = false;

		public AudioSource musicAudioSource;
		public AudioSource countdownAudioSource;
		public Text timerText;
		public Image trafficLights;
		public Sprite redLight;
		public Sprite amberLight;
		public Sprite greenLight;

		private bool gameOver = false;
		private int minutes;
		private int seconds;

		public void Awake(){
			SubscribeToServerReady();
		}

		public void Update (){
			ServerKeepTime();
			trafficLights.gameObject.SetActive(trafficLightsVisible);
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
		}

		public void OnDestroy(){
			StopAllCoroutines();
		}

		public int GetTime(){
			return time;
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
				RpcUpdateTrafficLights();
				yield return new WaitForSeconds(1);
				countdown --;
				RpcUpdateTrafficLights();
			}
			RpcStartTheGame();
			StartCoroutine(this.GameClock());
		}

		[Server]
		private IEnumerator GameClock(){
			while (!gameOver) {
				RpcUpdateClock();
				yield return new WaitForSeconds(1);
				if (State.GetInstance().Level() == State.LEVEL_PLAYING){
					time++;
				} else if (State.GetInstance().Level() == State.LEVEL_COMPLETE){
					gameOver = true;
				}
			}
		}

		[Server]
		private void ServerKeepTime(){
			minutes = Mathf.FloorToInt(time/60F);
			seconds = Mathf.FloorToInt(time - minutes * 60);
			niceTime = string.Format("{00:00}:{1:00}", minutes, seconds);
		}

		[ClientRpc]
		private void RpcUpdateTrafficLights(){
//			if (State.GetInstance().Level() == State.LEVEL_READY){
				State.GetInstance().Subscribe(new StateOption().LevelState(State.LEVEL_READY), () => {
					if (!countdownAudioSource.isPlaying){
						countdownAudioSource.Play();
					}
					trafficLightsVisible = true;
				} );
//			}
		}

		[ClientRpc]
		private void RpcUpdateClock(){
//			if (State.GetInstance().Level() == State.LEVEL_PLAYING){
			State.GetInstance().Subscribe(new StateOption().LevelState(State.LEVEL_PLAYING), () => {
				trafficLightsVisible = false;
				if (!musicAudioSource.isPlaying){
					musicAudioSource.Play();
				}
				if (goNuts){
					timerText.text = "Go Nuts!";
				} else {
					timerText.text = niceTime;
				}
			} );
//			} else {
			State.GetInstance().Subscribe(new StateOption().LevelState(State.LEVEL_COMPLETE), () => {
				gameOver = true;
				trafficLightsVisible = false;
//			}
			} );
		}

		private IEnumerator NutTime(int nutTimer){
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
			StartCoroutine(this.NutTime(3));
			State.GetInstance().Level(State.LEVEL_PLAYING).Publish();
		}
	}
}
