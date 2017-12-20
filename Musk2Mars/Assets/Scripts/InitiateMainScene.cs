using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitiateMainScene : MonoBehaviour {

	// Use this for initialization
	public GameObject MainCamera;
	public GameObject GSC;
	public GameObject DataControl;
	public GameObject EventSystem;
	public GameObject ContinueMenu;
	public GameObject GameMenu;
	public GameObject HUD;
	public GameObject Scoremenu;
	public GameObject Sounds;
	private GameStateController gameState;

	void Start () {
		Vector3 id = new Vector3(0, 0, 0);
		Instantiate(GSC, id, Quaternion.identity).name = "GameStateController";
		Instantiate(MainCamera, id, Quaternion.identity).name = "MainCamera";
		Instantiate(DataControl, id, Quaternion.identity).name = "DataControl";
		Instantiate(ContinueMenu, id, Quaternion.identity).name = "ContinueMenu";
		Instantiate(GameMenu, id, Quaternion.identity).name = "GameMenu";
		Instantiate(HUD, id, Quaternion.identity).name = "HUD";
		Instantiate(Scoremenu, id, Quaternion.identity).name = "Scoremenu";
		Instantiate(Sounds, id, Quaternion.identity).name = "Sounds";
		Instantiate(EventSystem, id, Quaternion.identity).name = "EventSystem";

		gameState = GameStateController.gameStateController;
		// Binding game objects
		gameState.gameMenu = GameObject.Find("GameMenu");
		gameState.continueMenu = GameObject.Find("ContinueMenu");
		gameState.continueMenu.SetActive(false);
		gameState.HUD = GameObject.Find("HUD");
		gameState.HUD.SetActive(false);
		gameState.displayScore = HUD.transform.Find("Score").GetComponent<Text>();
		gameState.displayCoins = gameState.gameMenu.transform.Find(
									"Coins").Find("CoinCount").GetComponent<Text>();
		gameState.displayCoinsHUD = HUD.transform.Find("Coins").Find(
									"HUDCoinCount").GetComponent<Text>();
		gameState.mainMenuHighScore = gameState.gameMenu.transform.Find(
									"HighScore").GetComponent<Text>();
		gameState.fuelBar = HUD.transform.Find("FuelBar").GetComponent<Slider>();
		gameState.backgroundMusic = GameObject.Find("Sounds").transform.Find(
									"Musk-To-Marz").GetComponent<AudioSource>();
		gameState.hitFuelSound = GameObject.Find("Sounds").transform.Find(
									"Get_2_v2_amp").GetComponent<AudioSource>();
		gameState.hitCoinSound = GameObject.Find("Sounds").transform.Find(
									"coin_free").GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
