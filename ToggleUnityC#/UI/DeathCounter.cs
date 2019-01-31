using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DeathCounter : MonoBehaviour {

    public TextMesh TM;
    public Text TextScript;
	
    SaveScript Saver;
    // Use this for initialization
	void Start () {
        if(SaveScript.TMD != null)
        {
            string num = SaveScript.TMD.noOfDeath.ToString();
            //Debug.Log("TextMesh: " + num);
            TM.text = SaveScript.TMD.noOfDeath.ToString();
        }
    }
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(SaveScript.TMD + " This shouldn't be null");
        if (SaveScript.TMD != null)
        {
            string num = SaveScript.TMD.noOfDeath.ToString();
            //Debug.Log("TextMesh: " + num);
            if (num == "1")
            {
                TextScript.text = num + " Death";
            } else
            {
                TextScript.text = num + " Deaths";
            }
            TM.text = SaveScript.TMD.noOfDeath.ToString() + " Deaths";

        }
    }
}
