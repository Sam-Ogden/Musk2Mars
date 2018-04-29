using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsControl : MonoBehaviour {

	public AudioMixer mixer;
	public Toggle musicToggle;
	public Toggle sfxToggle;

	private DataControl data;
	private float onMusicVolume = .0f;
	private float offMusicVolume = -80.0f;
	private float onSFXVolume = .0f;
	private float offSFXVolume = -80.0f;

	void Awake () {
		data = DataControl.control;

		string music = data.getValue("MusicVolume");
		string sfx = data.getValue("SFXVolume");
		if (music != null) {
			switch (music) {
				case "on": 	mixer.SetFloat("MusicVolume", onMusicVolume);
							break;
				case "off":	mixer.SetFloat("MusicVolume", offMusicVolume);
							break;
				default:	mixer.SetFloat("MusicVolume", onMusicVolume);
							data.updateVal("MusicVolume", "on");
							break;
			}
		} else {
			mixer.SetFloat("MusicVolume", onMusicVolume);
			data.updateVal("MusicVolume", "on");
		}

		if (sfx != null) {
			switch (sfx) {
				case "on": 	mixer.SetFloat("SFXVolume", onSFXVolume);
							break;
				case "off":	mixer.SetFloat("SFXVolume", offSFXVolume);
							break;
				default:	mixer.SetFloat("SFXVolume", onSFXVolume);
							data.updateVal("SFXVolume", "on");
							break;
			}
		} else {
			mixer.SetFloat("SFXVolume", onSFXVolume);
			data.updateVal("SFXVolume", "on");
		}
	}

	public void SetMusic (bool isOn) {
		switch (isOn) {
			case true: mixer.SetFloat("MusicVolume", onMusicVolume);
					   data.updateVal("MusicVolume", "on");
					   break;
			case false: mixer.SetFloat("MusicVolume", offMusicVolume);
						data.updateVal("MusicVolume", "off");
					   	break;
		}

	}

	public void SetSFX (bool isOn) {
		switch (isOn) {
			case true: mixer.SetFloat("SFXVolume", onSFXVolume);
					   data.updateVal("SFXVolume", "on");
					   break;
			case false: mixer.SetFloat("SFXVolume", offSFXVolume);
						data.updateVal("SFXVolume", "off");
					   	break;
		}

	}
}
