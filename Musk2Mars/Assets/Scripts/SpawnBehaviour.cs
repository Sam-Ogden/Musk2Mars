﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBehaviour : MonoBehaviour {

	public uint generatorNum;	// The number of generators. How many objects can be on the screen at once
	public GameObject generator;// Prefab of generator object, assign in UI
	public GameObject coin;		// Coin Prefab, assign in UI 💰
	public GameObject fuel;		// Fuel Prefab, assign in UI ⛽️
	public float upperOffset;	// Distance between top of screen and spawner line
	public float screenCutoff;	// Padding on sides of screen. Padding on each side: screenCutoff/2
	public float spawnGap;		// Gap between lines of spawned collectibles
	private Camera cam;			// Stores main camera 📹
	private float screenWidth;
	private float screenHeight;
	private GameObject[] generators;	// Holds all collectible generators
	private Queue lines;	// Queue of information about what lines of collectibles to spawn 📏
	private float lastY;	// Stores the Y coordinate of the last spawned line
	private GameObject[] collectibles;	// Stores the prefabs of collectibles, declared further up

	// Patterns should be added in reverse vertical order
	private byte[,,] patterns = {
		{
			{1,1,1,0,0,0},
			{1,1,1,0,0,0},
			{0,1,2,1,0,0},
			{0,1,1,1,0,0},
			{0,0,1,1,1,0},
			{0,0,1,1,1,0},
			{0,0,0,1,1,1},
			{0,0,0,1,1,1}
		}
	};
	
	// Use this for initialization
	void Start () {
		cam = Camera.main;	// Store main camera

		// Calculate screen sizes
		var screenBottomLeft = cam.ViewportToWorldPoint(
			new Vector3(0, 0, transform.position.z));
		var screenTopRight = cam.ViewportToWorldPoint(
			new Vector3(1, 1, transform.position.z));
		screenWidth = screenTopRight.x - screenBottomLeft.x;
		screenHeight = screenTopRight.y - screenBottomLeft.y;

		lines = new Queue();
		lastY = transform.position.y;
		collectibles = new GameObject[] {coin, fuel};	// Add more collectible prefabs here
		positionSpawn();	// Move parent spawn line to top of screen
		generateSpawners();	// Arrange spawners in line
	}
	
	// Update is called once per frame
	void Update () {
		if(lines.Count <= 2) {
			// Starting to run out of instructions so generate more
			addPattern();
		}
		if(transform.position.y >= lastY + spawnGap) {
			// Far enough from last spawned line, so spawn more
			spawn();
		}
	}

	// Positions spawn above view
	void positionSpawn() {
		Vector3 positionChange = new Vector3();
		positionChange.y = (screenHeight / 2) + upperOffset;
		transform.position += positionChange;
	}

	// Generate objects in order to keep track of positions without calculating each time
	void generateSpawners() {
		generators = new GameObject[generatorNum];
		float gap = (screenWidth - screenCutoff) / generatorNum;	// Distribute generators on width
		float xPosition = transform.position.x - ((screenWidth - screenCutoff) / 2);
		for(int i = 0; i < generatorNum; i ++) {
			generators[i] = Instantiate(generator, new Vector3(
				xPosition, transform.position.y, 0), Quaternion.identity, transform);
			xPosition += gap;
		}
	}

	void addPattern() {
		int chosen = Random.Range(0, patterns.GetLength(0) - 1);
		for(int i = 0; i < patterns.GetLength(1); i ++) {
			lines.Enqueue(new int[] {chosen, i});
		}
	}

	void spawn() {
		if(lines.Count != 0) {
			lastY = transform.position.y;
			int[] line = (int[]) lines.Dequeue();
			for(int i = 0; i < generatorNum; i ++) {
				if (patterns[ line[0],line[1],i ] > 0){
					GameObject item = collectibles[ patterns[ line[0],line[1],i ] - 1 ];
					Instantiate(item, generators[i].transform.position, Quaternion.identity);
				}
			}
		}
	}
}
