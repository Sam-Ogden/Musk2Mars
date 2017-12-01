using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBehaviour : MonoBehaviour {

	public GameStateController gameState;
	public GameObject generator;// Prefab of generator object, assign in UI
	public GameObject coin;		// Coin Prefab, assign in UI 💰
	public GameObject fuel;		// Fuel Prefab, assign in UI ⛽️
	public GameObject enemy1;	// Placeholder prefab for an enemy
	public float upperOffset;	// Distance between top of screen and spawner line
	public float lowerOffset;
	public float landBuffer;
	public float screenCutoff;	// Padding on sides of screen. Padding on each side: screenCutoff/2
	public float collectibleGap;// Gap between lines of spawned collectibles
	public float maxObstacleGap;
	public float minObstacleGap;
	private float obstacleGap;	// gap between lines of spawned obstacles
	private Camera cam;			// Stores main camera 📹
	private float screenWidth;
	private float screenHeight;
	private GameObject[] generators;	// Holds all collectible generators
	private Queue lines;	// Queue of information about what lines of collectibles to spawn 📏
	private float collectibleY;	// Stores the Y coordinate of the last spawned line
	private float obstacleY;	// Stores the Y coordinate of the last spawned obstacle
	private GameObject[] collectibles;	// Stores the prefabs of collectibles, declared further up
	private GameObject[] obstacles;
	private uint difficulty;
	private bool top;
	private bool bot;
	private float minY;
	private uint generatorNum;	// The number of generators. How many objects can be on the screen at once

	// Patterns should be added in reverse vertical order
	private byte[,,] patterns = {
		{
			{1,0,0,0,0,0,0,0,0,0,1},
			{0,1,0,0,0,0,0,0,0,1,0},
			{0,0,1,0,0,0,0,0,1,0,0},
			{0,0,0,1,0,0,0,1,0,0,0},
			{0,0,0,0,1,0,1,0,0,0,0},
			{0,0,2,0,0,1,0,0,2,0,0},
			{0,0,0,0,1,0,1,0,0,0,0},
			{0,0,0,1,0,0,0,1,0,0,0},
			{0,0,1,0,0,0,0,0,1,0,0},
			{0,1,0,0,0,0,0,0,0,1,0},
			{1,0,0,0,0,0,0,0,0,0,1}
		},
		{
			{1,1,1,1,1,1,1,1,1,1,1},
			{1,0,0,0,0,0,0,0,0,0,1},
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
		{
			{1,1,1,1,1,1,1,1,1,1,1},
			{1,1,1,1,1,1,1,1,1,1,1},
			{1,1,2,1,1,1,1,1,1,1,1},
			{1,1,1,1,1,1,1,1,1,1,1},
			{1,1,1,1,1,1,1,1,1,1,1},
			{1,1,1,1,1,1,1,1,1,1,1},
			{1,1,1,1,1,1,1,1,1,1,1},
			{1,1,1,1,1,1,1,1,1,1,1},
			{1,1,1,1,1,1,1,1,2,1,1},
			{1,1,1,1,1,1,1,1,1,1,1},
			{1,1,1,1,1,1,1,1,1,1,1}
		},
		{
			{0,0,0,0,0,1,1,1,1,1,1},
			{0,0,0,0,0,1,1,1,1,1,1},
			{0,0,0,0,0,0,0,0,0,0,0},
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
	void Start () {
		cam = Camera.main;	// Store main camera
		gameState = GameStateController.gameStateController;
		// Calculate screen sizes
		var screenBottomLeft = cam.ViewportToWorldPoint(
			new Vector3(0, 0, transform.position.z));
		var screenTopRight = cam.ViewportToWorldPoint(
			new Vector3(1, 1, transform.position.z));
		screenWidth = screenTopRight.x - screenBottomLeft.x;
		screenHeight = screenTopRight.y - screenBottomLeft.y;
		generatorNum = (uint) patterns.GetLength(2);
		collectibles = new GameObject[] { coin, fuel };	// Add more collectible prefabs here
		obstacles = new GameObject[] { enemy1 };	// Add more enemy/obstacle prefabs here
		positionSpawn(true, false);	// Move parent spawn line to top of screen
		generateSpawners();	// Arrange spawners in line
		obstacleGap = maxObstacleGap;
		difficulty = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if(gameState.isLanding() && !bot) {
			positionSpawn(false, true);
		} else if(gameState.gameIsRunning() && !top) {
			positionSpawn(true, false);
		}

		if(lines.Count <= 2) {
			// Starting to run out of instructions so generate more
			addPattern();
		}
		if(Mathf.Abs(transform.position.y - obstacleY) >= obstacleGap && transform.position.y > minY) {
			spawnObstacles();
		} else if(Mathf.Abs(transform.position.y - collectibleY) >= collectibleGap && transform.position.y > minY) {
			// Far enough from last spawned line, so spawn more
			spawnCollectibles();
		}
	}

	// Positions spawn above view
	void positionSpawn(bool newTop, bool newBot) {
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
	void generateSpawners() {
		generators = new GameObject[generatorNum];
		float gap = (screenWidth - screenCutoff) / (generatorNum - 1);	// Distribute generators on width
		float xPosition = transform.position.x - ((screenWidth - screenCutoff) / 2);
		for(int i = 0; i < generatorNum; i ++) {
			// Instantiate generators and make them children of the Spawn object
			generators[i] = Instantiate(generator, new Vector3(
				xPosition, transform.position.y, 0), Quaternion.identity, transform);
			xPosition += gap;
		}
	}

	void addPattern() {
		int[] pad = {-1, -1};
		int chosen = Random.Range(0, patterns.GetLength(0));
		for(int i = 0; i < patterns.GetLength(1); i ++) {
			lines.Enqueue(new int[] {chosen, i});
		}
		lines.Enqueue(pad);
		lines.Enqueue(pad);
		lines.Enqueue(pad);
	}

	void spawnCollectibles() {
		if(lines.Count != 0) {
			collectibleY = transform.position.y;
			int[] line = (int[]) lines.Dequeue();
			if(line[0] != -1) {
				for(int i = 0; i < generatorNum; i ++) {
					if (patterns[ line[0],line[1],i ] > 0) {
						GameObject item;
						if(top) {
							item = collectibles[ patterns[ line[0],line[1],i ] - 1 ];
						} else {
							item = coin;
						}
						Instantiate(item, generators[i].transform.position, Quaternion.identity);
					}
				}
			}
		}
	}

	void spawnObstacles() {
		if(difficulty < maxObstacleGap - minObstacleGap) {
			difficulty += (uint) (Time.deltaTime / 19);
		}
		obstacleGap = maxObstacleGap - difficulty;
		obstacleY = transform.position.y;
		collectibleY = transform.position.y;
		int i = Random.Range(0, obstacles.GetLength(0) - 1);
		int j = Random.Range(0, generators.GetLength(0) - 1);
		Instantiate(obstacles[i], generators[j].transform.position, Quaternion.identity);
	}
}
