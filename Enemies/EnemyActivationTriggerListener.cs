using Nez;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Enemies
{
    public class EnemyActivationTriggerListener : Component, ITriggerListener
    {
        EnemyEntity thisEnemy;

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            thisEnemy = Entity as EnemyEntity;
        }
        public void OnTriggerEnter(Collider other, Collider local)
        {
            if (other.PhysicsLayer.IsFlagSet(Data.PhysicsLayers.camera_activator))
            {
                thisEnemy.Activate();
            }
        }

        public void OnTriggerExit(Collider other, Collider local)
        {
            if (other.PhysicsLayer.IsFlagSet(Data.PhysicsLayers.camera_activator))
            {
                thisEnemy.Deactivate();
            }
        }
    }
}
