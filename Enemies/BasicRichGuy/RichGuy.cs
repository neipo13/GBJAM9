using GBJAM9.Effects;
using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Enemies.BasicRichGuy
{
    public class RichGuy : EnemyEntity
    {
        RichGuyController controller;
        public RichGuy(string entityName, WhiteFlashMaterial whiteFlashMaterial, SpriteAnimator animator) : base(entityName, whiteFlashMaterial, animator)
        {
            moveBox.SetSize(12, 24);
            hurtBox.SetSize(12, 24);
            hitBox.SetSize(12, 24);

            AddComponent(new Mover());

            controller = new RichGuyController();
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
            base.Deactivate();
            controller.Enabled = false;
        }

        public override void OnRemovedFromScene()
        {
            base.OnRemovedFromScene();
            whiteFlashMaterial = null;
        }
    }
}
