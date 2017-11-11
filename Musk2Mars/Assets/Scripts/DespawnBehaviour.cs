using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnBehaviour : MonoBehaviour {

	public float lowerOffset;	// Distance from bottom of screen to despawner
	public Camera cam;			// To store the main camera
	private float screenWidth;
	private float screenHeight;

	// Use this for initialization
	void Start () {
		cam = Camera.main;	// Store for later use

		// Calculate screen sizes
		var screenBottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, transform.position.z));
		var screenTopRight = cam.ViewportToWorldPoint(new Vector3(1, 1, transform.position.z));
		screenWidth = screenTopRight.x - screenBottomLeft.x;
		screenHeight = screenTopRight.y - screenBottomLeft.y;

		positionDespawn();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// Uses parameters to position despawner below view
	void positionDespawn() {
		Vector3 positionChange = new Vector3();
		positionChange.y = (screenHeight / 2) + lowerOffset;
		transform.position -= positionChange;

		// Scale slightly larger than screen to be safe
		transform.localScale = new Vector2(screenWidth + 0.5f, 1);
	}

	// Destroy things that get here
	void OnCollisionEnter2D(Collision2D obj) {
		// Most probably not relevant if it gets this low but can alter to be exclusive
		Destroy(obj.gameObject);
	}
}
