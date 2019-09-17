using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tentacleShot : MonoBehaviour {

    private GameObject callBackObject;
    public bool IsEnemyWeapon = false;
	// Use this for initialization
	void Start () {
        //Grab the callback object, what fired us, and then set it to null so we aren't inhibited by it's movement.
        callBackObject = this.gameObject.transform.parent.gameObject;
        this.gameObject.transform.parent = null;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Player" && other.tag != "Enemy")
        {
            return;
        }

        if(other.tag == "Enemy" && !IsEnemyWeapon)
        {
            other.GetComponent<EnemyHealth>().Stunned = true;
            other.GetComponent<Rigidbody>().velocity = Vector3.zero;
            callBackObject.GetComponent<AttackManager>().HandleGrab(other.gameObject);
        }

    }
}
