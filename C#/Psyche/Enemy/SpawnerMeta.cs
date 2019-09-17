using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SpawnerMeta : MonoBehaviour {

    //Exposed variables for Ben/Will to tweak per level without entering the code, as requested.
    public float ScalingRate;
    public float SpawnRate;
    public float TimeBetweenUpdateCheck;
    public float nextLevelDelay;
    public int MaxSpawnPerRound;
    public float HowManyRoundsTillEnemyIncrease;
    public GameObject EndText;

    private Dictionary<string, int> howManyEnemiesThisLevel;
    private LevelData levelData;
    private LevelEnemyDispenser LED;
    private int diedThisUpdate;
    private int leftThisRound;
    private int diedThisLevel;
    private int totalEnemiesThisLevel;
    private System.Random ran = new System.Random();


    //Decrement so the enemy record is accurate for spawning, also increment 
    //internal counter to update other counter. 
    public void OnEnemyDeath(string whoDied)
    {
        diedThisUpdate++;
    }


    // Use this for initialization
    void Start () {
        levelData = this.GetComponent<LevelData>();
        LED = this.GetComponent<LevelEnemyDispenser>();
        howManyEnemiesThisLevel = levelData.HowManyEnemiesThisLevel;
        totalEnemiesThisLevel = howManyEnemiesThisLevel.Values.Sum();
        StartCoroutine(SecondUpdate());        
	}

    //A ten second delayed update, used to check which enemies to send to the dispenser. 
    IEnumerator SecondUpdate()
    {
        while (this.isActiveAndEnabled)
        {            
            yield return new WaitForSecondsRealtime(TimeBetweenUpdateCheck);
            checkDead();
            dispenseEnemies();
        }
    }

    private void dispenseEnemies()
    {
        //Grab three distinct random numbers
        IEnumerable<int> sequence = Enumerable.Range(0, howManyEnemiesThisLevel.Count - 1).OrderBy(n => n * n * ran.Next());
        IEnumerable<int> result = sequence.Distinct().Take(3);
        var enumerator = result.GetEnumerator();

        //Storage variables for what we'll feed to LevelEnemyDispenser. The method we want takes 3 enemy names and 3 numbers to spawn.
        string[] whoIsSpawning = new string[3];
        int[] howManySpawning = new int[3];
        int tempSpawnNumber;

        int index = 0;
        int i = 0;
        //Iterate through each of the random numbers
        while (enumerator.MoveNext())
        {
            i = enumerator.Current;

            string whatE = levelData.WhatEnemy[i];
            //Figure out which enemy the number corresponds to with WhatEnemy[i]
            whoIsSpawning[index] = whatE;
                

            //Check how many enemies we are supposed to spawn this level and get reference to the number with tempSpawnNumber
            howManyEnemiesThisLevel.TryGetValue(whoIsSpawning[index], out tempSpawnNumber);

            //We only want up to 'MaxSpawnPerRound' enemies, so decrement from the dictionary appropriately and set the number
            //in the outbound array regardless.
            if(tempSpawnNumber > MaxSpawnPerRound)
            {
                tempSpawnNumber -= MaxSpawnPerRound;
                howManySpawning[index] = MaxSpawnPerRound;
            } else
            {
                howManySpawning[index] = tempSpawnNumber;
                tempSpawnNumber = 0;
            }
            //Increment index to indicate which array slot we're on but also not depend on i which will jump values.
            index++;
            if(index >= whoIsSpawning.Length || index < 0)
            {
                break;
            }
        }
       
        LED.SpawnEnemy(whoIsSpawning[0], whoIsSpawning[1], whoIsSpawning[2], howManySpawning[0], howManySpawning[1], howManySpawning[2]); 
    }

    private void checkDead()
    {
        diedThisLevel += diedThisUpdate;
        if(diedThisLevel >= totalEnemiesThisLevel)
        {
            EndText.SetActive(true);

            float curTime = 0;
            while (curTime < nextLevelDelay)
            {
                curTime += Time.deltaTime;
            }
            //TODO: Write something here that indicates the end of the level. 
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);

        }
    }


    // Update is called once per frame
    void Update () {
		
	}
}
