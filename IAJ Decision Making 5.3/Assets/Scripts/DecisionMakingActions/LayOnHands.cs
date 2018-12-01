using System;
using UnityEngine;
using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.Utils;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;

namespace Assets.Scripts.DecisionMakingActions {
    public class LayOnHands : IAJ.Unity.DecisionMaking.GOB.Action {
        public AutonomousCharacter Character { get; set; }

        int hpChange;

        public LayOnHands(AutonomousCharacter character) : base("LayOnHands") { 
            this.Character = character;
            this.hpChange = character.GameManager.characterData.MaxHP;
        }

        public override float GetGoalChange(Goal goal) {
            var change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL)
                change += this.hpChange;

            return change;
        }

        public override bool CanExecute() {
            if (!base.CanExecute())
                return false;
            return (this.Character.GameManager.characterData.Mana >= 7 && this.Character.GameManager.characterData.Level >= 2);
        }

        public override bool CanExecute(WorldModel worldModel) {
            if (!base.CanExecute(worldModel))
                return false;

            var mana = (int)worldModel.GetProperty(Properties.MANA);
            var level = (int)worldModel.GetProperty(Properties.LEVEL);
            return (mana >= 7 && level >= 2);
        }

        public override void Execute() {
            base.Execute();
            this.Character.GameManager.LayOnHands();
        }

        public override void ApplyActionEffects(WorldModel worldModel) {
            base.ApplyActionEffects(worldModel);

            var surviveValue = worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, surviveValue - this.hpChange);

            var hp = (int)worldModel.GetProperty(Properties.MAXHP);
            worldModel.SetProperty(Properties.HP, hp);

            var mana = (int)worldModel.GetProperty(Properties.MANA);
            worldModel.SetProperty(Properties.MANA, mana - 7);
        }
    }
}