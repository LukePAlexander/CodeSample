using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    public float CameraMoveSpeed = 120.0f;
    public GameObject cameraFollowObj;
    Vector3 followPOS;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        CameraUpdater();
	}

    void CameraUpdater()
    {
        Transform target = cameraFollowObj.transform;

        float step = CameraMoveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }
}
