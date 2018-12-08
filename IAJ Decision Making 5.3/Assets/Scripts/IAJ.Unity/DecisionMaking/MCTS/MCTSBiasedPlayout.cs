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

        protected override Reward Playout(IWorldModel initialPlayoutState)
        {
            IWorldModel state = initialPlayoutState.GenerateChildWorldModel();
            CurrentDepth = 0;
            while (!state.IsTerminal() && CurrentDepth < MaxPlayoutDepthAllowed)
            {
                List<KeyValuePair<int, GOB.Action>> actions = new List<KeyValuePair<int, GOB.Action>>();
                foreach (GOB.Action action in state.GetExecutableActions())
                {
                    actions.Add(new KeyValuePair<int, GOB.Action>((int)action.GetHValue(state), action));
                }
                actions.Sort(
                    delegate (KeyValuePair<int, GOB.Action> p1, KeyValuePair<int, GOB.Action> p2)
                    {
                        return p1.Key.CompareTo(p2.Key);
                    }
                );
                if (actions.Count == 0) break;

                int randomValue = this.RandomGenerator.Next((int)actions[actions.Count - 1].Key);

                foreach (KeyValuePair<int, GOB.Action> pair in actions)
                {
                    if (pair.Key > randomValue)
                    {
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
