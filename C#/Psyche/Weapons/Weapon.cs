using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public float damage;
    public bool isProjectile = false;
    public bool isEnemyWeapon = false;
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
            //print("Tag is: " + other.transform.tag);
            return;
        }

        if(other.transform.tag == "Player")
        {
            //print("Hit player !");

            PhaseManager PM = other.gameObject.GetComponent<PhaseManager>();

            PM.TakeDamage(damage);
        } else
        {
            if (!isEnemyWeapon)
            {
                print("Hit enemy!");
                EnemyHealth Enemy = other.gameObject.GetComponent<EnemyHealth>();

                Enemy.TakeDamage(damage);
            }
        }
        if (isProjectile)
        {
            Destroy(this.gameObject,.0f);
        }
    }
}
