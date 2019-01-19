using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    public class IClosedSetHashTable : IClosedSet
    {
        private Hashtable hashTable { get; set; }
        private List<NodeRecord> NodeRecords { get; set; } //needed just to draw the nodes

        public IClosedSetHashTable()
        {
            this.hashTable = new Hashtable();
            this.NodeRecords = new List<NodeRecord>();
        }

        public void Initialize()
        {
            this.NodeRecords.Clear();
            this.hashTable.Clear();
        }

        public void AddToClosed(NodeRecord nodeRecord)
        {
            this.NodeRecords.Add(nodeRecord);
            this.hashTable.Add(nodeRecord.node, nodeRecord);
        }

        public void RemoveFromClosed(NodeRecord nodeRecord)
        {
            this.NodeRecords.Remove(nodeRecord);
            this.hashTable.Remove(nodeRecord.node);
        }

        public NodeRecord SearchInClosed(NodeRecord nodeRecord)
        {
            return (NodeRecord)this.hashTable[nodeRecord.node];
        }

        public ICollection<NodeRecord> All()
        {
            //not needed
            return this.NodeRecords;
        }
    }
}