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
    public class PlayerController : Nez.AI.FSM.SimpleStateMachine<PlayerState>
    {
        SpriteAnimator animator;
        BoxCollider moveCollider;
        Mover mover;
        InputHandler input;
        CollisionResult collisionResult;
        List<CollisionResult> collisionResults;
        IWeapon[] weapons;
        public int currentWeaponIndex = 0;

        Vector2 velocity;

        public float moveSpeed = 95f;
        const float gravity = 700f;

        public bool isGrounded => collisionResult.Normal.Y < 0;

        public bool wasGroundedLastFrame;
        public const int offGroundInputBufferFrames = 8;
        public int offGroundInputBufferTimer = 0;
        public const int landingInputBufferFrames = 4;
        public int landingInputBufferTimer = 0;
        public int justJumpedBufferTimer = 0;
        public bool canJumpThisFrame => (isGrounded || (offGroundInputBufferTimer > 0 && justJumpedBufferTimer <= 0));


        SubpixelVector2 subPixelVector2 = new SubpixelVector2();

        readonly float jumpHeight = 8 * 6.5f; //tilesize * tiles high

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            animator = Entity.GetComponent<SpriteAnimator>();
            if (animator == null)
            {
                animator = new SpriteAnimator();
            }
            moveCollider = Entity.GetComponent<BoxCollider>();
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
            wasGroundedLastFrame = isGrounded;
            base.Update();
        }

        #region Normal

        void Normal_Tick()
        {
            Move();
            if(input.XInput < 0f)
            {
                animator.FlipX = true;
            }
            else if(input.XInput > 0f)
            {
                animator.FlipX = false;
            }
            if (input.ShootButton.IsPressed)
            {
                weapons[currentWeaponIndex]?.Shoot(!animator.FlipX);
            }
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
    }
}
