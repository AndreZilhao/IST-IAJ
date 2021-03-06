using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeComponent : MonoBehaviour {

    public List<NodeComponent> adjacencies;
    public float value;
    public bool Goal;
    public bool visited;
    private float waitTime = 8.0f;
    private float timer = 0.0f;
    Renderer r;
    Material m;
    Color c;

    private void Awake()
    {
        r = GetComponent<Renderer>();
        m = GetComponent<Renderer>().material;
    }

    private void Start()
    {
        visited = false;
        c = m.color;
        Unlight();
    }

    public bool Visit()
    {
        if (!visited)
        {
            visited = true;
            this.gameObject.layer = 12;
            r.enabled = false;
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
            c.a = 0.5f;
        }
        else
        {
            c.r = 0.7f;
            c.g = 0.5f;
            c.b = 0.5f;
            c.a = 0.5f;
        }
        // c.r = 1-value;
        // c.g = value;
        m.color = c;
        GetComponent<Renderer>().material = m;

        timer += Time.deltaTime;
        // Check if we have reached beyond 2 seconds.
        // Subtracting two is more accurate over time than resetting to zero.
        if (timer > waitTime)
        {
            Unlight();
        }
    }

    public void Unlight()
    {
            r.enabled = false;
    }

    public void Light()
    {
       
            r.enabled = true;
            timer = 0;
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
