using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Data
{
    public class Checkpoint : Entity
    {
        public int ID = 0;
        public Vector2 spawnPositionOffset;
        public Checkpoint(int ID, Vector2 size)
        {
            this.ID = ID;

            var collider = new BoxCollider(size.X, size.Y);
            collider.PhysicsLayer = Data.PhysicsLayers.checkpoint;
            collider.CollidesWithLayers = Data.PhysicsLayers.player_trigger;
            collider.IsTrigger = true;
            AddComponent(collider);

            var triggerHelper = new CheckpointComponent(ID);
            AddComponent(triggerHelper);

            spawnPositionOffset = new Vector2(0, size.Y / 2f);
        }
    }

    public class CheckpointComponent : Component, ITriggerListener
    {
        int ID;
        public CheckpointComponent(int ID)
        {
            this.ID = ID;
        }

        public void OnTriggerEnter(Collider other, Collider local)
        {
            if (other.PhysicsLayer.IsFlagSet(Data.PhysicsLayers.player_trigger))
            {
                Settings.Instance.currentCheckpoint = ID;
            }
        }

        public void OnTriggerExit(Collider other, Collider local)
        {
        }
    }
}
