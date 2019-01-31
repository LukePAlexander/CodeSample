using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class LevelSelectProgressTracking : MonoBehaviour {

	public float currentProgress;
	public int coinCount;

	Transform progBar;
	TextMesh coinTextMesh;
	SaveScript Saver;
	int buildIndex;
	// Use this for initialization
	void Start () {
		progBar = transform.GetChild(0);
		coinTextMesh = transform.GetChild(1).gameObject.GetComponent<TextMesh>();
		buildIndex = SceneManager.GetActiveScene().buildIndex;
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log("Level Select: " + buildIndex + " for Level :" + (buildIndex - 5));
		currentProgress = SaveScript.TMD.progress(buildIndex - 5);
		if (currentProgress == null){
			currentProgress = 0;
		}
		Debug.Log(currentProgress + " = " + currentProgress*100 + "%");
		progBar.localScale = new Vector3(currentProgress*100f*2.24f, progBar.localScale.y, progBar.localScale.z);		
		
		coinCount = SaveScript.TMD.coinProgress(buildIndex - 5);

		coinTextMesh.text = coinCount + "/5 Coins";
		
	}
}
