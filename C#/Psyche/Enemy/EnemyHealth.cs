using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour {

    public float StartHealth;
    public float CurHealth;
    public string MyName;
    //Use this for invuln window later
    //private bool isDamaged = false;
    public bool Dead = false;
    public bool Stunned;
    public float StunDuration;
    public class EnemyEvent : UnityEvent<string> { } //empty class; just needs to exist
    public SpawnerMeta SpawnerMeta;
    private int timer;
    private float onFire;
    private int colorChanged = 0;
    private Color storageColor;
    private NavMeshAgent NMA;
    private bool knockedBack;
    private Vector3 curKnockback;

    // Use this for initialization
    void Start () {
        CurHealth = StartHealth;
        storageColor = GetComponentInChildren<MeshRenderer>().material.color;
        NMA = GetComponent<NavMeshAgent>();
    }

// Update is called once per frame
void Update () {
		if(onFire > 0)
        {
            onFire -= Time.deltaTime;
            TakeDamage(1 * Time.deltaTime);
        } else if(knockedBack)
        {
            if (this.GetComponent<Rigidbody>().velocity.x < .2f && this.GetComponent<Rigidbody>().velocity.y < .2f)
            {
                knockedBack = false;
                this.GetComponent<NavMeshAgent>().enabled = true;
            } else
            {
                this.GetComponent<Rigidbody>().velocity += curKnockback;
                curKnockback = curKnockback / this.GetComponent<Rigidbody>().drag;
            }
        }
	}

    public void SetStartingHealth(float newHealth){
        CurHealth = newHealth;
    }


    public void SetFire(float duration)
    {
        onFire = duration;
    }

    public void SetStunned(float duration)
    {
        Stunned = true;
        StunDuration = duration;
        NMA.destination = transform.position;
        StartCoroutine(RemoveStunned(duration));
    }

    IEnumerator RemoveStunned(float duration)
    {
        MeshRenderer myMesh = GetComponentInChildren<MeshRenderer>();
        myMesh.material.color = Color.yellow;
        colorChanged++;

        yield return new WaitForSecondsRealtime(duration);
        Stunned = false;
        NMA.destination = GameObject.Find("Player").transform.position;
        StunDuration = 0;
        colorChanged--;

        if (colorChanged == 0)
        {
            myMesh.material.color = storageColor;
        }
    }

    public void TakeDamage(float damage)
    {
        if(damage == 0)
        {
            return;
        }
        CurHealth -= damage;
        StartCoroutine(DamageColor());
        
        if(CurHealth <= 0)
        {
            this.gameObject.GetComponent<Rigidbody>().freezeRotation = false;
            Dead = true;
            SpawnerMeta.OnEnemyDeath(MyName);

            Destroy(this.gameObject,1);
        }
    }

    //Wait a second, change back.
    private IEnumerator DamageColor()
    {
        MeshRenderer myMesh = GetComponentInChildren<MeshRenderer>();
        myMesh.material.color = Color.red;
        colorChanged++;

        //Debug.Break();
        yield return new WaitForSecondsRealtime(1);

        colorChanged--;

        if(colorChanged == 0)
        {
            myMesh.material.color = storageColor;
        }
    }

    private void updateVector(Vector3 newImpact)
    {
        curKnockback.x += newImpact.x;
        curKnockback.z += newImpact.z;
    }

    public void PushBack(float forceOfBullet, Vector3 newImpact)
    {

        NavMeshAgent navAgent = gameObject.GetComponent<NavMeshAgent>();
        navAgent.enabled = false;
        knockedBack = true;
        updateVector(newImpact * forceOfBullet);

        

        /*
        NavMeshAgent navAgent = gameObject.GetComponent<NavMeshAgent>();
        float speedStore, angularSpeed, acceleration;
        speedStore = navAgent.speed;
        angularSpeed = navAgent.angularSpeed;//Keeps the enemy facing forwad rther than spinning
        acceleration = navAgent.acceleration;

        navAgent.speed = 10;
        navAgent.angularSpeed = 0;//Keeps the enemy facing forwad rther than spinning
        navAgent.acceleration = 20;

        yield return new WaitForSeconds(0.2f); //Only knock the enemy back for a short time    

        //Reset to default values
        navAgent.speed = 4;
        navAgent.angularSpeed = 180;
        navAgent.acceleration = 10;
        */
    }
}
