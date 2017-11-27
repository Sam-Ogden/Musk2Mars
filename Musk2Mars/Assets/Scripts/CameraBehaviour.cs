using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour {
	
	public GameObject player;
 	private float forceY;
 	private float forceX;
 	private Transform playerTransform;
	public Color[] colors;
	public float colorDuration = 100F;
	public float landingColorDuration = 100F;
	
 	private float screenHeight;
 	//private float screenWidth;
	private GameStateController gameState;

	private float colorTime = 0;
	private float landingColorTime = 0;	
	private int color1 = 0;
	private int color2 = 1;

    Camera cam;

	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
		cam.backgroundColor = colors[0];

		gameState = GameStateController.gameStateController;
		playerTransform = player.GetComponent<Transform>();
 		forceY = 0;
 		forceX = 0;
		
		// Calculate screen dimensions
 		var screenBottomLeft = Camera.main.ViewportToWorldPoint(
			 new Vector3(0, 0, transform.position.z));
 		var screenTopRight = Camera.main.ViewportToWorldPoint(
			 new Vector3(1, 1, transform.position.z));
 		
 		//screenWidth = screenTopRight.x - screenBottomLeft.x;
 		screenHeight = screenTopRight.y - screenBottomLeft.y;
	}
	
	// Update is called once per frame
	void Update () {
		if(gameState.gameIsRunning()) {
			colorTime = colorTime + 1/colorDuration;
			cam.backgroundColor = Color.Lerp(colors[color1], colors[color2], colorTime);
			if(colorTime > 1 && color2 != colors.Length-1) {
				colorTime = 0;
				color1++;
				color2++;
			}
		} else if(gameState.isLanding()) {
			landingColorTime = landingColorTime + 1/landingColorDuration;
			cam.backgroundColor = Color.Lerp(colors[color2], colors[0], landingColorTime);
		}
	}

	// Fixed Update is used for physics stuff
	void FixedUpdate (){
 		// Trying to keep rocket height, always same proportion of screen
		if(gameState.isLanding()) {
			//check if ground on screen (difference = 5)
			if(gameState.getCamFloorDistance() > Camera.main.orthographicSize) {
				if(playerTransform.position.y < Camera.main.transform.position.y + screenHeight/4) {
					// Divide for smooth movement
					forceY = -Mathf.Abs(Camera.main.transform.position.y + 
								(screenHeight/4) - playerTransform.position.y);
				} else {
					forceY = Mathf.Abs(Camera.main.transform.position.y + 
								(screenHeight/4) - playerTransform.position.y);
				}
			} else {
				forceY = 0;
			}
		} else {
			if(playerTransform.position.y > Camera.main.transform.position.y - screenHeight/4) {
				// Divide for smooth movement
				forceY = Mathf.Abs(Camera.main.transform.position.y - 
							(screenHeight/4) - playerTransform.position.y);
			} else {
				forceY = 0;
			}
		}
 
 		Vector3 moveCam = new Vector3(forceX, forceY, 0);
 		this.transform.position += moveCam;
 	}
}
