using GBJAM9.Input;
using GBJAM9.Player.Weapons;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GBJAM9.Player
{
    public enum PlayerState
    {
        Normal,
        Hit,
        Dead,
        RoomTransition,
        DebugFly
    }
    public class PlayerController : Nez.AI.FSM.SimpleStateMachine<PlayerState>, ITriggerListener
    {
        SpriteAnimator animator;
        BoxCollider moveCollider;
        BoxCollider hurtCollider;
        Mover mover;
        InputHandler input;
        CollisionResult collisionResult;
        List<CollisionResult> collisionResults;
        IWeapon[] weapons;
        public int currentWeaponIndex = 0;

        Vector2 velocity;

        public float moveSpeed = 95f;
        public float hitSlideSpeed = 50f;
        const float gravity = 700f;

        public Entity aimPoint;

        public bool isGrounded => collisionResult.Normal.Y < 0;

        public bool wasGroundedLastFrame;
        public const int offGroundInputBufferFrames = 8;
        public int offGroundInputBufferTimer = 0;
        public const int landingInputBufferFrames = 4;
        public int landingInputBufferTimer = 0;
        public int justJumpedBufferTimer = 0;

        public float invincibilityTime = 0.75f;
        public float invincibilityTimer = 0f;
        public bool invincible => invincibilityTimer > 0f;
        public bool canJumpThisFrame => (isGrounded || (offGroundInputBufferTimer > 0 && justJumpedBufferTimer <= 0));


        SubpixelVector2 subPixelVector2 = new SubpixelVector2();

        readonly float jumpHeight = 8 * 6.5f; //tilesize * tiles high

        float hitDirX = 0f;

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            animator = Entity.GetComponent<SpriteAnimator>();
            var colliders = Entity.GetComponents<BoxCollider>();
            hurtCollider = colliders.SingleOrDefault(c => c.PhysicsLayer == Data.PhysicsLayers.player_hit);
            mover = Entity.GetComponent<Mover>();

            input = InputManager.Instance.GetInput(0); // just grab player 1 for now
            collisionResult = new CollisionResult();
            collisionResults = new List<CollisionResult>();


            var weapTypes = Data.Settings.Instance.weaponsAvailable;
            weapons = new IWeapon[weapTypes.Count];
            for (int i = 0; i < weapons.Length; i++)
            {
                var wType = weapTypes[i];
                IWeapon weapon = null;
                switch (wType)
                {
                    case WeaponType.Buster:
                        var buster = new BusterWeapon(Entity.Scene);
                        weapon = buster;
                        Entity.AddComponent(buster);
                        break;
                }
                weapons[i] = weapon;
            }

            InitialState = PlayerState.Normal;
        }

        public override void Update()
        {
            if(invincibilityTimer > 0f)
            {
                invincibilityTimer -= Time.DeltaTime;
            }
            wasGroundedLastFrame = isGrounded;
            base.Update();
        }

        public void SetState(PlayerState state)
        {
            CurrentState = state;
        }
        public void PlayAnim(string animName, SpriteAnimator.LoopMode loopMode = SpriteAnimator.LoopMode.Loop, bool forceRestart = false)
        {
            if (animName == animator.CurrentAnimationName && !forceRestart)
            {
                return;
            }
            animator.Play(animName, loopMode);
        }

        #region Normal

        void ChooseNormalAnimation()
        {
            if (isGrounded)
            {
                if (input.XInput < 0f)
                {
                    PlayAnim("run_left");
                }
                else if (input.XInput > 0f)
                {
                    PlayAnim("run_right");
                }
                else
                {
                    if (animator.FlipX)
                    {
                        PlayAnim("idle_left");
                    }
                    else
                    {
                        PlayAnim("idle_right");
                    }
                }
            }
            else
            {
                if (animator.FlipX)
                {
                    PlayAnim("jump_left");
                }
                else
                {
                    PlayAnim("jump_right");
                }
            }
        }
        void Normal_Enter()
        {

        }
        void Normal_Tick()
        {
            Move();
            if (input.XInput < 0f)
            {
                animator.FlipX = true;
                aimPoint.LocalPosition = new Vector2(Math.Abs(aimPoint.Transform.LocalPosition.X) * -1, aimPoint.Transform.LocalPosition.Y);
            }
            else if (input.XInput > 0f)
            {
                animator.FlipX = false;
                aimPoint.LocalPosition = new Vector2(Math.Abs(aimPoint.Transform.LocalPosition.X), aimPoint.Transform.LocalPosition.Y);
            }
            if (input.ShootButton.IsPressed)
            {
                IProjectile bullet = weapons[currentWeaponIndex]?.Shoot(!animator.FlipX);
                if(bullet != null)
                {
                    var direction = animator.FlipX ? -1 : 1;
                    bullet.position += new Vector2(4f * direction, 4f);
                }
            }
            ChooseNormalAnimation();
        }

        void Move()
        {
            //left-right
            velocity.X = moveSpeed * input.XInput;
            //jump
            if (input.JumpButton.IsPressed)
            {
                if (canJumpThisFrame) Jump();
                else landingInputBufferTimer = landingInputBufferFrames;
            }
            // jump if you recently hit jump before you landed
            else if (landingInputBufferTimer > 0)
            {
                landingInputBufferTimer--;
                if (isGrounded) Jump();
            }
            // handle variable jump height
            if (!isGrounded && input.JumpButton.IsReleased && velocity.Y < 0f)
            {
                velocity.Y *= 0.5f;
            }
            //gravity
            velocity.Y += gravity * Time.DeltaTime;

            var movement = velocity * Time.DeltaTime;
            mover.CalculateMovement(ref movement, out collisionResult);
            subPixelVector2.Update(ref movement);
            mover.ApplyMovement(movement);

            //don't let gravity build while you're grounded
            if (isGrounded && velocity.Y > 0f) velocity.Y = 0f;

            // tick jump input buffer timer
            if (offGroundInputBufferTimer > 0)
                offGroundInputBufferTimer--;
            if (justJumpedBufferTimer > 0)
                justJumpedBufferTimer--;
        }

        void Jump()
        {
            velocity.Y = -Mathf.Sqrt(2 * jumpHeight * gravity);
            landingInputBufferTimer = 0;
            justJumpedBufferTimer = offGroundInputBufferFrames;
        }
        #endregion

        #region Debug Fly Stuff
        public void DebugFly_Enter()
        {
            var collisionBoxes = Entity.GetComponents<BoxCollider>();
            foreach (var box in collisionBoxes)
            {
                box.Enabled = false;
            }
        }

        public void DebugFly_Tick()
        {
            velocity = input.LeftStickInput * moveSpeed;
            mover.Move(velocity * Time.DeltaTime, out collisionResult);
        }

        public void DebugFly_Exit()
        {
            var collisionBoxes = Entity.GetComponents<BoxCollider>();
            foreach (var box in collisionBoxes)
            {
                box.Enabled = true;
            }
        }

        public void SwitchModes()
        {
            if (CurrentState == PlayerState.Normal)
            {
                CurrentState = PlayerState.DebugFly;
            }
            else if (CurrentState == PlayerState.DebugFly)
            {
                CurrentState = PlayerState.Normal;
            }
        }
        #endregion

        #region Room Transition

        public void StartRoomTransition()
        {
            CurrentState = PlayerState.RoomTransition;
        }

        public void EndRoomTransition()
        {
            CurrentState = PlayerState.Normal;
        }
        #endregion

        #region Hit
        void Hit_Enter()
        {
            Jump();
            hurtCollider.SetEnabled(false);
            invincibilityTimer = invincibilityTime;
        }

        void Hit_Tick()
        {
            //change anim to hit
            PlayAnim("hurt", SpriteAnimator.LoopMode.ClampForever);

            velocity.X = hitSlideSpeed * -hitDirX;
            velocity.Y += gravity * Time.DeltaTime;

            var movement = velocity * Time.DeltaTime;
            mover.CalculateMovement(ref movement, out collisionResult);
            subPixelVector2.Update(ref movement);
            mover.ApplyMovement(movement);

            if (isGrounded) CurrentState = PlayerState.Normal;
        }

        void Hit_Exit()
        {
            //change anim to idle
            hurtCollider.SetEnabled(true);
            PlayAnim("recover", SpriteAnimator.LoopMode.ClampForever, true);
        }
        #endregion

        #region ITriggerListener
        public void OnTriggerEnter(Collider other, Collider local)
        {
            if (other.Enabled && (other.PhysicsLayer & Data.PhysicsLayers.enemy_shoot) != 0)
            {
                var health = this.Entity.GetComponent<SharedComponents.Health>();
                health.Hit(1);

                bool right = (other.Entity.Position.X - local.Entity.Position.X) > 0;

                if (right)
                {
                    hitDirX = 1;
                }
                else
                {
                    hitDirX = -1;
                }
            }
        }

        public void OnTriggerExit(Collider other, Collider local)
        {
        }
        #endregion
    }
}
