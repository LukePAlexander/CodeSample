using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TrackerScript : MonoBehaviour {

    public float topDistance;

	Transform startBlock;
	Transform endPoint;
	Slider progBar;
	SaveScript Saver;
	
	// Use this for initialization
	void Start () {
		startBlock = GameObject.Find("PlatformManager").GetComponent<BrickManager>().FirstPlatform.transform;
		endPoint = GameObject.Find("EndPoint").transform;
		progBar = GameObject.Find("HUDCanvas").transform.GetChild(3).GetComponent<Slider>();


		progBar.maxValue = 1;
        topDistance = SaveScript.TMD.progress(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("Top Distance on Level #" + (SceneManager.GetActiveScene().buildIndex - 1) + ": " + topDistance*10 + "%");
	}
	
	// Update is called once per frame
	void Update () {
		progBar.value = transform.position.x/endPoint.position.x;

        //If the top distance has been passed by the value/max value then save progress
        //Debug.Log(100*(progBar.value / progBar.maxValue) + "<" + topDistance);
        
        if(100*(progBar.value/progBar.maxValue) > topDistance)
        {
            float newTop = 100* (progBar.value / progBar.maxValue);
            //Debug.Log("New Max =" + newTopInt);
            SaveScript.TMD.saveProgress(SceneManager.GetActiveScene().buildIndex, progBar.value);
        }
	}
}
