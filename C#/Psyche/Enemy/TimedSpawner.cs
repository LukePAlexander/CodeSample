using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedSpawner : MonoBehaviour {

    //Delay between activate and actual spawning.
    public float activationDelay;
    //Time between spawns
    public float spawnRate = 5;
    public float scaleRate;
    private int countToSpawn;
    public GameObject TrackerPrefab;
    public GameObject FTrackerPrefab;
    public GameObject RangerPrefab;
    public GameObject FRangerPrefab;
    public GameObject TRangerPrefab;
    public GameObject JustSpawned;
    public string NameToActivate;
    public bool IsActive;
    [SerializeField]
    private GameObject currentPrefab;
    private SpawnerMeta Scaler;
    private bool spawningStill;
    private Queue<string> toSpawn = new Queue<string>();
    private Queue<int> toSpawnNumber = new Queue<int>();

    // Use this for initialization
    void Start()
    {
        Scaler = GameObject.Find("LevelSpawnMeta").GetComponent<SpawnerMeta>();
        
        StartCoroutine(DelayedUpdate());
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator DelayedUpdate()
    {
        while (this.isActiveAndEnabled)
        {
            yield return new WaitForSecondsRealtime(10f);
            if (!spawningStill)
            {
                spawnQueue();
            }
        }
    }


    public void Deactivate()
    { 
        StopAllCoroutines();
        IsActive = false;
    }

    public void NewName(string newEnemy)
    {
        if (this.IsActive)
        {
            //Debug.Log("Attempted name change while still spawning, race condition for enemyHealth.myName and actual prefab used");
        }
        NameToActivate = newEnemy;
        switch (newEnemy)
        {
            case "TrackerPrefab":
                 currentPrefab = TrackerPrefab;
                break;
            case "FTrackerPrefab":
                currentPrefab = FTrackerPrefab;
                break;
            case "RangerPrefab":
                currentPrefab = RangerPrefab;
                break;
            case "FRangerPrefab":
                currentPrefab = FRangerPrefab;
                break;
            case "TRangerPrefab":
                currentPrefab = TRangerPrefab;
                break;
            default:
                Debug.Log("Invalid input for spawner, make sure NameToActivate matches one of the gameobject var names above");
                break;
            }        
    }

    public IEnumerator SpawnEnemy(int countSpawned)
    {
        spawningStill = true;
        //We have this here so they will not continue scaling just because of a delay in spawn. 
        float scaleStopped = Scaler.ScalingRate;
        while (countSpawned-- > 0)
        {
            if (JustSpawned != null && JustSpawned.transform.position == transform.position + (transform.forward) + new Vector3(0, .8f, 0))
            {
                countSpawned++;
            }
            else
            {
                JustSpawned = Instantiate(currentPrefab, transform.position + (transform.forward) + new Vector3(0, .8f, 0), transform.rotation, this.transform);
                JustSpawned.GetComponent<EnemyHealth>().SetStartingHealth((Time.time) * scaleStopped);
                JustSpawned.GetComponent<EnemyHealth>().MyName = NameToActivate;
                JustSpawned.GetComponent<EnemyHealth>().SpawnerMeta = Scaler;
                JustSpawned.SetActive(true);
            }

            yield return new WaitForSecondsRealtime(Scaler.SpawnRate);
        }
        IsActive = toSpawn.Count != 0;
        spawningStill = false;
    }

    public void AddQueue(string newEnemy, int countSpawned)
    {
        IsActive = true;
        toSpawn.Enqueue(newEnemy);
        toSpawnNumber.Enqueue(countSpawned);
    }

    public void spawnQueue()
    {
        if(toSpawn.Count == 0)
        {
            return;
        }
        string newEnemy = toSpawn.Dequeue();

        NameToActivate = newEnemy;

        int howMany = toSpawnNumber.Dequeue();

        switch (newEnemy)
        {
            case "TrackerPrefab":
                currentPrefab = TrackerPrefab;
                break;
            case "FTrackerPrefab":
                currentPrefab = FTrackerPrefab;
                break;
            case "RangerPrefab":
                currentPrefab = RangerPrefab;
                break;
            case "FRangerPrefab":
                currentPrefab = FRangerPrefab;
                break;
            case "TRangerPrefab":

                currentPrefab = TRangerPrefab;
                break;
            default:
                Debug.Log("Invalid input for spawner, make sure NameToActivate matches one of the gameobject var names above");
                break;
        }

        StartCoroutine(SpawnEnemy(howMany));
    }


    //base spawn enemy. Will not spawn if last spawn is too close.
    private void SpawnEnemy(GameObject enemyType)
    {
        if (JustSpawned != null && JustSpawned.transform.position == transform.position + (transform.forward) + new Vector3(0, .8f, 0))
        {
        }
        else
        {
            JustSpawned = Instantiate(enemyType, transform.position + (transform.forward) + new Vector3(0, .8f, 0), transform.rotation, this.transform);
            JustSpawned.GetComponent<EnemyHealth>().SetStartingHealth((Time.time) * Scaler.ScalingRate);
            JustSpawned.SetActive(true);
        }
        IsActive = toSpawn.Count != 0;
    }



}
