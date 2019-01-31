/*
 * File:        PlayerPrefsSave.cs
 * Author:      Team Arborium, Kalvan K and Luke A
 * -  -  -  -  -  -  -  -  - 
 * Implementation Notes: None
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class PlayerPrefsSave : MonoBehaviour
{

    public static PlayerPrefsSave Instance = null;
    public AudioMixer masterMixer;


    // Use this for initialization
    void Start()
    {
        if (Instance == null)
            Instance = this;

        masterMixer.SetFloat("MasterVol", PlayerPrefs.GetFloat("MasterVol"));
        masterMixer.SetFloat("SoundFXVol", PlayerPrefs.GetFloat("SFXVol"));
        masterMixer.SetFloat("MusicVol", PlayerPrefs.GetFloat("MusicVol"));

    }

    public static void SaveMaster(float val)
    {
        PlayerPrefs.SetFloat("MasterVol", val);
        PlayerPrefs.Save();
    }

    public static void SaveSFX(float val)
    {
        PlayerPrefs.SetFloat("SFXVol", val);
        PlayerPrefs.Save();

    }

    public static void SaveMusic(float val)
    {
        PlayerPrefs.SetFloat("MusicVol", val);
        PlayerPrefs.Save();
    }

    public static void SaveHand(int val)
    {
        PlayerPrefs.SetInt("WhatHand", val);
        PlayerPrefs.Save();
    }
}
