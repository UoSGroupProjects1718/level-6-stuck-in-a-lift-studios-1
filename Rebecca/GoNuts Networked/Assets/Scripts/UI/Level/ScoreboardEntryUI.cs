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

		public void FlagCurrentPlayer(){
			currentPlayerBackground.SetActive(true);
		}
	}
}
