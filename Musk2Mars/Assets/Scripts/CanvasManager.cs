using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour {


	public GameObject continuePlaying;	//UI Button that plays an ad so user can continue playing
	public GameObject goToMainMenu;		//UI Button Return to main menu when game is over.

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//Hides or shows the end game menu. Used when user runs out of fuel in CharacterBehaviour.
	public void menuActive(bool showMenu) {
		goToMainMenu.SetActive (showMenu);
		continuePlaying.SetActive (showMenu);
	}

	//When user clicks continuePlaying button, a video ad is played then the game continues
	void showVideoAd() {

	}
}
