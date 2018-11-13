using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.GoalBounding;
using Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics;
using RAIN.Navigation.NavMesh;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures;
using RAIN.Navigation.Graph;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.GoalBounding
{
    public class GoalBoundingPathfinding : NodeArrayAStarPathFinding
    {
        public GoalBoundingTable GoalBoundingTable { get; protected set; }
        public int DiscardedEdges { get; protected set; }
        public int TotalEdges { get; protected set; }

        public GoalBoundingPathfinding(NavMeshPathGraph graph, IHeuristic heuristic, GoalBoundingTable goalBoundsTable) : base(graph, heuristic)
        {
            this.GoalBoundingTable = goalBoundsTable;
        }

        public override void InitializePathfindingSearch(Vector3 startPosition, Vector3 goalPosition)
        {
            this.DiscardedEdges = 0;
            this.TotalEdges = 0;
            base.InitializePathfindingSearch(startPosition, goalPosition);
        }

        protected override void ProcessChildNode(NodeRecord parentNode, NavigationGraphEdge connectionEdge, int edgeIndex)
        {
            //TODO: Implement this method for the GoalBoundingPathfinding to Work. If you implemented the NodeArrayAStar properly, you wont need to change the search method.
            //Fetching index of GoalBoundingTable
            int index = parentNode.node.NodeIndex;
            NodeGoalBounds nodeBounds = GoalBoundingTable.table[index];
       
            if (nodeBounds == null) //Special check for some nodes that have null nodeBounds.
            {
                base.ProcessChildNode(parentNode, connectionEdge, edgeIndex);
            }
            if (nodeBounds != null && nodeBounds.connectionBounds.Length > edgeIndex) // Special check for when bounds are not available for the node even if it exists.
            {
                //Obtain the parent bound corresponding to the edgeIndex of the child.
                DataStructures.GoalBounding.Bounds b = nodeBounds.connectionBounds[edgeIndex];
                if (!b.PositionInsideBounds(GoalPosition))
                {
                    this.DiscardedEdges++;
                    return;
                }
            }
                base.ProcessChildNode(parentNode, connectionEdge, edgeIndex);
        }
    }
}

