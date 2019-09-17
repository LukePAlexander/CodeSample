using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour {

    public Dictionary<string, int> HowManyEnemiesThisLevel = new Dictionary<string, int>();
    public List<string> WhatEnemy;
    public List<int> HowMany;

    public List<int> HowManyToIncreasePerIteration;

	// Use this for initialization
	void Awake () {
		for(int i = 0; i < WhatEnemy.Count; i++)
        {
            HowManyEnemiesThisLevel.Add(WhatEnemy[i], HowMany[i]);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
