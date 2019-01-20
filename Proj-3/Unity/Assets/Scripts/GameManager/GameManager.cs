﻿using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameManager
{
    public class GameManager : MonoBehaviour
    {
        private const float UPDATE_INTERVAL = 0.5f;
        //public fields, seen by Unity in Editor
        public GameObject character;
        public AutonomousCharacter autonomousCharacter;

        public Text HPText;
        public Text ShieldHPText;
        public Text ManaText;
        public Text TimeText;
        public Text XPText;
        public Text LevelText;
        public Text MoneyText;
        public GameObject GameEnd;

        //private fields
        public List<GameObject> chests;
        public List<GameObject> skeletons;
        public List<GameObject> orcs;
        public List<GameObject> dragons;
        public List<GameObject> enemies;

        public CharacterData characterData;
        public bool WorldChanged { get; set; }
        private DynamicCharacter enemyCharacter;
        private GameObject currentEnemy;

        private float nextUpdateTime = 0.0f;
        private Vector3 previousPosition;

        public void Start()
        {
            this.WorldChanged = false;
            this.characterData = new CharacterData(this.character);
            this.previousPosition = this.character.transform.position;

            this.enemies = new List<GameObject>();
            this.chests = GameObject.FindGameObjectsWithTag("Chest").ToList();
            this.skeletons = GameObject.FindGameObjectsWithTag("Skeleton").ToList();
            this.enemies.AddRange(this.skeletons);
            this.orcs = GameObject.FindGameObjectsWithTag("Orc").ToList();
            this.enemies.AddRange(this.orcs);
            this.dragons = GameObject.FindGameObjectsWithTag("Dragon").ToList();
            this.enemies.AddRange(this.dragons);

        }

        public void Update()
        {

            if (Time.time > this.nextUpdateTime)
            {
                this.nextUpdateTime = Time.time + UPDATE_INTERVAL;
                this.characterData.Time += UPDATE_INTERVAL;
            }

            if (enemyCharacter != null && currentEnemy != null && currentEnemy.activeSelf)
            {
                this.enemyCharacter.Movement.Target.position = this.character.transform.position;
                this.enemyCharacter.Update();
                this.SwordAttack(currentEnemy);
            }
            else
            {
                foreach (var enemy in this.enemies)
                {
                    if ((enemy.transform.position - this.character.transform.position).sqrMagnitude <= 100)
                    {
                        this.currentEnemy = enemy;
                        this.enemyCharacter = new DynamicCharacter(enemy)
                        {
                            MaxSpeed = 100
                        };
                        enemyCharacter.Movement = new DynamicSeek()
                        {
                            Character = enemyCharacter.KinematicData,
                            MaxAcceleration = 300,
                            Target = new IAJ.Unity.Movement.KinematicData()
                        };

                        break;
                    }
                }
            }

            this.HPText.text = "HP: " + this.characterData.HP;
            this.XPText.text = "XP: " + this.characterData.XP;
            this.ShieldHPText.text = "Shield HP: " + this.characterData.ShieldHP;
            this.LevelText.text = "Level: " + this.characterData.Level;
            this.TimeText.text = "Time: " + this.characterData.Time;
            this.ManaText.text = "Mana: " + this.characterData.Mana;
            this.MoneyText.text = "Money: " + this.characterData.Money;

            if (this.characterData.HP <= 0 || this.characterData.Time >= 200)
            {
                if (!this.GameEnd.activeSelf)
                    Debug.Log("Actions: " + this.autonomousCharacter.actionsPerformed
                   + "|" + "Coins: " + this.characterData.Money
                   + "|" + "Time: " + this.characterData.Time);
                this.GameEnd.GetComponentInChildren<Text>().text = "Game Over";
                this.GameEnd.SetActive(true);
            }
            else if (this.characterData.Money >= 25)
            {
                if (!this.GameEnd.activeSelf)
                    Debug.Log("Actions: " + this.autonomousCharacter.actionsPerformed
                    + "|" + "Coins: " + this.characterData.Money
                    + "|" + "Time: " + this.characterData.Time);
                this.GameEnd.GetComponentInChildren<Text>().text = "Victory";
                this.GameEnd.SetActive(true);

            }
        }

        public void SwordAttack(GameObject enemy)
        {
            int damage = 0;
            int xpGain = 0;
            int enemyAC = 10;

            if (enemy != null && enemy.activeSelf && InMeleeRange(enemy))
            {
                if (enemy.tag.Equals("Skeleton"))
                {
                    //1D6 Damage Die
                    damage = RandomHelper.RollD6();
                    xpGain = 3;
                    enemyAC = 10;
                }
                else if (enemy.tag.Equals("Orc"))
                {
                    //2D6 Damage Die
                    damage = RandomHelper.RollD10() + RandomHelper.RollD10();
                    xpGain = 10;
                    enemyAC = 14;
                }
                else if (enemy.tag.Equals("Dragon"))
                {
                    damage = RandomHelper.RollD12() + RandomHelper.RollD12() + RandomHelper.RollD12();
                    xpGain = 20;
                    enemyAC = 18;
                }

                //attack roll = D20 + attack modifier. Using 7 as attack modifier (+4 str modifier, +3 proficiency bonus)
                int attackRoll = RandomHelper.RollD20() + 7;

                if (attackRoll >= enemyAC)
                {
                    //there was an hit, enemy is destroyed, gain xp
                    this.enemies.Remove(enemy);
                    enemy.SetActive(false);
                    Object.Destroy(enemy);
                    this.characterData.XP += xpGain;
                }

                int remainingDamage = damage - this.characterData.ShieldHP;
                this.characterData.ShieldHP = Mathf.Max(0, this.characterData.ShieldHP - damage);

                if (remainingDamage > 0)
                {
                    this.characterData.HP -= remainingDamage;
                }
                this.autonomousCharacter.actionsPerformed++;
                this.WorldChanged = true;
            }
        }

        public void DivineSmite(GameObject enemy)
        {
            if (enemy != null && enemy.activeSelf && InDivineSmiteRange(enemy) && this.characterData.Mana >= 2)
            {
                if (enemy.tag.Equals("Skeleton"))
                {
                    this.characterData.XP += 3;
                    this.enemies.Remove(enemy);
                    enemy.SetActive(false);
                    Object.Destroy(enemy);
                }

                this.characterData.Mana -= 2;
                this.autonomousCharacter.actionsPerformed++;
                this.WorldChanged = true;
            }

        }

        public void ShieldOfFaith()
        {
            if (this.characterData.Mana >= 5)
            {
                this.characterData.ShieldHP = 5;
                this.characterData.Mana -= 5;

                this.WorldChanged = true;
                this.autonomousCharacter.actionsPerformed++;
            }

        }

        public void LayOnHands()
        {
            if (this.characterData.Level >= 2 && this.characterData.Mana >= 7)
            {
                this.characterData.HP = this.characterData.MaxHP;
                this.characterData.Mana -= 7;

                this.WorldChanged = true;
                this.autonomousCharacter.actionsPerformed++;
            }

        }

        public void DivineWrath()
        {
            if (this.characterData.Level >= 3 && this.characterData.Mana >= 10)
            {
                //kill all enemies in the map
                foreach (var enemy in this.enemies)
                {
                    if (enemy.tag.Equals("Skeleton"))
                    {
                        this.characterData.XP += 3;
                    }
                    else if (enemy.tag.Equals("Orc"))
                    {
                        this.characterData.XP += 10;
                    }
                    else if (enemy.tag.Equals("Dragon"))
                    {
                        this.characterData.XP += 20;
                    }

                    enemy.SetActive(false);
                    Object.Destroy(enemy);
                }
                this.characterData.Mana = 0;
                enemies.Clear();
                this.WorldChanged = true;
                this.autonomousCharacter.actionsPerformed++;
            }

        }

        public void PickUpChest(GameObject chest)
        {
            if (chest != null && chest.activeSelf && InChestRange(chest))
            {
                this.chests.Remove(chest);
                Object.Destroy(chest);
                this.characterData.Money += 5;
                this.WorldChanged = true;
                this.autonomousCharacter.actionsPerformed++;
            }

        }

        public void LevelUp()
        {
            if (this.characterData.Level >= 4) return;

            if (this.characterData.XP >= this.characterData.Level * 10)
            {
                this.characterData.Level++;
                this.characterData.MaxHP += 10;
                this.characterData.XP = 0;
                this.WorldChanged = true;
                this.autonomousCharacter.actionsPerformed++;
            }

        }

        public void GetManaPotion(GameObject manaPotion)
        {
            if (manaPotion != null && manaPotion.activeSelf && InPotionRange(manaPotion))
            {
                Object.Destroy(manaPotion);
                this.characterData.Mana = 10;
                this.WorldChanged = true;
                this.autonomousCharacter.actionsPerformed++;
            }

        }

        public void GetHealthPotion(GameObject potion)
        {
            if (potion != null && potion.activeSelf && InPotionRange(potion))
            {
                Object.Destroy(potion);
                this.characterData.HP = this.characterData.MaxHP;
                this.WorldChanged = true;
                this.autonomousCharacter.actionsPerformed++;
            }

        }


        private bool CheckRange(GameObject obj, float maximumSqrDistance)
        {
            return (obj.transform.position - this.characterData.CharacterGameObject.transform.position).sqrMagnitude <= maximumSqrDistance;
        }


        public bool InMeleeRange(GameObject enemy)
        {
            return this.CheckRange(enemy, 30.0f);
        }

        public bool InDivineSmiteRange(GameObject enemy)
        {
            return this.CheckRange(enemy, 400.0f);
        }

        public bool InChestRange(GameObject chest)
        {
            return this.CheckRange(chest, 50.0f);
        }

        public bool InPotionRange(GameObject potion)
        {
            return this.CheckRange(potion, 50.0f);
        }
    }
}