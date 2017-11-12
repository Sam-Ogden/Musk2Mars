using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterBehaviour : MonoBehaviour {

	public float initialFuel;		// Std fuel in rocket when start game
	public float moveSpeed;			// Horizontal movement speed
	public float flySpeed;
	public float maxVelocityChange;	// The max increase in speed at one step (aka accelleration...)
	public float maxFlyChange;
	public string inputMode;	// Type of input (tilt, tap, keyboard-for testing)
	public float takeOffForce;  // Force added to character for take off
	public float touchForce;    // Force used on touch input

	private float fuel;	// Ship's current level of fuel ⛽️
	private DataControl data;
	private GameStateController gameState;
	private int verticalSpeed;
	private int coinsCollected;
	private bool takeOff;
	private double score;
	private int frameCount;

	// Use this for initialization
	void Start () {
		data = DataControl.control;
		gameState = GameStateController.gameStateController;
		gameState.ChangeState("End Game");

		fuel = initialFuel;	// Always start with standard fuel level
		verticalSpeed = 0;
		coinsCollected = 0;

		if(data.containsKey("inputMethod")) {
			inputMode = data.getValue("inputMethod"); // GET INPUT MODE FROM DATA STORE
		} else {
			data.addPair("inputMethod", "tilt");
			inputMode = "tilt";
		}

		// REMOVE BEFORE PUSHING
		inputMode = "test";	// Left like this until testing time
		score = 0;
		takeOff = true;
	}
		
	// Update is called once per frame
	void Update () {
		// If no more fuel, game is over.
		if (fuel == 0) {
			outOfFuel ();
		}

		// Take off animation
		if(takeOff && gameState.gameIsRunning()) {
			TakeOff();
		}

		if(gameState.gameIsRunning()) {
			frameCount++;
			if((frameCount % 3) == 0) {
				score = score + 0.1;
				gameState.UpdateScore(System.Math.Round(score, 2));
			}
		}
	}

	// Physiscs go here
	void FixedUpdate() {
		if(gameState.gameIsRunning()) {
			fuel--;
			Vector2 velocityChange = new Vector2(0,0);
			// Calculate how fast player should be moving 🚀
			if(inputMode == "tilt") {
				// Input taking when tilting
				velocityChange = MovementForce(Input.acceleration.x, 0);
			} else if(inputMode == "touch") {
				// Input taking when using screen sides
				// Touch input = Input.GetTouch(0);
				if(Input.GetMouseButton(0)) {
					if(Input.mousePosition.x > Screen.width/2) {
						velocityChange = MovementForce(touchForce, 0);
					} else {
						velocityChange = MovementForce(-touchForce, 0);
					}
				} else {
					velocityChange = MovementForce(0, 0);
				}
			} else {
				// Test input
				velocityChange = MovementForce(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
			}

			GetComponent<Rigidbody2D>().AddForce(velocityChange,
											ForceMode2D.Force);
		}
	}

	public void StartGame() {
		gameState.ChangeState("Game Running");
	}

	Vector2 MovementForce(float forceX, float forceY) {
		Vector2 velocityChange = new Vector2();
		// Input taking when tilting
		Vector2 targetVelocityH = new Vector2(forceX, 0);
		Vector2 targetVelocityV = new Vector2(0, forceY);
		targetVelocityH = transform.TransformDirection(targetVelocityH);
		targetVelocityH *= moveSpeed;
		targetVelocityV = transform.TransformDirection(targetVelocityV);
		targetVelocityV *= flySpeed;

		// Apply a force that attempts to reach our target velocity
		Vector2 velocity = GetComponent<Rigidbody2D>().velocity;
		velocityChange = (targetVelocityH - velocity);

		velocityChange = (targetVelocityH + targetVelocityV - velocity);
		// Make sure change not too sudden
		velocityChange.x = Mathf.Clamp(velocityChange.x,
			-maxVelocityChange, maxVelocityChange);
		velocityChange.y = Mathf.Clamp(velocityChange.y,
			-maxFlyChange, maxFlyChange);
		return velocityChange;
	}
		
	// Collision handler
	void OnCollisionEnter2D(Collision2D obj) {
		if (obj.gameObject.CompareTag ("Coin")) {
			//Coin collected remove from game
			Destroy(obj.gameObject);
			// Play satisfying hit coin sound
			// Coint coins
			coinsCollected++;
		} else if(obj.gameObject.CompareTag ("Fuel")) {
			Destroy(obj.gameObject);
			fuel+=100;
		}
	}

	void TakeOff() {
		Rigidbody2D rb = GetComponent<Rigidbody2D>();
		rb.AddForce(new Vector2(0, takeOffForce), ForceMode2D.Impulse);
		// Gets bottom of screen based off camera size and puts character
		// quarter of the way up
		if(transform.position.y > (-Camera.main.orthographicSize/2)) {
			takeOff = false;
		}
	}

	// GAME OVER - pause game and Show view ad option to continue game
	void outOfFuel() {
		//pause game, save state

		// Show end game menu
		gameState.ChangeState ("Out Of Fuel");
	}
		
	// Returns vertical speed (used in moving objects down screen - allowing character to stay central)
	public int getVeticalSpeed() {
		return verticalSpeed;
	}

	// Called by CanvasManager when user is to begin landing
	public void beginLanding() {

	}

	// If user chooses to watch ad, then continue game after
	public void continueGame() {
		//reset fuel

	}

	// When the user has finished attempting to land, return to main menu
	void gameOver() {
		//save info

		// Fade into Menu screen
		gameState.ChangeState("End Game");
	}
}
