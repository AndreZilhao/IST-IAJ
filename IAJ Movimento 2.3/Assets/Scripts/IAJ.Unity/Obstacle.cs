using UnityEngine;
using UnityEditor;

public class Obstacle
{
    public GameObject GameObject;
    public Collider Collider;

    public Obstacle(GameObject gameObject, Collider collider)
    {
        GameObject = gameObject;
        Collider = collider;
    }
}