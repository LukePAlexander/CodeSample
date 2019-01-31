using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Catalog_BG : MonoBehaviour {

	public static Catalog_BG Instance { get { return instance; } }
	private static Catalog_BG instance = null;

	public static AudioClip MainMenu;
	public static AudioClip Level0;
	public static AudioClip Level1;
	public static AudioClip Level2;
	public static AudioClip Level3;
	public static AudioClip Level4;
	public static AudioClip Level5;

	static AudioSource audioSource;
	private int currentLevel;

	void Start(){
		currentLevel = SceneManager.GetActiveScene().buildIndex;
		//Debug.Log("Current Level in Catalog: " + (currentLevel));
	}

	void Awake(){
		//Check if any other instance, 
        if (instance != null && instance != this)
        {
            //Debug.Log("Another instance detected, " + SaveScript.TMD.noOfDeath + " destroyed");
            
            Destroy(this.gameObject);
        } else {
            instance = this;
        }        
        
        DontDestroyOnLoad(gameObject);
		audioSource = GetComponent<AudioSource>();
	}

	public static void UpdateMusic(int buildIndex){
		// AudioSource oldAudioSource = GetComponent<AudioSource>();
		// Debug.Log("oldAudioSource.clip: " + oldAudioSource.clip);
		// Destroy(oldAudioSource);
		// gameObject.AddComponent<AudioSource>().playOnAwake = false;
		// Debug.Log(audioSource);
		
		if (audioSource == null){
			Debug.Log("audioSource is null, but y tho");
		} else {
	        if (buildIndex == 0){
	            audioSource.clip = MainMenu;
	            audioSource.Play();
	        } else if (buildIndex == 1){
	            audioSource.clip = Level0;
	            audioSource.Play(); 
	        } else if (buildIndex == 2){
	        	Debug.Log("Recognized new  level...");
	        	//audioSource.enabled = false;
	        	Debug.Log("Audio should have stopped");
	            audioSource.clip = Level1;
				Debug.Log("new audioSource.clip: " + audioSource.clip);
	            audioSource.Play();
	        	//audioSource.enabled = true;
	        } else if (buildIndex == 3){
	            audioSource.clip = Level2;
	            audioSource.Play();
	        } else if (buildIndex == 4){
	            audioSource.clip = Level3;
	            audioSource.Play();
	        } else if (buildIndex == 5){
	            audioSource.clip = Level4;
	            audioSource.Play();
	        } else if (buildIndex == 6){
	            audioSource.clip = Level5;
	            audioSource.Play();
	        }
			
		}

	}

}
