using GameState;
using Nut;
using Player.SyncedData;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Player {
	public class PlayerController : NetworkBehaviour {

		public float baseGroundSpeed = 8;
		public float groundSpeedModifier = 1;
		public float baseGravityStrength = 12;
		public float baseJumpPower = 9;
        public float jumpTime = 0.3f;
		public float aerialSpeed = 0.1f;
		public float momentumMeter;
		public float grappleSpeed = 10f;
		public float grappleCooldownTime = 2f;
        public int maxDistance;

        public GameObject crosshairPrefab;
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
		private Image cooldownImage;
		private Image crosshairImage;
        private int airSpeed = 20;
        private int maxMeter = 100;
		private PlayerDataForClients playerData;
		private Quaternion inputRotation;
		private RaycastHit hit;
		private Text scoreText;
		private Transform nutTransform;
        private Vector3 groundedVelocity;
        private Vector3 input;
        private Vector3 location;
		private Vector3 velocity;
        private Vector3 wallNormal;

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
		}

		void Update () {
			if (!isLocalPlayer || State.GetInstance().Level() != State.LEVEL_PLAYING){
				crosshairPrefab.SetActive(false);
				return;
			} else {
				crosshairPrefab.SetActive(true);
			}
			if (State.GetInstance().Level() == State.LEVEL_PLAYING){
				if (!menuToggled){
					Cursor.lockState = CursorLockMode.Locked;
				} else {
					Cursor.lockState = CursorLockMode.None;
				}
			} else {
				Cursor.lockState = CursorLockMode.None;
			}

            LookInput();

            animator.SetFloat("Speed", controller.velocity.magnitude);

            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(transform.position, -Vector3.up, out hit))
            {
                var distanceToGround = hit.distance;
                float timeToGround = Mathf.Abs(distanceToGround / controller.velocity.y);
                Debug.Log("Time to Ground = " + timeToGround);
                animator.SetFloat("Vertical", timeToGround);
            }

            MovePlayer();

			if (Input.GetMouseButtonDown(0)){
				if (!grappleOnCooldown){
					StartCoroutine(GrappleCooldown());
				}
			}

			if (isFlying) {
				Flying();
				if (Input.GetMouseButtonDown(1)) {
				    isFlying = false;
				    canMove = true;
				    lineRenderer.enabled = false;
				}
			} else if (playerData.GetHasNutFlag()) {
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
		}

        private void LookInput()
        {
            //Look Input
            if (Input.GetButtonDown("Cancel"))
            {
                menuToggled = !menuToggled;
            }
            if (menuToggled)
            {
                //TODO Show options to quit game
                return;
            }

            Ray ray = new Ray(camera.transform.position, camera.transform.forward); //camera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;

            if (groundPlane.Raycast(ray, out rayDistance))
            {
                Vector3 point = ray.GetPoint(rayDistance);
                transform.LookAt(point);
            }
        }

		private void MovePlayer(){
			animator.SetBool("isGrounded", controller.isGrounded);

			if (!canMove || !playerData.GetCanMoveFlag()) {
				return;
			}
			if (!wallJumped) {
				input.x = Input.GetAxisRaw("Horizontal");
				input.z = Input.GetAxisRaw("Vertical");
			}
			input = Vector3.ClampMagnitude(input, 1f);

            #region Momentum
            //Do the running part
            if (controller.isGrounded){ //On the ground
                groundSpeed = baseGroundSpeed + (momentumMeter / 10);
				if (controller.velocity.magnitude > 0) {
					if (!movementAudioSource.isPlaying){
						movementAudioSource.Play();
					}
					//Increase Momentum when moving forward on the ground
//					momentumMeter += 0.03f;
					momentumMeter += 0.1f;
					if (momentumMeter > maxMeter){
						momentumMeter = maxMeter;
					}
				} else { //If on the ground and not moving
					DrainMomentumMeter();
				}
			} else { //We lose momentum when not on the ground
				DrainMomentumMeter();
				groundSpeed = (baseGroundSpeed - 1) + (momentumMeter / 10);
				if (airSpeed < groundSpeed) {
					groundSpeed = airSpeed;
				}
			}
            #endregion

            #region Gravity
            if (controller.velocity.y > 0) { // Going UP
                //normal gravity when going up
				gravityStrength = baseGravityStrength;
			} else if (controller.velocity.y < 0){ // Coming DOWN
				if (!Input.GetButton("Jump")) {
                    // drop faster when falling
					gravityStrength = baseGravityStrength * 1.5f;
				} else {
                    // drop slower when gliding
					gravityStrength = baseGravityStrength * (0.7f - (momentumMeter/10f));
				}
			}
            #endregion

            #region Jumping
            if ((controller.isGrounded || jumpInputTime > 0f) && Input.GetButton("Jump") && jumpInputTime < jumpTime) {
				jumpInputTime += Time.deltaTime;
			} else {
				jumpInputTime = 0f;
			}

			if (jumpInputTime > 0f) {
                animator.SetTrigger("Jump");
                jumpAudioSource.Play();
                verticalVelocity += baseJumpPower;

                //Walljumping
                if (canJump) {
                    if (onWall && !wallJumped) {
                        Vector3 reflection = Vector3.Reflect(velocity, wallNormal);
                        Vector3 projected = Vector3.ProjectOnPlane(reflection, Vector3.up);
                        groundedVelocity = (projected.normalized + wallNormal) / 15f * aerialSpeed;
                        wallJumped = true;
                    }
                }
            } else {
				verticalVelocity -= gravityStrength * Time.deltaTime;
			}

            if (!onWall && wallJumped) {
                wallJumped = false;
            }
            #endregion

            Vector3 moveVector = input;
			inputRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up), Vector3.up);
			moveVector = inputRotation * moveVector;

            moveVector *= groundSpeed;
//			moveVector = Vector3.ClampMagnitude(moveVector, groundSpeed); //Not sure this is helping

            moveVector.y = verticalVelocity;// * Time.deltaTime;
			moveVector.x += groundedVelocity.x;
			moveVector.z += groundedVelocity.z;

            moveVector *= Time.deltaTime; //moved this down from being after clamp magnitude

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
				cooldownImage.fillAmount = 1f;
				float elapsed = 0.0f;
				while (elapsed < grappleCooldownTime){
					elapsed += Time.deltaTime;
					cooldownImage.fillAmount = 1f - (elapsed/grappleCooldownTime);
					yield return new WaitForSeconds(0.01f);
				}
				grappleOnCooldown = false;
				cooldownImage.fillAmount = 0f;
			}
		}

		bool Findspot() {
			if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, maxDistance, cullingmask)){
				if (hit.transform.gameObject.tag == "Nut"){
					if (playerData.GetHasNutFlag()){
						return false;
					}
					nutTransform = hit.transform;
					nutTransform.gameObject.GetComponentInParent<NutSpin>().ToggleRotation(false);
					nutTransform.rotation = Quaternion.identity;
					nutTransform.LookAt(transform);
				} else {
					isFlying = true;
					groundedVelocity = Vector3.zero;
					canMove = false;
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
				verticalVelocity += baseJumpPower;
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
				scoreText.text = "Nutted!";
			}
			if (col.transform.tag == "Checkpoint"){
				if (playerData.GetHasNutFlag()){
					StartCoroutine(ScoreTextCooldown());
					playerData.CmdIncrementScore();
					playerData.CmdSetHasNutFlag(false);
				}
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
