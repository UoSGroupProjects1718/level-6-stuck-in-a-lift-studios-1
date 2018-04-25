using Modal;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu {
	public class MainMenuControls : MonoBehaviour {

		public RotateScript Focus;
		public GameObject playMenu;
		public GameObject settingsMenu;
        public GameObject credits;

		void Update () {
			switch (Focus.position) {
				case 0:
					//Play Menu
					playMenu.SetActive(true);
					settingsMenu.SetActive(false);
                    credits.SetActive(false);
					break;
				case 1:
					//Options Menu
					playMenu.SetActive(false);
					settingsMenu.SetActive(true);
                    credits.SetActive(false);
					break;
				case 2:
					playMenu.SetActive(false);
					settingsMenu.SetActive(false);
                    credits.SetActive(true);
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
					settingsMenu.SetActive(false);
					break;
			}
		}
	}
}
