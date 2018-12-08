using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using Assets.Scripts.GameManager;
using UnityEngine;
using System;

namespace Assets.Scripts.DecisionMakingActions
{
    public class ShieldOfFaith : IAJ.Unity.DecisionMaking.GOB.Action
    {
        public AutonomousCharacter Character { get; set; }

        int shieldChange;

        public ShieldOfFaith(AutonomousCharacter character) : base("ShieldOfFaith")
        {
            this.Character = character;
            shieldChange = 5;
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL)
            {
                change += -this.shieldChange;
            }
            return change;
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return (this.Character.GameManager.characterData.Mana >= 5 && this.Character.GameManager.characterData.ShieldHP < 5);
        }

        public override bool CanExecute(IWorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;

            var mana = (int)worldModel.GetProperty(Properties.MANA);
            var shield = (int)worldModel.GetProperty(Properties.SHIELDHP);
            return (mana >= 5 && shield < 5);
        }

        public override void Execute()
        {
            base.Execute();
            this.Character.GameManager.ShieldOfFaith();
        }

        public override void ApplyActionEffects(IWorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            var surviveValue = worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, surviveValue - this.shieldChange);

            worldModel.SetProperty(Properties.SHIELDHP, this.shieldChange);

            var mana = (int)worldModel.GetProperty(Properties.MANA);
            worldModel.SetProperty(Properties.MANA, mana - 5);
        }

        public override float GetHValue(IWorldModel worldModel)
        {
            return 0.0f;
        }
    }
}