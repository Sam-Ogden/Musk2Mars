using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupController : MonoBehaviour {

	public float stdMagnetTime;
	public static PowerupController controller;

	private DataControl data;
	private Func<object> unset;		// Stores the method to unset the last set powerup
	private bool isMagnet = false;	// Determines if the Magnet powerup is set
	private bool isActive = false;	// Determines if any powerups are set
	private float magnetTime;		// Stores the duration of the Magnet powerup
	private float timeTarget;		// Stores the time at which the set powerup should be unset

	void Awake() {
		controller = this;
	}

	void Start () {
		data = DataControl.control;

		// Attempt to retrieve the duration of the magnet powerup
		try {
			magnetTime = float.Parse(data.getValue("magnetTime"));
		} catch (ArgumentNullException) {
			data.updateVal("magnetTime", stdMagnetTime.ToString());
		}
	}
	
	void Update () {
		// If any powerups are active
		if(isActive) {
			// If the duration of the powerup is done
			if(Time.time > timeTarget) {
				// Call the method stored by whichever powerup setting method was called last
				unset();
			}
		}
	}

	// Methods to control magnet powerup activation
	public void SetMagnetOn() {
		if(!isActive) {
			isActive = true;
			isMagnet = true;
			timeTarget = Time.time + magnetTime; // Magnet effect will stop when this time reached
			unset = Utility.BuildMethod<object>(UnsetMagnet); // Will be called at timeTarget
		}
	}
	private object UnsetMagnet() {
		isActive = false;
		isMagnet = false;
		unset = null;
		return null;
	}

	public bool GetActive() {
		return isActive;
	}
}
