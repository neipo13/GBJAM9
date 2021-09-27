using GBJAM9.Effects;
using GBJAM9.SharedComponents;
using GBJAM9.Util;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Enemies
{
    public class EnemyEntity : Entity
    {
        public Health health;
        public SpriteAnimator animator;
        public BoxCollider hurtBox;
        public BoxCollider hitBox;
        public BoxCollider moveBox;
        public WhiteFlashMaterial whiteFlashMaterial;

        public const float FLASH_TIME = 0.15f;

        public EnemyEntity(string entityName, WhiteFlashMaterial whiteFlashMaterial, SpriteAnimator animator, int hp = 3) : base(entityName)
        {
            health = new Health(hp);

            this.whiteFlashMaterial = whiteFlashMaterial;

            this.animator = animator;
            animator.Enabled = false;
            AddComponent(this.animator);

            hurtBox = new BoxCollider(16, 16);
            hurtBox.PhysicsLayer = Data.PhysicsLayers.enemy_hit;
            hurtBox.CollidesWithLayers = Data.PhysicsLayers.player_shoot;
            hurtBox.IsTrigger = true;
            AddComponent(hurtBox);

            hitBox = new BoxCollider(16, 16);
            hitBox.PhysicsLayer = Data.PhysicsLayers.enemy_shoot;
            hitBox.CollidesWithLayers = Data.PhysicsLayers.player_hit;
            hitBox.IsTrigger = true;
            AddComponent(hitBox);

            moveBox = new BoxCollider(16, 16);
            moveBox.PhysicsLayer = Data.PhysicsLayers.move;
            moveBox.CollidesWithLayers = Data.PhysicsLayers.tiles;
            AddComponent(moveBox);

            var triggerBox = new BoxCollider(16, 16);
            triggerBox.PhysicsLayer = Data.PhysicsLayers.enemy_trigger;
            triggerBox.CollidesWithLayers = Data.PhysicsLayers.camera_activator;
            triggerBox.IsTrigger = true;
            AddComponent(triggerBox);

            health = new Health(4);
            health.OnHit = OnHit;
            health.OnDeath = OnDeath;
            AddComponent(health);

            var triggerActivator = new EnemyActivationTriggerListener();
            AddComponent(triggerActivator);

            Core.Schedule(0.1f, (t) => Deactivate());
        }

        protected virtual bool OnHit()
        {
            //white flash for a bit then remove
            if(animator != null)
            {
                animator.Material = whiteFlashMaterial;
                Core.Schedule(FLASH_TIME, (t) => {
                    if(animator != null) animator.Material = null;
                });
            }
            return true;
        }

        protected virtual void OnDeath()
        {
            //explosion
            var numExplosions = Nez.Random.Range(2, 6);
            for (int i = 0; i < numExplosions; i++)
            {
                var splodeAnim = Aseprite.AespriteLoader.LoadSpriteAnimatorFromAesprite("img/mmboom", Scene.Content);
                var splode = new Explosion(splodeAnim);
                var offsetX = Nez.Random.Range(-16f, 16f);
                var offsetY = Nez.Random.Range(-16f, 16f);
                splode.Position = Position + new Vector2(offsetX, offsetY);
                Scene.AddEntity(splode);
            }
            //remove
            animator.Material = null;
            whiteFlashMaterial = null;
            hitBox = null;
            hurtBox = null;
            moveBox = null;
            animator = null;
            health = null;
            this.Destroy();
        }

        public virtual void Activate()
        {
            animator.Enabled = true;
            hitBox.SetEnabled(true);
            hurtBox.SetEnabled(true);
            moveBox.SetEnabled(true);
        }

        public virtual void Deactivate()
        {
            animator.Enabled = false;
            hitBox.SetEnabled(false);
            hurtBox.SetEnabled(false);
            moveBox.SetEnabled(false);
        }
    }
}
