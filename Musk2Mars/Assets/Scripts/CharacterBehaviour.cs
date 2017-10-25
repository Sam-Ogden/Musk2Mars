using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBehaviour : MonoBehaviour {

	public float initialFuel;			//std fuel in rocket when start game
	public float moveSpeed;				//horizontal movement speed
	public float maxVelocityChange;
	public CanvasManager canvas;		//Canvas manager to show end game options and ads

	public string inputMode;

	private float fuel;

	// Use this for initialization
	void Start () {
		fuel = initialFuel;
		canvas.menuActive (false);
		
	}
	
	// Update is called once per frame
	void Update () {
		
		// REMOVE ************************
		fuel--;


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
		canvas.menuActive (true);
	}
}
