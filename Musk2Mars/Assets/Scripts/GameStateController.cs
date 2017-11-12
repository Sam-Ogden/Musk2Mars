/*
	Contolls game state and shows canvas elements depending on state
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameStateController : MonoBehaviour {

	public GameObject gameMenu;
	public GameObject continueMenu;
	public GameObject HUD;
	public Text displayScore;
	public CharacterBehaviour character;
	public static GameStateController gameStateController;

	private string currState;
	/*
        STATES:
         - Game Running    == remove all menus show HUD
         - Out of fuel     == show continueMenu keep HUD
         - End Game        == Show GameMenu hide HUD
         - Video Ad        == Show ad only 
	*/ 

	void Awake() { 
		DontDestroyOnLoad (gameObject);
		gameStateController = this;
	}

	public void ChangeState(string state) {
		currState = state;
		if(currState == "Game Running") {
			showCanvasElements(false, true, false);
		} else if(currState == "Out Of Fuel") {
			showCanvasElements(true, true, false);
		} else if(currState == "End Game") {
			showCanvasElements(false, false, true);
		} else if (currState == "Video Ad") {
			showCanvasElements(false, false, false);
		}
	}
		
	void showCanvasElements(bool showcontinueMenu, bool showHUD, bool showGameMenu) {
		continueMenu.SetActive(showcontinueMenu);
		HUD.SetActive(showHUD);
		gameMenu.SetActive (showGameMenu);
	}

	// When user clicks continuePlaying button, a video ad is played then the game continues (Landing)
	public void showVideoAd() {
		ChangeState ("Video Ad");
		//return to game
		ChangeState("Game Running");
		character.continueGame();
	}

	// Returns to character bahaviour for the landing section of the game
	public void characterLand() {
		ChangeState ("Game Running");
		character.beginLanding ();
	}

	public void UpdateScore(double score) {
		displayScore.text = score+"";
	}

	public string GetState() {
		return currState;
	}

	public bool gameIsRunning() {
		if(currState == "Game Running") return true;
		else return false;
	}
}
