using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBehaviour : MonoBehaviour {

	public float initialFuel;	//std fuel in rocket when start game
	public float moveSpeed;		//horizontal movement speed
	private float fuel;
	// Use this for initialization
	void Start () {
		fuel = initialFuel;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
