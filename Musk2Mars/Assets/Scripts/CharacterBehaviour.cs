using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBehaviour : MonoBehaviour {

	public float initialFuel;	//std fuel in rocket when start game
	private float fuel;
	public float moveSpeed;		//horizontal movement speed
	public float maxVelocityChange;
	// Use this for initialization
	void Start () {
		fuel = initialFuel;
	}
	
	// Update is called once per frame
	void Update () {
		//calculate how fast player should be moving
		Vector2 targetVelocityH = new Vector2(Input.acceleration.x, 0);
		targetVelocityH = transform.TransformDirection(targetVelocityH);
		targetVelocityH *= moveSpeed;

		// Apply a force that attempts to reach our target velocity
		Vector2 velocity = GetComponent<Rigidbody2D>().velocity;
		Vector2 velocityChange = (targetVelocityH - velocity);

		velocityChange.x = Mathf.Clamp(velocityChange.x,
							-maxVelocityChange, maxVelocityChange);
		GetComponent<Rigidbody2D>().AddForce(velocityChange,
										ForceMode2D.Force);
	}
}
