using Assets.Scripts.GameManager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public class DepthLimitedGOAPDecisionMaking
    {
        public const int MAX_DEPTH = 3;
        public int ActionCombinationsProcessedPerFrame { get; set; }
        public float TotalProcessingTime { get; set; }
        public int TotalActionCombinationsProcessed { get; set; }
        public bool InProgress { get; set; }

        public CurrentStateWorldModel InitialWorldModel { get; set; }
        private List<Goal> Goals { get; set; }
        private WorldModel[] Models { get; set; }
        private Action[] ActionPerLevel { get; set; }
        public Action[] BestActionSequence { get; private set; }
        public Action BestAction { get; private set; }
        public float BestDiscontentmentValue { get; private set; }
        private int CurrentDepth { get; set; }

        private float CurrentValue { get; set; }
        //private float BestValue { get; set; }
        //private Action nextAction;

        public DepthLimitedGOAPDecisionMaking(CurrentStateWorldModel currentStateWorldModel, List<Action> actions, List<Goal> goals)
        {
            this.ActionCombinationsProcessedPerFrame = 1000;
            this.Goals = goals;
            this.InitialWorldModel = currentStateWorldModel;
        }

        public void InitializeDecisionMakingProcess()
        {
            this.InProgress = true;
            this.TotalProcessingTime = 0.0f;
            this.TotalActionCombinationsProcessed = 0;
            this.CurrentDepth = 0;
            this.Models = new WorldModel[MAX_DEPTH + 1];
            this.Models[0] = this.InitialWorldModel;
            this.ActionPerLevel = new Action[MAX_DEPTH];
            this.BestActionSequence = new Action[MAX_DEPTH];
            this.BestAction = null;
            this.BestDiscontentmentValue = float.MaxValue;
            this.InitialWorldModel.Initialize();
            //this.BestValue = Mathf.Infinity;
        }

        public Action ChooseAction()
        {
            var processedActions = 0;

            var startTime = Time.realtimeSinceStartup;

            BestAction = null;
			BestDiscontentmentValue = int.MaxValue;

            while (CurrentDepth >= 0 && processedActions < ActionCombinationsProcessedPerFrame)
            {
                if (CurrentDepth >= MAX_DEPTH)
                {
                    processedActions++;
                    CurrentValue = Models[CurrentDepth].CalculateDiscontentment(Goals);

                    if (CurrentValue < BestDiscontentmentValue)
                    {
                        BestDiscontentmentValue = CurrentValue;
                        BestAction = ActionPerLevel[0];
                        BestActionSequence = ActionPerLevel.ToArray();
                    }
                    CurrentDepth -= 1;
                    continue;
                }
                Action nextAction = Models[CurrentDepth].GetNextAction();

                if (nextAction != null)
                {
                    var child = Models[CurrentDepth].GenerateChildWorldModel();
                    nextAction.ApplyActionEffects(child);
                    Models[CurrentDepth + 1] = child;
                    ActionPerLevel[CurrentDepth] = nextAction;
                    CurrentDepth++;
                }
                else
                {
                    CurrentDepth--;
                }
            }

            TotalActionCombinationsProcessed += processedActions;
            this.TotalProcessingTime += Time.realtimeSinceStartup - startTime;
            this.InProgress = false;
            return this.BestAction;
        }
    }
}