using System.Collections.Generic;
using Assets.Scripts.GameManager;
using System.Linq;
using UnityEngine;
using System;
using Assets.Scripts.DecisionMakingActions;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public class WorldModelFEAR : IWorldModel
    {
        private object[] properties { get; set; }
        private string[] Resources;
        private bool[] Enabled;
        private int NextPlayer { get; set; }
        private List<Action> Actions { get; set; }
        protected IEnumerator<Action> ActionEnumerator { get; set; }
        protected GameManager.GameManager gameManager { get; set; }
        public Action[] NextEnemyActions { get; set; }
        public Action NextEnemyAction { get; set; }

        public WorldModelFEAR(List<Action> actions, GameManager.GameManager gameManager)
        {
            this.properties = new object[9];
            this.Actions = actions;
            this.ActionEnumerator = actions.GetEnumerator();
            this.gameManager = gameManager;
            this.NextPlayer = 0;

            properties[0] = gameManager.characterData.Mana;
            properties[1] = gameManager.characterData.HP;
            properties[2] = gameManager.characterData.MaxHP;
            properties[3] = gameManager.characterData.ShieldHP;
            properties[4] = gameManager.characterData.XP;
            properties[5] = gameManager.characterData.Time;
            properties[6] = gameManager.characterData.Money;
            properties[7] = gameManager.characterData.Level;
            properties[8] = gameManager.characterData.CharacterGameObject.transform.position;

            GameObject go = GameObject.Find("ObjectContainer");

            List<string> n = new List<string>();

            foreach (Transform t in go.transform)
            {
                n.Add(t.name);
            }

            Resources = new string[n.Count];
            Enabled = new bool[n.Count];

            for (int i = 0; i < n.Count; i++)
            {
                Enabled[i] = true;
                Resources[i] = n[i];
            }
        }

        public WorldModelFEAR(WorldModelFEAR parent)
        {
            this.properties = new object[9];
            this.Resources = new string[19];
            this.Enabled = new bool[19];
            parent.properties.CopyTo(properties, 0);
            parent.Resources.CopyTo(Resources, 0);
            parent.Enabled.CopyTo(Enabled, 0);
            this.gameManager = parent.gameManager;
            this.Actions = parent.Actions;
            this.ActionEnumerator = this.Actions.GetEnumerator();
        }

        public object GetProperty(string propertyName)
        {
            foreach (string s in Resources)
            {
                //Debug.Log(s);
            }
            switch (propertyName)
            {
                case "Mana":
                    return properties[0];
                case "HP":
                    return properties[1];
                case "MAXHP":
                    return properties[2];
                case "SHIELDHP":
                    return properties[3];
                case "XP":
                    return properties[4];
                case "Time":
                    return properties[5];
                case "Money":
                    return properties[6];
                case "Level":
                    return properties[7];
                case "Position":
                    return properties[8];
                default:
                    int index = Array.IndexOf(Resources, propertyName);
                    if (index >= 0)
                        return Enabled[index];
                    break;
            }
            return false; 
        }

        public void SetProperty(string propertyName, object value)
        {
            switch (propertyName)
            {
                case "Mana":
                    properties[0] = value;
                    break;
                case "HP":
                    properties[1] = value;
                    break;
                case "MAXHP":
                    properties[2] = value;
                    break;
                case "SHIELDHP":
                    properties[3] = value;
                    break;
                case "XP":
                    properties[4] = value;
                    break;
                case "Time":
                    properties[5] = value;
                    break;
                case "Money":
                    properties[6] = value;
                    break;
                case "Level":
                    properties[7] = value;
                    break;
                case "Position":
                    properties[8] = value;
                    break;

                default:
                    int index = Array.IndexOf(Resources, propertyName);
                    if (index >= 0)
                        Enabled[index] = (bool)value;
                    break;
            }
        }

        //State visualization
        public void DumpState()
        {
            string str = "";
            foreach (var item in properties)
            {
                str += "|" + item + " ";
            }
            Debug.Log(str);
        }
       
        public IWorldModel GenerateChildWorldModel()
        {
            return new WorldModelFEAR(this);
        }

        public float GetGoalValue(string goalName)
        {
            return 0;
        }

        public void SetGoalValue(string goalName, float value)
        {
            return;
        }

        public float CalculateDiscontentment(List<Goal> goals)
        {
            return 0.0f;
        }

        public Action GetNextAction()
        {
            Action action = null;
            if (this.NextPlayer == 1)
            {
                action = this.NextEnemyAction;
                this.NextEnemyAction = null;
                return action;
            }
            //returns the next action that can be executed or null if no more executable actions exist
            if (this.ActionEnumerator.MoveNext())
            {
                action = this.ActionEnumerator.Current;
            }

            while (action != null && !action.CanExecute(this))
            {
                if (this.ActionEnumerator.MoveNext())
                {
                    action = this.ActionEnumerator.Current;
                }
                else
                {
                    action = null;
                }
            }

            return action;
        }

        public Action[] GetExecutableActions()
        {
            if (this.NextPlayer == 1)
            {
                return this.NextEnemyActions;
            }
            return Actions.Where(a => a.CanExecute(this)).ToArray();
        }

        public bool IsTerminal()
        {
            int HP = (int)this.GetProperty(Properties.HP);
            float time = (float)this.GetProperty(Properties.TIME);
            int money = (int)this.GetProperty(Properties.MONEY);

            return HP <= 0 || time >= 200 || money == 25;
        }

        public float GetScore()
        {
            if ((int)this.GetProperty(Properties.MONEY) == 25)
            {
                return 1;
            }
            return 0;
        }

        public int GetNextPlayer()
        {
            return NextPlayer;
        }

        public void CalculateNextPlayer()
        {
            Vector3 position = (Vector3)this.GetProperty(Properties.POSITION);
            bool enemyEnabled;

            //basically if the character is close enough to an enemy, the next player will be the enemy.
            foreach (var enemy in this.gameManager.enemies)
            {
                enemyEnabled = (bool)this.GetProperty(enemy.name);
                if (enemyEnabled && (enemy.transform.position - position).sqrMagnitude <= 400)
                {
                    this.NextPlayer = 1;
                    this.NextEnemyAction = new SwordAttack(this.gameManager.autonomousCharacter, enemy);
                    this.NextEnemyActions = new Action[] { this.NextEnemyAction };
                    return;
                }
            }
            this.NextPlayer = 0;
            //if not, then the next player will be player 0
        }
    }
}
