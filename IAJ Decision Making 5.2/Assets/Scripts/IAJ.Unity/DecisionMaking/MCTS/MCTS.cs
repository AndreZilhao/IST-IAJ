using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class MCTS
    {
        public const float C = 1.4f;
        public bool InProgress { get; private set; }
        public int MaxIterations { get; set; }
        public int MaxIterationsProcessedPerFrame { get; set; }
        public int MaxPlayoutDepthReached { get; private set; }
        public int MaxSelectionDepthReached { get; private set; }
        public float TotalProcessingTime { get; private set; }
        public MCTSNode BestFirstChild { get; set; }
        public List<GOB.Action> BestActionSequence { get; private set; }


        private int CurrentIterations { get; set; }
        private int CurrentIterationsInFrame { get; set; }
        private int CurrentDepth { get; set; }

        private CurrentStateWorldModel CurrentStateWorldModel { get; set; }
        private MCTSNode InitialNode { get; set; }
        private System.Random RandomGenerator { get; set; }



        public MCTS(CurrentStateWorldModel currentStateWorldModel)
        {
            this.InProgress = false;
            this.CurrentStateWorldModel = currentStateWorldModel;
            this.MaxIterations = 100;
            this.MaxIterationsProcessedPerFrame = 10;
            this.RandomGenerator = new System.Random();
        }


        public void InitializeMCTSearch()
        {
            this.MaxPlayoutDepthReached = 0;
            this.MaxSelectionDepthReached = 0;
            this.CurrentIterations = 0;
            this.CurrentIterationsInFrame = 0;
            this.TotalProcessingTime = 0.0f;
            this.CurrentStateWorldModel.Initialize();
            this.InitialNode = new MCTSNode(this.CurrentStateWorldModel)
            {
                Action = null,
                Parent = null,
                PlayerID = 0
            };
            this.InProgress = true;
            this.BestFirstChild = null;
            this.BestActionSequence = new List<GOB.Action>();
        }

        public GOB.Action Run()
        {
            MCTSNode selectedNode;
            Reward reward;

            var startTime = Time.realtimeSinceStartup;
            this.CurrentIterationsInFrame = 0;
            MCTSNode node = InitialNode;
            while (CurrentIterationsInFrame < MaxIterationsProcessedPerFrame)
            {
                node = Selection(InitialNode);
                reward = Playout(node.State);
                Backpropagate(node, reward);
                CurrentIterationsInFrame++;
            }

            BestFirstChild = BestChild(InitialNode);
            return BestFirstChild.Action;
        }

        private MCTSNode Selection(MCTSNode initialNode)
        {
            GOB.Action nextAction;
            MCTSNode currentNode = initialNode;
            MCTSNode bestChild;

            while (!currentNode.State.IsTerminal())
            {
                nextAction = currentNode.State.GetNextAction();
                if (nextAction != null)
                {
                    return Expand(currentNode, nextAction);
                }
                else
                {
                    currentNode = BestUCTChild(currentNode);
                }
            }
            return currentNode;
        }

        private Reward Playout(WorldModel initialPlayoutState)
        {
            WorldModel state = initialPlayoutState.GenerateChildWorldModel();

            while (!state.IsTerminal())
            {
                GOB.Action[] actions = state.GetExecutableActions();
                int randomAction = RandomGenerator.Next(actions.Length);
                actions[randomAction].ApplyActionEffects(state);
            }

            Reward reward = new Reward
            {
                PlayerID = 0,
                Value = state.GetScore(),

            };
            return reward;
        }

        private void Backpropagate(MCTSNode node, Reward reward)
        {
            while (node != null)
            {
                node.N++;
                node.Q += reward.Value;
                node = node.Parent;
            }
        }

        private MCTSNode Expand(MCTSNode parent, GOB.Action action)
        {
            MCTSNode childNode = new MCTSNode(parent.State)
            {
                Action = action,
                Parent = parent,
                PlayerID = 0,
            };
            childNode.Action.ApplyActionEffects(childNode.State);
            parent.ChildNodes.Add(childNode);
            return childNode;
        }

        //gets the best child of a node, using the UCT formula
        // usado na exploração
        private MCTSNode BestUCTChild(MCTSNode node)
        {
            MCTSNode bestChild = null;
            float bestUtility = 0;
            foreach (MCTSNode childNode in node.ChildNodes)
            {
                float utility = (node.Q / node.N) + C * Mathf.Sqrt(Mathf.Log(node.Parent.N) / node.N);
                if (utility > bestUtility)
                {
                    bestUtility = utility;
                    bestChild = childNode;
                }
            }
            return bestChild;
        }

        //this method is very similar to the bestUCTChild, but it is used to return the final action of the MCTS search, and so we do not care about
        //the exploration factor //RETORNAR NO FINAL, SEM TER EM CONTA A EXPLORAÇÃO (podemos obter isto através do numero de explorações feitas (BEST CHOICE), ou melhor Q/N)
        private MCTSNode BestChild(MCTSNode node)
        {
            return BestUCTChild(node);
            //for now simply use the UCT method 
        }
    }
}
