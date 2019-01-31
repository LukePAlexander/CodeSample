using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DisplayProgress : MonoBehaviour {

    public int coinProg;
    public float levelProg;
    public TextMesh displayText;
    public bool isCoins;

    SaveScript Saver;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (isCoins)
        {
            coinProg = SaveScript.TMD.coinProgress(SceneManager.GetActiveScene().buildIndex);
            displayText.text = coinProg + "/3";
        } else
        {
            levelProg = SaveScript.TMD.progress(SceneManager.GetActiveScene().buildIndex);
            displayText.text = levelProg + "/100";
        }

        //Debug.Log(showing);

	}
}
