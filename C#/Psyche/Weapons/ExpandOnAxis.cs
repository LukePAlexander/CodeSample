using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandOnAxis : MonoBehaviour {

    public string WhichAxis;
    //If less than zero, will collapse.
    public float HowFast;
    
    private Vector3 scaleVector;

	// Use this for initialization
	void Start () {
		switch(WhichAxis){
            case "Y":
                scaleVector = new Vector3(0, HowFast, 0);
                break;

            case "X":
                scaleVector = new Vector3(HowFast,0,0);
                break;

            case "Z":
                scaleVector = new Vector3(0,0, HowFast);
                break;

            default:
                print("String given to Expand on Axis invalid, try Y, X, or Z");
                break;
        }
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.localScale += scaleVector;
	}
}
