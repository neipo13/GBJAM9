using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Util
{
    public class Explosion : Entity
    {
        SpriteAnimator animator;
        public Explosion(SpriteAnimator animator): base("splode")
        {
            this.animator = animator;
            animator.RenderLayer = (int)Data.RenderLayer.Object;
            AddComponent(this.animator);
            this.animator.OnAnimationCompletedEvent += DestroySelf;
            animator.Play("boom", SpriteAnimator.LoopMode.ClampForever);
        }

        public void DestroySelf(string animName)
        {
            this.Destroy();
        }

        public override void OnRemovedFromScene()
        {
            animator.OnAnimationCompletedEvent -= DestroySelf;
            base.OnRemovedFromScene();
        }
    }
}
