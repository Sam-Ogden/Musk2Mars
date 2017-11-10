using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterBehaviour : MonoBehaviour {

	public float initialFuel;		// Std fuel in rocket when start game
	public float moveSpeed;			// Horizontal movement speed
	public float maxVelocityChange;	// The max increase in speed at one step (aka accelleration...)
	public CanvasManager canvas;	// Canvas manager to show end game options and ads
	public string inputMode;	// Type of input (tilt, tap, keyboard-for testing)
	public float takeOffForce;

	private float fuel;	// Ship's current level of fuel ⛽️
	private DataControl data;
	private int verticalSpeed;
	private int coinsCollected;
	private bool start;

	// Use this for initialization
	void Start () {
		data = DataControl.control;
		fuel = initialFuel;	// Always start with standard fuel level
		verticalSpeed = 0;
		coinsCollected = 0;

		if(data.containsKey("inputMethod")) {
			inputMode = data.getValue("inputMethod"); // GET INPUT MODE FROM DATA STORE
		} else {
			data.addPair("inputMethod", "tilt");
			inputMode = "tilt";
		}

		inputMode = "TEST"; /* REMOVE ME */
		start = true;
	}


	// Update is called once per frame
	void Update () {
		//fuel--;
		// If no more fuel, game is over.
		if (fuel == 0) {
			outOfFuel ();
		}

		if(start) {
			TakeOff();
		}
			
		Vector2 velocityChange;
		// Calculate how fast player should be moving 🚀
		if(inputMode == "tilt") {
			// Input taking when tilting
			Vector2 targetVelocityH = new Vector2(Input.acceleration.x, 0);
			targetVelocityH = transform.TransformDirection(targetVelocityH);
			targetVelocityH *= moveSpeed;

			// Apply a force that attempts to reach our target velocity
			Vector2 velocity = GetComponent<Rigidbody2D>().velocity;
			velocityChange = (targetVelocityH - velocity);
			// Make sure change not too sudden
			velocityChange.x = Mathf.Clamp(velocityChange.x,
							-maxVelocityChange, maxVelocityChange);
		} else if(inputMode == "touch") {
			// Input taking when using screen sides
			velocityChange = new Vector2();
		} else {
			// Test input taking (arrow keys or WASD)
			Vector2 targetVelocityH = new Vector2(Input.GetAxis("Horizontal"), 0);
			targetVelocityH = transform.TransformDirection(targetVelocityH);
			targetVelocityH *= moveSpeed;

			// Apply a force that attempts to reach our target velocity
			Vector2 velocity = GetComponent<Rigidbody2D>().velocity;
			velocityChange = (targetVelocityH - velocity);
			// Make sure change not too sudden
			velocityChange.x = Mathf.Clamp(velocityChange.x,
							-maxVelocityChange, maxVelocityChange);
		}

		GetComponent<Rigidbody2D>().AddForce(velocityChange,
										ForceMode2D.Force);
	}
		
	// Collision handler
	void OnCollisionEnter2D(Collision2D obj) {
		if (obj.gameObject.CompareTag ("Coin")) {
			/*
                NEED TO DELETE GAME OBJECT
			*/
			obj.gameObject.SetActive(false); //Coin collected remove from game
			// Play satisfying hit coin sound
			// Coint coins
			coinsCollected++;
		} else if(obj.gameObject.CompareTag ("Fuel")) {
			obj.gameObject.SetActive(false);
			fuel++;
		}
	}

	void TakeOff() {
		Rigidbody2D rb = GetComponent<Rigidbody2D>();
		rb.AddForce(new Vector2(0, takeOffForce), ForceMode2D.Impulse);
		// Gets bottom of screen based off camera size and puts character
		// quarter of the way up
		if(transform.position.y > (-Camera.main.orthographicSize/2)) {
			start = false;
		}
	}
	// GAME OVER - pause game and Show view ad option to continue game
	void outOfFuel() {
		//pause game, save state

		// Show menu
		if(canvas) canvas.menuActive (true);
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
		SceneManager.LoadScene ("Menu");
	}
}
