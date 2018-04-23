using GameState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UI {
	public class OutroCamera : MonoBehaviour {

		public float speed;
		public Transform target;
		public GameObject uiMain;
		public GameObject uiTracker;
		public Camera thisCamera;
		public Camera mainCamera;

		private Vector3 startPos;
		private Quaternion startRot;
		private bool returning = false;

		void Start () {
			thisCamera.enabled = true;
			mainCamera.enabled = false;
			startPos = transform.position;
			startRot = transform.rotation;
		}

		void Update () {
			if (this == null){
				return;
			}

			float step = speed * Time.deltaTime;
			if (!returning) {
				transform.position = Vector3.MoveTowards(transform.position, target.position, step);
			}
			if (Vector3.Distance(transform.position, target.position) < 5f){
				// Camera sweep is over
				Vector3 newDir = Vector3.RotateTowards(transform.forward, startPos, step, 0.0f);
				transform.rotation = Quaternion.LookRotation(newDir);
			}
			if (Vector3.Distance(transform.position, target.position) < 1f){
				returning = true;
				transform.position = Vector3.MoveTowards(transform.position, startPos, step);
			}
			if (Vector3.Distance(transform.position, startPos) < 5f){
				if (returning){
					Vector3 newDir = Vector3.RotateTowards(transform.forward, target.position, step, 0.0f);
					transform.rotation = Quaternion.LookRotation(newDir);
				}
			}
			if (Vector3.Distance(transform.position, startPos) < 1f){
				returning = false;
			}
		}
	}
}
