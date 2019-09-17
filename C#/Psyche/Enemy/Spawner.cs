using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public GameObject EnemySpawned;
    public GameObject JustSpawned;
	// Use this for initialization
	void Start () {
        StartCoroutine(SpawnEnemy());

    }

    // Update is called once per frame
    void Update () {
		
	}

    //Wait 8 seconds, spawn enemy.
    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            if (JustSpawned != null && JustSpawned.transform.position == transform.position + (transform.forward) + new Vector3(0, .8f, 0))
            {
            }
            else
            {
                JustSpawned = Instantiate(EnemySpawned, transform.position + (transform.forward) + new Vector3(0, .8f, 0), transform.rotation, this.transform);
                JustSpawned.SetActive(true);
            }

            yield return new WaitForSecondsRealtime(8);
        }
    }
}
