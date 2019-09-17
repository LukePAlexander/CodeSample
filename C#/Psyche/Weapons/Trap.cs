using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour {

    public float Damage;
    public bool hitsPlayer = true;
    public bool hitsEnemy = true;
    public bool shouldDissapate = false;
    public float Duration;
    //Type?
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        //print("Triggered!");
        if (other.transform.tag != "Player" && other.transform.tag != "Enemy")
        {
            //print("Tag is: " + other.transform.tag);
            return;
        }

        if (other.transform.tag == "Player")
        {

            PhaseManager PM = other.gameObject.GetComponent<PhaseManager>();
            PM.TakeDamage(Damage);
            PM.SetStunned(Duration);
        }
        else
        {
            EnemyHealth Enemy = other.gameObject.GetComponent<EnemyHealth>();

            Enemy.TakeDamage(Damage);
            Enemy.SetStunned(Duration);            
        }

        if (shouldDissapate)
        {
            Destroy(this.gameObject, .0f);
        }
    }
}
