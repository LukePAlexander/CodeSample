/*
 *  File:  SaveScript.cs
 * 
 *  Author: Team Arborium, Luke A
 * - - - - - - - - - - - - -
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using System;
using System.IO;


public class SaveScript : MonoBehaviour
{

    public static ArboriumMetaData AMD;
    public static SaveScript Instance { get; private set; }
    private int currentLevel;
    public int LevelOverride = 0;

    void Start()
    {    }

    void Update()
    {
        if (currentLevel != SceneManager.GetActiveScene().buildIndex)
        {
            Save();
        }
        currentLevel = SceneManager.GetActiveScene().buildIndex;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }

        Instance = this;

        Environment.SetEnvironmentVariable("MONO_REFLRECTION_SERIALIZER", "yes");

        if (AMD == null)
        {
            AMD = new ArboriumMetaData();
            AMD = Load();
        }

        if (LevelOverride > 0 && LevelOverride < 4)
            AMD.SetLevel(AMD.WhichWorld, LevelOverride);

        currentLevel = SceneManager.GetActiveScene().buildIndex;

        DontDestroyOnLoad(gameObject);
    }

    public static ArboriumMetaData Load()
    {
        string savePath = string.Format("{0}/ArboriumSave.dat", Application.persistentDataPath);
        try
        {
            if (File.Exists(savePath))
            {
                BinaryFormatter BF = new BinaryFormatter();
                FileStream FS = File.Open(savePath, FileMode.Open);

                AMD = (ArboriumMetaData)BF.Deserialize(FS);
                FS.Close();
            }
            else
            {
                AMD.NewGame();
                Save();
            }
        }
        catch (Exception e)
        {
            Debug.Log("Failed to load -> " + e.Message);
        }

        return AMD;
    }


    public static void Save()
    {
        string savePath = string.Format("{0}/ArboriumSave.dat", Application.persistentDataPath);

        BinaryFormatter BF = new BinaryFormatter();
        FileStream FS;

        try
        {
            if (File.Exists(savePath))
            {
                File.WriteAllText(savePath, string.Empty);
                FS = File.Open(savePath, FileMode.Open);
            }
            else
            {
                FS = File.Create(savePath);
            }
            if (AMD == null)
            {
                AMD = new ArboriumMetaData();
                AMD.NewGame();
                Save();
                return;
            }

            BF.Serialize(FS, AMD);
            FS.Close();
        }
        catch (Exception e)
        {
            Debug.Log("Failed to save to: " + savePath);
        }
    }

   

}
