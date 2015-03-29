using UnityEngine;
using System.Collections;

//This script is attached to the ball it handles ball related collisions
public class FootballScript : MonoBehaviour {
	
	private Vector3 frictionForce;
	private Vector3 angularDrag;

	private bool collidingWithTerrain = false;
	private bool moving = false; //Variable to determine if the ball is in motion
	private bool stationary = false;
	private bool crossedGoalLine = false; //This variable stops a goal from being trigger more than once
	private bool ballActive = false; //When the ball is ready to be kicked

	public AudioClip[] audioClips;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		//Check if the ball has come to a stop
		if (rigidbody.velocity.sqrMagnitude < .01 && rigidbody.angularVelocity.sqrMagnitude < .01) {
			if(moving){ //If the ball has come to a stop and not crossed the goal line it is a miss
				moving = false;

				if(!stationary && !crossedGoalLine){
					GameObject.Find ("GameManager").GetComponent<GameManager>().goalMissed();
					stationary = true;
				}
			}
		} else {
			if(!moving){
				moving = true;
			}

		}

		if(collidingWithTerrain){
			//Add drag to the balls rotation when it hits the ground
			angularDrag = rigidbody.angularVelocity * 0.8f;//new Vector3(rigidbody.angularVelocity.x * 0.8f, rigidbody.angularVelocity.y * 0.8f, rigidbody.angularVelocity.z * 0.8f );
			rigidbody.angularVelocity = angularDrag;

			frictionForce = gameObject.rigidbody.velocity*-0.2f;
			rigidbody.AddForce(frictionForce);
		}
	}

	void OnCollisionEnter (Collision col) {
        if(col.gameObject.tag == "Terrain") {


			if(!collidingWithTerrain && ballActive){
				playSound(0); //Play sound of the ball hitting the ground
			}

			if(!ballActive){
				ballActive = true;
			}

            collidingWithTerrain = true;
        }

		if(col.gameObject.tag == "Post") {
				playSound(1); //Play sound of the ball hitting the post
		}

		if(col.gameObject.tag == "Stadium" && !stationary) {
			GameObject.Find ("GameManager").GetComponent<GameManager>().goalMissed();
			stationary = true;
		}
	}

	void OnTriggerEnter(Collider col) {
		if(col.gameObject.tag == "GoalLine") {

			if(!crossedGoalLine && !stationary){
				gameObject.rigidbody.velocity = gameObject.rigidbody.velocity/4; //Reduce velocity when the ball enters the goal to aid net physics calculation

				GameObject.Find ("GameManager").GetComponent<GameManager>().goalScored();
				crossedGoalLine = true;
			}
		}

		//If the ball is out of bounds
		if(col.gameObject.tag == "Outofbounds" && !stationary && !crossedGoalLine) {
				GameObject.Find ("GameManager").GetComponent<GameManager>().goalMissed();
				stationary = true;

		}
	}
	
	void OnCollisionExit (Collision col) {
        if(col.gameObject.tag == "Terrain") {
            collidingWithTerrain = false;
        }

    }

	void playSound(int i){
		if(audioClips[i] != null){
			audio.clip = audioClips [i];
			audio.Play ();
		}
	}
}
