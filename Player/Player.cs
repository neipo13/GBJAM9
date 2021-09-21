using GBJAM9.SharedComponents;
using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Player
{
    public class Player : Entity
    {

        PlayerController controller;
        BoxCollider hurtBox;

        public Player() : base("player")
        {
            var sprite = new PrototypeSpriteRenderer(16, 24);
            sprite.Color = Color.Black;
            AddComponent(sprite);

            var moveCollider = new BoxCollider(16,24)
            {
                PhysicsLayer = Data.PhysicsLayers.move,
                CollidesWithLayers = Data.PhysicsLayers.tiles
            };
            AddComponent(moveCollider);

            hurtBox = new BoxCollider(12, 16);
            hurtBox.PhysicsLayer = Data.PhysicsLayers.player_hit;
            hurtBox.CollidesWithLayers = Data.PhysicsLayers.enemy_shoot;
            hurtBox.IsTrigger = true;
            AddComponent(hurtBox);

            var mover = new Mover();
            AddComponent(mover);

            var health = new Health(10);
            health.OnHit = OnHit;
            health.OnDeath = OnDeath;
            AddComponent(health);

            controller = new PlayerController();
            AddComponent(controller);
        }

        void OnHit()
        {
            controller.SetState(PlayerState.Hit);
        }

        void OnDeath()
        {
            Core.Scene = new Scenes.Game("test");
        }
    }
}
