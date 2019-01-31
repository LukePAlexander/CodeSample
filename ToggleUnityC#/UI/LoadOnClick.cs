using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadOnClick : MonoBehaviour {

	public void LoadByIndex(int sceneIndex){
		Debug.Log("Loading scene index: " + sceneIndex);	
		SceneManager.LoadScene(sceneIndex);
		Time.timeScale = 1;
	}
	public void LoadNextLevel(){
		Debug.Log("Going to next level!");
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}
	public void RestartLevel(){
		Debug.Log("Restarting Level");
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
	public void Quit(){
		Application.Quit();
	}
}
