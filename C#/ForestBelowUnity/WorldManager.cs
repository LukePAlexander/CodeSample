/* 
 * File: WorldManager.cs
 * Author:  Luke A, and April M
 * - - - - - - - - - - - -
 *  Implementation Notes: The World Manager object containing this script is to be dropped in each scene that has a world.
 *  It will grab each Tree System present in the world and activate/deactivate as neccessary.
 *  This will include all sub roots, by iterating through the roots folder and each root's child. 
 *  
 *  This script assumes that there are 3 levels to a world, as each world is divided into thirds.
 *  If you are making a test level, please use a copy of the WorldTest scene so we can modularly import the level later.
 *  
 *  Note: [ ] marks in comments are from Luke, to indicate what portions of the code I didn't write.
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldManager : MonoBehaviour {

    //UsedInGame allows freestanding levels be tested without requiring Main Menu metascripts.
    public bool UsedInGame = false;

    private GameObject ui;
    private GameController gameController;
    private CameraController cameraController;
    private ShadowController shadowController;
    private Button playButton;

    private GameObject[] levelTreeSystems;

    //Needs to be in Awake so the individual levels in the world don't initialize and clash before this script's start.
    void Awake () {
        ui = GameObject.Find("UI (NonWorld)");

        int curLevelInt;
        GameObject curTreeSystem;
        GameObject curLevel;

        //If level directly launched from editor + without save data. Shouldn't be reached since SaveScript initializes in the main menu and persists. 
        if (SaveScript.AMD == null)
        {
            curLevelInt = 1;
        }
        else
        {
            curLevelInt = SaveScript.AMD.WhichLevel;
        }

        //Grab individual level components for activation/deactivation.
        levelTreeSystems = new GameObject[3];
        for (int i = 1; i <= 3; i++)
        {
            string levelString = "Level_" + i;
            curLevel = GameObject.Find(levelString);
            curTreeSystem = curLevel.transform.Find("TreeSystem").gameObject;
            levelTreeSystems[i-1] = curTreeSystem; // levels are 1 indexed but levelTreeSystems is 0 indexed
            
            SetTreeSystemActive(curTreeSystem, i == curLevelInt);

            // Initialize growth targets if this is one of the game's actual levels.
            // Note: using the build settings instead of WhichWorld means that we can test without going through the menu.
            if (UsedInGame)
            {
                                    //All worlds are ordered sequentially in the build index, after a couple set scenes for menus and such.
                                    //So to find the world we're on take the build index and subtract the offset
                int levelIndex = (SceneManager.GetActiveScene().buildIndex - GameConstants.LEVEL_OFFSET) * 3 + i - 1;
                int T1Cost = GameConstants.LEVEL_GROWTH_TARGETS[levelIndex, 0];
                int T2Cost = GameConstants.LEVEL_GROWTH_TARGETS[levelIndex, 1];
                int T3Cost = GameConstants.LEVEL_GROWTH_TARGETS[levelIndex, 2];
                curTreeSystem.GetComponent<GrowHeight>().SetTierCosts(T1Cost, T2Cost, T3Cost);
            }
        }

        SetupGoalsUI(levelTreeSystems[curLevelInt - 1]);
	}

    void Start()
    {
        //[Written by April M, not Luke A]
        AudioPlayer.instance.SetBackgroundMusic(AudioPlayer.BackgroundMusic.World1);
        AudioPlayer.instance.PlayAmbience(AudioPlayer.Ambience.ForestAmbience);

        // We should try to get things more directly; moved here to amortize cost for now
        gameController = GameObject.Find("GameManager").GetComponent<GameController>();
        cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
        shadowController = GameObject.Find("Dirt_Shadow").GetComponent<ShadowController>();
        playButton = GameUtilities.GetPlayButton();

        //[End]
    }

    private void SetTreeSystemActive(GameObject treeSystem, bool active)
    {
        // Even if active=true, particles shouldn't be spawning at this point
        treeSystem.GetComponent<RootParticleSystem>().StopParticleEffect();
        
        foreach (MonoBehaviour c in treeSystem.GetComponents<MonoBehaviour>())
        {
            c.enabled = active;
        }

        // Disable/Enable all it's children except trees and fairies.
        for (int i = 0; i < treeSystem.transform.childCount; i++)
        {
            GameObject child = treeSystem.transform.GetChild(i).gameObject;
            
            //Because we save end states of the roots once a level is completed, we need to deactivate them individually.
            //If this is a new level however or a redo of an old level the loop will simply pass over as there are no children.
            if (child.name == "RootsFolder")
            {
                for (int j = 0; j < child.transform.childCount; j++)
                {
                    Transform root = child.transform.GetChild(j);
                    for (int r = 0; r < root.transform.childCount; r++)
                    {
                        root.GetChild(r).gameObject.SetActive(active);
                    }
                }
            }
            //Trees and faeries are both aesthetics that don't call for listeners, so it's preferable to leave them active/visible.
            else if (child.name != "Trees" && child.name != "FaeriesFolder")
            {
                child.SetActive(active);
            }
        }
    }

    /*
     * Before any level changes, turn off UI and deactivate trees. Called externally so don't move into IncrementLevel
     * [Written by April M]
     */
    public void PrepareSceneForTransition()
    {
        ui.SetActive(false);
        foreach (GameObject treeSystem in levelTreeSystems)
        {
            treeSystem.transform.parent.gameObject.SetActive(false);
        }
    }

    public void IncrementLevel()
    {
        ui.SetActive(true);
        //If level reaches 3 then increment world and load new world.
        if (SaveScript.AMD.WhichLevel >= 3)
        {
            int world = SaveScript.AMD.WhichWorld + 1;
            if (world > 3)
            {
                gameController.LoadScene("MainMenu");
                return;
            }

            SaveScript.AMD.SetLevel(world, 1);
            int nextLevelIndex = (SaveScript.AMD.WhichWorld - 1) * 3 + 1;
            ui.SetActive(false);
            gameController.LoadScene(nextLevelIndex);
            return;
        }

        SaveScript.AMD.SetLevel(SaveScript.AMD.WhichWorld, SaveScript.AMD.WhichLevel + 1);
        int newLevel = SaveScript.AMD.WhichLevel;

        //[Written by April M]
        playButton.onClick.RemoveAllListeners();
        //[End]
       
        GameObject prevTreeSystem = levelTreeSystems[(newLevel - 1) - 1]; // array is 0 indexed so -1 more
        SetTreeSystemActive(prevTreeSystem, false);

        GameObject curTreeSystem = levelTreeSystems[newLevel - 1];
        SetTreeSystemActive(curTreeSystem, true);

        SetupGoalsUI(curTreeSystem);

        //[Explained by April M to Luke A, written by both]
        StartCoroutine(
            cameraController.GoToTree(
                tierTracker: prevTreeSystem.GetComponent<GrowHeight>(),
                tryShowBanner: false, 
                proceedToNextLevel: true)
            );

        //Written by April M after being exported from Luke A's version.
        shadowController.RotateShadowOneLevel();
    }

    private void SetupGoalsUI(GameObject curTreeSystem)
    {
        foreach (GoalsUI goal in GameUtilities.GetGoalsUIs())
        {
            goal.SetTierCosts(curTreeSystem.GetComponent<GrowHeight>());
            goal.SetNextMedal(0);
        }
    }
}
