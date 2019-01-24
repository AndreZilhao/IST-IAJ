//Put this script on your blue cube.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class ScanAgent : Agent
{
    /// <summary>
    /// The ground. The bounds are used to spawn the elements.
    /// </summary>
    /// 
    public GameObject ground;

    public GameObject area;

    /// <summary>
    /// The area bounds.
    /// </summary>
	[HideInInspector]
    public Bounds areaBounds;


    public LayerMask layer;
    public LayerMask blockLayer;
    public LayerMask rewardCubes;
    public LayerMask wallsLayer;
    public GameObject pushAgent;
    public bool active;
    private List<NodeComponent> nodes;
    private List<NodeComponent> visibleNodes;
    PushBlockAcademy academy;

    /// <summary>
    /// The goal to push the block to.
    /// </summary>
    public GameObject goal;
    //public GameObject compass;

    /// <summary>
    /// The block to be pushed to the goal.
    /// </summary>
    public GameObject block;

    /// <summary>
    /// Detects when the block touches the goal.
    /// </summary>

    private int localDifficulty = 0;
    private int selectedDifficulty = 0;
    private GameObject map;

    Rigidbody blockRB;  //cached on initialization
    Rigidbody agentRB;  //cached on initialization
    Material groundMaterial; //cached on Awake()

    /// <summary>
    /// We will be changing the ground material based on success/failue
    /// </summary>
    Renderer groundRenderer;

    void Awake()
    {
        academy = FindObjectOfType<PushBlockAcademy>(); //cache the academy
        active = false;
    }

    public override void InitializeAgent()
    {
        base.InitializeAgent();
        localDifficulty = academy.difficulty;


        // Cache the agent rigidbody
        agentRB = GetComponent<Rigidbody>();
        // Cache the block rigidbody
        blockRB = block.GetComponent<Rigidbody>();
        // Get the ground's bounds
        areaBounds = ground.GetComponent<Collider>().bounds;
        // Get the ground renderer so we can change the material when a goal is scored
        groundRenderer = ground.GetComponent<Renderer>();
        // Starting material
        groundMaterial = groundRenderer.material;
    }

    public override void CollectObservations()
    {

        if (!active) return;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f, rewardCubes, QueryTriggerInteraction.Collide);
        LightClosestNodes(hitColliders);

        Vector3 rayHeight = transform.position + new Vector3(0, -0.2f, 0);
        RaycastHit hit;
        float rotation = 0;
        Quaternion rot = Quaternion.AngleAxis(rotation, Vector3.up);
        for (int i = 0; i < 16; i++)
        {
            if (Physics.Raycast(rayHeight, transform.TransformDirection(rot * Vector3.forward), out hit, 10f, rewardCubes))
            {

                NodeComponent n = hit.collider.GetComponent<NodeComponent>();
                if (n != null)
                {
                    n.Light();
                }

            }
            rotation += 22.5f;
            rot = Quaternion.AngleAxis(rotation, Vector3.up);
        }
    }

    private void LightClosestNodes(Collider[] Nodes)
    {
        Vector3 currentPosition = transform.position;
        NodeComponent nodeComponent = null;
        foreach (Collider potentialTarget in Nodes)
        {
            nodeComponent = potentialTarget.GetComponent<NodeComponent>();
            if (nodeComponent == null) continue;
            if (nodeComponent.Visit())
            {
                AddReward(0.05f);
            }
        }
    }

    /// <summary>
    /// Use the ground's bounds to pick a random spawn position.
    /// </summary>
    public Vector3 GetRandomSpawnPos()
    {
        bool foundNewSpawnLocation = false;
        Vector3 randomSpawnPos = Vector3.zero;
        while (foundNewSpawnLocation == false)
        {
            float randomPosX = Random.Range(-areaBounds.extents.x * academy.spawnAreaMarginMultiplier,
                                areaBounds.extents.x * academy.spawnAreaMarginMultiplier);

            float randomPosZ = Random.Range(-areaBounds.extents.z * academy.spawnAreaMarginMultiplier,
                                            areaBounds.extents.z * academy.spawnAreaMarginMultiplier);
            randomSpawnPos = ground.transform.position + new Vector3(randomPosX, 1f, randomPosZ);
            if (Physics.CheckBox(randomSpawnPos, new Vector3(2.5f, 0.01f, 2.5f), Quaternion.identity, layer) == false)
            {
                foundNewSpawnLocation = true;
            }
        }
        return randomSpawnPos;
    }

    /// <summary>
    /// Called when the agent moves the block into the goal.
    /// </summary>
    public void TouchDown()
    {
        pushAgent.SetActive(true);
        this.SetActive(false);
    }

    /// <summary>
    /// Swap ground material, wait time seconds, then swap back to the regular material.
    /// </summary>
    IEnumerator GoalScoredSwapGroundMaterial(Material mat, float time)
    {
        groundRenderer.material = mat;
        yield return new WaitForSeconds(time); // Wait for 2 sec
        groundRenderer.material = groundMaterial;
    }

    /// <summary>
    /// Moves the agent according to the selected action.
    /// </summary>
    public void MoveAgent(float[] act)
    {

        Vector3 dirToGo = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;

        int action = Mathf.FloorToInt(act[0]);

        // Goalies and Strikers have slightly different action spaces.
        switch (action)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                break;
            case 3:
                dirToGo = transform.right * +0.75f;
                break;
            case 4:
                dirToGo = transform.right * -0.75f;
                break;
            case 5:
                rotateDir = transform.up * 1f;
                break;
            case 6:
                rotateDir = transform.up * -1f;
                break;
        }
        transform.Rotate(rotateDir, Time.fixedDeltaTime * 200f);
        agentRB.AddForce(dirToGo * academy.agentRunSpeed,
                         ForceMode.VelocityChange);

    }

    /// <summary>
    /// Called every step of the engine. Here the agent takes an action.
    /// </summary>
    public override void AgentAction(float[] vectorAction, string textAction)
    {
       
        // Move the agent using the action.
        if (active)
        {
            MoveAgent(vectorAction);

            // Penalty given each step to encourage agent to finish task quickly.
            AddReward(-1f / agentParameters.maxStep);

            // Monitors the time left of the agent.
            Monitor.Log("Life:", (10000f - GetStepCount()) / 10000f, this.transform);
        }
       
    }
    public void SetActive(bool flag)
    {
        this.active = flag;
    }
}
