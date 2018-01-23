using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Menu {
	public class RotateScript : MonoBehaviour {

		public int position = -60;

		void Update () {
			if (Input.GetKeyDown (KeyCode.UpArrow) || Input.GetKeyDown (KeyCode.W)) {
				if (position > 0) {
					position--;
				}
			}
			if (Input.GetKeyDown (KeyCode.DownArrow) || Input.GetKeyDown (KeyCode.S)) {
				if (position < 2) {
					position++;
				}
			}

			

			gameObject.transform.rotation = Quaternion.Euler (0, 0, 30 * position);
		}
	}
}
