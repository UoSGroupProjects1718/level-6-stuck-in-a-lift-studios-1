using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu {
	public class SizeScript : MonoBehaviour {

		public RotateScript Focus;
		public GameObject one;
		public GameObject two;
		public GameObject three;

		public Color frontFocusColor = new Color(0,0,0,1f);
		public Color midFocusColor = new Color(0,0,0,0.9f);
		public Color backFocusColor = new Color(0,0,0,0.7f);

		void Update () {
			switch (Focus.position) {
				case 0:
					one.transform.localScale = new Vector3 (1.5f, 1.5f, 0);
					two.transform.localScale = new Vector3 (1, 1, 0);
					three.transform.localScale = new Vector3 (0.75f, 0.75f, 0);
					one.GetComponentInChildren<Text> ().color = frontFocusColor;
					two.GetComponentInChildren<Text> ().color = midFocusColor;
					three.GetComponentInChildren<Text> ().color = backFocusColor;
					break;
				case 1:
					one.transform.localScale = new Vector3 (1, 1, 0);
					two.transform.localScale = new Vector3 (1.5f, 1.5f, 0);
					three.transform.localScale = new Vector3 (1, 1, 0);
					one.GetComponentInChildren<Text> ().color = midFocusColor;
					two.GetComponentInChildren<Text> ().color = frontFocusColor;
					three.GetComponentInChildren<Text> ().color = midFocusColor;
					break;
				case 2:
					one.transform.localScale = new Vector3 (0.75f, 0.75f, 0);
					two.transform.localScale = new Vector3 (1, 1, 0);
					three.transform.localScale = new Vector3 (1.5f, 1.5f, 0);
					one.GetComponentInChildren<Text> ().color = backFocusColor;
					two.GetComponentInChildren<Text> ().color = midFocusColor;
					three.GetComponentInChildren<Text> ().color = frontFocusColor;
					break;
				default:
					one.transform.localScale = new Vector3 (1.5f, 1.5f, 0);
					two.transform.localScale = new Vector3 (1, 1, 0);
					three.transform.localScale = new Vector3 (0.75f, 0.75f, 0);
					one.GetComponentInChildren<Text> ().color = frontFocusColor;
					two.GetComponentInChildren<Text> ().color = midFocusColor;
					three.GetComponentInChildren<Text> ().color = backFocusColor;
					break;
			}
		}
	}
}
