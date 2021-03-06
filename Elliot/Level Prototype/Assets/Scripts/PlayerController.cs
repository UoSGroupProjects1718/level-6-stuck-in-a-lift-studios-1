﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public bool canMove;
	public float baseGroundSpeed = 4;
	public float groundSpeedModifier = 1;
	public float baseGravityStrength = 12;
	public float gravityStrengthModifier = 1;
	public float glideStregth;
	public float baseJumpPower = 9;
	public float jumpPowerModifier = 1;
	public float aerialSpeed;
	public float momentumMeter;
	private int maxMeter = 100;
	private int mSpeed = 16;
	private int mAirSpeed = 20;
	public Camera camera;

	public float grappleSpeed = 10f;
	public GameObject crosshairPrefab;
	public int maxDistance;
	public LayerMask cullingmask;
	public LineRenderer lineRenderer;
	public Transform hand;

	private bool isFlying;
	private RaycastHit hit;
	private Vector3 location;

	private bool canJump;
	private bool onWall;
	private bool canGlide = true;
	private CharacterController controller;
	private float verticalVelocity;
	private float groundSpeed;
	private float gravityStrength;
	private float jumpPower;
	private Quaternion inputRotation;
	private Vector3 input;
	[HideInInspector]
	public Vector3 groundedVelocity;
	private Vector3 moveVector;
	private Vector3 wallNormal;
	private Vector3 velocity;
	private bool wallJumped;

	void Start () {
		controller = GetComponent<CharacterController>();

		Cursor.lockState = CursorLockMode.Locked;
		if (crosshairPrefab != null) {
			crosshairPrefab = Instantiate(crosshairPrefab);
			ToggleCrosshair(false);
		}
	}

	void Update () {
		MovePlayer();

		if (Input.GetMouseButtonDown(0)){
			Findspot();
		}
		if (isFlying){
			Flying();
		}
		if (Input.GetMouseButtonDown(1) && isFlying){
			isFlying = false;
			canMove = true;
			lineRenderer.enabled = false;
		}
		if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, maxDistance, cullingmask)){
			PositionCrosshair(hit);
		} else {
			ToggleCrosshair(false);
		}
	}

	private void MovePlayer(){
		//Look Input
		Ray ray = new Ray(camera.transform.position, camera.transform.forward); //camera.ScreenPointToRay(Input.mousePosition);
		Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
		float rayDistance;

		if (groundPlane.Raycast(ray, out rayDistance)){
			Vector3 point = ray.GetPoint(rayDistance);
			Debug.DrawLine(ray.origin, point, Color.red);
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
			if (input.z > 0) {
				momentumMeter += 0.2f;
				if(momentumMeter > maxMeter){
					momentumMeter = maxMeter;
				}
			} else {
					momentumMeter -= 1f;
					if(momentumMeter < 0){
						momentumMeter = 0;
					}
			}
			if(canGlide){
				gravityStrength = 12;

				groundSpeed = 10 + (momentumMeter / 10);
				if (mSpeed < groundSpeed) {
					groundSpeed = mSpeed;
				}

				if (momentumMeter <= 10){
					jumpPower = baseJumpPower;
				} else {
					jumpPower = baseJumpPower + (jumpPowerModifier * (momentumMeter/20f));
				}

			} else {
				momentumMeter -= 0.5f;
				if(momentumMeter < 0){
					momentumMeter = 0;
					canGlide = true;
				}
			}
		} else {
			momentumMeter -= 0.05f;
			if(momentumMeter < 0){
				momentumMeter = 0;
				canGlide = true;
			}
			if (canGlide) {
				gravityStrength = baseGravityStrength;
			} else {
				groundSpeed = 4 + (momentumMeter / 10);
				if (mAirSpeed < groundSpeed) {
					groundSpeed = mAirSpeed;
				}
				if (momentumMeter <= 10) {
					gravityStrength = baseGravityStrength;
					momentumMeter = 0;
				} else {
					gravityStrength = baseGravityStrength - (gravityStrengthModifier * (momentumMeter/10f));
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
			if (onWall && !wallJumped){
				Vector3 reflection = Vector3.Reflect(velocity, wallNormal);
				Vector3 projected = Vector3.ProjectOnPlane(reflection, Vector3.up);
			   
				groundedVelocity = (projected.normalized+ wallNormal) / 10f * aerialSpeed;
				Debug.Log("groundedVelocity: " + groundedVelocity + " " + "projected.normalized: " + projected.normalized + " " + "wallNormal: " + wallNormal);
				wallJumped = true;
			}
			if (canJump){
				verticalVelocity += jumpPower;
			}
		}

		if (!onWall && wallJumped){
			wallJumped = false;
		}

		moveVector.y = verticalVelocity * Time.deltaTime;
		moveVector += groundedVelocity;

		CollisionFlags flags = controller.Move(moveVector);
		velocity = moveVector / Time.deltaTime;

		if ((flags & CollisionFlags.Below) != 0){
			//groundedVelocity = Vector3.ProjectOnPlane(velocity, Vector3.up);
			canJump = true;
			canGlide = true;
			onWall = false;
			groundedVelocity = Vector3.zero;
			verticalVelocity = -1f;
		} else if ((flags & CollisionFlags.Sides) != 0){
			canJump = true;
			canGlide = true;
			onWall = true;
			groundedVelocity = Vector3.zero;
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

	void Findspot() {
		if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, maxDistance, cullingmask)){
			isFlying = true;
			groundedVelocity = Vector3.zero;
			location = hit.point;
			canMove = false;
			lineRenderer.enabled = true;
			lineRenderer.SetPosition(1, location);
		}
	}

	void Flying() {
		transform.position = Vector3.Lerp(transform.position, location, grappleSpeed * Time.deltaTime / Vector3.Distance(transform.position, location));
		lineRenderer.SetPosition(0, hand.position);

		if (Vector3.Distance(transform.position, location) < 1f){
			isFlying = false;
			canMove = true;
			lineRenderer.enabled = false;
		}
	}

	void PositionCrosshair(RaycastHit hit){
		if (crosshairPrefab != null) {
			ToggleCrosshair(true);
			crosshairPrefab.transform.position = hit.point;
			crosshairPrefab.transform.LookAt(camera.transform);
		}
	}

	void ToggleCrosshair(bool enabled) {
		if (crosshairPrefab != null) {
			crosshairPrefab.SetActive(enabled);
		}
	}
}
