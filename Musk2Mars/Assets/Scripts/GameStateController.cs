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
	public GameObject ground;
	public GameObject landingPad;
	public GameObject ship;
	public Text displayScore;
	public Text displayCoins;
	public Text displayCoinsHUD;	
	public Text mainMenuHighScore;
	public Slider fuelBar;
	public static GameStateController gameStateController;
	public float initialFuel;		// Std fuel in rocket when start game
	public float maxFuel;
	public float initialLandingFuel;
	public float groundHeight;
	public float groundCenterOffset;
	public float padOffset;
	public int landDistance;
	public AudioSource backgroundMusic;
	public AudioSource hitFuelSound;
	public AudioSource hitCoinSound;
	
	private static float fuel;
	private static int coins = 0;
	private static double score = 0;
	private static string currState;
	private DataControl data;
	private GameObject landingGround;
	private bool seenAd;
	private bool successfullyLanded;
	private int frameCount;
	private float screenHeight;
 	//private float screenWidth;
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
		showCanvasElements(false, false, true); // Set initial state manually to stop saveData
		updateMainMenuHighScore();
		displayCoins.text = data.getValue("Coins");
		successfullyLanded = false;
		seenAd = false;

		var screenBottomLeft = Camera.main.ViewportToWorldPoint(
			 new Vector3(0, 0, transform.position.z));
 		var screenTopRight = Camera.main.ViewportToWorldPoint(
			 new Vector3(1, 1, transform.position.z));
		// screenWidth = screenTopRight.x - screenBottomLeft.x;
 		screenHeight = screenTopRight.y - screenBottomLeft.y;
		Instantiate(ship, new Vector3(
			Camera.main.transform.position.x,
			Camera.main.transform.position.y - (screenHeight/2) -groundCenterOffset + groundHeight,
			0
		), Quaternion.identity).name = "Player"; // Instantiate ship and give it standard name
		Instantiate(ground, new Vector3(
			Camera.main.transform.position.x,
			Camera.main.transform.position.y - (screenHeight/2) - groundCenterOffset,
			0
		),Quaternion.identity).name = "Ground";
		Instantiate(landingPad, new Vector3(
			Camera.main.transform.position.x,
			Camera.main.transform.position.y - (screenHeight/2) - groundCenterOffset + padOffset,
			0
		), Quaternion.identity);
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
			backgroundMusic.Play();
		} else if(currState == "First Death") {
			if(seenAd) characterLand();
			else showCanvasElements(true, true, false);
		} else if(currState == "Landing") {
			showCanvasElements(false, true, false);
			backgroundMusic.Play();		
		} else if(currState == "End Game") {
			showCanvasElements(false, false, true);
			saveData();
			if(prevState != currState) {
				SceneManager.LoadScene ("MainGame");
			}
			score = 0;
			coins = 0;
			Destroy(gameObject);
			backgroundMusic.Stop();		
		} else if (currState == "Video Ad") {
			showCanvasElements(false, false, false);
			backgroundMusic.Stop();				
		}
	}
		
	void showCanvasElements(bool showcontinueMenu, bool showHUD, bool showGameMenu) {
		continueMenu.SetActive(showcontinueMenu);
		HUD.SetActive(showHUD);
		gameMenu.SetActive (showGameMenu);
	}

	// When user clicks continuePlaying button, a video ad is played then the game continues (Landing)
	public void showVideoAd() {
		backgroundMusic.Stop();				
		#if UNITY_ADS
		if (Advertisement.IsReady("rewardedVideo")) {
			var options = new ShowOptions { resultCallback = HandleShowAdResult };
			Advertisement.Show("rewardedVideo", options);
		}
		#endif
	}

	// Callback function for when unity finishes showing ad in showVideoAd
	private void HandleShowAdResult(ShowResult result) {
		switch (result)
		{
			case ShowResult.Finished:
			Debug.Log("The ad was successfully shown.");
			characterContinue();
			seenAd = true;
			break;
			case ShowResult.Skipped:
			Debug.Log("The ad was skipped before reaching the end.");
			characterContinue();
			break;
			case ShowResult.Failed:
			Debug.LogError("The ad failed to be shown.");
			characterContinue();
			break;
		}
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
		// Spawn ground below camera
		float yPos = Camera.main.transform.position.y - landDistance;
		Vector3 groundLoc = new Vector3(0, yPos, -1);
		Vector3 padLoc = Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(0,Screen.width),0,0));
		padLoc.y = yPos + padOffset;
		padLoc.z = -1;
		landingGround = Instantiate(ground, groundLoc, Quaternion.identity);
		landingGround.tag = "LandingGround";
		Instantiate(landingPad, padLoc, Quaternion.identity).tag = "LandingPad";	
	}

	// Update score once every 3 frames
	public void updateScore() {
		frameCount++;
		if((frameCount % 3) == 0) {
			score = System.Math.Round(score + 0.1, 2);
		}
		displayScore.text = System.Math.Round(score) + "";
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
		displayCoinsHUD.text = "" + coins;
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
		return Camera.main.transform.position.y - landingGround.transform.position.y - groundCenterOffset;
	}

	// Successful landing, reward player double or 1.5x coins
	public void successfulLanding(float coinMultiplier) {	
		// Make sure only multiple coins once (each ship clone calls this method)
		if(!successfullyLanded) {
			coins = (int) Mathf.Round(coins * coinMultiplier);
		}	
		successfullyLanded = true;
	}

	public void PlayHitFuelSound() {
		hitFuelSound.Play();
	}

	public void PlayHitCoinSound() {
		hitCoinSound.Play();
	}
}