using UnityEngine;
using System.Collections;

namespace Nut {
	public class NutSpin : MonoBehaviour {

		private bool rotate = true;

		void Update (){
			if (rotate){
				transform.Rotate (new Vector3 (15, 30, 45) * Time.deltaTime);
			}
		}

		public void ToggleRotation(bool toggle){
			rotate = toggle;
		}

	}
}
