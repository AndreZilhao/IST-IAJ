﻿using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class Reward
    {
        float hp;
        float maxhp;
        float xp;
        float enemiesKilled;
        float coinsCollected;
        float time;
        public float Value { get; set; }
        public int PlayerID { get; set; }

        public Reward(IWorldModel world, int player)
        {
            this.PlayerID = player;
            this.hp = (int)world.GetProperty(Properties.HP);
            this.xp = (int)world.GetProperty(Properties.LEVEL);
            this.maxhp = (int)world.GetProperty(Properties.MAXHP);
            this.coinsCollected = (int)world.GetProperty(Properties.MONEY);
            this.time = (float)world.GetProperty(Properties.TIME);

            for (int i = 1; i <= 7; i++)
            {
                if (!(bool)world.GetProperty("Skeleton" + i)) enemiesKilled++;
            }
            for (int i = 1; i <= 2; i++)
            {
                if (!(bool)world.GetProperty("Orc" + i)) enemiesKilled++;
            }
            if (!(bool)world.GetProperty("Dragon")) enemiesKilled++;

            //Normalize the values

            this.hp = hp / 30;
            this.xp = (xp - 1) / 2;
            this.enemiesKilled = enemiesKilled / 10;
            this.coinsCollected = coinsCollected / 25;
            this.time = 1 - (time / 200);

            Value = xp * 0.2f + hp * 0.4f + enemiesKilled * 0.3f + coinsCollected * 0.0f + time * 0.3f;

        }



    }
}
