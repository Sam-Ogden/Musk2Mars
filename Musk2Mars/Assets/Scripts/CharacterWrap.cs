using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWrap : MonoBehaviour {

	private float screenWidth;	// Used for calculating if player slipping off screen		
	private float screenHeight;	// Idem
	private Transform[] ghosts = new Transform[2];
	public Transform ghost;	// Rocket prefab for creating ghosts
	private bool isVisible;
	private Renderer[] renderers;

	// Use this for initialization
	void Start () {
		var cam = Camera.main;	// Store the main game camera to get sizes based on device
		var screenBottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, transform.position.z));
		var screenTopRight = cam.ViewportToWorldPoint(new Vector3(1, 1, transform.position.z));
		
		renderers = GetComponentsInChildren<Renderer>();
		screenWidth = screenTopRight.x - screenBottomLeft.x;
		CreateGhostShips();
		PositionGhostShips();
	}
	
	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate() {
		isVisible = CheckRenderers();	//check if any renderers are visible
		if(!isVisible) {
			SwapShips();
		}
	}

	void CreateGhostShips() {
		for(int i = 0; i < 2; i++) {
			ghosts[i] = Instantiate(ghost, Vector3.zero, Quaternion.identity) as Transform;
			Destroy(ghosts[i].GetComponent<CharacterWrap>());
		}
	}

	void PositionGhostShips() {
		// All ghost positions will be relative to the ships (this) transform,
		// so let's star with that.
		var ghostPosition = transform.position;
	
		// We're positioning the ghosts clockwise behind the edges of the screen.
		// Let's start with the far right.
		ghostPosition.x = transform.position.x + screenWidth;
		ghostPosition.y = transform.position.y;
		ghosts[0].position = ghostPosition;
	
		// Far left
		ghostPosition.x = transform.position.x - screenWidth;
		ghostPosition.y = transform.position.y;
		ghosts[1].position = ghostPosition;
	
		// All ghost ships should have the same rotation as the main ship
		for(int i = 0; i < 2; i++) {
			ghosts[i].rotation = transform.rotation;
		}
	}

	void SwapShips() {
		// Reposition main ship in the new position
		foreach(var ghost in ghosts) {
			if (ghost.position.x < screenWidth && ghost.position.x > -screenWidth) {
				transform.position = ghost.position;	// Move main ship to the visible position
				break;
			}
		}
		// Resposition ghosts on sides of main ship
		PositionGhostShips();
	}

	bool CheckRenderers() {
		foreach(var renderer in renderers) {
			// If at least one render is visible, return true
			if(renderer.isVisible) {
				return true;
			}
		}
		// Otherwise, the object is invisible
		return false;
	}
}
