using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelEnemyDispenser : MonoBehaviour {



    private GameObject player;
    private GameObject[] allSpawners;
    private System.Random ran = new System.Random();


    // Use this for initialization
    void Start () {
        player = GameObject.Find("PlayerPrefab");
        allSpawners = GameObject.FindGameObjectsWithTag("Spawner");
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    private bool isRightFarther(GameObject left, GameObject right)
    {
            //Return if the distance between the player and left is less than the distance between the player and right.
        return (Vector3.Distance(player.transform.position, left.transform.position) < Vector3.Distance(player.transform.position, right.transform.position));
    }

    /*
     * Scan for the furthest three spawners, and return the furthest three
     */
    private GameObject[] getFarSpawners()
    {

        //If the number of spawners found is less than 3, use all 3.
        if (allSpawners.Length < 3)
        {
            Debug.Log("Please Add more than 3 total Advanced spawners in the level");
            Debug.Break();
        } else if (allSpawners.Length == 3)
        {
            return allSpawners;
        }

        GameObject[] farthestSpawners = new GameObject[3];

        //All objects start as the player, so literally any other comparison will be greater.
        farthestSpawners[0] = player;
        farthestSpawners[1] = player;
        farthestSpawners[2] = player;

        for(int i = 0; i < allSpawners.Length; i++)
        {
            if(isRightFarther(farthestSpawners[2], allSpawners[i]))
            {
                farthestSpawners[0] = farthestSpawners[1];
                farthestSpawners[1] = farthestSpawners[2];
                farthestSpawners[2] = allSpawners[i];

            } else if (isRightFarther(farthestSpawners[1], allSpawners[i]))
            {
                farthestSpawners[0] = farthestSpawners[1];
                farthestSpawners[1] = allSpawners[i];
            } else if(isRightFarther(farthestSpawners[0] , allSpawners[i]))
            {
                farthestSpawners[0] = allSpawners[i];
            }
        }

        return farthestSpawners;

        int[] farthestSpawnerIndex = new int[farthestSpawners.Length];

        int storageVar;

        //Compare all of the spawners vs the three saved farthest and replace if possible. 
        for (int i = 0; i < allSpawners.Length; i++)
        {
           
            //Loop for the farthest spawners we currently have.
            for(int j = 0;  j < farthestSpawners.Length; j++)
            {

                //If the current entry we're on is null then replace with the current item
                if(farthestSpawners[j] == null)
                {
                    farthestSpawners[j] = allSpawners[i];
                    farthestSpawnerIndex[j] = i;
                    break;

                    //If it is a valid entry, check if it's closer than the current spawner
                    //we're comparing with, 
                } else
                {
                    //Added to try to fix a bug, leaving for security.
                    if (GameObject.ReferenceEquals(farthestSpawners[j], allSpawners[i]))
                    {
                        continue;
                    }

                    if (isRightFarther(farthestSpawners[j], allSpawners[i]))
                    {
                        //If the new one is further, save the old furthest spawner in a temp
                        //and iterate up the rest of the furthestSpawners array 
                        storageVar = farthestSpawnerIndex[j];

                        farthestSpawners[j] = allSpawners[i];
                        farthestSpawnerIndex[j] = i;

                        //Iterating starts at the entry index we're on + 1,
                        //it then iterates up the farthest spawner array
                        //just swapping objects up the line. 
                        for(int n = j + 1; n < farthestSpawners.Length; n++)
                        {
                            int secondStorageVar = farthestSpawnerIndex[n];
                            farthestSpawners[n] = allSpawners[storageVar];
                            farthestSpawnerIndex[n] = storageVar;
                            storageVar = secondStorageVar;
                        }
                    }
                }                
            }
        }

        foreach(GameObject spawner in farthestSpawners)
        {
            Debug.Log("Spawner at: " + spawner.transform);
        }

        return farthestSpawners;
    }

    public void SpawnEnemy(string whichEnemy, int howMany)
    {
        Debug.Log("Small spawn called");

        //Grab the farthest spawners from the player
        GameObject[] farthestSpawners = getFarSpawners();
        
        //If less than 2 enemys to spawn, just grab the first spawner and pop them out.
        if (howMany <= 2)
        {
            farthestSpawners[0].GetComponent<TimedSpawner>().AddQueue(whichEnemy, howMany);
            return;
        }

        //We pick randomly from the three fathest spawners
        int whichSpawner = Random.Range(1, 4);

        //We will disperse the enemies throughout, so figure out how many to spawn for each and add the leftovers to the first one.
        int leftOver = howMany % 3;
        int hmPerSpawner = howMany / 3;

        //loop through the spawners starting at whichSpawner and continuing through the three. 
        for(int i = 0; i < 3; i++)
        {
            farthestSpawners[(whichSpawner + i) % 3].GetComponent<TimedSpawner>().AddQueue(whichEnemy, hmPerSpawner + leftOver);
            leftOver = 0;
        }
    }

    public void SpawnEnemy(string whichEnemy, string whichEnemy2, string whichEnemy3, int howMany, int howMany2, int howMany3)
    {
        Debug.Log("Long spawn called");
        //Grab the farthest spawners from the player
        GameObject[] farthestSpawners = getFarSpawners();

        if(GameObject.ReferenceEquals(farthestSpawners[0], farthestSpawners[1]))
        {
            Debug.Log("References equal");
        }

        farthestSpawners[0].GetComponent<TimedSpawner>().AddQueue(whichEnemy, howMany);

        farthestSpawners[1].GetComponent<TimedSpawner>().AddQueue(whichEnemy2, howMany2);

        farthestSpawners[2].GetComponent<TimedSpawner>().AddQueue(whichEnemy3, howMany3);

        return; 

        StartCoroutine(farthestSpawners[0].GetComponent<TimedSpawner>().SpawnEnemy(howMany));

        farthestSpawners[1].GetComponent<TimedSpawner>().NewName(whichEnemy2);
        farthestSpawners[1].GetComponent<TimedSpawner>().IsActive = true;

        StartCoroutine(farthestSpawners[1].GetComponent<TimedSpawner>().SpawnEnemy(howMany2));

        farthestSpawners[2].GetComponent<TimedSpawner>().NewName(whichEnemy3);
        farthestSpawners[2].GetComponent<TimedSpawner>().IsActive = true;

        StartCoroutine(farthestSpawners[2].GetComponent<TimedSpawner>().SpawnEnemy(howMany3));

    }


}
