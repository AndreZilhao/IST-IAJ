using Assets.Scripts.GameManager;
using System;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class MCTSBiasedPlayout : MCTS
    {
        public MCTSBiasedPlayout(CurrentStateWorldModel currentStateWorldModel) : base(currentStateWorldModel)
        {
        }

        protected override Reward Playout(WorldModel initialPlayoutState)
        {
            WorldModel state = initialPlayoutState.GenerateChildWorldModel();
            CurrentDepth = 0;
            while (!state.IsTerminal() && CurrentDepth < MaxPlayoutDepthAllowed)
            {
                List<KeyValuePair<int,GOB.Action>> actions = new List<KeyValuePair<int, GOB.Action>>();
                foreach (GOB.Action action in state.GetExecutableActions())
                {
                    actions.Add(new KeyValuePair<int,GOB.Action>((int)action.GetHValue(state), action));
                }

                actions.Sort(
                    delegate (KeyValuePair<int, GOB.Action> p1, KeyValuePair<int, GOB.Action> p2)
                    {
                        return p1.Key.CompareTo(p2.Key);
                    }
                );

                int randomValue = this.RandomGenerator.Next((int)actions[actions.Count - 1].Key);
               // Debug.Log("random :");  // debug
               // Debug.Log(randomValue); // debug

                foreach (KeyValuePair<int, GOB.Action> pair in actions)
                {
                  //  Debug.Log("\n"); // debug
                   // Debug.Log("pair<int,action> :"); // debug
                   // Debug.Log(pair.Key); // debug
                   // Debug.Log(" , "); // debug
                   // Debug.Log(pair.Value.Name); // debug

                    if (pair.Key > randomValue)
                    {
                        //Debug.Log("Action selected: "+ pair.Value.Name + "Random: " + randomValue + "pair.key: " + pair.Key);
                        state = StochasticPlayout(pair.Value, state, MaxPlayoutSimulations);
                        pair.Value.ApplyActionEffects(state);
                        break;
                    }

                }
                state.CalculateNextPlayer();
                CurrentDepth++;
            }

            Reward reward = new Reward(state, state.GetNextPlayer());
            return reward;
        }
    }
}
