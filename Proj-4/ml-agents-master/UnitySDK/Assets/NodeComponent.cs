using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeComponent : MonoBehaviour {

    public List<NodeComponent> adjacencies;
    public float value;
    public bool Goal;

    private void Update()
    {
        Color c = GetComponent<Renderer>().material.color;
        c.r = 1-value;
        c.g = value;
        GetComponent<Renderer>().material.color = c;
    }

    public void UpdateValue(float val)
    {
        if (val > value)
            value = val;
    }
}
