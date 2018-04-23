using UnityEngine;
using UnityEngine.UI;

namespace UI.Level {
	public class ScoreboardEntryUI : MonoBehaviour {

		public GameObject currentPlayerBackground;
		public Text nameText;
		public Text scoreText;

		public void SetName(string name){
			nameText.text = name;
		}

		public void SetScore(int score){
			scoreText.text = score.ToString();
		}

		public void SetTime(int time){
			if (time == float.MaxValue){
				scoreText.text = "DNF";
			} else {
				var minutes = Mathf.FloorToInt(time/60F);
				var seconds = Mathf.FloorToInt(time - minutes * 60);
				var niceTime = string.Format("{00:00}:{1:00}", minutes, seconds);
				scoreText.text = niceTime;
			}
		}

		public void FlagCurrentPlayer(){
			currentPlayerBackground.SetActive(true);
		}
	}
}
