using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	private int numberOfCameras;
	private GameObject ball;
	private GUIText viewLabel;
	private bool cameraActive = true; //This allows players to turn on and off the alternate angle camera

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		if (ball != null) {
			transform.LookAt (ball.transform);
		}

		//Press 'c' to turn on and off action camera
		if(Input.GetKeyUp("c")){
			
			if(cameraActive){
				cameraActive = false;
				Hide ();
			} else {
				cameraActive = true;
			}
			
		}
	}

	public void Display() {
		if (cameraActive) {
			gameObject.camera.rect = new Rect (0.0f, 0.0f, 1f, 1f);
		}
	}

	public void Hide() {
		gameObject.camera.rect = new Rect(0.0f, 0.0f, 0.0f, 0.0f);
	}

	public void setBall(GameObject o){
		ball = o;
	}
}
