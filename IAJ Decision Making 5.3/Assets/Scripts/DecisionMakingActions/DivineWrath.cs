using System;
using UnityEngine;
using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.Utils;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;

namespace Assets.Scripts.DecisionMakingActions { 
    public class DivineWrath : IAJ.Unity.DecisionMaking.GOB.Action {
        public AutonomousCharacter Character { get; set; }

        public DivineWrath(AutonomousCharacter character) : base("DivineWrath") {
            this.Character = character;
        }

        public override float GetGoalChange(Goal goal) {
            var change = base.GetGoalChange(goal);
            if (goal.Name == AutonomousCharacter.GET_RICH_GOAL) change -= 5.0f;
            return change;
        }

        public override bool CanExecute() {
            if (!base.CanExecute())
                return false;
            return (this.Character.GameManager.characterData.Mana >= 10 && this.Character.GameManager.characterData.Level >= 3);
        }

        public override bool CanExecute(WorldModel worldModel) {
            if (!base.CanExecute(worldModel))
                return false;

            var mana = (int)worldModel.GetProperty(Properties.MANA);
            var level = (int)worldModel.GetProperty(Properties.LEVEL);
            return (mana >= 10 && level >= 3);
        }

        public override void Execute() {
            base.Execute();
            this.Character.GameManager.DivineWrath();
        }

        public override void ApplyActionEffects(WorldModel worldModel) {
            base.ApplyActionEffects(worldModel);

            var goalValue = worldModel.GetGoalValue(AutonomousCharacter.GET_RICH_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.GET_RICH_GOAL, goalValue - 5.0f);

            var mana = (int)worldModel.GetProperty(Properties.MANA);
            worldModel.SetProperty(Properties.MANA, mana - 10);
        }

        public override float GetHValue(WorldModel worldModel)
        {
            //this is OP
            return 200.0f;
        }
    }
}