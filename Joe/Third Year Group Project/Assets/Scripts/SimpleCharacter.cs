using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCharacter : MonoBehaviour {

	public bool canMove;
	public float groundSpeed;
	public float gravityStrength;
	public float glideStregth;
	public float jumpPower;
	public float aerialSpeed;
	public float momentumMeter;
	private int maxMeter = 100;
	public Camera camera;

	private bool canJump;
	private bool onWall;
	private bool canGlide = true;
	private CharacterController controller;
	private float verticalVelocity;
	private Quaternion inputRotation;
	private Vector3 input;
	private Vector3 groundedVelocity;
	private Vector3 moveVector;
	private Vector3 wallNormal;
	private Vector3 velocity;

	void Start () {
		controller = GetComponent<CharacterController>();
	}

	void Update () {
		MovePlayer();
	}

	private void MovePlayer(){
		//Look Input
		Ray ray = new Ray(camera.transform.position, camera.transform.forward); //camera.ScreenPointToRay(Input.mousePosition);
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
			inputRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up), Vector3.up);
			moveVector = inputRotation * moveVector;
			moveVector *= groundSpeed;
			if (Input.GetKey(KeyCode.W)) {
				momentumMeter += 0.2f;
				if(momentumMeter > maxMeter){
					momentumMeter = maxMeter;
				}
			}else{
					momentumMeter -= 1f;
					if(momentumMeter < 0){
						momentumMeter = 0;
					}
			}
			if(canGlide){
				gravityStrength = 12;
				if(momentumMeter < 25){
					groundSpeed = 6;
				}
				if(momentumMeter >= 25 && momentumMeter < 60){
					groundSpeed = 9;
				}
				if(momentumMeter >= 60){
					groundSpeed = 12;
				}
			}else{
				momentumMeter -= 0.05f;
				if(momentumMeter < 0){
					momentumMeter = 0;
					canGlide = true;
				}

			}
				
		} else {
			/*
			moveVector = inputRotation * moveVector;
			moveVector += groundedVelocity;
			moveVector += input * aerialSpeed;
			*/
			momentumMeter -= 0.05f;
			if(momentumMeter < 0){
				momentumMeter = 0;
				canGlide = true;
			}
			if (canGlide) {
				gravityStrength = 12;
			} else {
			if(momentumMeter < 25){
					groundSpeed = 9;
					gravityStrength = 9;
				}
			if(momentumMeter >= 25 && momentumMeter < 60){
					groundSpeed = 9;
					gravityStrength = 6;
				}
			if(momentumMeter >= 60){
					groundSpeed = 12;
					gravityStrength = 3;
				}
			}
			if (Input.GetKeyDown(KeyCode.E) && canGlide == true){
				canGlide = false;
			}

			moveVector = input;
			inputRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up), Vector3.up);
			moveVector = inputRotation * moveVector;
			moveVector *= groundSpeed;
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
			canGlide = true;
			onWall = false;
			verticalVelocity = -1f;
		} else if ((flags & CollisionFlags.Sides) != 0){
			canJump = true;
			canGlide = true;
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

