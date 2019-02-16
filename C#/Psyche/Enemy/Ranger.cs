using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger : MonoBehaviour {


    private GameObject Player;
    private readonly int sightDistance = 1000;
    public int MoveSpeed = 4;
    public float MinDist = 2;
    public float MaxDist = 0;

    public GameObject meleeBox;
    public bool Dead = false;

    private Transform playerTrans;
    private RaycastHit hit;
    private GameObject tempShot = null;
    private EnemyHealth myHealth;


    public GameObject Bullet;
    public int delayToDestroyBullet = 5;
    public int ShotSpeed;
    private bool attacking = false;

    // Use this for initialization
    void Start()
    {
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
        transform.LookAt(playerTrans.position + new Vector3(0, 1, 0));

        Vector3 NextDirection = new Vector3(transform.forward.x, 0, transform.forward.z);

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, sightDistance))
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
        tempShot = Instantiate(Bullet, this.transform.position + (transform.forward / 2) + new Vector3(0, .8f, 0), transform.rotation, null);
        tempShot.SetActive(true);
        tempShot.GetComponent<Rigidbody>().velocity = tempShot.transform.forward * 2;
        tempShot.transform.rotation = new Quaternion(tempShot.transform.rotation.x, 0, tempShot.transform.rotation.z, tempShot.transform.rotation.w);
        Physics.IgnoreCollision(tempShot.GetComponent<Collider>(), GetComponent<Collider>());
        Destroy(tempShot, delayToDestroyBullet);
        StartCoroutine(AttackDeactivation());
    }

    IEnumerator AttackDeactivation()
    {
        yield return new WaitForSecondsRealtime(delayToDestroyBullet);
        attacking = false;
    }
}
