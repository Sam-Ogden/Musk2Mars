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

	public void menuActive(bool showMenu) {
		goToMainMenu.SetActive (showMenu);
		continuePlaying.SetActive (showMenu);
	}
}
