using GameState;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

	public bool canMove;
	public float groundSpeed;
	public float gravityStrength;
	public float jumpPower;
	public float aerialSpeed;
	public float mouseSensitivity = 2f;

	public float zoomMin = -3f;
	public float zoomMax = -6f;

	private float mouseX, mouseY;
	private float moveFrontBack, moveLeftRight;
	private float zoom;
	private float zoomSpeed = 2f;
	private Transform centrePoint;

	private bool canJump;
	private bool onWall;
	private CharacterController controller;
	private float verticalVelocity;
	private Quaternion inputRotation;
	private Vector3 input;
	private Vector3 groundedVelocity;
	private Vector3 moveVector;
	private Vector3 wallNormal;
	private Vector3 velocity;

	void Start () {
		if (!isLocalPlayer){
			return;
		}
		GameObject[] centrePointGO = GameObject.FindGameObjectsWithTag("CentrePoint");
		centrePoint = centrePointGO[0].transform;
		controller = GetComponent<CharacterController>();
		zoom = -4.5f;
	}

	void Update () {
		if (!isLocalPlayer || State.GetInstance().Level() != State.LEVEL_PLAYING){
			return;
		}
		MoveCamera();
		MovePlayer();
	}

	private void MoveCamera(){
		//Zooming
		zoom += Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
		if (zoom > zoomMin){
			zoom = zoomMin;
		}
		if (zoom < zoomMax){
			zoom = zoomMax;
		}
		Camera.main.transform.localPosition = new Vector3(0, 0, zoom);

		//Free Camera Rotate
		mouseX += Input.GetAxis("Mouse X") * mouseSensitivity;
		mouseY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
		mouseY = Mathf.Clamp(mouseY, -45f, 60f);
		Camera.main.transform.LookAt(centrePoint);
		centrePoint.localRotation = Quaternion.Euler(mouseY, mouseX, 0);

		//Camera Movement
		centrePoint.position = new Vector3 (transform.position.x, transform.position.y + 2.3f, transform.position.z);
	}

	private void MovePlayer(){
		//Character look
		Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
		Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
		float rayDistance;

		if (groundPlane.Raycast(ray, out rayDistance)){
			Vector3 point = ray.GetPoint(rayDistance);
//			Debug.DrawLine(ray.origin, point, Color.red);
			transform.LookAt(point);
		}

		if (!canMove){
			return;
		}

		input.x = Input.GetAxisRaw("Horizontal");
		input.z = Input.GetAxisRaw("Vertical");
		input = Vector3.ClampMagnitude(input, 1f);

		if (controller.isGrounded){
			moveVector = input;
			inputRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up), Vector3.up);
			moveVector = inputRotation * moveVector;
			moveVector *= groundSpeed;
		} else {
			moveVector = inputRotation * moveVector;
			moveVector += groundedVelocity;
			moveVector += input * aerialSpeed;
		}
		moveVector = Vector3.ClampMagnitude(moveVector, groundSpeed);
		moveVector *= Time.deltaTime;

		verticalVelocity -= gravityStrength*Time.deltaTime;
		if (Input.GetButtonDown("Jump")){
			if (onWall){
				Vector3 reflection = Vector3.Reflect(velocity, wallNormal);
				Vector3 projected = Vector3.ProjectOnPlane(reflection, Vector3.up);
				groundedVelocity = projected.normalized * groundSpeed + wallNormal*aerialSpeed;
			}
			if (canJump){
				verticalVelocity += jumpPower;
			}
		}
		moveVector.y = verticalVelocity * Time.deltaTime;

		CollisionFlags flags = controller.Move(moveVector);
		velocity = moveVector / Time.deltaTime;

		if ((flags & CollisionFlags.Below) != 0){
			groundedVelocity = Vector3.ProjectOnPlane(velocity, Vector3.up);
			canJump = true;
			onWall = false;
			verticalVelocity = -1f;
		} else if ((flags & CollisionFlags.Sides) != 0){
			canJump = true;
			onWall = true;
		} else {
			canJump = false;
			onWall = false;
			if ((flags & CollisionFlags.Above) != 0){
				verticalVelocity = 0f;
			}
		}
	}

	void OnControllerColliderHit(ControllerColliderHit hit){
		wallNormal = hit.normal;
	}
}
