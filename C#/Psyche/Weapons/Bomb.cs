using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {
    public float BombRadius;
    public float Damage;
    public bool hitsPlayer = true;
    public bool hitsEnemy = true;
    public bool Flash;
    public bool Ignite;
    public bool DetonateOnGround;
    public bool IsPulseExplosion;
    public float FlashDuration;
    public float IgniteDuration;
    public float NumOfPulses;
    public float PulseDelay;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    //generate sphere centered on this and register damage with collisions
    private void explode()
    {
        //Create a sphere for the 'explosion' and proc damage on all collisions of appropriate type
        Collider[]
        hits = Physics.OverlapSphere(this.transform.position, BombRadius);
        Collider hit;

        for (int i = 0; i < hits.Length; i++)
        {
            hit = hits[i];
            if (hit.gameObject.tag == "Player")
            {
                PhaseManager PM = hit.gameObject.GetComponent<PhaseManager>();
                PM.TakeDamage(Damage);

                if (Flash)
                {
                    PM.SetStunned(FlashDuration);
                }
                if (Ignite)
                {
                    //Don't have ignite yet
                }
            }
            else if (hit.gameObject.tag == "Enemy")
            {
                EnemyHealth EH = hit.gameObject.GetComponent<EnemyHealth>();
                EH.TakeDamage(Damage);

                if (Flash)
                {
                    EH.SetStunned(FlashDuration);
                }
                if (Ignite)
                {
                    EH.SetFire(IgniteDuration);
                }
            }
        }

    }
        

    //On collision with trigger, check if we should explode then do so
    private void OnTriggerEnter(Collider other)
    {
        //If we don't want to detonate on the ground and we just hit the ground ignore it
        if (!DetonateOnGround)
        {
            if(other.gameObject.tag == "Ground")
            {
                return;
            }
        }

        if (IsPulseExplosion)
        {
            StartCoroutine(pulseExplosion());
        }
        else
        {
            explode();
        }

       
    }

    IEnumerator pulseExplosion()
    {
        float i = NumOfPulses;
        ParticleSystem PS = GetComponent<ParticleSystem>();

        explode();
        PS.Play();
        i--;

        while (i > 0)
        {
            yield return new WaitForSecondsRealtime(.5f);
            explode();
            PS.Play();
            i--;
        }


        Destroy(this.gameObject, PS.main.duration);

    }
}
