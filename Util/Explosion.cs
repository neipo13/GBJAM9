using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Util
{
    public class Explosion : Entity
    {
        bool causesShake = false;
        SpriteAnimator animator;
        public Explosion(SpriteAnimator animator, bool causesShake = true): base("splode")
        {
            this.animator = animator;
            this.causesShake = causesShake;
            animator.RenderLayer = (int)Data.RenderLayer.Object;
            AddComponent(this.animator);
            this.animator.OnAnimationCompletedEvent += DestroySelf;
            animator.Play("boom", SpriteAnimator.LoopMode.ClampForever);
        }

        public void DestroySelf(string animName)
        {
            this.Destroy();
        }

        public override void OnAddedToScene()
        {
            base.OnAddedToScene();
            if(causesShake)
            {
                Scene.Camera.GetComponent<CameraShake>().Shake(8f, 0.4f);
            }
        }

        public override void OnRemovedFromScene()
        {
            animator.OnAnimationCompletedEvent -= DestroySelf;
            base.OnRemovedFromScene();
        }
    }
}
