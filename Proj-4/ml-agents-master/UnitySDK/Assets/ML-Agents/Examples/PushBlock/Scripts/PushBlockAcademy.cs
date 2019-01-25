//Every scene needs an academy script. 
//Create an empty gameObject and attach this script.
//The brain needs to be a child of the Academy gameObject.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class PushBlockAcademy : Academy
{
    [HideInInspector]
    public GameObject[][] wallDifficulties;

    /// <summary>
    /// The "walking speed" of the agents in the scene. 
    /// </summary>
	public float agentRunSpeed;

    /// <summary>
    /// Whether to use only the maps of the current difficulty. Useful for benchmarking 
    /// </summary>
    public bool useLessMaps;
    [HideInInspector]
    public int wins = 0;
    [HideInInspector]
    public int resets = 0;

    //Curriculum stats
    [HideInInspector]

    public int difficulty = 2;

    /// <summary>
    /// The agent rotation speed.
    /// Every agent will use this setting.
    /// </summary>
	public float agentRotationSpeed;

    /// <summary>
    /// The spawn area margin multiplier.
    /// ex: .9 means 90% of spawn area will be used. 
    /// .1 margin will be left (so players don't spawn off of the edge). 
    /// The higher this value, the longer training time required.
    /// </summary>
	public float spawnAreaMarginMultiplier;

    /// <summary>
    /// When a goal is scored the ground will switch to this 
    /// material for a few seconds.
    /// </summary>
    public Material goalScoredMaterial;

    /// <summary>
    /// When an agent fails, the ground will turn this material for a few seconds.
    /// </summary>
    public Material failMaterial;

    /// <summary>
    /// The gravity multiplier.
    /// Use ~3 to make things less floaty
    /// </summary>
	public float gravityMultiplier;

    public GameObject[] wallsEasy;
    public GameObject[] wallsMedium;
    public GameObject[] wallsHard;
    public GameObject[] wallsVeryHard;

    public bool useScan;
    public bool testing;
    public int numTests;
    public int numAgentsTesting;

    void State()
    {
        Physics.gravity *= gravityMultiplier;

    }

    public override void AcademyReset()
    {
        base.AcademyReset();
        difficulty = (int)resetParameters["Difficulty"];
    }

    public override void InitializeAcademy()
    {
        resets -= numAgentsTesting;
        base.InitializeAcademy();
        wallDifficulties = new GameObject[4][];
        wallDifficulties[0] = wallsEasy;
        wallDifficulties[1] = wallsMedium;
        wallDifficulties[2] = wallsHard;
        wallDifficulties[3] = wallsVeryHard;
    }
}
