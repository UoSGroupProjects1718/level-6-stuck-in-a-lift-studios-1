using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UI {
	public class CutsceneCamera : NetworkBehaviour {

		public float speed;
		public Transform target;
		public GameObject uiMain;
		public GameObject uiTracker;
		public Camera thisCamera;
		public Camera mainCamera;

		void Start () {
			thisCamera.enabled = true;
			mainCamera.enabled = false;
		}

		void Update () {
			float step = speed * Time.deltaTime;
			transform.position = Vector3.MoveTowards(transform.position, target.position, step);
			if (Vector3.Distance(transform.position, target.position) < 5f){
				// Camera sweep is over
				thisCamera.enabled = false;
				mainCamera.enabled = true;
			}
		}
	}
}
