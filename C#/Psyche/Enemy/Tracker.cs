using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker : MonoBehaviour {

    private GameObject Player;
    private readonly int sightDistance = 1000;
    public int MoveSpeed = 4;
    public float MinDist = 0;
    public GameObject meleeBox;
    public GameObject tempBox;

    private Transform playerTrans;
    private RaycastHit hit;
    private bool attacking = false;
    private EnemyHealth myHealth;

    // Use this for initialization
    void Start () {
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        myHealth = this.transform.GetComponent<EnemyHealth>();
        Player = Players[0];
	}

    // Update is called once per frame
    void Update()
    {
        if (myHealth.Dead)
        {
            return;
        }
        playerTrans = Player.transform;
        transform.LookAt(playerTrans.position + new Vector3(0,1,0));


        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, sightDistance))
        {
            if (hit.collider.gameObject.tag != "Player")
            {
                return;
            }

            if (Vector3.Distance(transform.position, playerTrans.position) >= MinDist || attacking)
            {

                transform.position += transform.forward * MoveSpeed * Time.deltaTime;

            } else
            {
                Attack();
            }
        }
    }

    private void Attack()
    {
        if (attacking)
        {
            return;
        } else
        {
            attacking = true;
        }
        tempBox = Instantiate(meleeBox, transform.position + (transform.forward), transform.rotation, this.transform);
        tempBox.SetActive(true);
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
