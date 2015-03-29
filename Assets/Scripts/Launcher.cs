using UnityEngine;
using System.Collections;

public class Launcher : MonoBehaviour {

	public GameObject ballPrefab;
	private GameObject Ball;

	//Indicates if the ball has been instatiated and is ready to kick
	private bool readyToShoot = false;

	//Variable to handle powerbar
	public float fullWidth; //This is the full width of the power bar

	private bool increasing = false;
	private bool shooting = false;
	private float barSpeed = 75; //Speed at which the power increases
	private float shotPower;
	public GUITexture powerBarCover; //This is the texture we will be resizing


	//Variables to handle player animation
	private Animator playerAnimator;
	private float animationLength = 1.5f;
	private bool kicking = false;
	private bool ballKicked = false;

	//Sound effects
	public AudioClip kickingBallSfx;

	private CameraScript cameraScript; //Allows me to access the 'replay' camera

	//Wind Settings
	public GUITexture windIconLeft, windIconRight;
	public GUIText leftText, rightText;
	private bool windEnabled;
	public float windEffect = 100.0f;
	
	// Use this for initialization
	void Start () {
		//Access the camera script
		cameraScript = (CameraScript) GameObject.Find ("AltCamera").GetComponent<CameraScript> (); 

		//Enabled wind for the level
		if (Application.loadedLevel >= 3) {
			windEnabled = true;		
		}

		instantiateBall ();

		powerBarCover.pixelInset = new Rect(powerBarCover.pixelInset.x, powerBarCover.pixelInset.y, -fullWidth ,powerBarCover.pixelInset.height);
	
		//Access the players animator
		playerAnimator = GameObject.Find ("Player").GetComponent<Animator> ();




	}
	
	// Update is called once per frame
	void Update () {

		//When the control key is being held increase the power
		if(!shooting && Input.GetKeyDown(KeyCode.LeftControl) && readyToShoot){
			increasing=true;
		}

	
		if(!shooting && Input.GetKeyUp(KeyCode.LeftControl) && increasing){
			//reset increasing to stop charge of the power bar
			increasing = false;
			kicking = true; //Start the kicking animation
			readyToShoot = false;
			playerAnimator.SetBool ("Kicking", true);

		}

		if(increasing) {
			//add to shotPower variable using Time.deltaTime multiplied by barSpeed
			shotPower += Time.deltaTime * barSpeed;

			//stop shotPowerr from exceeding fullWidth using Clamp
			shotPower = Mathf.Clamp(shotPower, 0, fullWidth);
			
			//set the width of the GUI Texture to represent the power
			powerBarCover.pixelInset = new Rect(powerBarCover.pixelInset.x, powerBarCover.pixelInset.y, (-fullWidth + shotPower) ,powerBarCover.pixelInset.height);

		}

		//Handles the kicking animation
		if(kicking){
			animationLength -= Time.deltaTime;
		}


		if (animationLength <= 0.5f && animationLength >= 0.4f) {
			//Play sound when kicking the ball
			audio.clip = kickingBallSfx;
			audio.Play ();

		}

		if(animationLength <= 0){
			
			animationLength = 1.5f;
			fireBall();

		}

	}

	//Function to create a new ball
	public void instantiateBall(){

		ballKicked = false;

		if (windEnabled) {
			generateWind();
		}
		
		cameraScript.Hide (); //Change back to main Camera

		powerBarCover.pixelInset = new Rect(powerBarCover.pixelInset.x, powerBarCover.pixelInset.y, -fullWidth ,powerBarCover.pixelInset.height); //Reset the power bar visually

		Ball = (GameObject) Instantiate(ballPrefab, transform.position ,  Quaternion.identity);
		Ball.transform.eulerAngles = new Vector3(0, -90, 0);

		cameraScript.setBall(Ball);

		readyToShoot = true;


	}

	void fireBall () {

		Vector3 shootingDirection = Ball.transform.root.forward * (shotPower*0.75f);

		//When the ball is kicked, hide the arrow displaying direction
		for(int i = 0; i < Ball.transform.childCount; i++){

			if(Ball.transform.GetChild(i).name == "Arrow"){
				Ball.transform.GetChild(i).renderer.enabled = false;
				Ball.transform.GetComponent<BallMouseLook>().enabled = false;
				Ball.transform.GetComponent<FootballScript>().enabled = true; //Turn on the script that listens for collisions
			}
		}

		//Apply force and spin to the ball
		Ball.rigidbody.AddForce(shootingDirection, ForceMode.Impulse);
		Ball.rigidbody.AddTorque(Vector3.up * (shotPower*2), ForceMode.Impulse);
		Ball.rigidbody.AddTorque(Vector3.right * (shotPower), ForceMode.Impulse);

		Ball.rigidbody.AddRelativeForce (new Vector3(1,0,0) * windEffect);
		
		kicking = false;
		ballKicked = true;
		playerAnimator.SetBool ("Kicking", false);

		shotPower = 0;

		cameraScript.Display (); //Change to alternate camera

	}

	//generates random wind power and direction
	void generateWind(){

		float direction = Random.Range (1.0f, 10.0f);
		float strength = Random.Range (1.0f, 12.0f);
		string power;

		//Setting the power of the wind
		if (strength >= 1 && strength < 5) {
			windEffect = 100;
			power = "Light Wind";
		} else if(strength >= 5 && strength < 9) {
			windEffect = 200;
			power = "Medium Wind";
		} else  {
			windEffect = 300;
			power = "Strong Wind";
		}

		//Wind going right
		if (direction <= 5) {
			windIconRight.guiTexture.enabled = true;
			rightText.guiText.enabled = true;
			rightText.guiText.text = power;
			windIconLeft.guiTexture.enabled = false;
			leftText.guiText.enabled = false;
		} else {//Wind going left
			windEffect*=-1;
			windIconLeft.guiTexture.enabled = true;
			leftText.guiText.enabled = true;
			leftText.guiText.text = power;
			windIconRight.guiTexture.enabled = false;
			rightText.guiText.enabled = false;
		}

	}
	
}
