using UnityEngine;
using System.Collections;

public class NewGameButton : MonoBehaviour {

	SaveScript Saver;
	// Use this for initialization
	void Start () {
		Saver = GameObject.Find("MetaObject").GetComponent<SaveScript>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseDown()
    {
        //Debug.Log("Triggered");
        SaveScript.TMD.newGame();
        SaveScript.save();
    }
}
