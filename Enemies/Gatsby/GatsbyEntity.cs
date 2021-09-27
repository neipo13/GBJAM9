using GBJAM9.Effects;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Enemies.Gatsby
{
    public class GatsbyEntity : EnemyEntity
    {
        GatsbyController controller;
        public GatsbyEntity(SpriteAnimator animator, WhiteFlashMaterial mat, List<Vector2> martiniPositions) : base("Gatsby", mat, animator, 5)
        {
            moveBox.SetSize(14, 32);
            hurtBox.SetSize(14, 32);
            hitBox.SetSize(14, 32);


            AddComponent(new Mover());

            controller = new GatsbyController();
            controller.bombSpawnLocations = martiniPositions;
            controller.Enabled = false;
            AddComponent(controller);
        }
        public override void Activate()
        {
            base.Activate();
            controller.Enabled = true;
        }
        public override void Deactivate()
        {
        }

        public override void OnRemovedFromScene()
        {
            base.OnRemovedFromScene();
            whiteFlashMaterial = null;
        }

        protected override bool OnHit()
        {
            base.OnHit();
            controller.SetState(GatsbyState.Hit);
            return true;
        }

        protected override void OnDeath()
        {
            controller.SetState(GatsbyState.Die);
        }
    }
}
