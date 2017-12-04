using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextAnmator : MonoBehaviour {

	public int duration;
	public Text tapToPlay;
	public Color color1 = Color.white;
	public Color color2 = Color.grey;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float t = Mathf.PingPong(Time.time, duration) / duration;
        tapToPlay.color = Color.Lerp(color1, color2, t);
	}
}
