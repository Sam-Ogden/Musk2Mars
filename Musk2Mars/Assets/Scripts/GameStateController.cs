/*
	Contolls game state and shows canvas elements depending on state
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_ADS
using UnityEngine.Advertisements; // only compile Ads code on supported platforms
#endif

public class GameStateController : MonoBehaviour {

	public GameObject gameMenu;
	public GameObject continueMenu;
	public GameObject HUD;
	public Text displayScore;
	public static GameStateController gameStateController;
	public int initialFuel;		// Std fuel in rocket when start game

	private static int fuel;
	private static int coins = 0;
	private static double score = 0;
	private static string currState;
	private int frameCount;

	/*
        STATES:
         - Game Running    == remove all menus show HUD
         - Out of fuel     == show continueMenu keep HUD
         - End Game        == Show GameMenu hide HUD
         - Video Ad        == Show ad only 
	*/ 

	void Start() {
		if (Advertisement.isSupported) {
			Advertisement.Initialize("1582720", true);
		}
		fuel = initialFuel;
	}

	public void StartGame() {
		ChangeState("Game Running");
	}

	void Awake() { 
		DontDestroyOnLoad (gameObject);
		gameStateController = this;
		if (gameStateController == null) {
			DontDestroyOnLoad (gameObject);
			gameStateController = this;
		} else if (gameStateController != this) {
			Destroy (gameObject);
		}
	}

	public void ChangeState(string state) {
		string prevState = currState;
		currState = state;
		if(currState == "Game Running") {
			showCanvasElements(false, true, false);
		} else if(currState == "Out Of Fuel") {
			showCanvasElements(true, true, false);
		} else if(currState == "End Game") {
			showCanvasElements(false, false, true);
			saveData();
			if(prevState != currState) {
				SceneManager.LoadScene ("MainGame");
			}
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
		Advertisement.Show("rewardedVideo");
		//return to game
		characterContinue();
	}

	void characterContinue() {
		ChangeState ("Game Running");
		fuel = initialFuel;
	}
	// Returns to character bahaviour for the landing section of the game
	void characterLand() {
		ChangeState ("End Game");
	}

	public void UpdateScore() {
		frameCount++;
		if((frameCount % 3) == 0) {
			score = System.Math.Round(score+0.1, 2);
		}
		displayScore.text = score+"";
	}

	public void updateFuel(int newFuel) {
		fuel = fuel + newFuel;
		if(fuel <= 0) ChangeState("Out Of Fuel");
	}

	public void updateCoins(int newCoins) {
		coins = coins + newCoins;
	}

	public string GetState() {
		return currState;
	}

	public bool gameIsRunning() {
		if(currState == "Game Running") return true;
		else return false;
	}

	void saveData() {

	}
}
