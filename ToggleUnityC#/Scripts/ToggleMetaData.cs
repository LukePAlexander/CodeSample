using UnityEngine;
using System.Collections;

[System.Serializable]
public class ToggleMetaData
{


    // put public variables here
    //Public variables should automatically be serializable.
    //private string currentLevel;
    //public int coinsCollected;
    public int coins0;
    public int coins1;
    public int coins2;
    public int coins3;
    public int coins4;
    public int coins5;

    public int noOfDeath;

    public float prog0;
    public float prog1;
    public float prog2;
    public float prog3;
    public float prog4;
    public float prog5;

    bool deathFlag = false;



    // Use this for initialization
    void Start()
    {
        //newGame();
    }

    // Update is called once per frame
    void Update() { }



    public void newGame()
    {
        //Debug.Log("New Game Triggered");
        coins0 = 0;
        coins1 = 0;
        coins2 = 0;
        coins3 = 0;
        coins4 = 0;
        coins5 = 0;

        noOfDeath = 0;

        prog0 = 0;
        prog1 = 0;
        prog2 = 0;
        prog3 = 0;
        prog4 = 0;
        prog5 = 0;
        //Set all variables here to 0 or some equivalent
    }

    public void died()
    {
        //if (!deathFlag)
        //{
        //    deathFlag = !deathFlag;
        //    return;
        //}
        //deathFlag = !deathFlag;

        noOfDeath++;
        //Debug.Log(noOfDeath);
    }

    public void addCoin(int buildIndex)
    {
        switch (buildIndex)
        {
            case (1):
                //Debug.Log("Working");
                coins0++;
                break;
            case (2):
                Debug.Log("Working 1-1");
                coins1++;
                break;
            case (3):
                coins2++;
                Debug.Log("Working 1-2");
                break;
            case (4):
                //Debug.Log("Working");
                coins3++;
                break;
            case (5):
                //Debug.Log("Working");
                coins4++;
                break;
            case (6):
                //Debug.Log("Working");
                coins5++;
                break;
            default:
                //Debug.Log("Default Case");
                break;
        }
    }

    public float progress(int buildIndex)
    {
        switch (buildIndex)
        {
            // Cases are found in File -> Build Settings
            case (1): // Scenes/Level 0
                return prog0;

            case (2): // Scenes/Level 1
                return prog1;
            case (3):
                return prog2;

            case (4):
                return prog3;

            case (5):
                return prog4;

            case (6):
                return prog5;

            default:
                return -1;
        }
    }


    public int coinProgress(int buildIndex)
    {
        switch (buildIndex)
        {
            case (1):
                return coins0;
            case (2):
                return coins1;

            case (3):
                return coins2;

            case (4):
                return coins3;

            case (5):
                return coins4;

            case (6):
                return coins5;

            default:
                return 100;
        }
    }

    public void saveProgress(int buildIndex, float bestProgress)
    {
        switch (buildIndex)
        {
            case (1):
                prog0 = bestProgress;
                break;
            case (2):
                prog1 = bestProgress;
                break; 
            case (3):
                prog2 = bestProgress;
                break;

            case (4):
                prog3 = bestProgress;
                break;

            case (5):
                prog4 = bestProgress;
                break;

            case (6):
                prog5 = bestProgress;
                break;

            default:
                return;
        }
    }
}
