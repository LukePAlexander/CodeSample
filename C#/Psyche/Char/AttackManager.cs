using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour {


    public GameObject curBullModel;
    public GameObject meleeBox;
    public GameObject AOEPrefab;
    public GameObject MinePrefab;
    public GameObject FlashPrefab;
    public GameObject TrapPrefab;
    public GameObject MolotovPrefab;
    public GameObject WraithShroudPrefab;
    public GameObject MiasmaPrefab;
    public GameObject tentaclePrefab;
    public GameObject tentaclePrefab2;


    //After artists tweak, set to private, named as private
    public float bulletDamage;
    public float grabSlashDamage;
    public bool shotgunEquipped = false;
    public int numPellets = 5;
    public int numSprayBullets = 5;
    public int curShotSpeed;
    public int shotgunSpeed;
    public float throwPower;
    public float grabMinimumDistance;
    public float shotgunDelay;
    public float pistolSprayDelay;
    public float pistolSpraySpread;
    public float rangedDelay;
    public float sniperDelay;
    public float mineDelay;
    public float flashDelay;
    public float trapDelay;
    public float molotovDelay;
    public float dashSlashDelay;
    public float chargeSpeed;
    private float tentacleDelay;
    private float attackCD;
    private float abilityCD;
    private float attackCharge;
    private float dashChargeAmount = 1;
    private bool isCharging;
    private KeyCode chargingButton; 



    private GameObject AOEEffect;
    private GameObject tempShot;

    Animator anim;
    
    //When the melee animation changes, change this so the box only spawns for this long.
    private float meleeAnimDuration = .633f;
    private bool meleeing = false;
    public bool CanAttack = true;

    private PhaseManager PM;
    private ThirdPersonCharacter TPC;
    private GameObject curMeleeBox;




    // Use this for initialization
    void Start()
    {
        PM = transform.gameObject.GetComponent<PhaseManager>();
        anim = GetComponent<Animator>();
        TPC = GetComponent<ThirdPersonCharacter>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F))
        {
            PM.TakeDamage(30);
        }
        if (abilityCD > 0 || attackCD > 0)
        {
            abilityCD -= Time.deltaTime;
            attackCD -= Time.deltaTime;
        }

        if (gameObject.GetComponent<PhaseManager>().Stunned || !CanAttack)
        {
            return;
        }

        if (isCharging)
        {
            if (Input.GetKeyUp(chargingButton))
            {
                setNotCharging();

                //If we are still charging we don't want to allow attacks/abilities, so return.
            } else
            {
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            setCharging(KeyCode.Space, true, false);
            StartCoroutine(dashSlash());
            return;
        }

        //Attack button Left Mouse
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (attackCD > 0)
            {
                return;
            }
            switch (PM.phase)
            {
                case 1:
                    PM.addIns(-1f);
                    rangedAttack();
                    break;
                case 2:
                    PM.addIns(-2.5f);

                    meleeAttack();
                    break;
                case 3:
                    PM.addIns(-5f);

                    AOEAttack();
                    break;
                default:
                    print("Phase " + PM.phase + " is invalid for mouse 1");
                    break;

            }
        }

        //Ability button Right Mouse
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (abilityCD > 0)
            {
                return;
            }

            switch (PM.phase)
            {
                case 1:
                    PM.addIns(2.5f);

                    sniper();
                    break;
                case 2:
                    PM.addIns(2.5f);

                    StartCoroutine(PistolSpray());
                    break;
                case 3:
                    PM.addIns(5f);

                    tentacleGrabShot();
                    break;
                default:
                    print("Phase " + PM.phase + " is invalid for mouse 2");
                    break;
            }
        }

        //Ability 1 parse
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (abilityCD > 0)
            {
                return;
            }

            switch (PM.phase)
            {
                case 1:
                    PM.addIns(-1f);

                    shotgun();
                    break;
                case 2:
                    PM.addIns(7.5f);

                    molotov();
                    break;
                case 3:
                    PM.addIns(15f);
                    wraithShroud();
                    break;
                default:
                    print("Phase " + PM.phase + " is invalid for key 1");
                    break;
            }
        }

        //Ability 2 parse
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (abilityCD > 0)
            {
                return;
            }

            switch (PM.phase)
            {
                case 1:
                    PM.addIns(-1f);
                    mine();
                    break;
                case 2:
                    PM.addIns(-6f);
                    Miasma();
                    break;
                case 3:
                    PM.addIns(-15f);
                    setCharging(KeyCode.Alpha2, true, false);
                    StartCoroutine(dashSlash());
                    break;
                default:
                    print("Phase " + PM.phase + " is invalid for key 2");
                    break;
            }
        }

        //Ability 3 parse
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (abilityCD > 0)
            {
                return;
            }

            switch (PM.phase)
            {
                case 1:
                    flash();
                    break;
                case 2:
                    trap();
                    break;
                case 3:
                    wraithShroud();
                    break;
                default:
                    print("Phase " + PM.phase + " is invalid for key 3");
                    break;
            }
        }


    }

    private void setCharging(KeyCode inputKey, bool lockMove, bool lockRotation)
    {
        isCharging = true;
        if (lockMove)
        {
            TPC.MovementLocked = true;
        }
        if (lockRotation)
        {
            TPC.RotationLocked = true;
        }
        chargingButton = inputKey;
    }

    private void setNotCharging()
    {
        isCharging = false;
        TPC.MovementLocked = false;
        TPC.RotationLocked = false;        
        chargingButton = KeyCode.None;
    }

    private IEnumerator dashSlash()
    {
        while (attackCharge < dashChargeAmount)
        {
            //Hey Will, eventually we want to put some climbing variable here for the animation to play while charging various attacks. 
            //I imagine it like the melee attack layer except with a float like walking rather than a bool. - Luke
            yield return new WaitForFixedUpdate();
            attackCharge += chargeSpeed * Time.deltaTime;
        }
        attackCharge = 0;

        this.GetComponent<Rigidbody>().AddForce(this.transform.up * 10, ForceMode.Impulse);
        this.GetComponent<Rigidbody>().AddForce(this.transform.forward * 20, ForceMode.Impulse);

        TPC.CheckGroundStatus();

        meleeAttack(10);

        setNotCharging();
    }

    private void flash()
    {
        abilityCD = flashDelay;
        tempShot = Instantiate(FlashPrefab, this.transform.position + (transform.forward / 2) + new Vector3(0, .8f, 0), transform.rotation, null);
        tempShot.SetActive(true);
        tempShot.transform.Rotate(-45, 0, 0);
        Rigidbody RB = tempShot.GetComponent<Rigidbody>();
        RB.AddForce(transform.forward * throwPower + transform.up * throwPower, ForceMode.Impulse);
        RB.AddTorque(transform.right * 10f, ForceMode.Impulse);

        Physics.IgnoreCollision(tempShot.GetComponent<Collider>(), GetComponent<Collider>());
    }

    private void molotov()
    {
        print("Molo");
        abilityCD = molotovDelay;
        tempShot = Instantiate(MolotovPrefab, this.transform.position + (transform.forward / 2) + new Vector3(0, .8f, 0), transform.rotation, null);
        tempShot.SetActive(true);
        tempShot.transform.Rotate(-45, 0, 0);
        Rigidbody RB = tempShot.GetComponent<Rigidbody>();
        RB.AddForce(transform.forward * throwPower + transform.up * throwPower, ForceMode.Impulse);
        RB.AddTorque(transform.right * 10f, ForceMode.Impulse);

        Physics.IgnoreCollision(tempShot.GetComponent<Collider>(), GetComponent<Collider>());
    }

    private void mine()
    {
        abilityCD = mineDelay;
        tempShot = Instantiate(MinePrefab, this.transform.position + new Vector3(0, .5f, 0), transform.rotation, null);
        tempShot.SetActive(true);
        Physics.IgnoreCollision(tempShot.GetComponent<Collider>(), GetComponent<Collider>());
    }

    private void trap()
    {
        abilityCD = trapDelay;
        tempShot = Instantiate(TrapPrefab, this.transform.position + new Vector3(0,.5f,0), transform.rotation, null);
        tempShot.SetActive(true);
        Physics.IgnoreCollision(tempShot.GetComponent<Collider>(), GetComponent<Collider>());
    }

    //AOE attack, spawns aoe sphere at feet, sphere collider should proc damage on enemies.
    private void AOEAttack()
    {
        print("Aoe");
        AOEEffect = Instantiate(AOEPrefab, this.transform.position, transform.rotation, this.transform);
        Destroy(AOEEffect, 2f);
    }

    private void wraithShroud()
    {
        print("Shroud");
        AOEEffect = Instantiate(WraithShroudPrefab, this.transform.position, transform.rotation, this.transform);
        Destroy(AOEEffect, 2f);
    }

    //grabbed from controller.cs, changed so instantiate starts at player pos with player rotation.
    //Ranged Attack, spawns bullet firing away from player, bullet collider should proc damage on enemies.
    private void rangedAttack()
    {
        attackCD = rangedDelay;
        anim.SetFloat("ASM", 2);
        anim.SetTrigger("Attack");
        tempShot = Instantiate(curBullModel, this.transform.position + (transform.forward / 2) + new Vector3(0, .7f, 0), transform.rotation, null);
        tempShot.SetActive(true);
        tempShot.GetComponent<Rigidbody>().velocity = tempShot.transform.forward * curShotSpeed;
        Physics.IgnoreCollision(tempShot.GetComponent<Collider>(), GetComponent<Collider>());
    }

    //Spawns one bullet facing towards parent's forward, sets velocity to curShotSpeed
    private void sniper()
    {
        attackCD = sniperDelay;
        tempShot = Instantiate(curBullModel, this.transform.position + (transform.forward / 2) + new Vector3(0, .8f, 0), transform.rotation, null);
        tempShot.SetActive(true);
        tempShot.GetComponent<Weapon>().shouldDissapate = false;
        tempShot.GetComponent<Rigidbody>().velocity = tempShot.transform.forward * curShotSpeed * 3;
        Physics.IgnoreCollision(tempShot.GetComponent<Collider>(), GetComponent<Collider>());
    }

    private IEnumerator PistolSpray()
    {
        TPC.RotationLocked = true;
        abilityCD = pistolSprayDelay;
        for (int i = 0; i <= numSprayBullets; i++)
        {
            tempShot = Instantiate(curBullModel, this.transform.position + (transform.forward / 2) + new Vector3(0, .8f, 0), transform.rotation, null);
            tempShot.SetActive(true);
            Weapon Weap = tempShot.GetComponent<Weapon>();
            Weap.damage = bulletDamage / 2;
            tempShot.transform.Rotate(0, (i * pistolSpraySpread) - (numSprayBullets/2 * pistolSpraySpread), 0);

            tempShot.GetComponent<Rigidbody>().velocity = tempShot.transform.forward * curShotSpeed;
            Physics.IgnoreCollision(tempShot.GetComponent<Collider>(), GetComponent<Collider>());
            yield return new WaitForSecondsRealtime(pistolSprayDelay/numSprayBullets);
        }
        TPC.RotationLocked = false;
    }


    //Spawn numPellets bullets, each firing forward after being turned randrange 10 degrees left or right.
    private void shotgun()
    {
        abilityCD = shotgunDelay;
        for (int i = 0; i <= numPellets; i++)
        {
            tempShot = Instantiate(curBullModel, this.transform.position + (transform.forward / 2) + new Vector3(0, .8f, 0), transform.rotation, null);
            tempShot.SetActive(true);
            Weapon Weap = tempShot.GetComponent<Weapon>();
            Weap.damage = bulletDamage / 2;
            tempShot.transform.Rotate(0, UnityEngine.Random.Range(-10, 10), 0);

            tempShot.GetComponent<Rigidbody>().velocity = tempShot.transform.forward * shotgunSpeed;
            Physics.IgnoreCollision(tempShot.GetComponent<Collider>(), GetComponent<Collider>());
        }
        return;
    }

    private void Miasma()
    {
        //print("Aoe");
        AOEEffect = Instantiate(MiasmaPrefab, this.transform.position + new Vector3(0, .8f, 0), transform.rotation, null);
    }
    //Also grabbed from controller.cs.
    //Melee Attack, currently spawns 'sword' box in front of player, 
    private void meleeAttack()
    {
        attackCD = meleeAnimDuration;
        anim.SetTrigger("Attack");
        meleeBox.SetActive(true);
        Physics.IgnoreCollision(meleeBox.GetComponent<Collider>(), GetComponent<Collider>());
        StartCoroutine(meleeBoxDeactivation());
    }

    private void meleeAttack(int damage)
    {
        attackCD = meleeAnimDuration;
        anim.SetTrigger("Attack");
        meleeBox.SetActive(true);
        Physics.IgnoreCollision(meleeBox.GetComponent<Collider>(), GetComponent<Collider>());
        meleeBox.GetComponent<Weapon>().damage = grabSlashDamage;
        StartCoroutine(meleeBoxDeactivation());
    }

    IEnumerator meleeBoxDeactivation()
    {
        yield return new WaitForSecondsRealtime(meleeAnimDuration);
        meleeBox.SetActive(false);        
    }

    //Unused as of yet, i liked shot better. if team does as well delete this
    private void tentacleGrab()
    {
        attackCD = tentacleDelay;
        tempShot = Instantiate(tentaclePrefab, this.transform.position + (transform.forward / 2) + new Vector3(0, .8f, 0), transform.rotation, this.gameObject.transform);
        Physics.IgnoreCollision(tempShot.GetComponent<Collider>(), GetComponent<Collider>());
    }

    private void tentacleGrabShot()
    {
        attackCD = tentacleDelay;
        tempShot = Instantiate(tentaclePrefab2, this.transform.position + (transform.forward / 2) + new Vector3(0, .8f, 0), transform.rotation, this.gameObject.transform);
        Physics.IgnoreCollision(tempShot.GetComponent<Collider>(), GetComponent<Collider>());
        tempShot.GetComponent<Rigidbody>().velocity = tempShot.transform.forward * curShotSpeed;
    }

    public void HandleGrab(GameObject Grabbed)
    {
        this.GetComponent<ThirdPersonCharacter>().MovementLocked = true;
        this.GetComponent<ThirdPersonCharacter>().RotationLocked = true;
        //If Grabbed has no rigidbody we don't care about it, so return
        if(Grabbed.GetComponent<Rigidbody>() == null)
        {
            return;
        }

        StartCoroutine(pullGrabbed(Grabbed));
    }

    //Pull the character 
    IEnumerator pullGrabbed(GameObject Grabbed)
    {
        while(Vector3.Distance(Grabbed.transform.position, this.transform.position) > grabMinimumDistance)
        {
            Vector3 GrabbedToUs = this.transform.position - Grabbed.transform.position;

            GrabbedToUs.Normalize();
            yield return new WaitForSecondsRealtime(.1f);

            Grabbed.GetComponent<Rigidbody>().AddForce(GrabbedToUs * 10, ForceMode.Impulse);
        }
        Grabbed.GetComponent<Rigidbody>().velocity = Vector3.zero;
        meleeAttack(10);

        yield return new WaitForSecondsRealtime(meleeAnimDuration);

        pullFinish(Grabbed);
    }

    private void pullFinish(GameObject Grabbed)
    {
        this.GetComponent<ThirdPersonCharacter>().MovementLocked = false;
        this.GetComponent<ThirdPersonCharacter>().RotationLocked = false;

        Grabbed.GetComponent<EnemyHealth>().Stunned = false;
    }
}
