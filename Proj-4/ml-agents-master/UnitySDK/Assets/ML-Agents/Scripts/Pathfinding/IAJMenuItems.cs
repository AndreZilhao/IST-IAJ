#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

using UnityEngine.SceneManagement;

public class IAJMenuItems
{
    public class NDList<T> : List<T>
    {
        public void Add(T item)
        {
            if (!Contains(item))
                base.Add(item);
        }
    }

    [MenuItem("IAJ/Calculate Adjancencies")]
    private static void CalculateAdjacencies()
    {
        //get the NavMeshGraph from the current scene
        Transform navMesh = GameObject.Find("grid-world").GetComponent<Transform>();
        NodeComponent[] children = GetTopLevelChildren(navMesh);
        LayerMask l = LayerMask.GetMask("reward");

        if (navMesh == null)
        {
            Debug.Log("Couldn't find navmesh. Make sure there is a map selected, and it is properly named.");
            return;
        }

        foreach (NodeComponent node in children)
        {

            Collider[] hitColliders = Physics.OverlapSphere(node.GetComponent<Transform>().position, 2f, l, QueryTriggerInteraction.Collide);
            int i = 0;
            while (i < hitColliders.Length)
            {
                if (hitColliders[i].gameObject.Equals(node.gameObject))
                {
                    i++;
                    continue;
                }
                node.adjacencies.Add(hitColliders[i].gameObject.GetComponent<NodeComponent>());
                i++;
            }


            EditorUtility.SetDirty(node);
        }
        if (GUI.changed)
        {
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }

    [MenuItem("IAJ/Clear Adjancencies")]
    private static void ClearAdjacencies()
    {
        //get the NavMeshGraph from the current scene
        Transform navMesh = GameObject.Find("grid-world").GetComponent<Transform>();
        if(navMesh == null)
        {
            Debug.Log("Couldn't find navmesh. Make sure there is a map selected, and it is properly named.");
            return;
        }
        NodeComponent[] children = GetTopLevelChildren(navMesh);

        foreach (NodeComponent node in children)
        {
            node.adjacencies.Clear();
            EditorUtility.SetDirty(node);
        }
        if (GUI.changed)
        {
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }

    [MenuItem("IAJ/Calculate Rewards")]
    private static void CalculateRewards()
    {
        List<NodeComponent> goalNodes = new List<NodeComponent>();
        //get the NavMeshGraph from the current scene
        Transform navMesh = GameObject.Find("grid-world").GetComponent<Transform>();
        if (navMesh == null)
        {
            Debug.Log("Couldn't find navmesh. Make sure there is a map selected, and it is properly named.");
            return;
        }
        NodeComponent[] children = GetTopLevelChildren(navMesh);

        //Start by clearing up the values, and setting up the goal nodes.
        foreach (NodeComponent node in children)
        {
            node.value = 0;
            EditorUtility.SetDirty(node);
            if (node.Goal) goalNodes.Add(node);
        }

        Debug.Log("Num GoalNodes: " + goalNodes.Count);
        float multFactor = 0.85f;
        foreach (NodeComponent goalNode in goalNodes)
        {
            goalNode.value = 1f;
            
            NDList<NodeComponent> visited = new NDList<NodeComponent>();
            NDList<NodeComponent> open = new NDList<NodeComponent>();
            open.Add(goalNode);
            while (open.Count > 0)
            {
                NodeComponent currentNode = open[0];
                float value = currentNode.value * multFactor;
                foreach (NodeComponent node in currentNode.adjacencies)
                {
                    node.UpdateValue(value);
                    if (!visited.Contains(node))
                        open.Add(node);
                }
                open.Remove(currentNode);
                visited.Add(currentNode);
            }
        }
        if (GUI.changed)
        {
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }

    public static NodeComponent[] GetTopLevelChildren(Transform Parent)
    {
        NodeComponent[] Children = new NodeComponent[Parent.childCount];
        for (int ID = 0; ID < Parent.childCount; ID++)
        {
            Children[ID] = Parent.GetChild(ID).GetComponent<NodeComponent>();
        }
        return Children;
    }

}
#endif
