using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBehaviour : MonoBehaviour {

	public GameObject generator;// Prefab of generator object, assign in UI
	public GameObject coin;		// Coin Prefab, assign in UI 💰
	public GameObject fuel;		// Fuel Prefab, assign in UI ⛽️
	public GameObject enemy1;	// Placeholder prefab for an enemy
	public GameObject magnet;	// Magnet powerup Prefab, assign in UI
	public float upperOffset;	// Distance between top of screen and spawner line
	public float lowerOffset;
	public float landBuffer;
	public float screenCutoff;	// Padding on sides of screen. Padding on each side: screenCutoff/2
	public float collectibleGap;// Gap between lines of spawned collectibles
	public float maxObstacleGap;
	public float minObstacleGap;
	public uint powerupFrequency; // The required number of patterns between powerup appearences
	
	private GameStateController gameState;
	private PowerupController powerupController;
	private Camera cam;		// Stores main camera 📹
	private GameObject[] generators; // Holds all collectible generators
	private GameObject[] collectibles; // Stores the prefabs of collectibles, declared further up
	private GameObject[] obstacles;
	private GameObject[] powerups;
	private Queue lines; // Queue of information about what lines of collectibles to spawn 📏
	private float obstacleGap;	// gap between lines of spawned obstacles
	private float screenWidth;
	private float screenHeight;
	private float collectibleY;	// Stores the Y coordinate of the last spawned line
	private float obstacleY;	// Stores the Y coordinate of the last spawned obstacle
	private float minY;
	private uint difficulty;
	private uint generatorNum; // The number of generators. How many objects can be on the screen at once
	private uint patternCount = 0; // The count of queued patterns since last powerup
	private int obstaclePos; // The number of the generator that last spawned an obstacle
	private const int pow = 2;	// Holds powerup to be spawned. Coin if not time for a powerup
	private bool top;
	private bool bot;

	// Patterns should be added in reverse vertical order
	// 0=nothing, 1=coin, 2=fuel, 3=Powerup
	private byte[,,] patterns = {
		{
			{1,0,0,0,0,0,0,0,0,0,1},
			{0,1,0,0,0,0,0,0,0,1,0},
			{0,0,1,0,0,0,0,0,1,0,0},
			{0,0,0,1,0,0,0,1,0,0,0},
			{0,0,0,0,1,0,1,0,0,0,0},
			{0,0,2,0,0,3,0,0,2,0,0},
			{0,0,0,0,1,0,1,0,0,0,0},
			{0,0,0,1,0,0,0,1,0,0,0},
			{0,0,1,0,0,0,0,0,1,0,0},
			{0,1,0,0,0,0,0,0,0,1,0},
			{1,0,0,0,0,0,0,0,0,0,1}
		},
		{
			{1,1,1,1,1,1,1,1,1,1,1},
			{1,0,0,0,0,3,0,0,0,0,1},
			{1,0,1,0,0,0,0,0,1,0,1},
			{1,0,0,0,0,0,0,0,0,0,1},
			{1,0,0,0,0,0,0,0,0,0,1},
			{1,0,0,0,0,2,0,0,0,0,1},
			{1,0,0,0,0,0,0,0,0,0,1},
			{1,0,0,0,0,0,0,0,0,0,1},
			{1,0,1,0,0,0,0,0,1,0,1},
			{1,0,0,0,0,0,0,0,0,0,1},
			{1,1,1,1,1,1,1,1,1,1,1}
		},
		// {
		// 	{1,1,1,1,1,1,1,1,1,1,1},
		// 	{1,1,1,1,1,1,1,1,1,1,1},
		// 	{1,1,2,1,1,1,1,1,1,1,1},
		// 	{1,1,1,1,1,1,1,1,1,1,1},
		// 	{1,1,1,1,1,1,1,1,1,1,1},
		// 	{1,1,1,1,1,1,1,1,1,1,1},
		// 	{1,1,1,1,1,1,1,1,1,1,1},
		// 	{1,1,1,1,1,1,1,1,1,1,1},
		// 	{1,1,1,1,1,1,1,1,2,1,1},
		// 	{1,1,1,1,1,1,1,1,1,1,1},
		// 	{1,1,1,1,1,1,1,1,1,1,1}
		// },
		{
			{0,0,0,0,0,1,1,1,1,1,1},
			{0,0,0,0,0,1,1,1,1,1,1},
			{0,0,3,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0},
			{1,1,1,1,1,1,1,1,1,1,1},
			{0,0,0,0,0,0,0,0,0,2,0},
			{1,1,1,1,1,1,1,1,1,1,1},
			{0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0},
			{1,1,1,1,1,1,0,0,0,0,0},
			{1,1,1,1,1,1,0,0,0,0,0}
		}
	};
	
	// Use this for initialization
	void Start() {
		cam = Camera.main; // Store main camera
		gameState = GameStateController.gameStateController;
		powerupController = PowerupController.controller;
		powerupFrequency *= (uint) patterns.GetLength(1);

		// Calculate screen sizes
		var screenBottomLeft = cam.ViewportToWorldPoint(
			new Vector3(0, 0, transform.position.z));
		var screenTopRight = cam.ViewportToWorldPoint(
			new Vector3(1, 1, transform.position.z));
		screenWidth = screenTopRight.x - screenBottomLeft.x;
		screenHeight = screenTopRight.y - screenBottomLeft.y;

		// Initiate generators and objects to be spawned
		generatorNum = (uint) patterns.GetLength(2);
		collectibles = new GameObject[] { coin, fuel, null }; // Add more collectible prefabs here
		obstacles = new GameObject[] { enemy1 }; // Add more enemy/obstacle prefabs here
		powerups = new GameObject[] { magnet };	 // Add more powerups here
		DeselectPow(); // Pick what Powerup should be spawned if the time comes
		PositionSpawn(true, false);	// Move parent spawn line to top of screen
		GenerateSpawners();	// Arrange spawners in line
		obstacleGap = maxObstacleGap;
		difficulty = 0;
	}
	
	// Update is called once per frame
	void Update() {
		if(gameState.isLanding() && !bot) {
			PositionSpawn(false, true);
		} else if(gameState.gameIsRunning() && !top) {
			PositionSpawn(true, false);
		}

		if(lines.Count <= 2) {
			// Starting to run out of instructions so generate more
			AddPattern();
		}
		if(Mathf.Abs(transform.position.y - obstacleY) >= obstacleGap && transform.position.y > minY) {
			SpawnObstacles();
		} else if(Mathf.Abs(transform.position.y - collectibleY) >= collectibleGap && transform.position.y > minY) {
			// Far enough from last spawned line, so spawn more
			SpawnCollectibles();
		}
	}

	// Positions spawn above view
	void PositionSpawn(bool newTop, bool newBot) {
		if(newTop) {
			Vector3 topPosition = cam.transform.position;
			topPosition.y += (screenHeight / 2) + upperOffset;
			transform.position = topPosition;
			minY = -1000000000.0f;
		} else if(newBot) {
			Vector3 botPosition = cam.transform.position;
			botPosition.y -= (screenHeight / 2) + lowerOffset;
			transform.position = botPosition;
			minY = transform.position.y - (float) gameState.landDistance + landBuffer;
		}
			
		top = newTop;
		bot = newBot;
		lines = new Queue();
		collectibleY = transform.position.y;
		obstacleY = transform.position.y;
	}

	// Generate objects in order to keep track of positions without calculating each time
	void GenerateSpawners() {
		generators = new GameObject[generatorNum];
		float gap = (screenWidth - screenCutoff) / (generatorNum - 1);	// Distribute generators on width
		float xPosition = transform.position.x - ((screenWidth - screenCutoff) / 2);
		for(uint i = 0; i < generatorNum; i ++) {
			// Instantiate generators and make them children of the Spawn object
			generators[i] = Instantiate(generator, new Vector3(
				xPosition, transform.position.y, 0), Quaternion.identity, transform);
			xPosition += gap;
		}
	}

	void AddPattern() {
		int[] pad = {-1, -1};
		int chosen = UnityEngine.Random.Range(0, patterns.GetLength(0));
		for(int i = 0; i < patterns.GetLength(1); i ++) {
			lines.Enqueue(new int[] {chosen, i});
		}
		lines.Enqueue(pad);
		lines.Enqueue(pad);
		lines.Enqueue(pad);
	}

	void SpawnCollectibles() {
		if(lines.Count != 0) {
			if(patternCount == powerupFrequency) {
				patternCount = 0;
				if(!powerupController.GetActive()) {
					SelectPow();
				} else {
					collectibles[pow] = coin;
				}
			} else if(patternCount == patterns.GetLength(1)) {
				collectibles[pow] = coin;
			}
			collectibleY = transform.position.y;
			int[] line = (int[]) lines.Dequeue();
			if(line[0] != -1) {
				for(uint i = 0; i < generatorNum; i ++) {
					if (patterns[ line[0],line[1],i ] > 0) {
						GameObject item;
						if(top) {
							item = collectibles[ patterns[ line[0],line[1],i ] - 1 ];
						} else {
							item = coin;
						}
						if(!(collectibleY == obstacleY && obstaclePos == i)) {
							Instantiate(item, generators[i].transform.position, Quaternion.identity);
						}
					}
				}
			}
			patternCount++;
		}
	}

	void SpawnObstacles() {
		if(difficulty < maxObstacleGap - minObstacleGap) {
			difficulty += (uint) (Time.deltaTime / 19);
		}
		obstacleGap = maxObstacleGap - difficulty;
		obstacleY = transform.position.y;
		collectibleY = transform.position.y;
		int i = UnityEngine.Random.Range(0, obstacles.GetLength(0) - 1);
		int j = UnityEngine.Random.Range(0, generators.GetLength(0) - 1);
		obstaclePos = j;
		Instantiate(obstacles[i], generators[j].transform.position, Quaternion.identity);
	}

	void SelectPow() {
		collectibles[pow] = powerups[UnityEngine.Random.Range(0, powerups.GetLength(0) - 1)];
	}
	void DeselectPow() {
		collectibles[pow] = coin;
	}
}
