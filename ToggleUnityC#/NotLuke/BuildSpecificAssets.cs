using UnityEngine;
using System.Collections;

public class BuildSpecificAssets : MonoBehaviour {

	// Use this for initialization
	void Start () {
		#if !UNITY_ANDROID && !UNITY_IPHONE && !UNITY_BLACKBERRY && !UNITY_WINRT
            // Jumping (if tapped on the left side of the screen)
        transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
        #endif

        #if UNITY_ANDROID || UNITY_IPHONE || UNITY_BLACKBERRY || UNITY_WINRT
        transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
        #endif
	}
}
