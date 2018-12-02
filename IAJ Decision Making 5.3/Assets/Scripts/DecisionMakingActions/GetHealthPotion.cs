using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;
using UnityEngine;

namespace Assets.Scripts.DecisionMakingActions
{
    public class GetHealthPotion : WalkToTargetAndExecuteAction
    {
        private int hpChange;

        public GetHealthPotion(AutonomousCharacter character, GameObject target) : base("GetHealthPotion",character,target)
        {
            this.hpChange = character.GameManager.characterData.MaxHP - character.GameManager.characterData.HP;
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL)
            {
                change += -this.hpChange;
            }

            return change;
        }

        public override bool CanExecute()
		{
            if (!base.CanExecute()) return false;
            return this.Character.GameManager.characterData.HP < this.Character.GameManager.characterData.MaxHP;
        }

		public override bool CanExecute(WorldModel worldModel)
		{
            if (!base.CanExecute(worldModel)) return false;

            var hp = (int)worldModel.GetProperty(Properties.HP);
            var maxHp = (int)worldModel.GetProperty(Properties.MAXHP);
            return hp < maxHp;
        }

		public override void Execute()
		{
            base.Execute();
            this.Character.GameManager.GetHealthPotion(this.Target);
        }

		public override void ApplyActionEffects(WorldModel worldModel)
		{
            base.ApplyActionEffects(worldModel);

            var surviveValue = worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, surviveValue - this.hpChange);


            var hp = (int)worldModel.GetProperty(Properties.MAXHP);

            worldModel.SetProperty(Properties.HP,hp);
            //disables the target object so that it can't be reused again
            worldModel.SetProperty(this.Target.name, false);
        }

        public override float GetHValue(WorldModel worldModel)
        {
            float distance = Vector3.Distance(Character.transform.position, Target.transform.position);
            float distanceBonus = 10.0f - (float)((distance * 10.0f) / 530);

            int hp = (int)worldModel.GetProperty(Properties.HP);
            int maxhp = (int)worldModel.GetProperty(Properties.MAXHP);
            // 0 a 100
            return 100.0f - (float)((hp * 100) / maxhp) + distanceBonus;
        }
    }
}
