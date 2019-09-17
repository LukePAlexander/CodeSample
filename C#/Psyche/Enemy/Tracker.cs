using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Tracker : MonoBehaviour {

    private GameObject Player;
    private readonly int sightDistance = 1000;
    public int MoveSpeed = 4;
    public float MinDist = 0;
    public float attackRange;
    public GameObject meleeBox;
    public GameObject tempBox;

    private Transform playerTrans;
    private RaycastHit hit;
    private bool attacking = false;
    public bool MoveWhileAttacking;
    private EnemyHealth myHealth;
    private LayerMask sightMask;
    private  NavMeshAgent NMAgent;

    // Use this for initialization
    void Start () {
        NMAgent = GetComponent<NavMeshAgent>();
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        myHealth = this.transform.GetComponent<EnemyHealth>();
        Player = Players[0];
        StartCoroutine(delayedUpdate());

        return;
	}

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator delayedUpdate()
    {
        while (isActiveAndEnabled)
        {
            //If stunned or dead, delay by stun duration or death duration and loop.
            if(myHealth.Stunned || myHealth.Dead)
            {
                //We don't update nav position here because we updated it in update.
                //If we're dead, wait for the amount of time MyHealth takes to destroy this object, currently one, if different in the future set to var and check var reference
                if (myHealth.Dead)
                {
                    yield return new WaitForSecondsRealtime(1);
                }
                yield return new WaitForSecondsRealtime(myHealth.StunDuration);
                continue;
            }



            //If we aren't attacking or move while attack, update where to move.
            if (!attacking || MoveWhileAttacking)
            {
                NMAgent.destination = Player.transform.position;
                
            //If we are attacking and don't move during it, the destination is here
            } else
            {
                NMAgent.destination = transform.position;
            }      

            //If within range, attack.
            if (Vector3.Distance(Player.transform.position, transform.position) <= attackRange)
            {
                Attack();
            }
            yield return new WaitForSecondsRealtime(.5f);
        } 
    }

    private void Attack()
    {
        
        //Double check we're not double attacking.
        if (attacking)
        {
            return;
        } else
        {
            attacking = true;
        }
        tempBox = Instantiate(meleeBox, transform.position + (transform.forward), transform.rotation, this.transform);
        tempBox.SetActive(true);
        tempBox.GetComponent<Weapon>().isEnemyWeapon = true;
        Physics.IgnoreCollision(tempBox.GetComponent<Collider>(), GetComponent<Collider>());
        Destroy(tempBox, 1);
        StartCoroutine(meleeBoxDeactivation());
    }

    IEnumerator meleeBoxDeactivation()
    {
        yield return new WaitForSecondsRealtime(1f);
        attacking = false;
    }
}
