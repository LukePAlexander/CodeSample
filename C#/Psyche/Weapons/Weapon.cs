using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Weapon : MonoBehaviour {

    public float damage;
    public float ForceOfBullet = 100000;
    public bool shouldDissapate = false;
    public bool isEnemyWeapon = false;
    public bool isPlayerWeapon = false;
    public bool applyForce;
    private GameObject EO;
    //Type?
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        //print("Triggered!");
        if (other.transform.tag != "Player" && other.transform.tag != "Enemy")
        {
            if (shouldDissapate)
            {
                Destroy(this.gameObject, .0f);
            }
            //print("Tag is: " + other.transform.tag);
            return;
        }

        if(other.transform.tag == "Player")
        {
            //print("Hit player !");
            if (!isPlayerWeapon)
            {
                PhaseManager PM = other.gameObject.GetComponent<PhaseManager>();

                PM.TakeDamage(damage);
            }
        } else
        {
            if (!isEnemyWeapon)
            {
                EnemyHealth Enemy = other.gameObject.GetComponent<EnemyHealth>();

                Enemy.TakeDamage(damage);

                if (applyForce)
                {
                    Enemy.PushBack(ForceOfBullet, this.GetComponent<Rigidbody>().velocity);
                }
            }
        }
        if (shouldDissapate)
        {
            Destroy(this.gameObject,.0f);
        }
    }

}

