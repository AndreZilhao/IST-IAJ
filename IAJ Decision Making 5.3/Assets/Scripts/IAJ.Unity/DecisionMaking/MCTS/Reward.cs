using Assets.Scripts.GameManager;
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
        public float Value { get; set; }
        public int PlayerID { get; set; }

        public Reward(WorldModel world, int player)
        {
            this.PlayerID = player;
            this.hp = (int)world.GetProperty(Properties.HP);
            this.xp = (int)world.GetProperty(Properties.LEVEL);
            this.maxhp = (int)world.GetProperty(Properties.MAXHP);
            this.coinsCollected = (int)world.GetProperty(Properties.MONEY);

            for (int i = 0; i < 7; i++)
            {
                if (!(bool)world.GetProperty("Skeleton" + i)) enemiesKilled++;
            }
            for (int i = 0; i < 2; i++)
            {
                if (!(bool)world.GetProperty("Orc" + i)) enemiesKilled++;
            }
            if (!(bool)world.GetProperty("Dragon")) enemiesKilled++;

            //Normalize the values

            this.hp = hp / maxhp;
            this.xp = (xp - 1) / 2;
            this.enemiesKilled = enemiesKilled / 10;
            this.coinsCollected = coinsCollected / 25;
            {
                Value = xp * 0.2f + hp * 0.3f + enemiesKilled * 0.3f + coinsCollected * 0f;
            }
            
        }


        
    }
}
