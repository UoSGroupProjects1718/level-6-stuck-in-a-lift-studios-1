using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu {
	public class RotateScript : MonoBehaviour {

        public SizeScript menu;
		public int position = -60;
		public GameObject mainInputField;
		public AudioSource menuAudioSource;

		void Update () {
			if (mainInputField.GetComponent<InputField>().isFocused){
				return;
			}
			if (Input.GetKeyDown (KeyCode.UpArrow) || Input.GetKeyDown (KeyCode.W) || Input.GetAxis("Mouse ScrollWheel") > 0f) {
				if (position > 0) {
					position--;
					menuAudioSource.Play();
				}
			}
			if (Input.GetKeyDown (KeyCode.DownArrow) || Input.GetKeyDown (KeyCode.S) || Input.GetAxis("Mouse ScrollWheel") < 0f) {
				if (position < 2) {
					position++;
					menuAudioSource.Play();
				}
			}
			gameObject.transform.rotation = Quaternion.Euler (0, 0, 30 * position);
		}

        public void changePos (int pos) {
            position = pos;
        }
    }
}
