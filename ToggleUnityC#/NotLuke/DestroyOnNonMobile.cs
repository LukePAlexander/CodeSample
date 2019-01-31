using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class DestroyOnNonMobile : MonoBehaviour {

	GameObject JumpButton;
	GameObject ToggleButton;
	// Use this for initialization
	void Start () {
		#if !UNITY_ANDROID && !UNITY_IPHONE && !UNITY_BLACKBERRY && !UNITY_WINRT
		Destroy(this.gameObject);		
		#endif

		JumpButton = gameObject.transform.GetChild(0).gameObject;
		ToggleButton = gameObject.transform.GetChild(1).gameObject;
	
	}

	// public void OnPointerDown(PointerEventData data) {
	// 	Debug.Log("pressed");
	// 	if (data.selectedObject.name == "JumpBtn") {
	// 		GameObject.Find("Player_2D").GetComponent<PlayerMovement>().Jump();	
	// 	} else if (data.selectedObject.name == "ToggleBtn") {
	// 		GameObject.Find("Player_2D").GetComponent<PlayerMovement>().Toggle();
	// 	}
	// }
}
