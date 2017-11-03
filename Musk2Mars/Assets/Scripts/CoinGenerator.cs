using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
  Camera controls placing of coins and patterns
*/

public class CoinGenerator : MonoBehaviour {

	public CharacterBehaviour character;
	public Transform coinPrefab;

	private int speed;
	private Transform[] coins;

	void Start() {
		//Generate initial coins
		coins = new Transform[10];
		for(int i = 0; i < 10; i++) {
			coins[i] = generateCoin(new Vector3(0, i-3));
		}
	}

	// Get speed for character, move all coins down at that rate.
	void Update() {
		speed = character.getVeticalSpeed();

		//move coins down

		//remove coins that are below bottom of screen
		checkOfScreen();

		//generateCoin(); 
	}

	// Removes any coin whos y val is less than bottom of screen
	void checkOfScreen() {
		//remove coin from scene
		for(int i = 0; i < coins.Length; i++) {
			if(coins[i].transform.position.y < 0) {
				//coin of screen, remove it
			}
		}
	}

	// Generates new coin off top of screen
	Transform generateCoin(Vector3 position) {
		return Instantiate(coinPrefab, position, Quaternion.identity) as Transform;
	}

	/*
        Patterns: Create function for each possible coin patter
	*/
}
