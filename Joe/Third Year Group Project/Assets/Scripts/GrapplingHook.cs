using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour {

	public Camera camera;
	public SimpleCharacter player;
	public float speed = 10f;
	public GameObject crosshairPrefab;
	public int maxDistance;
	public LayerMask cullingmask;
	public LineRenderer lineRenderer;
	public Transform hand;

	private bool isFlying;
	private RaycastHit hit;
	private Vector3 location;

	void Start () {
		Cursor.lockState = CursorLockMode.Locked;
		if (crosshairPrefab != null){
			crosshairPrefab = Instantiate(crosshairPrefab);
			ToggleCrosshair(false);
		}
	}

	void Update () {
		if (Input.GetMouseButtonDown(0)){
			Findspot();
		}
		if (isFlying){
			Flying();
		}
		if (Input.GetMouseButtonDown(1) && isFlying){
			isFlying = false;
			player.canMove = true;
			lineRenderer.enabled = false;
		}
		if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, maxDistance, cullingmask)){
			PositionCrosshair(hit);
		} else {
			ToggleCrosshair(false);
		}
	}

	void Findspot(){
		if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, maxDistance, cullingmask)){
			isFlying = true;
			location = hit.point;
			player.canMove = false;
			lineRenderer.enabled = true;
			lineRenderer.SetPosition(1, location);
		}
	}

	void Flying(){
		transform.position = Vector3.Lerp(transform.position, location, speed * Time.deltaTime / Vector3.Distance(transform.position, location));
		lineRenderer.SetPosition(0, hand.position);

		if (Vector3.Distance(transform.position, location) < 1f){
			isFlying = false;
			player.canMove = true;
			lineRenderer.enabled = false;
		}
	}

	void PositionCrosshair(RaycastHit hit){
		if (crosshairPrefab != null){
			ToggleCrosshair(true);
			crosshairPrefab.transform.position = hit.point;
			crosshairPrefab.transform.LookAt(camera.transform);
		}
	}

	void ToggleCrosshair(bool enabled){
		if (crosshairPrefab != null){
			crosshairPrefab.SetActive(enabled);
		}
	}
}
