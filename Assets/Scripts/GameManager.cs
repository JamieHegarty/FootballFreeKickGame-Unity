using UnityEngine;
using System.Collections;

//Game manager script handles the management of the game such as changing levels, increase score etc..
public class GameManager : MonoBehaviour {
	
	public Transform[] playerPositions = new Transform[5]; //This allows me to set the 5 shooting positions from the editior for each level
	public Transform playerModelPosition;
	private bool movePlayer = false;
	private const float TIME_BETWEEN_SHOTS = 1.9f; //How long untill a player moves to the next position
	private float timeAfterShot = TIME_BETWEEN_SHOTS; //Timer

	private GameObject playerSetup; //This is the access point to the player object (launcher, camera and player model)
	private GameObject playerModel;
	private Launcher launcher; //Access point to the launched (gives control over instatiating the football)

	public int level;
	private int score = 0;
	private int shots = 0;
	private int currentPosition = 0;

	public GameObject greenDot, redDot; //These are gui texture used to represent the score

	public AudioClip[] audioClips; //Sound effects
	private bool muted = false;

	public GUITexture instructionScreen, levelWin, levelLose, gameWin, levelBrief; //These are the different menus to be displayed
	private bool levelWon = false;
	private bool levelLost = false;
	private bool gameWon = false;
	private bool showInstructions = true;
	private bool showBrief = false;
	private float showBriefTime = 3.0f;

	private float button_x = 588.7f, button_w = 135, button_h = 63.9f; //Variables for displaying buttons on the screen


	// Use this for initialization
	void Start () {

		if(instructionScreen != null){
			instructionScreen.guiTexture.enabled = true;
			levelWin.guiTexture.enabled = false;
			levelLose.guiTexture.enabled = false;
		}

		//Dispaly level info
		if (level > 1) {
			instructionScreen.guiTexture.enabled = false;
			levelBrief.guiTexture.enabled = true;	
			showBrief = true;
		}

		playerSetup = GameObject.Find ("Player_Setup");

		if (playerSetup != null) {
			playerModel = playerSetup.transform.FindChild("Player").gameObject;

			launcher = (Launcher) playerSetup.transform.FindChild("Launcher").gameObject.GetComponent<Launcher>();
		}

		if(playerPositions[currentPosition] != null) {
			playerSetup.transform.position = playerPositions[currentPosition].position; //Move the player to the first shooting position
			playerSetup.transform.rotation = playerPositions[currentPosition].rotation; 

			playSound(0); //Play whistle sound

		}

	}
	
	// Update is called once per frame
	void Update () {
		if (showBrief) {
			showBriefTime -= Time.deltaTime;
		}
		//Timer to hide level briefing
		if (showBriefTime <= 0) {
			showBrief = false;
			levelBrief.guiTexture.enabled = false;
		}

		//Handles moving the player to next location
		if(movePlayer){
			timeAfterShot -= Time.deltaTime;
		}
		
		if(timeAfterShot <= 0){
			
			timeAfterShot = TIME_BETWEEN_SHOTS;

			//Add a delay to when the player is moved so it happens after the ball has had a chance to hit the net && animation has finished
			if(playerPositions[currentPosition] != null) {
				playerSetup.transform.localPosition = playerPositions[currentPosition].position; //Move the player to the next shooting position
				playerSetup.transform.localRotation = playerPositions[currentPosition].rotation; 
				resetPlayerPosition();
			}

			movePlayer = false;
			
		}

		//Press 'i' to view game instructions
		if(Input.GetKeyUp("i") && !levelWon && !levelLost && !showBrief){

			if(showInstructions){
				instructionScreen.guiTexture.enabled = false;
				showInstructions = false;
			} else {
				instructionScreen.guiTexture.enabled = true;
				showInstructions = true;
			}

		}

		//Press 'm' to mute all game sounds
		if(Input.GetKeyUp("m")){
			
			if(muted){
				AudioListener.volume = 1;
				muted = false;
			} else {
				AudioListener.volume = 0;
				muted = true;
			}
			
		}
		
	}

	//This resets the position of the player model
	void resetPlayerPosition() {
		if (playerModelPosition != null && playerModel != null && launcher != null) {
			playerModel.transform.position = playerModelPosition.position;
			playerModel.transform.rotation = playerModelPosition.rotation; 

			//Create a new football && handle what happens when player misses the goal
			launcher.instantiateBall();
			playSound(0); //Play whistle sound
		}
	}

