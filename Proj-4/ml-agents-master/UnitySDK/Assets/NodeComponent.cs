using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeComponent : MonoBehaviour {

    public List<NodeComponent> adjacencies;
    public float value;
    public bool Goal;
    Material m;
    Color c;

    private void Start()
    {
        m = GetComponent<Renderer>().material;
        c = m.color;
    }

    private void Update()
    {
        c.r = 1-value;
        c.g = value;
        m.color = c;
        GetComponent<Renderer>().material = m;
    }

    public void OnDestroy()
    {
        Destroy(m);
    }

    public void UpdateValue(float val)
    {
        if (val > value)
            value = val;
    }
}
