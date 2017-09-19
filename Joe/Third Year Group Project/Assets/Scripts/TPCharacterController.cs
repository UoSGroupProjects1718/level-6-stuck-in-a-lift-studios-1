using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPCharacterController : MonoBehaviour {

	public Transform playerCamera;
	public Transform character;
	public Transform centrePoint;
	public float mouseSensitivity = 2f;

	public float zoom;
	public float zoomMin = -3f;
	public float zoomMax = -6f;

	private float mouseX, mouseY;
	private float moveFrontBack, moveLeftRight;
	private float zoomSpeed = 2f;


	void Start () {
		zoom = -4.5f;
	}

	void Update () {
		//Zooming
		zoom += Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
		if (zoom > zoomMin){
			zoom = zoomMin;
		}
		if (zoom < zoomMax){
			zoom = zoomMax;
		}
		playerCamera.localPosition = new Vector3(0, 0, zoom);

		//Free Rotate
		mouseX += Input.GetAxis("Mouse X") * mouseSensitivity;
		mouseY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
		mouseY = Mathf.Clamp(mouseY, -45f, 60f);
		playerCamera.LookAt(centrePoint);
		centrePoint.localRotation = Quaternion.Euler(mouseY, mouseX, 0);

		//Movement
		centrePoint.position = new Vector3 (character.position.x, character.position.y + 2.3f, character.position.z);
	}
}
