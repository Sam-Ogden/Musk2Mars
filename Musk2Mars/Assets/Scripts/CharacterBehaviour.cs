using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterBehaviour : MonoBehaviour {

	public float moveSpeed;			// Horizontal movement speed
	public float flySpeed;
	public float maxVelocityChange;	// The max increase in speed at one step (aka accelleration...)
	public float maxFlyChange;
	public string inputMode;	// Type of input (tilt, tap, keyboard-for testing)
	public float takeOffForce;  // Force added to character for take off
	public float touchForce;    // Force used on touch input
	public float fuelPackValue;   // Amount of fuel gained from a fuel pack

	private DataControl data;
	private GameStateController gameState;
	private int verticalSpeed;
	private bool takeOff;

	// Use this for initialization
	void Start () {
		data = DataControl.control;
		gameState = GameStateController.gameStateController;

		if(data.containsKey("inputMethod")) {
			inputMode = data.getValue("inputMethod"); // GET INPUT MODE FROM DATA STORE
		} else{
			data.updateVal("inputMethod", "tilt");
			inputMode = "tilt";
		}

		if(inputMode == "touch") {
			GetComponent<Rigidbody2D>().mass = 0.5f;
		} else {
			GetComponent<Rigidbody2D>().mass = 0.5f; 
		}
		// REMOVE BEFORE PUSHING
		//inputMode = "test";	// Left like this until testing time
		takeOff = true;
	}
		
	// Update is called once per frame
	void Update () {
		// Take off animation
		if(takeOff && gameState.gameIsRunning()) {
			TakeOff();
		}
			
		if(gameState.gameIsRunning()) {
			gameState.UpdateScore();
			gameState.updateFuel(-1f);
		}
	}

	// Physiscs go here
	void FixedUpdate() {
		if(gameState.gameIsRunning()) {
			gameObject.GetComponent<Rigidbody2D>().gravityScale = -4;
			MoveCharacter();
		} else if(gameState.GetState() == "First Death") {
			outOfFuel();
		} else if(gameState.isLanding()) {
			Landing();
		}
	}

	void MoveCharacter() {
		Vector2 velocityChange = new Vector2(0,0);
		// Calculate how fast player should be moving 🚀
		if(inputMode == "tilt") {
			// Input taking when tilting
			velocityChange = MovementForce(Input.acceleration.x * 1.5f, 0);
		} else if(inputMode == "touch") {
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
			gameState.updateCoins(1);
		} else if(obj.gameObject.CompareTag ("Fuel")) {
			Destroy(obj.gameObject);
			gameState.updateFuel(150f);
		} else if(obj.gameObject.CompareTag ("Obstacle")) {
			gameState.ChangeState("First Death");
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
		GetComponent<Rigidbody2D>().gravityScale = 0;
		GetComponent<Rigidbody2D>().AddForce(new Vector2(0,0), ForceMode2D.Impulse);		// Show end game menu
	}
		
	// Called by CanvasManager when user is to begin landing
	public void Landing() {
		GetComponent<Rigidbody2D>().gravityScale = 1;
		MoveCharacter();	
	}

	public void continueGame() {
		gameObject.GetComponent<Rigidbody2D>().gravityScale = -4;
	}
}
