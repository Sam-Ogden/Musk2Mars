/*
	Contolls the canvas in main game scene
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CanvasManager : MonoBehaviour {


	public GameObject continuePlaying;	// UI Button that plays an ad so user can continue playing
	public GameObject goToLanding;		// UI Button Return to game to land.
	public Text displayScore;
	public CharacterBehaviour character;

	// Use this for initialization
	void Start () {
		menuActive (false);
	}

	// Hides or shows the out of fuel menu. Used when user runs out of fuel in CharacterBehaviour.
	public void menuActive(bool showMenu) {
		goToLanding.SetActive (showMenu);
		continuePlaying.SetActive (showMenu);
	}

	// When user clicks continuePlaying button, a video ad is played then the game continues (Landing)
	public void showVideoAd() {
		menuActive (false);

		//return to game
		character.continueGame();	
	}

	// Returns to character bahaviour for the landing section of the game
	public void characterLand() {
		menuActive (false);
		character.beginLanding ();
	}

	public void UpdateScore(double score) {
		displayScore.text = score + " km";
	}
}
