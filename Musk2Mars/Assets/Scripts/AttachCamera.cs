using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachCamera : MonoBehaviour {

	void Awake() {
		gameObject.GetComponent<Canvas>().worldCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
	}
}
