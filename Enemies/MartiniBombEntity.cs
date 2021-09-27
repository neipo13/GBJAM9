using GBJAM9.SharedComponents;
using GBJAM9.Util;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Enemies
{
    public class MartiniBombEntity : Entity
    {
        public MartiniBombEntity(Sprite sprite): base("e_martini")
        {
            //triggerbox
            var collider = new BoxCollider(8, 8);
            collider.IsTrigger = true;
            collider.PhysicsLayer = Data.PhysicsLayers.enemy_shoot;
            collider.CollidesWithLayers = Data.PhysicsLayers.player_hit | Data.PhysicsLayers.tiles;
            AddComponent(collider);

            //sprite renderer
            var renderer = new SpriteRenderer(sprite);
            renderer.RenderLayer = (int)Data.RenderLayer.Object;
            AddComponent(renderer);

            AddComponent(new Mover());

            //rotation
            var controller = new MartiniBombComponent();
            AddComponent(controller);
            
        }

    }
    public class MartiniBombComponent : Component, IUpdatable, ITriggerListener
    {
        ColliderTriggerHelper triggerHelper;
        CollisionResult collisionResult = new CollisionResult();
        Mover mover;

        float rotationSpeed = (float)Math.PI * 1.0f;

        float fallSpeed = 0f;
        float gravity = 300f;
        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            triggerHelper = new ColliderTriggerHelper(Entity);
            mover = Entity.GetComponent<Mover>();
        }

        public void OnTriggerEnter(Collider other, Collider local)
        {
            if(other.Enabled && local.Enabled)
            {
                var health = other.Entity.GetComponent<Health>();
                if(health != null)
                {
                    health.Hit(1);
                }
                var splodeAnim = Aseprite.AespriteLoader.LoadSpriteAnimatorFromAesprite("img/mmboom", Entity.Scene.Content);
                var splode = new Explosion(splodeAnim);
                splode.Position = local.Entity.Position;
                Entity.Scene.AddEntity(splode);
                local.Entity.Destroy();
            }
        }

        public void OnTriggerExit(Collider other, Collider local)
        {
        }

        public void Update()
        {
            triggerHelper.Update();
            Entity.Rotation += rotationSpeed * Time.DeltaTime;

            fallSpeed += gravity * Time.DeltaTime;

            mover.Move(new Vector2(0, fallSpeed * Time.DeltaTime), out collisionResult);
        }
    }
}
