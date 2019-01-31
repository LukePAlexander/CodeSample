using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour {

	GameObject ActiveHUD;
	GameObject PauseHUD;
	GameObject GameOverHUD;

	void Awake (){
		ActiveHUD = transform.GetChild(0).gameObject;
		PauseHUD = transform.GetChild(1).gameObject;
		GameOverHUD = transform.GetChild(2).gameObject;
	}

	// Use this for initialization
	void Start () {
		PauseHUD.SetActive(false);
		GameOverHUD.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		#if !UNITY_ANDROID && !UNITY_IPHONE && !UNITY_BLACKBERRY && !UNITY_WINRT	
		if (Input.GetButtonDown("Cancel")){
			if (ActiveHUD.activeSelf){
				Time.timeScale = 0;
				ActiveHUD.SetActive(false);
				PauseHUD.SetActive(true);
			} else {	
				Time.timeScale = 1;
				ActiveHUD.SetActive(true);
				PauseHUD.SetActive(false);
			}
		}
		#endif
	}

	public void PauseMenu(){
		if (GameOverHUD.activeSelf == false){
			if (ActiveHUD.activeSelf){
				Time.timeScale = 0;
				ActiveHUD.SetActive(false);
				PauseHUD.SetActive(true);					
			} else {
				Time.timeScale = 1;
				transform.GetChild(0).gameObject.SetActive(true);
				PauseHUD.SetActive(false);		
				
			}			
		}
	}
	public void GameOver(){
		GameOverHUD.SetActive(true);
	}
}
