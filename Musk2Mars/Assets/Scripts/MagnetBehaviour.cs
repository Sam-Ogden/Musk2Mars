using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetBehaviour : MonoBehaviour {

	private PowerupController powerupController;

	// Use this for initialization
	void Start() {
		powerupController = PowerupController.controller;
	}
	
	// Update is called once per frame
	void Update() {
		
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if(coll.gameObject.CompareTag("Player")) {
			powerupController.SetMagnetOn();
			Destroy(gameObject);
		}
	}
}
