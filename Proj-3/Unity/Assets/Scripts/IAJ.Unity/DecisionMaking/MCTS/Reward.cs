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
        float enemiesKilled = 0;
        float coinsCollected;
        float resourcesConsumed = 0;
        float time;
        float shieldhp = 0;
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
            this.shieldhp = (int)world.GetProperty(Properties.SHIELDHP);

            for (int i = 1; i <= 2; i++)
            {
                if (!(bool)world.GetProperty("ManaPotion" + i)) resourcesConsumed++;
            }
            for (int i = 1; i <= 2; i++)
            {
                if (!(bool)world.GetProperty("HealthPotion" + i)) resourcesConsumed++;
            }
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

            this.resourcesConsumed = (4 - resourcesConsumed)/4;
            this.hp = hp / 30;
            this.xp = (xp - 1) / 2;
            this.enemiesKilled = enemiesKilled / 10;
            this.coinsCollected = coinsCollected / 25;
            this.time = 1 - (time / 200);
            if(this.xp > 0)
            {
                this.shieldhp = shieldhp / 5f;
            } else
            {
                this.shieldhp = 0;
            }


            //BIAS Continuous
            Value = xp * 0.2f + hp * 0.40f + enemiesKilled * 0.3f + resourcesConsumed * 0.1f + time * 0.1f;

            //NO BIAS Continous
            //Value = xp * 0.1f + hp * 0.60f + enemiesKilled * 0.2f + resourcesConsumed * 0.0f + time * 0.1f;

            //FLAT REWARD
            //Value = world.GetScore();
        }



    }
}
