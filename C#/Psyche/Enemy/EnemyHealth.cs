using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {

    public float StartHealth;
    public float CurHealth;
    //Use this for invuln window later
    //private bool isDamaged = false;
    private int timer;
    public bool Dead = false;
    private int justDamaged = 0;
    private Color storageColor;

	// Use this for initialization
	void Start () {
        CurHealth = StartHealth;
        storageColor = GetComponentInChildren<MeshRenderer>().material.color;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TakeDamage(float damage)
    {
        CurHealth -= damage;
        StartCoroutine(DamageColor());
        print("Ow! I got hit! Now at: " + CurHealth);
        
        if(CurHealth <= 0)
        {
            Dead = true;
            print("I died!");
            Destroy(this.gameObject,1);
        }
    }

    //Wait a second, change back.
    IEnumerator DamageColor()
    {
        MeshRenderer myMesh = GetComponentInChildren<MeshRenderer>();
        myMesh.material.color = Color.red;
        justDamaged++;

        //Debug.Break();
        yield return new WaitForSecondsRealtime(1);

        justDamaged--;

        if(justDamaged == 0)
        {
            myMesh.material.color = storageColor;
        }

    }
}
