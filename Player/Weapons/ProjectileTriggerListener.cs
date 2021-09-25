using Nez;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Player.Weapons
{
    public class ProjectileTriggerListener : Component, ITriggerListener
    {
        public IProjectile projectile;

        public int dmg = 1;
        public void OnTriggerEnter(Collider other, Collider local)
        {
            if (other.Enabled && local.Enabled && other.PhysicsLayer.IsFlagSet(Data.PhysicsLayers.enemy_hit))
            {
                var enemy = other.Entity as Enemies.EnemyEntity;
                if(enemy != null)
                {
                    enemy.health?.Hit(dmg);
                }
            }
            if (other.Enabled) projectile?.TurnOnOff(false);
        }

        public void OnTriggerExit(Collider other, Collider local)
        {
        }
    }
}
