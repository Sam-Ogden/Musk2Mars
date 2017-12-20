/*
	Controls game data and options. See youtube video of whats going on.
	Saves data in a file and loads again so data can be saved between sessions.
	https://unity3d.com/learn/tutorials/topics/scripting/persistence-saving-and-loading-data

	- create empty game object DataControl, add this script to it. Create a prefab from the game object
	- add prefab to each scene that needs to access data.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class DataControl : MonoBehaviour {

	public static DataControl control;

	//Key value pairs of info to store
	private Dictionary<string, string> data = new Dictionary<string, string>();

	public bool containsKey(string k) {
		if (data.ContainsKey (k)) return true;
		return false;
	}

	// Updates a value or adds it if it doesnt exist
	public void updateVal(string k, string v) {
		if(data.ContainsKey(k)) {
			data[k] = v;
		} else {
			data.Add (k, v);
		}
		save ();
	}

	public string getValue(string k) {
		return data [k];
	}

	public void setValue(string k, string v) {
		data[k] = v;
		save ();
	}
	/*	
	  	check if control already exists
		if it does exist then delete it and it current scene will use the already created one.
		Allows us to have 1 datacontrol across all scenes 
	*/
	void Awake() { 
		if (control == null) {
			DontDestroyOnLoad (gameObject);
			control = this;
			load();
		} else if (control != this) {
			Destroy (gameObject);
		}
	}

	public void save() {
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/playerInfo.dat");

		PlayerData dataToSave = new PlayerData ();
		dataToSave.data = data;

		//Turn PlayerData object into a file
		bf.Serialize (file, dataToSave);
		file.Close ();
	}

	public void load() {
		if(File.Exists(Application.persistentDataPath + "/playerInfo.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);

			//take file and turn it back into a PlayerData object
			PlayerData loadedData = (PlayerData) bf.Deserialize (file);
			file.Close ();

			data = loadedData.data;
		} else {
			save();
		}
	}
}

//This is the object that is saved in the file
[Serializable]
class PlayerData {
	
	public Dictionary<string, string> data = new Dictionary<string, string>();

}