using GameState;
using UnityEngine;
using UnityEngine.Networking;

public class GrapplingHook : NetworkBehaviour {

	public PlayerController player;
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
		if (!isLocalPlayer){
			return;
		}
		if (crosshairPrefab != null){
			crosshairPrefab = Instantiate(crosshairPrefab);
			ToggleCrosshair(false);
		}
		if (State.GetInstance().Level() == State.LEVEL_PLAYING){
			Cursor.lockState = CursorLockMode.Locked;
		}
	}

	void Update () {
		if (!isLocalPlayer || State.GetInstance().Level() != State.LEVEL_PLAYING){
			Cursor.lockState = CursorLockMode.None;
			return;
		}
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
		if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDistance, cullingmask)){
			PositionCrosshair(hit);
		} else {
			ToggleCrosshair(false);
		}
	}

	void Findspot(){
		if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDistance, cullingmask)){
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
			crosshairPrefab.transform.LookAt(Camera.main.transform);
		}
	}

	void ToggleCrosshair(bool enabled){
		if (crosshairPrefab != null){
			crosshairPrefab.SetActive(enabled);
		}
	}
}
