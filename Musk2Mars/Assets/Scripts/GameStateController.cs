/*
	Contolls game state and shows canvas elements depending on state
*/
/*
	- Landing state
		- if watched ad or clicked begin landing change state to landing 
	- In character Behaviour, need to know if running end game or landing
	- if landing
		- Check if score high enough for next level (mars etc) land on nearest level
		- Fall for 5s (Background animation)
		- Touch = boost up
		- If land slowly enough => success
		- Else blow up, restart from currently reached level
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
	public Slider fuelBar;
	public Text mainMenuHighScore;
	public static GameStateController gameStateController;
	public float initialFuel;		// Std fuel in rocket when start game
	public float maxFuel;
	public float initialLandingFuel;
	public GameObject ground;
	public int landDistance;
	private static float fuel;
	private static int coins = 0;
	private static double score = 0;
	private static string currState;
	private int frameCount;
	private DataControl data;
	private bool seenAd;
	private GameObject landingGround;
	private bool outOfLandingFuel;
	/*
        STATES:
         - Game Running    == remove all menus show HUD
         - First Death     == show continueMenu keep HUD
         - End Game        == Show GameMenu hide HUD
         - Video Ad        == Show ad only 
	*/ 	

	void Start() {
		#if UNITY_ADS
		if (Advertisement.isSupported) {
			Advertisement.Initialize("1582720", true);
		}
		#endif
		fuel = initialFuel;
		data = DataControl.control;
		outOfLandingFuel = false;
		showCanvasElements(false, false, true); // Set initial state manually to stop saveData
		updateMainMenuHighScore();
		seenAd = false;
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
		} else if(currState == "First Death") {
			if(seenAd) characterLand();
			else showCanvasElements(true, true, false);
		} else if(currState == "Landing") {
			showCanvasElements(false, true, false);
		} else if(currState == "End Game") {
			showCanvasElements(false, false, true);
			saveData();
			if(prevState != currState) {
				SceneManager.LoadScene ("MainGame");
			}
			score = 0;
			coins = 0;
			Destroy(gameObject);
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
		#if UNITY_ADS
		Advertisement.Show("rewardedVideo");
		#endif
		seenAd = true;
		//return to game
		characterContinue();
	}
	
	void characterContinue() {
		ChangeState ("Game Running");
		updateFuel(initialFuel);
	}
	// Returns to character bahaviour for the landing section of the game
	void characterLand() {
		ChangeState ("Landing");
		fuel = 0;
		updateFuel(initialLandingFuel);
		// spawn ground below camera
		Vector3 spawnLocation = new Vector3(0, Camera.main.transform.position.y-landDistance, -1);
		landingGround = Instantiate(ground, spawnLocation, Quaternion.identity);
		// Sets foregrounds tag so can detect landing
		landingGround.transform.Find("ForeGround").tag = "LandingGround";
	}

	// Update score once every 3 frames
	public void UpdateScore() {
		frameCount++;
		if((frameCount % 3) == 0) {
			score = System.Math.Round(score+0.1, 2);
		}
		displayScore.text = score+"";
	}

	public void updateFuel(float newFuel) {
		fuel = fuel + newFuel;
		if(fuel > maxFuel) {
			fuel = maxFuel;
		}
		fuelBar.value = fuel / maxFuel;
		if(fuel <= 0 && currState != "Landing") ChangeState("First Death");
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

	// Save score to data control
	public void saveData() {
		if(data.containsKey("HighScore")) {
			float currHighScore = System.Convert.ToSingle(data.getValue("HighScore"));
			if(score > currHighScore) {
				data.updateVal("HighScore", score.ToString());
			}
		} else {
			data.updateVal("HighScore", score.ToString());
		}

		if(data.containsKey("Coins")) {
			int currCoins = System.Int32.Parse(data.getValue("Coins"));
			int newCoins = currCoins + coins;
			data.updateVal("Coins", newCoins.ToString());
		} else {
			data.updateVal("Coins", coins.ToString());
		}
	}

	void updateMainMenuHighScore() {
		if(data.containsKey("HighScore")) {			
			mainMenuHighScore.text = "Best: "+ data.getValue("HighScore");
		} else {
			mainMenuHighScore.text = "Best: 0";
		}
	}

	public bool isLanding() {
		if(currState == "Landing") return true;
		return false;
	}

	public bool haveFuel() {
		if(fuel > 0) return true;
		return false;
	}
	// Returns the distance between clone floor and camera
	public float getCamFloorDistance() {
		return Camera.main.transform.position.y - landingGround.transform.position.y;
	}
}