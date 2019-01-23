using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeComponent : MonoBehaviour {

    public List<NodeComponent> adjacencies;
    public float value;
    public bool Goal;
    public bool visited;
    Material m;
    Color c;

    private void Start()
    {
        visited = false;
        m = GetComponent<Renderer>().material;
        c = m.color;
    }

    public bool Visit()
    {
        if (!visited)
        {
            visited = true;
            return true;
        }
        return false;
    }

    private void Update()
    {
        if (visited)
        {
            c.r = 0;
            c.g = 0;
            c.b = 1;
        } else
        {
            c.r = 1;
            c.g = 0;
            c.b = 0;
        }
       // c.r = 1-value;
       // c.g = value;
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
