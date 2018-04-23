using GameState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UI {
	public class CutsceneCamera : MonoBehaviour {

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
			State.GetInstance().Subscribe(new StateOption().LevelState(State.LEVEL_READY), () => {
				thisCamera.enabled = false;
				mainCamera.enabled = true;
			} );
			Debug.Log("Camera State = " + State.GetInstance().Level());
			if (State.GetInstance().Level() == State.LEVEL_READY){
				thisCamera.enabled = false;
				mainCamera.enabled = true;
			}

			float step = speed * Time.deltaTime;
			if (!returning) {
				transform.position = Vector3.MoveTowards(transform.position, target.position, step);
				transform.LookAt(target.position);
			}
			else {
				transform.position = Vector3.MoveTowards(transform.position, startPos, step);
				transform.LookAt(startPos);
			}
			if (Vector3.Distance(transform.position, target.position) < 5f){
				returning = true;
			}
			if (Vector3.Distance(transform.position, startPos) < 5f){
				if (returning){
					returning = false;
				}
			}
		}
	}
}
