using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnBehaviour : MonoBehaviour {

	public GameStateController gameState;
	public float lowerOffset;	// Distance from bottom of screen to despawner
	public float upperOffset;
	private Camera cam;			// To store the main camera
	private float screenWidth;
	private float screenHeight;
	private bool top;
	private bool bot;

	// Use this for initialization
	void Start () {
		cam = Camera.main;	// Store for later use
		gameState = GameStateController.gameStateController;
		// Calculate screen sizes
		var screenBottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, transform.position.z));
		var screenTopRight = cam.ViewportToWorldPoint(new Vector3(1, 1, transform.position.z));
		screenWidth = screenTopRight.x - screenBottomLeft.x;
		screenHeight = screenTopRight.y - screenBottomLeft.y;

		positionDespawn(false, true, true);
	}
	
	// Update is called once per frame
	void Update () {
		if(gameState.isLanding() && !top) {
			positionDespawn(true, false, false);
		} else if(gameState.gameIsRunning() && !bot) {
			positionDespawn(false, true, false);
		}
	}

	// Uses parameters to position despawner below view
	void positionDespawn(bool newTop, bool newBot, bool scale) {
		if(newTop) {
			Vector3 topPosition = cam.transform.position;
			topPosition.y += (screenHeight / 2) + upperOffset;
			transform.position = topPosition;
		} else if(newBot) {
			Vector3 botPosition = cam.transform.position;
			botPosition.y -= (screenHeight / 2) + lowerOffset;
			transform.position = botPosition;
		}

		if(scale) {
			transform.localScale = new Vector2(screenWidth + 0.5f, 1);
		}
		
		top = newTop;
		bot = newBot;	
	}

	// Destroy things that get here
	void OnCollisionEnter2D(Collision2D obj) {
		// Most probably not relevant if it gets this low but can alter to be exclusive
		Destroy(obj.gameObject);
	}
}
