using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnBehaviour : MonoBehaviour {

	public float lowerOffset;
	public Camera cam;
	private float screenWidth;
	private float screenHeight;

	// Use this for initialization
	void Start () {
		cam = Camera.main;
		var screenBottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, transform.position.z));
		var screenTopRight = cam.ViewportToWorldPoint(new Vector3(1, 1, transform.position.z));
		screenWidth = screenTopRight.x - screenBottomLeft.x;
		screenHeight = screenTopRight.y - screenBottomLeft.y;

		positionDespawn();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void positionDespawn() {
		Vector3 positionChange = new Vector3();

		positionChange.y = (screenHeight / 2) + lowerOffset;
		transform.position -= positionChange;

		transform.localScale = new Vector2(screenWidth + 0.5f, 1);
	}

	void OnCollisionEnter2D(Collision2D obj) {
		Destroy(obj.gameObject);
	}
}
