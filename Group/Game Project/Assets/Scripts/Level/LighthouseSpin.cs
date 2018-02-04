using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level {
	public class LighthouseSpin : MonoBehaviour {

		public float rotateSpeed = 30f;

		void Update (){
			transform.Rotate (new Vector3 (0, rotateSpeed, 0) * Time.deltaTime);
		}
	}
}
