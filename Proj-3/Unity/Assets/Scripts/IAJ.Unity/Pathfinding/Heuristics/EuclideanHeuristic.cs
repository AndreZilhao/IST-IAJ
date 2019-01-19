using RAIN.Navigation.Graph;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics
{
    public class EuclideanHeuristic : IHeuristic
    {
        public float H(NavigationGraphNode node, NavigationGraphNode goalNode)
        {
            return Mathf.Sqrt((goalNode.Position.x - node.Position.x) * (goalNode.Position.x - node.Position.x) + (goalNode.Position.y - node.Position.y) * (goalNode.Position.y - node.Position.y));
        }

        public float Fast_H(Vector3 node, Vector3 goalNode)
        {
            return Mathf.Sqrt((goalNode.x - node.x) * (goalNode.x - node.x) + (goalNode.y - node.y) * (goalNode.y - node.y));
        }
    }
}