using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterBehaviour : MonoBehaviour {

	public ParticleSystem particles;
	public float horizontalFlySpeed; // Horizontal movement speed
	public float verticalFlySpeed;
	public float landBoostForce;
	public float maxVelocityChange; // The max increase in speed at one step (aka accelleration...)
	public float maxFlyChange;
	public float takeOffForce; // Force added to character for take off
	public float fuelPackValue; // Amount of fuel gained from a fuel pack
	public bool testMode;
	public string inputMode; // Type of input (tilt, tap, keyboard-for testing)
	
	private DataControl data;
	private GameStateController gameState;
	private int verticalSpeed;
	private bool takeOff;
	//private float screenWidth;
	private float screenHeight;

	// Use this for initialization
	void Start () {
		Camera cam = Camera.main; // Store the main game camera to get sizes based on device
		var screenBottomLeft = cam.ViewportToWorldPoint (new Vector3 (0, 0, transform.position.z));
		var screenTopRight = cam.ViewportToWorldPoint (new Vector3 (1, 1, transform.position.z));

		//screenWidth = screenTopRight.x - screenBottomLeft.x;
		screenHeight = screenTopRight.y - screenBottomLeft.y;
		data = DataControl.control;
		gameState = GameStateController.gameStateController;

		data.updateVal ("inputMethod", "tilt");
		inputMode = "tilt";

		// REMOVE BEFORE PUSHING
		if(testMode) inputMode = "test";
		takeOff = true;
		particles.Stop();
	}

	// Update is called once per frame
	void Update () {
		// Take off animation
		if (takeOff && gameState.gameIsRunning ()) {
			TakeOff ();
			gameState.UpdateScore ();
			particles.Play();
		} else if (!takeOff && gameState.gameIsRunning ()) {
			gameState.UpdateScore ();
			gameState.updateFuel (-1f);
		}
	}

	// Physiscs go here
	void FixedUpdate () {
		if (!takeOff && gameState.gameIsRunning ()) {
			continueGame ();
			MoveCharacter ();
		} else if (gameState.GetState () == "First Death") {
			outOfFuel ();
		} else if (gameState.isLanding ()) {
			Landing ();
		}
	}

	void MoveCharacter () {
		Vector2 velocityChange = new Vector2 (0, 0);
		// Calculate how fast player should be moving 🚀
		if (inputMode == "tilt") {
			// Input taking when tilting
			velocityChange = MovementForce (Input.acceleration.x * 1.5f, 0);
		} else {
			// Test input
			velocityChange = MovementForce (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));
		}
		GetComponent<Rigidbody2D> ().AddForce (velocityChange,
			ForceMode2D.Force);
	}

	Vector2 MovementForce (float forceX, float forceY) {
		Vector2 velocityChange = new Vector2 ();
		// Input taking when tilting
		Vector2 targetVelocityH = new Vector2 (forceX, 0);
		Vector2 targetVelocityV = new Vector2 (0, forceY);
		targetVelocityH = transform.TransformDirection (targetVelocityH);
		targetVelocityH *= horizontalFlySpeed;
		targetVelocityV = transform.TransformDirection (targetVelocityV);
		targetVelocityV *= horizontalFlySpeed;

		// Apply a force that attempts to reach our target velocity
		Vector2 velocity = GetComponent<Rigidbody2D> ().velocity;
		velocityChange = (targetVelocityH - velocity);

		velocityChange = (targetVelocityH + targetVelocityV - velocity);
		// Make sure change not too sudden
		velocityChange.x = Mathf.Clamp (velocityChange.x, -maxVelocityChange, maxVelocityChange);
		velocityChange.y = Mathf.Clamp (velocityChange.y, -maxFlyChange, maxFlyChange);
		return velocityChange;
	}

	// Collision handler
	void OnCollisionEnter2D (Collision2D obj) {
		if (obj.gameObject.CompareTag ("Coin")) {
			// Coin collected remove from game
			Destroy (obj.gameObject);
			gameState.PlayHitCoinSound();
			// Coint coins
			gameState.updateCoins (1);
		} else if (obj.gameObject.CompareTag ("Fuel")) {
			Destroy (obj.gameObject);
			gameState.updateFuel (150f);
			gameState.PlayHitFuelSound();
		} else if (obj.gameObject.CompareTag ("Obstacle")) {
			if(!gameState.isLanding()) {
				gameState.ChangeState ("First Death");
			}
		} else if(obj.gameObject.CompareTag ("LandingGround")) {
			// Did we land safely?
			if(obj.relativeVelocity.x < 0.5 && obj.relativeVelocity.y < 2.2) {
				Debug.Log("SUCCESSFUL LANDING");
				gameState.successfulLanding(1.5f);
			}
			gameState.ChangeState("End Game");
		} else if(obj.gameObject.CompareTag ("LandingPad") && gameState.isLanding()) {
			if(obj.relativeVelocity.x < 0.5 && obj.relativeVelocity.y < 2.2) {
				Debug.Log("SUPER SUCCESSFUL LANDING");
				gameState.successfulLanding(2f);
			}
			gameState.ChangeState("End Game");			
		}
	}

	void TakeOff () {
		Rigidbody2D rb = GetComponent<Rigidbody2D> ();
		rb.AddForce (new Vector2 (0, takeOffForce));
		// Gets bottom of screen based off camera size and puts character
		// quarter of the way up
		if (transform.position.y > (-screenHeight / 2)) {
			takeOff = false;
		}
	}

	// GAME OVER - pause game and Show view ad option to continue game
	void outOfFuel () {
		//pause game, save state
		GetComponent<Rigidbody2D> ().gravityScale = 0; // stop falling on first death
		GetComponent<Rigidbody2D> ().AddForce (new Vector2 (0, 0), ForceMode2D.Impulse); // Show end game menu
	}

	// Handles landing logic
	public void Landing () {
		GetComponent<Rigidbody2D> ().gravityScale = 1;
		MoveCharacter ();
		particles.Stop();
		if (Input.GetMouseButton (0) && gameState.haveFuel()) {
			gameState.updateFuel(-1f);
			GetComponent<Rigidbody2D> ().AddForce (transform.up * landBoostForce);
			particles.Play();
		}
	}

	public void continueGame () {
		GetComponent<Rigidbody2D> ().gravityScale = 1;
		gameObject.GetComponent<Rigidbody2D> ().AddForce (transform.up * verticalFlySpeed);
	}
}