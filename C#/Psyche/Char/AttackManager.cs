using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour {


    public GameObject curBullModel;
    public GameObject Sniperbullet;
    public GameObject meleeBox;
    public GameObject AOEPrefab;

    public float BulletDamage;
    public bool shotgunEquipped = false;
    public int numPellets = 5;
    public int CurShotSpeed;
    public int p1ShotSpeed;
    public int p2ShotSpeed;
    public int shotgunSpeed;
    public float shotgunDelay;
    public float sniperDelay;
    private float shotCD;



    private GameObject AOEEffect;
    private GameObject tempBox;
    private GameObject tempShot;
    private int numHits;
    private Vector3 curRotate;

    Animator anim;


    private Animator m_Animator;
    //When the melee animation changes, change this so the box only spawns for this long.
    private float meleeAnimDuration = .633f;
    private bool meleeing = false;

    private PhaseManager PM;
    private GameObject curMeleeBox;




    // Use this for initialization
    void Start()
    {
        PM = transform.gameObject.GetComponent<PhaseManager>();
        anim = GetComponent<Animator>();

        //Change this if we want to start with different ammo
        curBullModel = Sniperbullet;
        CurShotSpeed = p1ShotSpeed;

    }

    // Update is called once per frame
    void Update()
    {
        if (shotCD > 0)
        {
            shotCD -= Time.deltaTime;
            return;
        }

        //Attack button Mouse
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            switch (PM.phase)
            {
                case 1:
                    rangedAttack();
                    break;
                case 2:
                    meleeAttack();
                    break;
                case 3:
                    AOEAttack();
                    break;
                default:
                    print("Phase is invalid,");
                    break;

            }
        }
    }

    //AOE attack, spawns aoe sphere at feet, sphere collider should proc damage on enemies.
    public void AOEAttack()
    {
        //print("Aoe");
        AOEEffect = Instantiate(AOEPrefab, this.transform.position + new Vector3(0, .8f, 0), transform.rotation, this.transform);
        AOEEffect.SetActive(true);
        Physics.IgnoreCollision(AOEEffect.GetComponent<Collider>(), GetComponent<Collider>());

        Destroy(AOEEffect, .5f);
    }

    //grabbed from controller.cs, changed so instantiate starts at player pos with player rotation.
    //Ranged Attack, spawns bullet firing away from player, bullet collider should proc damage on enemies.
    public void rangedAttack()
    {
        //print("Ranged");
        if (shotgunEquipped)
        {
            shotgun();
            return;
        }
        else
        {
            sniper();
        }

    }

    //Spawns one bullet facing towards parent's forward, sets velocity to curShotSpeed
    private void sniper()
    {
        shotCD = sniperDelay;
        tempShot = Instantiate(curBullModel, this.transform.position + (transform.forward / 2) + new Vector3(0, .8f, 0), transform.rotation, null);
        tempShot.SetActive(true);
        tempShot.GetComponent<Rigidbody>().velocity = tempShot.transform.forward * CurShotSpeed;
        Physics.IgnoreCollision(tempShot.GetComponent<Collider>(), GetComponent<Collider>());
    }


    //Spawn numPellets bullets, each firing forward after being turned randrange 10 degrees left or right.
    private void shotgun()
    {
        shotCD = shotgunDelay;
        for (int i = 0; i <= numPellets; i++)
        {
            tempShot = Instantiate(curBullModel, this.transform.position + (transform.forward / 2) + new Vector3(0, .8f, 0), transform.rotation, null);
            tempShot.SetActive(true);
            Weapon Weap = tempShot.GetComponent<Weapon>();
            Weap.damage = BulletDamage / 2;
            tempShot.transform.Rotate(0, UnityEngine.Random.Range(-10, 10), 0);

            tempShot.GetComponent<Rigidbody>().velocity = tempShot.transform.forward * shotgunSpeed;
            Physics.IgnoreCollision(tempShot.GetComponent<Collider>(), GetComponent<Collider>());
        }
        return;
    }


    //Also grabbed from controller.cs.
    //Melee Attack, currently spawns 'sword' box in front of player, 
    public void meleeAttack()
    {
        //print("melee");
        if (meleeing)
        {
            return;
        }
        meleeing = true;
        anim.SetTrigger("Attack");
        meleeBox.SetActive(true);
        Physics.IgnoreCollision(meleeBox.GetComponent<Collider>(), GetComponent<Collider>());
        //This extra curMelee is in case we phase change during an attack, save the old box to deactivate properly.
        StartCoroutine(meleeBoxDeactivation());
    }

    IEnumerator meleeBoxDeactivation()
    {
        yield return new WaitForSecondsRealtime(meleeAnimDuration);
        //We double check the attack hasn't been deactivated by a phase change.

        meleeBox.SetActive(false);
        
        meleeing = false;
    }



    //This will eventually hold a switch case that changes details about the shot we fire, such as the prefab and speed
    public void ChangeBulletType(string newType)
    {

    }

    
}
