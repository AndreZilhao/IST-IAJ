using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using UnityEngine;

namespace Assets.Scripts.DecisionMakingActions
{
    public class GetManaPotion : WalkToTargetAndExecuteAction
    {
        public GetManaPotion(AutonomousCharacter character, GameObject target) : base("GetManaPotion",character,target)
        {
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return this.Character.GameManager.characterData.Mana < 10;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;

            var mana = (int)worldModel.GetProperty(Properties.MANA);
            return mana < 10;
        }

        public override void Execute()
        {
            base.Execute();
            this.Character.GameManager.GetManaPotion(this.Target);
        }


        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);
            worldModel.SetProperty(Properties.MANA, 10);
            //disables the target object so that it can't be reused again
            worldModel.SetProperty(this.Target.name, false);
        }

        public override float GetHValue(WorldModel worldModel)
        {
            float distance = Vector3.Distance(Character.transform.position, Target.transform.position);
            float distanceBonus = 10.0f - (float)((distance * 10.0f) / 530);

            int mana = (int)worldModel.GetProperty(Properties.MANA);
            int level = (int)worldModel.GetProperty(Properties.LEVEL);

            if (level == 3)
            {
                if (mana < 9)
                    return 100.0f + distanceBonus;
                else
                    return 0.0f;
            }
            else
            {
                if (mana < 2)
                    return 100.0f + distanceBonus;
                else
                    return 0.0f;
            }
        }
    }
}
