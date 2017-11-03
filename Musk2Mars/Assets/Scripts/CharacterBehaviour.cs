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

	private float fuel;	// Ship's current level of fuel ⛽️
	private DataControl data;
	public string inputMode;	// Type of input (tilt, tap, keyboard-for testing)

	// Use this for initialization
	void Start () {
		data = DataControl.control;
		fuel = initialFuel;	// Always start with standard fuel level
		if(data.containsKey("inputMethod")) {
			inputMode = data.getValue("inputMethod"); // GET INPUT MODE FROM DATA STORE
		} else {
			data.addPair("inputMethod", "tilt");
			inputMode = "tilt";
		}

		inputMode = "TEST"; /* REMOVE ME */

	}


	// Update is called once per frame
	void Update () {
		fuel--;
		// If no more fuel, game is over.
		if (fuel == 0) {
			outOfFuel ();
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

	// GAME OVER - pause game and Show view ad option to continue game
	void outOfFuel() {
		//pause game, save state

		// Show menu
		if(canvas) canvas.menuActive (true);
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
