using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ranger : MonoBehaviour {


    private GameObject Player;
    private readonly int sightDistance = 1000;
    public float attackRangeMin;
    public float attackRangeMax;
    public int MoveSpeed = 4;
    public float MinDist = 2;
    public float MaxDist = 0;

    private Transform playerTrans;
    private RaycastHit hit;
    private GameObject tempShot = null;
    private EnemyHealth myHealth;



    public GameObject Bullet;
    public int delayToDestroyBullet = 5;
    public int ShotSpeed;
    private bool attacking = false;
    public bool MoveWhileAttacking;

    private LayerMask sightMask;
    private NavMeshAgent NMAgent;


    // Use this for initialization
    void Start()
    {
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
        return;
        if (myHealth.Dead || myHealth.Stunned)
        {
            return;
        }
        playerTrans = Player.transform;
        transform.LookAt(playerTrans.position + new Vector3(0, 1, 0));

        Vector3 NextDirection = new Vector3(transform.forward.x, 0, transform.forward.z);

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, sightDistance, sightMask))
        {
            if (hit.collider.gameObject.tag != "Player")
            {
                return;
            }

            //print("Hit player");
            if (Vector3.Distance(transform.position, playerTrans.position) <= MinDist)
            {
                transform.position -= NextDirection * MoveSpeed * 2 * Time.deltaTime;
            }
             else if (Vector3.Distance(transform.position, playerTrans.position) >= MaxDist)
            {
                transform.position += NextDirection * MoveSpeed * Time.deltaTime;
            }
            else
            {
                Attack();
            }
        }
    }



    IEnumerator delayedUpdate()
    {
        while (isActiveAndEnabled)
        {
            //If stunned or dead, delay by stun duration or death duration and loop.
            if (myHealth.Stunned || myHealth.Dead)
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

            //If within range, attack.
            if (Vector3.Distance(Player.transform.position, transform.position) >= attackRangeMin && Vector3.Distance(Player.transform.position, transform.position) <= attackRangeMax)
            {
                Attack();
            }

            //If we aren't attacking or move while attack, update where to move.
            if (!attacking || MoveWhileAttacking)
            {
                //If out of max range, get closer
                if (Vector3.Distance(Player.transform.position, transform.position) >= attackRangeMax)
                {
                    NMAgent.destination = Player.transform.position;
                }
                //If within min range, check if backwards is a valid point, then move backwards if possible.
                else if (Vector3.Distance(Player.transform.position, transform.position) <= attackRangeMin)
                {
                    Vector3 awayVec = transform.position - Player.transform.position;
                    awayVec.Normalize();

                    Vector3 awayPoint = transform.position + awayVec;

                    NavMeshHit navHit;

                    bool isValid = NavMesh.Raycast(awayPoint, awayPoint - (Vector3.down * 2), out navHit, NavMesh.AllAreas);

                    NMAgent.destination = navHit.position;

                    Debug.DrawRay(navHit.position, Vector3.down * 2, Color.green, 2f);

                }

                //If we are attacking and don't move during it, the destination is here
            }
            else
            {
                NMAgent.destination = transform.position;
            }


            yield return new WaitForSecondsRealtime(.5f);
        }
    }

    private void Attack()
    {
        if (attacking)
        {
            //print("Already attacking");
            return;
        } else
        {
            attacking = true;
        }


        playerTrans = Player.transform;

        tempShot = Instantiate(Bullet, this.transform.position + (transform.forward / 2) + new Vector3(0, .8f, 0), transform.rotation, null);
        tempShot.SetActive(true);
        Vector3 relativePos = Player.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);

        tempShot.transform.rotation = rotation;
        tempShot.GetComponent<Rigidbody>().AddForce(relativePos.normalized * ShotSpeed, ForceMode.Impulse);
        //tempShot.GetComponent<Rigidbody>().velocity = tempShot.transform.forward * 2;
        Physics.IgnoreCollision(tempShot.GetComponent<Collider>(), GetComponent<Collider>());
        Destroy(tempShot, delayToDestroyBullet);
        StartCoroutine(AttackDeactivation());
    }

    IEnumerator AttackDeactivation()
    {
        yield return new WaitForSecondsRealtime(.5f);
        attacking = false;
    }
}
