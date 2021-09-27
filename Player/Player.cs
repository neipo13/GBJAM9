using GBJAM9.SharedComponents;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Player
{
    public class Player : Entity
    {

        PlayerController controller;
        BoxCollider hurtBox;
        Health health;
        SpriteAnimator animator;

        public Entity aimPoint;

        public Player(SpriteAnimator sprite) : base("player")
        {
            animator = sprite;
            AddComponent(animator);

            var moveCollider = new BoxCollider(10, 24);
            moveCollider.LocalOffset = new Vector2(0, 4);
            moveCollider.PhysicsLayer = Data.PhysicsLayers.move;
            moveCollider.CollidesWithLayers = Data.PhysicsLayers.tiles;
            AddComponent(moveCollider);

            hurtBox = new BoxCollider(8, 16);
            hurtBox.LocalOffset = new Vector2(0, 4);
            hurtBox.PhysicsLayer = Data.PhysicsLayers.player_hit;
            hurtBox.CollidesWithLayers = Data.PhysicsLayers.enemy_shoot;
            hurtBox.IsTrigger = true;
            AddComponent(hurtBox);

            var triggerBox = new BoxCollider(16, 24);
            triggerBox.LocalOffset = new Vector2(0, 4);
            triggerBox.PhysicsLayer = Data.PhysicsLayers.player_trigger;
            triggerBox.CollidesWithLayers = Data.PhysicsLayers.checkpoint;
            triggerBox.IsTrigger = true;
            AddComponent(triggerBox);

            var mover = new Mover();
            AddComponent(mover);

            health = new Health(3);
            health.OnHit = OnHit;
            health.OnDeath = OnDeath;
            AddComponent(health);

            aimPoint = new Entity("aim");
            aimPoint.SetParent(this);
            aimPoint.LocalPosition = new Vector2(16f, 0f);

            controller = new PlayerController();
            controller.aimPoint = aimPoint;
            AddComponent(controller);
        }

        bool OnHit()
        {
            if (controller.invincible) return false;
            controller.SetState(PlayerState.Hit);
            Scene.Camera.GetComponent<CameraShake>().Shake(10f);
            return true;
        }

        void OnDeath()
        {
            health.Reset();
            //get checkpoint location & set player position
            var gameScene = Core.Scene as Scenes.GameScene;
            var checkpoint = gameScene.checkpoints[Data.Settings.Instance.currentCheckpoint];
            Position = checkpoint.Position + checkpoint.spawnPositionOffset;
        }
    }
}
