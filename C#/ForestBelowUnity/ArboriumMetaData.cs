/*
 * 
 * File: ArboriumMetaData1.cs
 * Author:  Team Arborium, Luke A
 * - - - - - - - - - - - -
 *  Implementation Notes: None
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ArboriumMetaData
{
    [SerializeField]
    public int NoOfTokens;
    public Dictionary<ResourceTypes, int> persistentTokens;

    // We want everyone to go through the SetLevel method, hence why these have set privalges set to private
    public int WhichLevel { get; private set; }
    public int WhichWorld { get; private set; }

    public int[] Levels;
    public int HighestCompletedLevel = 0;
    [NonSerialized]
    public int LastMenuOpen=-1;
    // Level index is used to correctly index into the Levels array object above. 
    private int levelIndex = 0;

    // Use this for initialization
    void Start()
    {
        NewGame();
    }

    public void NewGame()
    {
        ResetTokens();
        ResetProgress();
        SetLevel(1, 1);
        HighestCompletedLevel = 0;
    }

    public void ResetTokens()
    {
        NoOfTokens = 0;
        persistentTokens = new Dictionary<ResourceTypes, int>();
    }

    public void AddToken(ResourceTypes tokenType)
    {
        if (!persistentTokens.ContainsKey(tokenType))
        {
            persistentTokens.Add(tokenType, 0);
        }
        persistentTokens[tokenType]++;
    }

    public int TakeToken(ResourceTypes tokenType)
    {
        int tokensLeft = GetTokenCount(tokenType);

        if (tokensLeft > 0)
        {
            persistentTokens[tokenType]--;
            return 1;
        }
        return 0;
    }

    public int GetTokenCount(ResourceTypes tokenType)
    {
        if (!persistentTokens.ContainsKey(tokenType) || persistentTokens[tokenType] == 0)
        {
            return 0;
        }
        return persistentTokens[tokenType];
    }

    public void IncreaseTierInfo(int currTierIndex)
    {
        // -1 because the levels are 1 indexed, not 0 indexed
        if (currTierIndex > Levels[levelIndex])
        {
            Levels[levelIndex] = currTierIndex;
        }
    }

    public void Win()
    {
        // We need the below offset as the Highest completed level is a factor of 3. 
        // For example, lets say we complete level 2-1. This should translate to level 4 being the HighestCompletedLevel
        //      1 + (2-1)*3 = 4
        if (HighestCompletedLevel < WhichLevel + (WhichWorld-1) * 3)
        {
            HighestCompletedLevel = WhichLevel + (WhichWorld - 1) * 3;
        }
    }

    public void SaveProgress(int whichLevel, int completed)
    {
        if (completed < 0)
        {
            Debug.Log("Improper completion given");
            return;
        }

        Levels[levelIndex] = completed;
        if (HighestCompletedLevel < whichLevel)
        {
            HighestCompletedLevel = whichLevel;
        }
    }

    public void ResetProgress()
    {
        Levels = new int[(SceneManager.sceneCountInBuildSettings - GameConstants.LEVEL_OFFSET)*3];
    }

    // This method allows outside sources to change what the current level in the ArboriumMetaData is. 
    public void SetLevel(int world, int level)
    {
        WhichWorld = world;
        WhichLevel = level;
        levelIndex = WhichLevel - 1 + (WhichWorld - 1) * 3;
    }

    public int GetCurrentTierOfLevel()
    {
        return Levels[levelIndex];
    }
}
