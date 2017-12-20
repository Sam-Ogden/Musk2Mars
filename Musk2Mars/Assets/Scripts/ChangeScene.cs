using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour {

	/* 	To make a button change scene, go to the on click() section in
		in inspector, choose main camera, then ChangeScene.changeScene as the funtion.
		Set the final value to the scene name you want it to swap to 
	*/
	
	public void changeScene(string sceneName) {
		SceneManager.LoadScene (sceneName );
	}
}
