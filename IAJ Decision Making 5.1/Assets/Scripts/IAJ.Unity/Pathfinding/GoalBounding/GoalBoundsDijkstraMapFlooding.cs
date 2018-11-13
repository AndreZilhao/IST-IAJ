using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.GoalBounding;
using RAIN.Navigation.Graph;
using RAIN.Navigation.NavMesh;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.GoalBounding
{
    //The Dijkstra algorithm is similar to the A* but with a couple of differences
    //1) no heuristic function
    //2) it will not stop until the open list is empty
    //3) we dont need to execute the algorithm in multiple steps (because it will be executed offline)
    //4) we don't need to return any path (partial or complete)
    //5) we don't need to do anything when a node is already in closed
    public class GoalBoundsDijkstraMapFlooding
    {
        public NavMeshPathGraph NavMeshGraph { get; protected set; }
        public NavigationGraphNode StartNode { get; protected set; }
        public NodeGoalBounds NodeGoalBounds { get; protected set; }
        protected NodeRecordArray NodeRecordArray { get; set; }

        public IOpenSet Open { get; protected set; }
        public IClosedSet Closed { get; protected set; }

        public GoalBoundsDijkstraMapFlooding(NavMeshPathGraph graph)
        {
            this.NavMeshGraph = graph;
            //do not change this
            var nodes = this.GetNodesHack(graph);
            this.NodeRecordArray = new NodeRecordArray(nodes);
            this.Open = this.NodeRecordArray;
            this.Closed = this.NodeRecordArray;
        }

        public void Search(NavigationGraphNode startNode, NodeGoalBounds nodeGoalBounds)
        {
            //TODO: Implement the algorithm that calculates the goal bounds using a dijkstra
            //Given that the nodes in the graph correspond to the edges of a polygon, we won't be able to use the vertices of the polygon to update the bounding boxes
            this.Open.Initialize();
            this.Closed.Initialize();

            //Initialize starting node for Dijkstra
            NodeRecord StartNode = NodeRecordArray.GetNodeRecord(startNode);
            StartNode.gValue = 0;
            StartNode.StartNodeOutConnectionIndex = -1; // -1 corresponds to not having a "color". Valid indices will start at 0
            Open.AddToOpen(StartNode);

            //Dijkstra
            while (Open.CountOpen() > 0)
            {
                NodeRecord currentNode = Open.GetBestAndRemove();

                Open.RemoveFromOpen(currentNode);
                Closed.AddToClosed(currentNode);

                //We don't fill out the starting position as it is colorless
                if (currentNode.StartNodeOutConnectionIndex != -1)
                {
                    var outFillConnections = currentNode.node.OutEdgeCount;

                    Vector3 edgePosition;
                    //Update the bounding box with all positions of EdgeOuts of the current Node
                    for (int i = 0; i < outFillConnections; i++)
                    {
                        edgePosition = currentNode.node.EdgeOut(i).ToNode.Position;
                        nodeGoalBounds.connectionBounds[currentNode.StartNodeOutConnectionIndex].UpdateBounds(edgePosition); //update only the bound corresponding to the ConnectionIndex
                    }
                }

                //Process Child Nodes
                var outConnections = currentNode.node.OutEdgeCount;
                for (int i = 0; i < outConnections; i++)
                {
                    this.ProcessChildNode(currentNode, currentNode.node.EdgeOut(i), i);
                }
            }
        }


        protected void ProcessChildNode(NodeRecord parent, NavigationGraphEdge connectionEdge, int connectionIndex)
        {
            //TODO: Implement this method that processes a child node. Then you can use it in the Search method above.
            var childNode = connectionEdge.ToNode;
            var childNodeRecord = this.NodeRecordArray.GetNodeRecord(childNode);

            //Distribute initial colors throughout startNode children:
            int startIndex = parent.StartNodeOutConnectionIndex;
            if (startIndex == -1)
            {
                startIndex = connectionIndex;
            }

            float g = parent.gValue + (childNode.LocalPosition - parent.node.LocalPosition).magnitude;
            NodeRecord nodeInOpen = Open.SearchInOpen(childNodeRecord);
            NodeRecord nodeInClosed = Closed.SearchInClosed(childNodeRecord);

            if (nodeInOpen == null && nodeInClosed == null)
            {
                childNodeRecord.parent = parent;
                childNodeRecord.gValue = g;
                childNodeRecord.fValue = g;
                childNodeRecord.StartNodeOutConnectionIndex = startIndex;
                Open.AddToOpen(childNodeRecord);
            }
            else if (nodeInOpen != null && nodeInOpen.gValue > g)
            {
                childNodeRecord.parent = parent;
                childNodeRecord.gValue = g;
                childNodeRecord.fValue = g;
                childNodeRecord.StartNodeOutConnectionIndex = startIndex;
                Open.Replace(nodeInOpen, childNodeRecord);
            }
        }

        private List<NavigationGraphNode> GetNodesHack(NavMeshPathGraph graph)
        {
            //this hack is needed because in order to implement NodeArrayA* you need to have full acess to all the nodes in the navigation graph in the beginning of the search
            //unfortunately in RAINNavigationGraph class the field which contains the full List of Nodes is private
            //I cannot change the field to public, however there is a trick in C#. If you know the name of the field, you can access it using reflection (even if it is private)
            //using reflection is not very efficient, but it is ok because this is only called once in the creation of the class
            //by the way, NavMeshPathGraph is a derived class from RAINNavigationGraph class and the _pathNodes field is defined in the base class,
            //that's why we're using the type of the base class in the reflection call
            return (List<NavigationGraphNode>)Utils.Reflection.GetInstanceField(typeof(RAINNavigationGraph), graph, "_pathNodes");
        }

    }
}
