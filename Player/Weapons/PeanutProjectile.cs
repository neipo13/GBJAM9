using GBJAM9.Aseprite;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Player.Weapons
{
    public class PeanutProjectile : Entity, IProjectile
    {
        public string SpriteTexturePath => "img/peanut";

        public Vector2 direction { get; set; }
        public float speed { get; set; } = 200f;
        public SpriteAnimator animator { get; set; }
        public int PhysicsLayer { get; set; }
        public BoxCollider collider { get; }
        public bool active { get; set; }
        public Vector2 position 
        { 
            get => this.Transform.Position; 
            set 
            {
                this.Transform.Position = value;
            } 
        }

        float distanceTraveled = 0f;
        float maxDistanceSqrd = 200f;

        ProjectileMover mover;

        public PeanutProjectile(Scene scene) : base("peanut-shot")
        {
            animator = AespriteLoader.LoadSpriteAnimatorFromAesprite(SpriteTexturePath, scene.Content);
            animator.Play("shoot", SpriteAnimator.LoopMode.Loop);
            animator.Enabled = false;
            AddComponent(animator);

            collider = new BoxCollider(4, 4);
            collider.IsTrigger = true;
            collider.PhysicsLayer = Data.PhysicsLayers.player_shoot;
            collider.CollidesWithLayers = Data.PhysicsLayers.enemy_hit | Data.PhysicsLayers.tiles;
            collider.Enabled = false;
            AddComponent(collider);

            var listener = new ProjectileTriggerListener();
            listener.projectile = this;
            AddComponent(listener);

            mover = new ProjectileMover();
            AddComponent(mover);
        }

        public override void Update()
        {
            base.Update();
            var movement = direction * speed * Time.DeltaTime;
            mover.Move(movement);
            // track roughly how far we went to turn the bullet back off so it doesnt go forever 
            distanceTraveled += movement.LengthSquared();
            if(distanceTraveled > maxDistanceSqrd)
            {
                TurnOnOff(false);
            }
        }

        public void TurnOnOff(bool on)
        {
            active = on;
            collider.Enabled = on;
            animator.Enabled = on;
            distanceTraveled = 0f;
            Enabled = on;
        }
    }
}
