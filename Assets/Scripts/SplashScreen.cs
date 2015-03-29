using UnityEngine;
using System.Collections;

public class SplashScreen : MonoBehaviour {

	public float rotateSpeed = 30;
	private float button_x = 558.4f, button_y = 292.2f, button_w = 161.2f, button_h = 72.3f; //Variables for displaying buttons on the screen

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (Vector3.up * Time.deltaTime * rotateSpeed, Space.World);
	}

	
	void OnGUI () {
		
		if(GUI.Button(new Rect(button_x,button_y,button_w, button_h), "")) {
			Application.LoadLevel(1);
		}

	}
}
