using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour {

	public Slider inputSlider;
	private DataControl data = DataControl.control;

	void Start() {
	}

	public void updateInputMethod() {
		string input = "";
		if(inputSlider.value == 0) input = "tilt";
		else input = "touch";

		if (data.containsKey ("inputMethod")) data.setValue ("inputMethod", input);
		else data.addPair ("inputMethod", input);
	}
}
