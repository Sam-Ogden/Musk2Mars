using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

	public GameObject player;
	private float forceY;
	private float forceX;
	private Transform playerTransform;
	private float screenHeight;
	private float screenWidth;

	// Use this for initialization
	void Start () {
		playerTransform = player.GetComponent<Transform>();
		forceY = 0;
		forceX = 0;

		var screenBottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, transform.position.z));
		var screenTopRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, transform.position.z));
		
		screenWidth = screenTopRight.x - screenBottomLeft.x;
		screenHeight = screenTopRight.y - screenBottomLeft.y;
		//Debug.Log(screenHeight);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate (){
		// Trying to keep rocket height, always same proportion of screen
		if(playerTransform.position.y > Camera.main.transform.position.y - screenHeight/4) {
			// Divide for smooth movement
			Debug.Log("Moveing Cam");
			forceY = Mathf.Abs(Camera.main.transform.position.y - (screenHeight/4) - playerTransform.position.y)/2;
		}

		Vector3 moveCam = new Vector3(forceX, forceY, 0);
		this.transform.position += moveCam;

		getInput();	// Only used for testing;
	}

	void getInput() {
		// Nothing for now
	}
}
