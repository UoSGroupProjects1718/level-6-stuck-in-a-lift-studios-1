using Modal;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu {
	public class MainMenuControls : MonoBehaviour {

		public RotateScript Focus;
		public GameObject playMenu;

		void Update () {
			switch (Focus.position) {
				case 0:
					//Play Menu
					Debug.Log("Play!");
					playMenu.SetActive(true);
					break;
				case 1:
					//Options Menu
					Debug.Log("Options!");
					playMenu.SetActive(false);
					break;
				case 2:
					Debug.Log("Exit");
					playMenu.SetActive(false);
					if (Input.GetButtonDown("Submit")){
						//Exit the Game
						ModalManager.GetInstance().Show(
							"Ready to quit the game?",
							"Yes!",
							"Not yet...",
							() => {
								Application.Quit();
							},
							() => {
								ModalManager.GetInstance().Hide();
								}
						);
					}
					break;
				default:
					playMenu.SetActive(false);
					break;
			}
		}
	}
}
