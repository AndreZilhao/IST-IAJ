using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Action = Assets.Scripts.IAJ.Unity.DecisionMaking.GOB.Action;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class MCTS
    {
        public const float C = 1.4f;
        public bool InProgress { get; private set; }
        public int MaxIterations { get; set; }
        public int NumberOfRuns { get; set; }
        public int MaxIterationsProcessedPerFrame { get; set; }
        public int MaxPlayoutDepthReached { get; private set; }
        public int MaxSelectionDepthReached { get; private set; }
        public int MaxPlayoutDepthAllowed { get; private set; }
        public float TotalProcessingTime { get; private set; }
        public MCTSNode BestFirstChild { get; set; }
        public List<GOB.Action> BestActionSequence { get; private set; }


        protected int CurrentIterations { get; set; }
        protected int CurrentIterationsInFrame { get; set; }
        protected int CurrentDepth { get; set; }

        private CurrentStateWorldModel CurrentStateWorldModel { get; set; }
        private MCTSNode[] InitialNodes { get; set; }
        protected System.Random RandomGenerator { get; set; }



        public MCTS(CurrentStateWorldModel currentStateWorldModel)
        {
            this.InProgress = false;
            this.CurrentStateWorldModel = currentStateWorldModel;
            this.MaxIterations = 2000;
            this.NumberOfRuns = 8;
            this.MaxIterationsProcessedPerFrame = 100;
            this.MaxPlayoutDepthAllowed = 5;
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
            this.InitialNodes = new MCTSNode[this.NumberOfRuns];
            for (int i = 0; i < NumberOfRuns; i++)
            {
                InitialNodes[i] = new MCTSNode(this.CurrentStateWorldModel.GenerateChildWorldModel());
                InitialNodes[i].Action = null;
                InitialNodes[i].Parent = null;
                InitialNodes[i].PlayerID = 0;
            }
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
            int currentMCTS = 0;
            while (CurrentIterations < MaxIterations)
            {
                if (CurrentIterationsInFrame >= MaxIterationsProcessedPerFrame)
                {
                    return null;
                }

                selectedNode = Selection(InitialNodes[currentMCTS]);
                if (selectedNode == InitialNodes[currentMCTS])
                    break;
                reward = Playout(selectedNode.State);
                Backpropagate(selectedNode, reward);
                CurrentIterationsInFrame++;
                CurrentIterations++;
                currentMCTS++;
                if (currentMCTS == NumberOfRuns) currentMCTS = 0;
            }

            BestFirstChild = BestChildFromSeveral(InitialNodes); ;
            MCTSNode child = BestFirstChild;
            BestActionSequence.Clear();
            while (child != null)
            {
                BestActionSequence.Add(child.Action);
                child = BestChild(child);
            }
            InProgress = false;
            if (BestFirstChild != null)
            {
                Debug.Log(BestFirstChild.Action.Name);
                return BestFirstChild.Action;
            }
            return null;
        }

        private MCTSNode Selection(MCTSNode initialNode)
        {
            GOB.Action nextAction;
            MCTSNode currentNode = initialNode;
            MCTSNode bestChild; //should I use this?

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

        virtual protected Reward Playout(WorldModel initialPlayoutState)
        {
            WorldModel prevState = initialPlayoutState.GenerateChildWorldModel();
            CurrentDepth = 0;
            //Perform n playouts on the next state [Possible solution to deal with stochastic nature]
            int n = 5; //change n to > 0 to enable this feature
            while (!prevState.IsTerminal() && CurrentDepth < MaxPlayoutDepthAllowed)
            {
                GOB.Action[] actions = prevState.GetExecutableActions();

                int randomAction = RandomGenerator.Next(actions.Length);
                //Debug.Log(actions[randomAction].Name);
                if (actions[randomAction].Name.Contains("SwordAttack") && n > 0)
                {
                    WorldModel[] testStates = new WorldModel[n];
                    for (int i = 0; i < n; i++) 
                    {
                        testStates[i] = prevState.GenerateChildWorldModel();
                        actions[randomAction].ApplyActionEffects(testStates[i]);
                    }
                    prevState = MergeStates(testStates, actions[randomAction].Name);
                }
                else
                {
                    prevState = prevState.GenerateChildWorldModel();
                    actions[randomAction].ApplyActionEffects(prevState);
                }
                //Debug.Log("Resulting");
                //prevState.DumpState();
                prevState.CalculateNextPlayer();
                CurrentDepth++;
            }
            //Debug.Log("CurrentDepth:" + CurrentDepth);
            Reward reward = new Reward(prevState, prevState.GetNextPlayer());
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
            MCTSNode childNode = new MCTSNode(parent.State.GenerateChildWorldModel())
            {
                Action = action,
                Parent = parent,
                PlayerID = 0,
            };
            childNode.Action.ApplyActionEffects(childNode.State);
            childNode.State.CalculateNextPlayer();
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
                float utility = (childNode.Q / childNode.N) + C * Mathf.Sqrt(Mathf.Log(node.N) / childNode.N);
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
            MCTSNode bestChild = null;
            float mostExplored = 0;
            foreach (MCTSNode childNode in node.ChildNodes)
            {
                if (childNode.N > mostExplored)
                {
                    mostExplored = childNode.N;
                    bestChild = childNode;
                }
            }
            return bestChild;
        }

        //finds the most picked action from a number of MCTS runs
        private MCTSNode BestChildFromSeveral(MCTSNode[] nodes)
        {
            if (nodes[0].ChildNodes.Count == 0) return null;
            //find the overall best action and store it in 'max'
            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (MCTSNode n in nodes)
            {
                MCTSNode b = BestChild(n);
                //Debug.Log(b.Action.Name);
                //Debug.Log("n:" + b.N + "q:" + b.Q);
                if (dict.ContainsKey(b.Action.Name))
                {
                    dict[b.Action.Name] += b.N;
                }
                else
                {
                    dict[b.Action.Name] = b.N;
                }
            }
              var max  = dict.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;

            //find the selected best action in the first tree and return it
            foreach (MCTSNode n in nodes[0].ChildNodes)
            {
                if (n.Action.Name == max)
                {
                    return n;
                }
            }
            return null;
        }


        //method to average out and apply the effects of a sword attack playout in a stochastic world
        private WorldModel MergeStates(WorldModel[] testStates, string enemy)
        {
            int hp = 0;
            int shieldHP = 0;
            int enemyDeadCount = 0;
            bool enemyAlive = true;
            int xp = 0;
            int n = testStates.Length;

            //Tokenizing the enemy string
            enemy = enemy.Remove(0, 11);
            enemy = enemy.Replace("(", "");
            enemy = enemy.Replace(")", "");

            for (int i = 0; i < n; i++)
            {
                hp += (int)testStates[i].GetProperty(Properties.HP);
                shieldHP += (int)testStates[i].GetProperty(Properties.SHIELDHP);
                xp += (int)testStates[i].GetProperty(Properties.XP);
                if ((bool)testStates[i].GetProperty(enemy) != true) enemyDeadCount++;
            }

            hp = hp / n;
            shieldHP = shieldHP / n;

            if (enemyDeadCount > ((float)n / 2)) //enemy is dead on average, rounded up
            {
                xp = xp / enemyDeadCount;
                enemyAlive = false;
            }
            else
            {
                xp = 0;
            }

            //returning the testState[0] as the resulting average
            WorldModel returnState = testStates[0];
            returnState.SetProperty(Properties.HP, hp);
            returnState.SetProperty(Properties.SHIELDHP, shieldHP);
            returnState.SetProperty(enemy, enemyAlive);
            returnState.SetProperty(Properties.XP, xp);
            return returnState;
        }
    }
}
