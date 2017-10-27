using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterBehaviour : MonoBehaviour {

	public float initialFuel;			//std fuel in rocket when start game
	public float moveSpeed;				//horizontal movement speed
	public float maxVelocityChange;
	public CanvasManager canvas;		//Canvas manager to show end game options and ads

	private float fuel;
	private DataControl data = DataControl.control;
	public string inputMode;

	// Use this for initialization
	void Start () {
		fuel = initialFuel;
		data.load ();
		if(data.containsKey("inputMethod")) {
			inputMode = data.getValue("inputMethod"); //GET INPUT MODE FROM DATA STORE
		} else {
			data.addPair("inputMethod", "tilt");
			inputMode = "tilt";
		}
	}


	// Update is called once per frame
	void Update () {
		Debug.Log(inputMode);
		// If no more fuel, game is over.
		if (fuel == 0) {
			outOfFuel ();
		}

		Vector2 velocityChange;
		//calculate how fast player should be moving
		if(inputMode == "tilt") {
			//input taking when tilting
			Vector2 targetVelocityH = new Vector2(Input.acceleration.x, 0);
			targetVelocityH = transform.TransformDirection(targetVelocityH);
			targetVelocityH *= moveSpeed;

			// Apply a force that attempts to reach our target velocity
			Vector2 velocity = GetComponent<Rigidbody2D>().velocity;
			velocityChange = (targetVelocityH - velocity);
			//make sure change not too sudden
			velocityChange.x = Mathf.Clamp(velocityChange.x,
							-maxVelocityChange, maxVelocityChange);
		} else if(inputMode == "touch") {
			//input taking when using screen sides
			velocityChange = new Vector2();
		} else {
			//test input taking (arrow keys or WASD)
			Vector2 targetVelocityH = new Vector2(Input.GetAxis("Horizontal"), 0);
			targetVelocityH = transform.TransformDirection(targetVelocityH);
			targetVelocityH *= moveSpeed;

			// Apply a force that attempts to reach our target velocity
			Vector2 velocity = GetComponent<Rigidbody2D>().velocity;
			velocityChange = (targetVelocityH - velocity);
			//make sure change not too sudden
			velocityChange.x = Mathf.Clamp(velocityChange.x,
							-maxVelocityChange, maxVelocityChange);
		}

		GetComponent<Rigidbody2D>().AddForce(velocityChange,
										ForceMode2D.Force);
	}

	// GAME OVER - pause game and Show view ad option to continue game
	void outOfFuel() {
		//pause game, save state

		//show menu
		canvas.menuActive (true);
	}

	//called by CanvasManager when user is to begin landing
	public void beginLanding() {

	}

	//If user chooses to watch ad, then continue game after
	public void continueGame() {
		//reset fuel

	}

	//When the user has finished attempting to land, return to main menu
	void gameOver() {
		//save info

		//fade into Menu screen
		SceneManager.LoadScene ("Menu");
	}
}
