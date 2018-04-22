using GameState;
using Nut;
using Player.SyncedData;
using System.Collections;
using UI.Level;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Player {
	public class PlayerController : NetworkBehaviour {

		public float baseGroundSpeed = 8;
		public float groundSpeedModifier = 1;
		public float baseGravityStrength = 12;
		public float baseJumpPower = 9;
		public float aerialSpeed = 0.1f;
		[HideInInspector]
		public float momentumMeter;
		public float grappleSpeed = 10f;
		public float grappleCooldownTime = 2f;
		public GameObject crosshairPrefab;
		public int maxDistance;
		public LayerMask cullingmask;
		public LineRenderer lineRenderer;
		public Transform hand;

		public AudioSource grappleAudioSource;
		public AudioSource jumpAudioSource;
		public AudioSource movementAudioSource;
		public AudioSource nutPickupAudioSource;
		public AudioSource scoreAudioSource;

		private Animator animator;
		private bool menuToggled = false;
		private bool canMove = true;
		private bool canJump;
		private bool isFlying;
		private bool onWall;
		private bool canGlide = true;
		private bool wallJumped;
		private bool grappleOnCooldown = false;
		private Camera camera;
		private CharacterController controller;
		private float verticalVelocity;
		private float groundSpeed;
		private float gravityStrength;
		private float jumpInputTime = 0f;
		private float idleTime = 0f;
		private Image cooldownImage;
		private Image crosshairImage;
		private int maxMeter = 100;
		private int maxSpeed = 16;
		private int mAirSpeed = 20;
		private PlayerDataForClients playerData;
		private Quaternion inputRotation;
		private RaycastHit hit;
		private Text scoreText;
		private Transform nutTransform;
		private Vector3 location;
		private Vector3 input;
		private Vector3 groundedVelocity;
		private Vector3 moveVector;
		private Vector3 wallNormal;
		private Vector3 velocity;

		void Start () {
			if (!isLocalPlayer){
				return;
			}
			playerData = transform.gameObject.GetComponent<PlayerDataForClients>();

			camera = Camera.main;
			controller = GetComponent<CharacterController>();

			animator = GetComponentInChildren<Animator>();

			if (crosshairPrefab != null) {
				crosshairPrefab = Instantiate(crosshairPrefab);
				Image[] crosshairImages = crosshairPrefab.GetComponentsInChildren<Image>();
				foreach (Image image in crosshairImages){
					if (image.gameObject.tag == "Cooldown"){
						cooldownImage = image;
					} else if (image.gameObject.tag == "Crosshair"){
						crosshairImage = image;
					}
				}
				cooldownImage.fillAmount = 0f;
				crosshairPrefab.SetActive(false);
				ToggleCrosshair(false);
			}
			GameObject scoreTextObj = GameObject.FindGameObjectWithTag("ScoreText");
			if (scoreTextObj != null){
				scoreText = scoreTextObj.GetComponent<Text>();
			}

			animator.SetTrigger("Warmup");
		}

		void Update () {
			if (!isLocalPlayer || State.GetInstance().Level() != State.LEVEL_PLAYING){
				crosshairPrefab.SetActive(false);
				return;
			} else {
				animator.SetTrigger("Startgame");
				crosshairPrefab.SetActive(true);
			}
			if (State.GetInstance().Level() == State.LEVEL_PLAYING){
				if (!menuToggled){
					Cursor.lockState = CursorLockMode.Locked;
				} else {
					Cursor.lockState = CursorLockMode.None;
				}
			} else {
				GetComponent<Hint>().ShowHintObjective(true);
				StartCoroutine(ShowHintCooldown("Objective"));
				Cursor.lockState = CursorLockMode.None;
			}

			GetComponent<Hint>().ShowHintMove(true);
			MovePlayer();

			if (Input.GetMouseButtonDown(0)){
				if (!grappleOnCooldown){
					StartCoroutine(GrappleCooldown());
				}
			}

			if (isFlying){
				Flying();
				if (Input.GetMouseButtonDown(1)){
					animator.SetBool("Grapple", false);
					isFlying = false;
					canMove = true;
					lineRenderer.enabled = false;
				}
			} else if (playerData.GetHasNutFlag()){
				lineRenderer.enabled = false;
			}

			if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, maxDistance, cullingmask)){
				PositionCrosshair(hit);
			} else {
				ToggleCrosshair(false);
			}

			if (nutTransform != null && !playerData.GetHasNutFlag()){
				nutTransform.position = Vector3.Lerp(nutTransform.position, transform.position, grappleSpeed * Time.deltaTime / Vector3.Distance(nutTransform.position, transform.position));
				lineRenderer.SetPosition(0, hand.position);
				lineRenderer.SetPosition(1, nutTransform.position);
			}

			if (controller.isGrounded){
				animator.SetFloat("Speed", controller.velocity.magnitude);
			}
			animator.SetFloat("Vertical", controller.velocity.y);
			animator.SetBool("isIdle", idleTime > 2f);
		}

		private void MovePlayer(){
			//Look Input
			if (Input.GetButtonDown("Cancel")){
				menuToggled = !menuToggled;
			}
			if (menuToggled){
				return;
			}

			Ray ray = new Ray(camera.transform.position, camera.transform.forward);
			Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
			float rayDistance;

			if (groundPlane.Raycast(ray, out rayDistance)){
				Vector3 point = ray.GetPoint(rayDistance);
				transform.LookAt(point);
			}

			animator.SetBool("isGrounded", controller.isGrounded);

			if (!canMove || !playerData.GetCanMoveFlag()){
				return;
			}
			if (!wallJumped){
				input.z = Input.GetAxisRaw("Vertical");
				input.x = Input.GetAxisRaw("Horizontal");
			}
			input = Vector3.ClampMagnitude(input, 1f);

			//Do the running part
			if (controller.isGrounded){ //On the ground
				if (controller.velocity.magnitude > 0) { //Are we on the ground and moving?
					GetComponent<Hint>().ShowHintMove(false);
					GetComponent<Hint>().ShowHintJump(true);
					if (!movementAudioSource.isPlaying){
						movementAudioSource.Play();
					}
					//Increase Momentum when moving forward on the ground
					momentumMeter += 0.03f;
//					momentumMeter += 0.1f;
					if(momentumMeter > maxMeter){
						momentumMeter = maxMeter;
					}
					idleTime = 0f;
				} else { //If on the ground and not moving
					DrainMomentumMeter();
					idleTime += Time.deltaTime;
				}
			} else { //In the Air
				idleTime = 0f;
				DrainMomentumMeter(); //We lose momentum when not on the ground
				groundSpeed = (baseGroundSpeed-1) + (momentumMeter / 10);
				if (mAirSpeed < groundSpeed) {
					groundSpeed = mAirSpeed;
				}
			}

			if (controller.velocity.y > 0){ // Going UP
				gravityStrength = baseGravityStrength;
			} else if (controller.velocity.y < 0){ // Coming DOWN
				if (!Input.GetButton("Jump")){ // drop faster when not gliding
					gravityStrength = baseGravityStrength * 2f;
				} else { // drop slower when gliding
					gravityStrength = baseGravityStrength * (0.5f - (momentumMeter/10f));
				}
			}

			//Do the jump part
			if (Input.GetButtonDown("Jump")){
				if (onWall && !wallJumped){
					Vector3 reflection = Vector3.Reflect(velocity, wallNormal);
					Vector3 projected = Vector3.ProjectOnPlane(reflection, Vector3.up);
					groundedVelocity = (projected.normalized + wallNormal)/15f * aerialSpeed;
					wallJumped = true;
				}
			}

			if ((controller.isGrounded || jumpInputTime > 0f) && Input.GetButton("Jump") && jumpInputTime < 0.3f) {
				jumpInputTime += Time.deltaTime;
			} else {
				jumpInputTime = 0f;
			}

			if (jumpInputTime > 0f){
				animator.SetBool("isJumping", true);
				verticalVelocity = baseJumpPower;
				groundSpeed = (baseGroundSpeed + (momentumMeter / 10)) * 1.5f;
				GetComponent<Hint>().ShowHintJump(false);
				GetComponent<Hint>().ShowHintGlide(true);
				StartCoroutine(ShowHintCooldown("Glide"));
				jumpAudioSource.Play();
			} else {
				animator.SetBool("isJumping", false);
				verticalVelocity -= gravityStrength * Time.deltaTime;
				groundSpeed = baseGroundSpeed + (momentumMeter / 10);
			}

			moveVector = input;
			inputRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up), Vector3.up);
			moveVector = inputRotation * moveVector;
			moveVector *= groundSpeed;

			moveVector = Vector3.ClampMagnitude(moveVector, groundSpeed);
			moveVector *= Time.deltaTime;

			if (!onWall && wallJumped){
				wallJumped = false;
			}

			moveVector.y = verticalVelocity * Time.deltaTime;
			moveVector += groundedVelocity;

			CollisionFlags flags = controller.Move(moveVector);
			if (controller.velocity.magnitude > 0.1f && controller.isGrounded){
				transform.rotation = Quaternion.LookRotation(moveVector);
			}
			velocity = moveVector / Time.deltaTime;

			if ((flags & CollisionFlags.Below) != 0){
//				groundedVelocity = Vector3.ProjectOnPlane(velocity, Vector3.up);
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

		private void DrainMomentumMeter(){
			momentumMeter -= 1f;
			if(momentumMeter < 0){
				momentumMeter = 0;
			}
		}

		void OnControllerColliderHit(ControllerColliderHit hit){
			wallNormal = hit.normal;
		}

		private IEnumerator GrappleCooldown(){
			if (Findspot()){
				animator.SetBool("Grapple", true);
				grappleAudioSource.Play();
				grappleOnCooldown = true;
				GetComponent<Hint>().ShowHintGrappleButton(false);
				cooldownImage.fillAmount = 1f;
				float elapsed = 0.0f;
				while (elapsed < grappleCooldownTime){
					elapsed += Time.deltaTime;
					cooldownImage.fillAmount = 1f - (elapsed/grappleCooldownTime);
					yield return new WaitForSeconds(0.01f);
				}
				grappleOnCooldown = false;
				cooldownImage.fillAmount = 0f;
			} else {
				GetComponent<Hint>().ShowHintGrappleNope(true);
				StartCoroutine(ShowHintCooldown("Nope"));
			}
		}

		bool Findspot() {
			if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, maxDistance, cullingmask)){
				if (hit.transform.gameObject.tag == "Nut"){
					if (playerData.GetHasNutFlag()){
						GetComponent<Hint>().ShowHintOnlyOne(true);
						StartCoroutine(ShowHintCooldown("One"));
						return false;
					}
					GetComponent<Hint>().ShowHintGrappleNut(true);
					nutTransform = hit.transform;
					nutTransform.gameObject.GetComponentInParent<NutSpin>().ToggleRotation(false);
					nutTransform.rotation = Quaternion.identity;
					nutTransform.LookAt(transform);
				} else {
					isFlying = true;
					groundedVelocity = Vector3.zero;
					canMove = false;
					GameObject grappleTarget = hit.transform.gameObject;
					grappleTarget.GetComponent<Collider>().enabled = false;
					float distance = Vector3.Distance(transform.position, hit.point) + 1;
					float velocity = controller.velocity.magnitude + 1;
					int timeToTarget = (int)(distance/velocity) + 1;
					StartCoroutine(GrappleTargetCooldown(grappleTarget, timeToTarget + 1));
				}
				location = hit.point;
				lineRenderer.enabled = true;
				lineRenderer.SetPosition(1, location);
				return true;
			}
			return false;
		}

		void Flying() {
			transform.position = Vector3.Lerp(transform.position, location, grappleSpeed * Time.deltaTime / Vector3.Distance(transform.position, location));
			lineRenderer.SetPosition(0, hand.position);

			if (Vector3.Distance(transform.position, location) < 1f){
				verticalVelocity += 10f;
				animator.SetBool("Grapple", false);
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
				if (enabled){
					crosshairImage.color = Color.green;
					GetComponent<Hint>().ShowHintGrappleButton(true);
				} else {
					crosshairImage.color = Color.red;
				}
			}
		}

		void OnTriggerEnter(Collider col){
			if (!isLocalPlayer){
				return;
			}
			if (col.gameObject.tag == "Nut"){
				if (playerData.GetHasNutFlag()){
					return;
				}
				animator.SetBool("Grapple", false);
				nutPickupAudioSource.Play();
				playerData.CmdSetHasNutFlag(true);
				CmdDestroyObject(col.gameObject.GetComponentInParent<NetworkIdentity>().netId);
				GetComponent<Hint>().ShowHintGrappleNut(false);
				scoreText.text = "Nutted!";
				GetComponent<Hint>().ShowHintBack(true);
				StartCoroutine(ShowHintCooldown("Back"));
			}
			if (col.transform.tag == "Checkpoint"){
				if (playerData.GetHasNutFlag()){
					StartCoroutine(ScoreTextCooldown());
					playerData.CmdIncrementScore();
					playerData.CmdSetHasNutFlag(false);
					GetComponent<Hint>().ShowHintAnother(true);
					StartCoroutine(ShowHintCooldown("Another"));
				}
			}
		}

		private IEnumerator GrappleTargetCooldown(GameObject grappleTarget, int time){
			float elapsed = 0.0f;
			while (elapsed < time){
				elapsed += Time.deltaTime;
				yield return null;
			}
			grappleTarget.gameObject.GetComponent<Collider>().enabled = true;
		}

		private IEnumerator ShowHintCooldown(string name){
			float elapsed = 0.0f;
			while (elapsed < 1f){
				elapsed += Time.deltaTime;
				yield return null;
			}
			switch (name){
				case "One":
					GetComponent<Hint>().ShowHintOnlyOne(false);
					break;
				case "Nope":
					GetComponent<Hint>().ShowHintGrappleNope(false);
					break;
				case "Back":
					GetComponent<Hint>().ShowHintBack(false);
					break;
				case "Another":
					GetComponent<Hint>().ShowHintAnother(false);
					break;
				case "Glide":
					GetComponent<Hint>().ShowHintGlide(false);
					break;
				case "Objective":
					GetComponent<Hint>().ShowHintObjective(false);
					break;
			}
		}

		private IEnumerator ScoreTextCooldown(){
			scoreAudioSource.Play();
			scoreText.text = "Score!";
			float elapsed = 0.0f;
			while (elapsed < 3f){
				elapsed += Time.deltaTime;
				yield return new WaitForSeconds(0.01f);
			}
			scoreText.text = "Go Nuts!";
		}

		[Command]
		public void CmdDestroyObject(NetworkInstanceId netID){
			GameObject theObject = NetworkServer.FindLocalObject (netID);
			NetworkServer.Destroy (theObject);
		}
	}
}
