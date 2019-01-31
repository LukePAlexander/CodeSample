using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.InteropServices;
using System;

public class SaveScript : MonoBehaviour
{
    public static int currentLevel;

    public static SaveScript Instance { get { return instance;} }

    private static SaveScript instance = null;

    public static ToggleMetaData TMD;
    //private bool firstTimeFlag = true;

    [DllImport("__Internal")]
    private static extern void SyncFiles();

    [DllImport("__Internal")]
    private static extern void WindowAlert(string message);

    // Use this for initialization
    void Start()
    {
        TMD = new ToggleMetaData();
        TMD = Load();
        currentLevel = SceneManager.GetActiveScene().buildIndex;
    }

    void Awake()
    {
        Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");

        //Check if any other instance, 
        if (instance != null && instance != this)
        {
            //Debug.Log("Another instance detected, " + SaveScript.TMD.noOfDeath + " destroyed");
            
            Destroy(this.gameObject);
        } else {
            instance = this;
        }        
        
        DontDestroyOnLoad(gameObject);

        if (currentLevel != SceneManager.GetActiveScene().buildIndex)
        {
            Debug.Log("peanuts");
            //Debug.Log("Level Change Detected: " + currentLevel + "!=" + SceneManager.GetActiveScene().buildIndex);
            //save();
        }
        currentLevel = SceneManager.GetActiveScene().buildIndex;


    }


    // Update is called once per frame
    void Update()
    {
        if (currentLevel != SceneManager.GetActiveScene().buildIndex)
        {
            //Debug.Log("Level Change Detected");
            save();
        }
        currentLevel = SceneManager.GetActiveScene().buildIndex;

    }

    /// <summary>
    /// Save the game, this will be called with various methods through the
    /// ToggleMetaData class.
    /// </summary>
    /// <param name="TMD"></param>
    public static void save()
    {
        //Debug.Log("Saved");
        //Create all the stuff needed to write a file.
        string dataPath = string.Format("{0}/ToggleSave.dat", Application.persistentDataPath);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream;



        try
        {
            if (File.Exists(dataPath))
            {
                File.WriteAllText(dataPath, string.Empty);
                fileStream = File.Open(dataPath, FileMode.Open);
            }
            else
            {
                fileStream = File.Create(dataPath);
            }
            //if(TMD )
            binaryFormatter.Serialize(fileStream, TMD);
            fileStream.Close();

            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                SyncFiles();
            }
        }
        catch (Exception e)
        {
            Debug.Log("Failed to Save: " + e.Message);
        }
        //Debug.Log("Stopped Saving");
    }

    private static void PlatformSafeMessage(string message)
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            WindowAlert(message);
        }
        else
        {
            Debug.Log(message);
        }
    }

    /// <summary>
    /// This should be called upon game start or a load button maybe.
    /// </summary>
    /// <param name="TMD"></param>
    /// <returns></returns>
    public static ToggleMetaData Load()
    {
        //Debug.Log("Loaded");
        //Find the file path
        string dataPath = string.Format("{0}/ToggleSave.dat", Application.persistentDataPath);

        //Debug.Log(dataPath);

        try
        {
            //If the file exists then write it to TMD
            if (File.Exists(dataPath))
            {
                //Debug.Log("File made");
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                FileStream fileStream = File.Open(dataPath, FileMode.Open);
                
                TMD = (ToggleMetaData)binaryFormatter.Deserialize(fileStream);
                fileStream.Close();
                //Debug.Log(TMD.noOfDeath);

                //If it doesn't exist then create a newgame and save it.
            } else
            {
                //Debug.Log("New Game");
                TMD.newGame();
                save();
            }            
        }
        catch (Exception e)
        {
            Debug.Log("Failed to Load: " + e.Message);
        }

        //Debug.Log("Stopped Loading");
        return TMD;
    }

}

