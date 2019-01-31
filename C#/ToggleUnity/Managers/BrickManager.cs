using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BrickManager : MonoBehaviour {

	public GameObject[] WhitePlatforms;
	public GameObject[] BlackPlatforms;
	public string Active;
	public Color WhiteColor;
	public Color BlackColor;
	public Sprite WhiteSprite;
	public Sprite BlackSprite;
	public GameObject FirstPlatform;
    public GameObject whitePlayer;
    public GameObject blackPlayer;

	void Awake() {
        whitePlayer = GameObject.Find("Player_2D").transform.GetChild(0).gameObject;
        blackPlayer = GameObject.Find("Player_2D").transform.GetChild(1).gameObject;

		FirstPlatform = GameObject.FindGameObjectWithTag("WhitePlatform");
	}

}
