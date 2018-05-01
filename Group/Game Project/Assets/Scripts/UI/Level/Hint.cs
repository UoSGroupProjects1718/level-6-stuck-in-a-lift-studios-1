using GameState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UI.Level {
	public class Hint : NetworkBehaviour {

		public string moveForward = "Press W to run forward";
		public string turning = "Move the MOUSE to turn";
		public string jump = "Press SPACE to jump";
		public string glide = "Hold SPACE to glide";
		public string grappleButton = "Click MOUSE LEFT to grapple";
		public string grappleNope = "You can't grapple that";
		public string objective = "Collect a nut from the tree";
		public string onlyOne = "You can only carry one nut";
		public string grappleNut = "Use the grapple to gather the nut";
		public string bringBack = "Bring the nut to the statue";
		public string another = "Go get one more!";
		public string eagle = "Watch out for the eagle!";

		public int maxPanelOpacity;
		public int maxTextOpacity;
		public float fadeTime;

		private Text hintText;
		private Image hintPanel;
		private GameObject hintPanelObj;
		private GameObject hintTextObj;

		private bool gameOver = false;
		private int showMoveCount = 0;
		private int showTurningCount = 0;
		private int showJumpCount = 0;
		private int showGlideCount = 0;
		private int showGrappleButtonCount = 0;
		private int showGrappleNopeCount = 0;
		private int showObjectiveCount = 0;
		private int showOnlyOneCount = 0;
		private int showGrappleNutCount = 0;
		private int showBringBackCount = 0;
		private int showAnotherCount = 0;

		void Start () {
			gameOver = false;
			hintTextObj = GameObject.FindGameObjectWithTag("HintText");
			if (hintTextObj != null){
				hintText = hintTextObj.GetComponent<Text>();
			}
			hintPanelObj = GameObject.FindGameObjectWithTag("HintPanel");
			if (hintPanelObj != null){
				hintPanel = hintPanelObj.GetComponent<Image>();
			}
		}

		void Update () {
			if ( State.GetInstance().Level() != State.LEVEL_COMPLETE ){
				hintPanelObj.SetActive(false);
				gameOver = true;
			}
		}

		public void ShowHintMove(bool show){
			if (gameOver || showMoveCount++ > 2){
				return;
			}
			hintText.text = moveForward;
			Fade(show);
		}

		public void ShowHintTurning(bool show){
			if (gameOver || showTurningCount++ > 2){
				return;
			}
			hintText.text = turning;
			Fade(show);
		}

		public void ShowHintJump(bool show){
			if (gameOver || showJumpCount++ > 2){
				return;
			}
			hintText.text = jump;
			Fade(show);
		}

		public void ShowHintGlide(bool show){
			if (gameOver || showGlideCount++ > 2){
				return;
			}
			hintText.text = glide;
			Fade(show);
		}

		public void ShowHintGrappleButton(bool show){
			if (gameOver || showGrappleButtonCount++ > 3){
				return;
			}
			hintText.text = grappleButton;
			Fade(show);
		}

		public void ShowHintGrappleNope(bool show){
			if (gameOver || showGrappleNopeCount++ > 2){
				return;
			}
			hintText.text = grappleNope;
			Fade(show);
		}

		public void ShowHintObjective(bool show){
			if (gameOver || showObjectiveCount++ > 2){
				return;
			}
			hintText.text = objective;
			Fade(show);
		}

		public void ShowHintOnlyOne(bool show){
			if (gameOver || showOnlyOneCount++ > 2){
				return;
			}
			hintText.text = onlyOne;
			Fade(show);
		}

		public void ShowHintGrappleNut(bool show){
			if (gameOver || showGrappleNutCount++ > 2){
				return;
			}
			hintText.text = grappleNut;
			Fade(show);
		}

		public void ShowHintBack(bool show){
			if (gameOver || showBringBackCount++ > 4){
				return;
			}
			hintText.text = bringBack;
			Fade(show);
		}

		public void ShowHintAnother(bool show ){
			if (gameOver || showAnotherCount++ > 4){
				return;
			}
			hintText.text = another;
			Fade(show);
		}

		public void ShowHintEagle(bool show){
			if (gameOver){
				return;
			}
			hintText.text = eagle;
			Fade(show);
		}

		private void Fade(bool fadeIn){
			if (fadeIn){
				StartCoroutine("FadeIn");
			} else {
				StartCoroutine("FadeOut");
			}
		}

		private IEnumerator FadeIn(){
			float elapsed = 0.0f;
			while (elapsed < fadeTime){
				elapsed += Time.deltaTime;

				float textAlpha = Mathf.Lerp(0, (maxTextOpacity/255f), elapsed / fadeTime);
				hintText.color = new Color(hintText.color.r, hintText.color.g, hintText.color.b, textAlpha);
				float panelAlpha = Mathf.Lerp(0, (maxPanelOpacity/255f), elapsed / fadeTime);
				hintPanel.color = new Color(hintPanel.color.r, hintPanel.color.g, hintPanel.color.b, panelAlpha);

				yield return null;
			}
		}

		private IEnumerator FadeOut(){
			float elapsed = 0.0f;
			while (elapsed < fadeTime){
				elapsed += Time.deltaTime;

				float textAlpha = Mathf.Lerp((maxTextOpacity/255), 0, elapsed / fadeTime);
				hintText.color = new Color(hintText.color.r, hintText.color.g, hintText.color.b, textAlpha);
				float panelAlpha = Mathf.Lerp((maxPanelOpacity/255), 0, elapsed / fadeTime);
				hintPanel.color = new Color(hintPanel.color.r, hintPanel.color.g, hintPanel.color.b, panelAlpha);

				yield return null;
			}
		}
	}
}
