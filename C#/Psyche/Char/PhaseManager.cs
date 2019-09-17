using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhaseManager : MonoBehaviour
{

    //Eventually want to turn to private, leave open for Artists to force phase/anim
    public int phase;
    //When Under phase2Threshold we go to phase2
    public int phase2Threshold;
    //When Under phase3Threshold we go to phase 3
    public int phase3Threshold;

    private GameObject HBar;
    private GameObject IBar;
    public GameObject GOText;

    private float maxHealth = 100f;
    private float curHealth;
    private float maxIns = 100f;
    private float curIns;
    public float RegenDelay;
    public int RegenAmountPerSecond;
    private float curDelay;

    private float restartTimer;
    private float restartDelay = 5f;
    public bool Stunned;

    private GameObject player;
    private Animator anim;
    private GameObject phase1;
    private Animator p1Anim;
    private RuntimeAnimatorController p1Controller;
    private Avatar p1Avatar;
    private GameObject phase2;
    private Animator p2Anim;
    private RuntimeAnimatorController p2Controller;
    private Avatar p2Avatar;

    private GameObject curPhaseObj;

    private AttackManager AM;

    ThirdPersonCharacter TPC;



    // Use this for initialization
    void Start()
    {

        AM = transform.gameObject.GetComponent<AttackManager>();
        //We should try to find a prev phase if we have different level loading,
        phase = 1;

        TPC = gameObject.GetComponent<ThirdPersonCharacter>();
        anim = GetComponent<Animator>();
        player = GameObject.Find("/PlayerPrefab/Player");
        phase1 = GameObject.Find("/PlayerPrefab/Player/Female");
        p1Anim = phase1.GetComponent<Animator>();
        p1Controller = p1Anim.runtimeAnimatorController;
        p1Avatar = p1Anim.avatar;
        phase2 = GameObject.Find("/PlayerPrefab/Player/Wraith");
        p2Anim = phase2.GetComponent<Animator>();
        p2Controller = p2Anim.runtimeAnimatorController;
        p2Avatar = p2Anim.avatar;

        curPhaseObj = phase1;

        //Set starting full health.
        curHealth = 100f;
        curIns = 100f;


        HBar = GameObject.Find("HealthBar");
        IBar = GameObject.Find("InsanityBar");
    }

    // Update is called once per frame
    void Update()
    {
        //If curDelay is greater than zero, decrement the time.
        if (curDelay > 0)
        {
            curDelay -= Time.deltaTime;

            //If the curDelay is less than 0 and the health isn't full, regen health.
        } else if (curHealth < 100) 
        {
            //We relate the amount to the time.deltaTime so that if update works faster or slower we regen the same amount.
            curHealth += (RegenAmountPerSecond * Time.deltaTime);
            RefreshHealthAndIns();
        }

        //Game Dev change phase cheats
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            if(phase == 1)
            {
                return;
            } else
            {
                phase--;
                print("phase-- is now :" + phase);

                ChangeToPhase(phase);
            }
        }
        //Game Dev change phase cheats
        if (Input.GetKeyDown(KeyCode.Period))
        {

            if (phase == 3)
            {
                return;
            }
            else
            {
                phase++;
                print("phase++ is now :" + phase);

                ChangeToPhase(phase);
            }
        }

        return;
    }

    public void SetStunned(float duration)
    {
        //Stunned is read by attackmanager, no further needed, however we keep stunned here
        //because it's associated with explosions or taking damage.
        Stunned = true;
        StartCoroutine(RemoveStunned(duration));
    }

    IEnumerator RemoveStunned(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        Stunned = false;
    }

    //Add insanity to our bar. 
    public void addIns(float howMuch)
    {
        curIns = curIns + howMuch;
        
        if(curIns > 100)
        {
            curIns = 100;
        } else if(curIns < 0)
        {
            curIns = 0;
        }

        checkPhaseTransition();
        RefreshHealthAndIns();

    }


    public void TakeDamage(float howMuch)
    {
        if (howMuch > curHealth)
        {
            curHealth = 0;
        }
        else
        {
            curHealth -= howMuch;
        }
        if (howMuch > curIns)
        {
            curIns = 0;
        }
        else
        {
            curIns -= howMuch;
        }
        curDelay = RegenDelay;


        checkPhaseTransition();

        RefreshHealthAndIns();

        //Handle Death
        if (curHealth <= 0)
        {
            die();
        }
    }

    private void checkPhaseTransition()
    {
    
        if (curIns > phase2Threshold && phase != 1)
        {
            curPhaseObj.SetActive(false);

            phase1.SetActive(true);
            anim.runtimeAnimatorController = p1Controller;
            anim.avatar = p1Avatar;
            phase = 1;
            curPhaseObj = phase1;

            //If damaged under the threshold, enter phase 2
        }
        else if (curIns <= phase2Threshold && curIns > phase3Threshold && phase != 2)
        {
            curPhaseObj.SetActive(false);

            phase2.SetActive(true);
            anim.runtimeAnimatorController = p2Controller;
            anim.avatar = p2Avatar;
            phase = 2;
            curPhaseObj = phase2;

            //Same to enter phase 3
        }

        //Don't have phase 3 models yet, so set to 2.
        else if (curIns <= phase3Threshold && phase != 3)
        {
            curPhaseObj.SetActive(false);

            phase2.SetActive(true);
            anim.runtimeAnimatorController = p2Controller;
            anim.avatar = p2Avatar;
            phase = 3;
        }
    }

    //Manipulate the scale of the colored sprite making up the bar in order to indicate the current health and insanity. 
    private void RefreshHealthAndIns()
    {
        HBar.transform.localScale = new Vector3((curHealth / maxHealth), IBar.transform.localScale.y, 1);
        IBar.transform.localScale = new Vector3((curIns / maxIns), IBar.transform.localScale.y, 1);
    }


    //Depreciated except for cheat changes, keep until models/anims fully implemented.
    private void ChangeToPhase(int newPhase)
    {
        curPhaseObj.SetActive(false);
        if (newPhase == 1)
        {
            phase1.SetActive(true);
            anim.runtimeAnimatorController = p1Controller;
            anim.avatar = p1Avatar;
            phase = 1;
            curPhaseObj = phase1;
        }
        else if (newPhase == 2)
        {
            phase2.SetActive(true);
            anim.runtimeAnimatorController = p2Controller;
            anim.avatar = p2Avatar;
            phase = 2;
            curPhaseObj = phase2;

        }
        //Don't have phase 3 yet, so set to 2
        else if (newPhase == 3)
        {
            curPhaseObj.SetActive(false);
            phase2.SetActive(true);
            anim.runtimeAnimatorController = p2Controller;
            anim.avatar = p2Avatar;
            phase = 3;
        }
    }

    private void die()
    {
        GOText.SetActive(true);

        TPC.MovementLocked = true;
        TPC.RotationLocked = true;
        AM.CanAttack = false;
        while (true)
        {   
            // .. increment a timer to count up to restarting.
            restartTimer += Time.deltaTime;

            // .. if it reaches the restart delay...
            if (restartTimer >= restartDelay)
            {
                // .. then reload the currently loaded level.
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            }
            else
            {
                return;
            }
        }
    }
   
}
