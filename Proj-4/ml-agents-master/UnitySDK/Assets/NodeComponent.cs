using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeComponent : MonoBehaviour {

    public List<NodeComponent> adjacencies;
    public float value;
    public bool Goal;
    public bool visited;
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
            if( !this.gameObject.tag.Equals("block")) gameObject.SetActive(false);
            return true;
        }
        return false;
    }

    private void Update()
    {
        if (visited && !this.gameObject.tag.Equals("block"))
        {
            c.r = 0;
            c.g = 0;
            c.b = 1;
        } else
        {
            GetComponent<Renderer>().enabled = false;
        }
       // c.r = 1-value;
       // c.g = value;
        m.color = c;
        GetComponent<Renderer>().material = m;
    }

    public void Unlight()
    {
        if(this.gameObject.activeInHierarchy)
            r.enabled = false;
    }

    public void Light()
    {
        r.enabled = true;
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
