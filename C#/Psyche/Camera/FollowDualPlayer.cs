using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowDualPlayer : MonoBehaviour
{

    private Vector3 curVelocity;
    public float smoothTime;
    public float CameraMoveSpeed = .75f;
    public GameObject cameraFollowObj1;
    public GameObject cameraFollowObj2;
    Vector3 followPOS;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CameraUpdater();
    }

    void CameraUpdater()
    {
        Transform target1 = cameraFollowObj1.transform;
        Transform target2;
        float step = CameraMoveSpeed * Time.deltaTime;

        if (cameraFollowObj2 != null)
        {
            target2 = cameraFollowObj2.transform;
            transform.position = Vector3.MoveTowards(transform.position, (target1.position + target2.position) / 2, step);
            return;
        }

        //print(target1.position.x);
        //Hey this may need to change but ffs when there's no /2 on target1.pos then it just flings the char off the left side.
        if(Vector3.Distance(transform.position, target1.position) < .2f)
        {
            return;
        }
        smoothTime = CameraMoveSpeed / Vector3.Distance(transform.position, target1.position);
        
        //transform.position = Vector3.MoveTowards(transform.position, target1.position, step);
        transform.position = Vector3.SmoothDamp(transform.position, target1.position, ref curVelocity, smoothTime);
    }
}