	public void goalScored() {
		playSound(1); //Play goal scored sound

		//Draw Green dot
		GameObject scoreIcon = (GameObject) Instantiate(greenDot, transform.position ,  Quaternion.identity);
		scoreIcon.transform.position = new Vector3 ((0.135f) + (0.02f*shots), 0.08f, 2);
		scoreIcon.tag = "scoreIcon";

		shots += 1; //Increase shots
		score += 1; //Increase score

		// Move player to the next shooting position
		if(currentPosition < 4) {
			currentPosition += 1;
			movePlayer = true;
		}

		Debug.Log ("Score increased: " + score);

		//Draw green dot

		if (shots == 5) {
			endLevel();		
		}
	}

	public void goalMissed() {
		//Draw red dot
		GameObject scoreIcon = (GameObject) Instantiate(redDot, transform.position ,  Quaternion.identity);
		scoreIcon.transform.position = new Vector3 ((0.135f) + (0.02f*shots), 0.08f, 2);
		scoreIcon.tag = "scoreIcon";

		shots += 1; //Increase shots

				
		// Move player to the next shooting position
		if(currentPosition < 4) {
			currentPosition += 1;
			movePlayer = true;
		}
		
		Debug.Log ("Shot missed");

	

		if (shots == 5) {
			endLevel();		
		}
	}

	void endLevel() {
		instructionScreen.guiTexture.enabled = false;
		showInstructions = false;
		
		if (score >= 4) {
			if(level == 4){
				gameWin.guiTexture.enabled = true;
				gameWon = true;
			}else {
				levelWin.guiTexture.enabled = true;	
				levelWon = true;
			}

		} else {
			levelLose.guiTexture.enabled = true;	
			levelLost = true;
		}
	}

	void restartLevel() {

		levelWon = false;
		levelLost = false;
		
		levelWin.guiTexture.enabled = false;
		levelLose.guiTexture.enabled = false;

		score = 0;
		shots = 0;
		currentPosition = 0;

		playerSetup.transform.position = playerPositions[currentPosition].position; //Move the player to the first shooting position
		playerSetup.transform.rotation = playerPositions[currentPosition].rotation; 
		
		cleanUp (); //Cleans up the scene (removes balls etc)
		resetPlayerPosition ();
		
		Debug.Log ("Restart Level");

	}

	void loadNextLevel() {
		levelWon = false;
		levelLost = false;

		levelWin.guiTexture.enabled = false;
		levelLose.guiTexture.enabled = false;
		
		Debug.Log ("Load Next Level");

		if (level < 4) {
			Application.LoadLevel(level+1);		
		}
	}

	void cleanUp() {
		GameObject[] gameObjects = GameObject.FindGameObjectsWithTag ("Football");

		//Remove all the old footballs
		for(var i = 0 ; i < gameObjects.Length ; i ++) {
			Destroy(gameObjects[i]);
		}

		//Remove old score indicators
		gameObjects = GameObject.FindGameObjectsWithTag ("scoreIcon");
		
		for(var i = 0 ; i < gameObjects.Length ; i ++) {
			Destroy(gameObjects[i]);
		}
	}
	
	void OnGUI () {
		
		//When showing the game win screen set up required buttons
		if(levelWon){
			// Make the continue button
			if(GUI.Button(new Rect(button_x,261.8f,button_w, button_h), "Continue")) {
				loadNextLevel();
			}

			if(GUI.Button(new Rect(button_x,350.3f,button_w, button_h), "Retry Level")) {
				restartLevel();
			}

			if(GUI.Button(new Rect(button_x,439.8f,button_w, button_h), "Quit")) {
				Application.LoadLevel(0);
			}
		}

		if(levelLost){
			// Make the continue button
			if(GUI.Button(new Rect(button_x - 3, 323.1f, button_w + 3, button_h), "Retry Level")) {
				restartLevel();
			}
			
			if(GUI.Button(new Rect(button_x - 3, 417.57f, button_w + 3, button_h), "Quit")) {
				Application.LoadLevel(0);
			}

		}

		if(gameWon){

			if(GUI.Button(new Rect(button_x - 2,380.57f,button_w + 2, button_h), "Quit")) {
				Application.LoadLevel(0);
			}
		}

	}

	void playSound(int i){

		if(audioClips[i] != null){
			audio.clip = audioClips [i];
			audio.Play ();
		}
	}
}
