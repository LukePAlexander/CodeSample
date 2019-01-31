using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	GameObject Player;
	PlayerMovement playerMovement;

	private Vector3 offset;
	private Vector3 reversedOffset;
	// Use this for initialization
	void Start () {
		offset = new Vector3(3, 0, -10);
		reversedOffset = new Vector3(-3,0,-10);
		Player = GameObject.Find("Player_2D");
		playerMovement = Player.GetComponent<PlayerMovement>();

		// Turns camera landscape mode for mobile
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		//GameObject.transform.x 
	}
	
	// Update is called once per frame
	void Update () {
		if (!playerMovement.isReversed){
			transform.position = Player.transform.position + offset;		
		} else if (playerMovement.isReversed){
			Debug.Log("woah it worked");
			transform.position = Player.transform.position + reversedOffset;
		}
	}
}
