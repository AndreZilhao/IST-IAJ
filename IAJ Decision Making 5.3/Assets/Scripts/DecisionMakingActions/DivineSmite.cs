using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using UnityEngine;
using System;
using Assets.Scripts.GameManager;

namespace Assets.Scripts.DecisionMakingActions
{
    public class DivineSmite : WalkToTargetAndExecuteAction
    {
        private int xpChange;
        private bool isSkeleton;

		public DivineSmite(AutonomousCharacter character, GameObject target) : base("DivineSmite",character,target)
		{
            this.isSkeleton = false;
            if (target.tag.Equals("Skeleton"))
            {
                this.isSkeleton = true;
                this.xpChange = 3;
            }
        }

		public override float GetGoalChange(Goal goal)
		{
            var change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.GAIN_XP_GOAL)
            {
                change += -this.xpChange;
            }

            return change;
        }

		public override bool CanExecute()
		{
            if (!base.CanExecute()) return false;
            return (this.Character.GameManager.characterData.Mana >= 2 && this.isSkeleton);
        }

		public override bool CanExecute(IWorldModel worldModel)
		{
            if (!base.CanExecute(worldModel)) return false;

            var mana = (int)worldModel.GetProperty(Properties.MANA);
            return (mana >= 2 && this.isSkeleton);
        }

		public override void Execute()
		{
            base.Execute();
            this.Character.GameManager.DivineSmite(this.Target);
        }


		public override void ApplyActionEffects(IWorldModel worldModel)
		{
            base.ApplyActionEffects(worldModel);

            var xpValue = worldModel.GetGoalValue(AutonomousCharacter.GAIN_XP_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.GAIN_XP_GOAL, xpValue - this.xpChange);

            var xp = (int)worldModel.GetProperty(Properties.XP);
            worldModel.SetProperty(Properties.XP, xp + this.xpChange);

            var mana = (int)worldModel.GetProperty(Properties.MANA);
            worldModel.SetProperty(Properties.MANA, mana - 2);

            //disables the target object so that it can't be reused again
            worldModel.SetProperty(this.Target.name, false);
        }

        public override float GetHValue(IWorldModel worldModel)
        {
            float distance = Vector3.Distance(Character.transform.position, Target.transform.position);
            float distanceBonus = 10.0f - (float)((distance * 10.0f) / 530);

            int level = (int)worldModel.GetProperty(Properties.LEVEL);

            //if (level == 1)
                return 100.0f + distanceBonus*3;
            //else
                return 0.0f;
        }
    }
}
