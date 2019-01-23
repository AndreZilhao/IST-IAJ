//Put this script on your blue cube.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class PushAgentBasic : Agent
{
    /// <summary>
    /// The ground. The bounds are used to spawn the elements.
    /// </summary>
    /// 
    public GameObject[] walls;
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
    private List<NodeComponent> nodes;
    private List<NodeComponent> visibleNodes;
    PushBlockAcademy academy;




    /// <summary>
    /// The goal to push the block to.
    /// </summary>
    public GameObject goal;

    /// <summary>
    /// The block to be pushed to the goal.
    /// </summary>
    public GameObject block;

    /// <summary>
    /// Detects when the block touches the goal.
    /// </summary>
	[HideInInspector]
    public GoalDetect goalDetect;

    public bool useVectorObs;
    public bool useNodesObs;
    private int localDifficulty = 0;
    private int selectedDifficulty = 0;
    private GameObject map;

    Rigidbody blockRB;  //cached on initialization
    Rigidbody agentRB;  //cached on initialization
    Material groundMaterial; //cached on Awake()
    RayPerception rayPer;

    /// <summary>
    /// We will be changing the ground material based on success/failue
    /// </summary>
    Renderer groundRenderer;

    void Awake()
    {
        academy = FindObjectOfType<PushBlockAcademy>(); //cache the academy
    }

    public override void InitializeAgent()
    {
        visibleNodes = new List<NodeComponent>();
        base.InitializeAgent();
        goalDetect = block.GetComponent<GoalDetect>();
        goalDetect.agent = this;
        rayPer = GetComponent<RayPerception>();
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
        /*
        foreach(NodeComponent n in visibleNodes)
        {
            n.Unlight();
        }*/
        //visibleNodes.Clear();
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
                    visibleNodes.Add(n);
                    n.Light();
                }

            }
            if (Physics.Raycast(rayHeight + new Vector3(0,1f,0), transform.TransformDirection(rot * Vector3.forward), out hit, 10f, rewardCubes))
            {

                NodeComponent n = hit.collider.GetComponent<NodeComponent>();
                if (n != null)
                {
                    visibleNodes.Add(n);
                    n.Light();
                }

            }
            rotation += 22.5f;
            rot = Quaternion.AngleAxis(rotation, Vector3.up);
        }

        if (useVectorObs)
        {
            var rayDistance1 = 6f;
            var rayDistance2 = 12f;
            float[] rayAngles1 = { 0f, 180f, 110f, 70f };
            float[] rayAngles2 = { 45f, 90f, 135f};
            var detectableObjects = new[] { "block", "goal", "wall" };
            AddVectorObs(rayPer.Perceive(rayDistance1, rayAngles1, detectableObjects, 0f, 0f));
            AddVectorObs(rayPer.Perceive(rayDistance1, rayAngles1, detectableObjects, 1.5f, 0f));
            AddVectorObs(rayPer.Perceive(rayDistance2, rayAngles2, detectableObjects, 0f, 0f));
            AddVectorObs(rayPer.Perceive(rayDistance2, rayAngles2, detectableObjects, 1.5f, 0f));
            float[] f = CollectRewardObs();
            //Debug.Log("-90:" + f[0] + "  -45:" + f[1] + "  00:" + f[2] + "  +45:" + f[3] + "  +90:" + f[4] );
            // Aditional reward-cube observations.
            if(useNodesObs) AddVectorObs(f);
        }

    }

    public float[] CollectRewardObs()
    {
        Vector3 rayHeight = transform.position + new Vector3(0, -0.5f, 0);
        RaycastHit hit;
        int rotation = -90;
        Quaternion rot = Quaternion.AngleAxis(rotation, Vector3.up);
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f, rewardCubes, QueryTriggerInteraction.Collide);
        Collider nc = GetClosestNode(hitColliders);
        float startValue = 0;
        NodeComponent ncc = null;
        if (nc != null)
        {
            ncc = nc.GetComponent<NodeComponent>();
            startValue = ncc.value;
        }

        //Debug.Log(startValue);
        float[] ret = { 0, 0, 0, 0, 0, 0, 0, 0 };

        for (int i = 0; i < 8; i++)
        {
            if (Physics.Raycast(rayHeight, transform.TransformDirection(rot * Vector3.forward), out hit, 3f, rewardCubes))
            {
                NodeComponent n = hit.collider.GetComponent<NodeComponent>();
                if (n != null)
                {
                    //normalized distance to start value
                    if (n.value > startValue) ret[i] = 1;
                    if (n.value < startValue) ret[i] = -1;
                }
            }
            rotation += 45;
            rot = Quaternion.AngleAxis(rotation, Vector3.up);
        }
        return ret;
    }

    Collider GetClosestNode(Collider[] Nodes)
    {
        Collider bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        NodeComponent nodeComponent = null;
        foreach (Collider potentialTarget in Nodes)
        {
            nodeComponent = potentialTarget.GetComponent<NodeComponent>();
            if (nodeComponent == null) continue;
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (nodeComponent.Visit())
            {
                AddReward(0.05f);
                Debug.Log("yay!");
            }
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }
        return bestTarget;
    }

    void ExplosionDamage(Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        int i = 0;
        while (i < hitColliders.Length)
        {
            hitColliders[i].SendMessage("AddDamage");
            i++;
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
    public void IScoredAGoal()
    {
        // We use a reward of 2 + Difficulty level.
        //AddReward(2f + academy.difficulty);

        // By marking an agent as done AgentReset() will be called automatically.
        Done();

        // Swap ground material for a bit to indicate we scored.
        StartCoroutine(GoalScoredSwapGroundMaterial(academy.goalScoredMaterial, 0.5f));
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
                rotateDir = transform.up * 1f;
                break;
            case 4:
                rotateDir = transform.up * -1f;
                break;
            case 5:
                dirToGo = transform.right * -0.75f;
                break;
            case 6:
                dirToGo = transform.right * 0.75f;
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
        MoveAgent(vectorAction);

        // Penalty given each step to encourage agent to finish task quickly.
        AddReward(-1f / agentParameters.maxStep);

        // Monitors the time left of the agent.
        Monitor.Log("Life:", (10000f - GetStepCount()) / 10000f, this.transform);
    }

    /// <summary>
    /// Resets the block position and velocities.
    /// </summary>
    void ResetBlock()
    {
        // Get a random position for the block.
        block.transform.position = GetRandomSpawnPos();

        // Reset block velocity back to zero.
        blockRB.velocity = Vector3.zero;

        // Reset block angularVelocity back to zero.
        blockRB.angularVelocity = Vector3.zero;
    }

    void SelectRandomWall(int difficulty)
    {
        //Get wall selection for given difficulty
        Transform[] children = GetTopLevelChildren(walls[difficulty].transform);

        int selectedWall = Random.Range(0, children.Length);
        DeactivateWalls(children);
        //Activate the randomly selected wall
        children[selectedWall].GetComponentInChildren<Transform>(true).gameObject.SetActive(true);
    }

    void DeactivateWalls(int difficulty)
    {
        Transform[] children = GetTopLevelChildren(walls[difficulty].transform);
        for (int i = 0; i < children.Length; i++)
        {
            children[i].GetComponentInChildren<Transform>(true).gameObject.SetActive(false);
        }
    }

    void DeactivateWalls(Transform[] children)
    {
        for (int i = 0; i < children.Length; i++)
        {
            children[i].GetComponentInChildren<Transform>(true).gameObject.SetActive(false);
        }
    }

    public static Transform[] GetTopLevelChildren(Transform Parent)
    {
        Transform[] Children = new Transform[Parent.childCount];
        for (int ID = 0; ID < Parent.childCount; ID++)
        {
            Children[ID] = Parent.GetChild(ID);
        }
        return Children;
    }


    /// <summary>
    /// In the editor, if "Reset On Done" is checked then AgentReset() will be 
    /// called automatically anytime we mark done = true in an agent script.
    /// </summary>
	public override void AgentReset()
    {

        //PopulateNodes();
        Monitor.SetActive(true);
        block.GetComponent<Renderer>().enabled = false;
        if (localDifficulty < academy.difficulty)
        {
            localDifficulty = academy.difficulty;
        }

        //Selects a random map from available difficulties if not using less maps.
        if (academy.useLessMaps)
        {
            selectedDifficulty = 3;
        }
        else
        {
            selectedDifficulty = Random.Range(0, localDifficulty + 1);
        }
           
        GameObject[] samples = academy.wallDifficulties[selectedDifficulty];

        //Using custom maps
        if (samples.Length != 0)
        {
            Destroy(map);
            int n = Random.Range(0, samples.Length);
            map = Instantiate(samples[n], area.transform);
            Monitor.Log("Map: ", selectedDifficulty.ToString() + "-" + n);
            Monitor.Log("Difficulty: ", localDifficulty.ToString());
        }

        int rotation = Random.Range(0, 4);
        float rotationAngle = rotation * 90f;
        area.transform.Rotate(new Vector3(0f, rotationAngle, 0f));
        ResetBlock();
        transform.position = GetRandomSpawnPos();
        agentRB.velocity = Vector3.zero;
        agentRB.angularVelocity = Vector3.zero;
    }
}
