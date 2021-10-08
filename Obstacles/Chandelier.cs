using GBJAM9.Player.Weapons;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tweens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GBJAM9.Obstacles
{
    public class Chandelier : Entity
    {

        public Chandelier(Sprite sprite)
        {
            //sprite renderer
            var renderer = new SpriteRenderer(sprite);
            renderer.FollowsParentRotation = false;
            renderer.RenderLayer = (int)Data.RenderLayer.Tiles;
            AddComponent(renderer);

            //mover
            AddComponent(new Mover());

            // collision box (tiles layer)
            var moveBox = new BoxCollider(32, 14);
            moveBox.LocalOffset = new Vector2(0, 2);
            moveBox.PhysicsLayer = Data.PhysicsLayers.tiles;
            moveBox.CollidesWithLayers = Data.PhysicsLayers.move;
            AddComponent(moveBox);

            // trigger box for falling
            var triggerBox = new BoxCollider(32, 500);
            triggerBox.IsTrigger = true;
            triggerBox.PhysicsLayer = Data.PhysicsLayers.checkpoint;
            triggerBox.CollidesWithLayers = Data.PhysicsLayers.player_trigger;
            AddComponent(triggerBox);

            // hit box to dmg player
            var hitBox = new BoxCollider(28, 8);
            hitBox.LocalOffset = new Vector2(0, 16);
            hitBox.IsTrigger = true;
            hitBox.PhysicsLayer = Data.PhysicsLayers.enemy_shoot | Data.PhysicsLayers.player_shoot;
            hitBox.CollidesWithLayers = Data.PhysicsLayers.player_hit | Data.PhysicsLayers.enemy_hit;
            AddComponent(hitBox);
            hitBox.Enabled = false;

            //projectile trigger listener
            var proj = new Player.Weapons.ProjectileTriggerListener();
            proj.dmg = 10;
            AddComponent(proj);
            proj.Enabled = false;

            var platformController = new PlatformController(Vector2.Zero, 0f, false);
            AddComponent(platformController);

            // controller
            var controller = new ChandelierController();
            AddComponent(controller);


        }
    }


    public class ChandelierController : Component, IUpdatable, ITriggerListener
    {
        SpriteRenderer sprite;
        Mover mover;
        PlatformController platformController;

        bool wobbling = false;
        bool falling = false;

        float gravity = 200f;

        float maxVelocity = 150f;
        Vector2 velocity = new Vector2();
        CollisionResult collisionResult = new CollisionResult();
        SubpixelVector2 subPixelVector2 = new SubpixelVector2();

        ColliderTriggerHelper triggerHelper;
        ProjectileTriggerListener projectileTriggerListener;

        BoxCollider moveBox, triggerBox, hitBox;

        

        float startX;
        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            sprite = Entity.GetComponent<SpriteRenderer>();
            mover = Entity.GetComponent<Mover>();
            triggerHelper = new ColliderTriggerHelper(Entity);
            platformController = Entity.GetComponent<PlatformController>();

            var colliders = Entity.GetComponents<BoxCollider>();
            moveBox = colliders.SingleOrDefault(c => c.PhysicsLayer.IsFlagSet(Data.PhysicsLayers.tiles));
            triggerBox = colliders.SingleOrDefault(c => c.PhysicsLayer.IsFlagSet(Data.PhysicsLayers.checkpoint));
            hitBox = colliders.SingleOrDefault(c => c.PhysicsLayer.IsFlagSet(Data.PhysicsLayers.enemy_shoot));


            projectileTriggerListener = Entity.GetComponent<ProjectileTriggerListener>();
            projectileTriggerListener.Enabled = false;
            startX = Entity.Position.X;
        }
        public void OnTriggerEnter(Collider other, Collider local)
        {
            if (other.Enabled && other.Entity.Name == "player" && !wobbling && !falling)
            {
                wobbling = true;
                float wobblerotation = (float)Math.PI / 10f;
                sprite.Tween("Rotation", wobblerotation, 0.3f)
                    .SetNextTween(sprite.Tween("Rotation", -wobblerotation, 0.6f)
                    .SetNextTween(sprite.Tween("Rotation", 0f, 0.3f)
                    .SetCompletionHandler(StartFall)))
                    .Start();

            }
            
        }

        public void StartFall(ITween<float> tween)
        {
            triggerHelper = null;
            triggerBox.Enabled = false;
            hitBox.Enabled = true;
            projectileTriggerListener.Enabled = true;
            Core.Schedule(0.1f, t =>
            {
                falling = true;
            });
        }

        public void OnTriggerExit(Collider other, Collider local) { }
        public void Update()
        {
            triggerHelper?.Update();
            if (falling)
            {
                velocity.X = 0f;
                velocity.Y += gravity * Time.DeltaTime;
                if (velocity.Y > maxVelocity) velocity.Y = maxVelocity;
                if (velocity.Y < 0) velocity.Y = 100f;
                //move
                var movement = velocity * Time.DeltaTime;
                mover.Move(movement, out collisionResult);
                platformController.UpdateRiders(movement);

                if (startX != Entity.Position.X)
                {
                    var test = 1;
                }

                if (Entity.Position.Y > 1000 || Entity.Position.Y < -1000) this.Entity.Destroy();
            }
        }
    }
}
