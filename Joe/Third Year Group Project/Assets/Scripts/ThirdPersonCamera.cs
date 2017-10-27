using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {

	public GameObject Target;
	public Vector3 cameraOffset = new Vector3(0, 1, -2);
	public float cameraSpeed = 10f;

	private float mouseX, mouseY;
	public Transform centrePoint;
	public float mouseSensitivity = 2f;
	private Camera _camera;
	// Use this for initialization
	void Start () {
		_camera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(_camera != null && Target != null)
		{
			Vector3 targetPos = Target.transform.position;
			Vector3 offset = cameraOffset;

			float cameraAngle = _camera.transform.eulerAngles.y;
			float targetAngle = Target.transform.eulerAngles.y;

			if (Input.GetAxisRaw("Vertical") < 0.2f)
			{
				targetAngle = cameraAngle;
			}

			targetAngle = Mathf.LerpAngle(cameraAngle, targetAngle, cameraSpeed * Time.deltaTime);
			offset = Quaternion.Euler(0, targetAngle, 0) * offset;
			
			_camera.transform.position = Vector3.Lerp(_camera.transform.position, targetPos + offset, cameraSpeed * Time.deltaTime);
			_camera.transform.LookAt(targetPos);

			//mouseX += Input.GetAxis("Mouse X") * mouseSensitivity;
			//mouseY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
			//mouseY = Mathf.Clamp(mouseY, -45f, 60f);
			//_camera.transform.LookAt(centrePoint);
			//centrePoint.localRotation = Quaternion.Euler(mouseY, mouseX, 0);
		}
	}

}
