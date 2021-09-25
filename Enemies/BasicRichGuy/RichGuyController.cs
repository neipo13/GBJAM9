using System;
using System.Collections.Generic;
using System.Text;
using Nez;
using Microsoft.Xna.Framework;
using Nez.Sprites;
using System.Linq;

namespace GBJAM9.Enemies.BasicRichGuy
{
    public enum RichGuyState
    {
        Walk,
        Hurt
    }
    public class RichGuyController : Nez.AI.FSM.SimpleStateMachine<RichGuyState>
    {

        float moveSpeed = 40f;
        float gravity = 700f;

        Vector2 velocity = new Vector2();

        Mover mover;
        CollisionResult collisionResult = new CollisionResult();
        List<CollisionResult> collisionResults = new List<CollisionResult>();
        SubpixelVector2 subPixelVector2 = new SubpixelVector2();
        public bool isGrounded => collisionResults.Any(c => c.Normal.Y < 0);

        public int direction = -1;

        SpriteAnimator animator;

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            mover = Entity.GetComponent<Mover>();
            animator = Entity.GetComponent<SpriteAnimator>();
            if(direction < 0)
            {
                animator.FlipX = true;
            }

            InitialState = RichGuyState.Walk;
        }


        #region Walk

        public void Walk_Enter()
        {
            animator.Play("run", SpriteAnimator.LoopMode.Loop);
        }

        public void Walk_Tick()
        {
            //just walk
            velocity.X = moveSpeed * direction;
            //grav
            velocity.Y += gravity * Time.DeltaTime;
            //move
            var movement = velocity * Time.DeltaTime;
            var expectedMoveX = movement.X;
            var moved = mover.AdvancedCalculateMovement(ref movement, collisionResults);
            subPixelVector2.Update(ref movement);
            mover.ApplyMovement(movement);
            //don't let gravity build while you're grounded
            if (isGrounded && velocity.Y > 0f) velocity.Y = 0f;

            if(collisionResults.Any(c => direction > 0 ? c.Normal.X < 0f : c.Normal.X > 0f))
            {
                direction *= -1;
                animator.FlipX = !animator.FlipX;
            }
        }
        #endregion
    }
}
