using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Menu {
	public class DonutControlScript : MonoBehaviour {

		public float maxSize = 10f;
		public float minSize = 0.1f;
		public float scaleOffset = 1.25f;

		float currentScale = 1f;
		bool upScale;
		int frames = 100;
		float scaling;
		float timeDelay = 0.05f;

		void Start () {
			int rand = Random.Range (1, 3);
			if (rand == 1) {
				upScale = true;
			}
			if (rand == 2) {
				upScale = false;
			}

			currentScale = Random.Range(minSize,maxSize - scaleOffset);
			scaling = (maxSize - currentScale)/frames;
			StartCoroutine(Scale());
		}


		IEnumerator Scale(){
			while (true){
				while (upScale) {
					currentScale += scaling;
					if (currentScale > maxSize) {
						upScale = false;
						currentScale = maxSize;
					}
					gameObject.transform.localScale = new Vector3(1, 1, 0) * currentScale;
					yield return new WaitForSeconds(timeDelay);
				}
				while (!upScale) {
					currentScale -= scaling;
					if (currentScale < minSize) {
						upScale = true;
						currentScale = minSize;
					}
					gameObject.transform.localScale = new Vector3(1, 1, 0) * currentScale;
					yield return new WaitForSeconds(timeDelay);
				}
			}
		}
	}
}

