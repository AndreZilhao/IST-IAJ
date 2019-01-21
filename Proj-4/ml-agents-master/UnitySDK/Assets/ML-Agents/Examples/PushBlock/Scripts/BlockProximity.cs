using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockProximity : MonoBehaviour
{

    public GameObject agent;
    public LayerMask agentMask;

    // Update is called once per frame
    void Update()
    {
        if (Physics.CheckBox(this.transform.position, new Vector3(1.0f, 2.5f, 1.0f), Quaternion.identity, agentMask))
        {
            Debug.Log("I found you");
            agent.GetComponent<PushAgentBasic>().IScoredAGoal();
        }
    }
}
