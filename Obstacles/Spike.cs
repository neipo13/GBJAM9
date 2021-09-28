using Nez;
using Nez.Sprites;
using Nez.Textures;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Obstacles
{
    public class Spike : Entity
    {
        public Spike(Sprite sprite) : base("spike")
        {
            var spriteRenderer = new SpriteRenderer(sprite);
            spriteRenderer.RenderLayer = (int)Data.RenderLayer.Tiles;
            AddComponent(spriteRenderer);

            var collider = new BoxCollider(6, 6);
            collider.IsTrigger = true;
            collider.PhysicsLayer = Data.PhysicsLayers.enemy_shoot;
            collider.CollidesWithLayers = Data.PhysicsLayers.player_hit;
            AddComponent(collider);

            var controller = new SpikeController();
            AddComponent(controller);
        }

        public class SpikeController : Component, IUpdatable, ITriggerListener
        {
            ColliderTriggerHelper triggerHelper;

            public override void OnAddedToEntity()
            {
                base.OnAddedToEntity();
                triggerHelper = new ColliderTriggerHelper(Entity);
            }
            public void OnTriggerEnter(Collider other, Collider local)
            {
                if(other.Enabled && local.Enabled && other.PhysicsLayer.IsFlagSet(Data.PhysicsLayers.player_hit))
                {
                    var health = other.Entity.GetComponent<SharedComponents.Health>();
                    if (health != null)
                    {
                        health.Hit(999);
                    }
                }
            }

            public void OnTriggerExit(Collider other, Collider local)
            {
            }

            public void Update()
            {
                triggerHelper.Update();
            }
        }

    }
}
