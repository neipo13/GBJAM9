using GBJAM9.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GBJAM9.Enemies.Gatsby
{
    public enum GatsbyState
    {
        Idle,
        Hit,
        JumpUp,
        RunAcross,
        Die
    }
    public class GatsbyController : Nez.AI.FSM.SimpleStateMachine<GatsbyState>
    {
        public float moveSpeed = 50f;
        readonly float jumpHeight = 8f * 12f; //tilesize * tiles high
        Mover mover;
        SubpixelVector2 subPixelVector2 = new SubpixelVector2();
        CollisionResult collisionResult;
        const float gravity = 200f;
        public bool isGrounded, groundedLastFrame;
        int direction = 1;

        BoxCollider hurtBox;

        Vector2 velocity;

        SpriteAnimator animator;

        public List<Vector2> bombSpawnLocations = new List<Vector2>();

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            mover = Entity.GetComponent<Mover>();
            animator = Entity.GetComponent<SpriteAnimator>();

            var colliders = Entity.GetComponents<BoxCollider>();
            hurtBox = colliders.SingleOrDefault(c => c.PhysicsLayer.IsFlagSet(Data.PhysicsLayers.enemy_hit));
            animator.FlipX = direction < 0f;
            animator.Speed = 0.75f;
            InitialState = GatsbyState.Idle;
        }
        public void PlayAnim(string animName, SpriteAnimator.LoopMode loopMode = SpriteAnimator.LoopMode.Loop, bool forceRestart = false)
        {
            if (animName == animator.CurrentAnimationName && !forceRestart)
            {
                return;
            }
            animator.Play(animName, loopMode);
        }

        public void SetState(GatsbyState state)
        {
            CurrentState = state;
        }

        public void Move()
        {
            var movement = velocity * Time.DeltaTime;
            mover.CalculateMovement(ref movement, out collisionResult);
            subPixelVector2.Update(ref movement);
            mover.ApplyMovement(movement);
        }

        public override void Update()
        {
            base.Update();
            groundedLastFrame = isGrounded;
            isGrounded = collisionResult.Normal.Y < 0;
        }

        public void SpawnMartiniBomb(Vector2 pos)
        {
            if (this.Entity == null || this.Entity.Scene == null) return;
            var texture = Entity.Scene.Content.Load<Texture2D>("img/martiniBomb");
            var bomb = new MartiniBombEntity(new Nez.Textures.Sprite(texture));
            bomb.Position = pos;
            Entity.Scene.AddEntity(bomb);
        }

        public Vector2 ClosestBombSpawnToLocation(Vector2 pos)
        {
            return bombSpawnLocations.OrderBy((l) => (l - pos).LengthSquared()).First();
        }



        #region Idle
        public float maxBombSpawnTime = 3.0f;
        public float minBombSpawnTime = 2.0f;
        public float bombSpawnTimer = 0f;

        void Idle_Enter()
        {
            //flip direction (only can enter this state when hitting the end of our run)
            direction *= -1;
            animator.FlipX = direction < 0f;
            //turn on hitbox
            bombSpawnTimer = Nez.Random.Range(minBombSpawnTime, maxBombSpawnTime);
            hurtBox.Enabled = true;
        }

        void Idle_Tick()
        {
            PlayAnim("idle");

            //spawn bombs at random points via bomb spawn intervals
            bombSpawnTimer -= Time.DeltaTime;
            if(bombSpawnTimer < 0f)
            {
                //spawn bomb
                var player = Entity.Scene.FindEntity("player");
                var pos = ClosestBombSpawnToLocation(player.Position);
                SpawnMartiniBomb(pos);

                //reset clock
                bombSpawnTimer = Nez.Random.Range(minBombSpawnTime, maxBombSpawnTime);
            }
        }
        #endregion

        #region JumpUp
        bool jumped = false;
        void JumpUp_Enter()
        {
            jumped = false;
            velocity = new Vector2();
            PlayAnim("jump", SpriteAnimator.LoopMode.ClampForever, true);
            animator.Speed = 0.5f;
            animator.CurrentFrame = 0;
        }

        void JumpUp_Tick()
        {
            if(isGrounded && animator.CurrentAnimationName == "jump" && animator.AnimationState == SpriteAnimator.State.Completed)
            {
                jumped = true;
                velocity.X = moveSpeed * direction * 0.8f;
                velocity.Y = -Mathf.Sqrt(2 * jumpHeight * gravity);
                animator.Speed = 0.75f;
            }
            else if(!jumped)
            {
                velocity.Y += gravity * Time.DeltaTime;
            }
            Move();
            if (!jumped) return;
            if (!isGrounded)
            {
                if (velocity.Y < 0)
                {
                    PlayAnim("jump_up", SpriteAnimator.LoopMode.ClampForever);
                }
                else
                {
                    PlayAnim("jump_down", SpriteAnimator.LoopMode.ClampForever);
                }
            }
            else
            {
                if(isGrounded && !groundedLastFrame)
                {
                    velocity.X = 0f;
                    Core.Schedule(0.5f, t => CurrentState = GatsbyState.RunAcross);
                }
                PlayAnim("land", SpriteAnimator.LoopMode.ClampForever);
            }

            //gravity
            velocity.Y += gravity * Time.DeltaTime;
        }

        #endregion

        #region RunAcross

        bool fell = false;

        Dictionary<Vector2, bool> martiniSpawned;

        void RunAcross_Enter()
        {
            fell = false;
            velocity = new Vector2();
            isGrounded = true;
            martiniSpawned = new Dictionary<Vector2, bool>();
            var orderedBombLocations = bombSpawnLocations.OrderBy(l => l.X).ToArray();
            for (int i = 0; i < orderedBombLocations.Length; i++)
            {
                // dont spawn the last 2 to give space
                if ((direction == -1 && i < 2) || (direction == 1 && i > (orderedBombLocations.Length - 3)))
                {
                    continue;
                }
                martiniSpawned.Add(orderedBombLocations[i], false);
            }
        }
        void RunAcross_Tick()
        {
            //gravity
            velocity.Y += gravity * Time.DeltaTime;

            if (!fell)
            {
                PlayAnim("run");
                velocity.X = direction * moveSpeed;
            }
            else
            {
                PlayAnim("jump_down", SpriteAnimator.LoopMode.ClampForever);
            }

            Move();
            //spawn bombs when passing over certain points
            var nextClosestPos = ClosestBombSpawnToLocation(Entity.Position);
            var nextClosestIncluded = martiniSpawned.ContainsKey(nextClosestPos);
            if (nextClosestIncluded)
            {
                var nextClosestSpawned = martiniSpawned[nextClosestPos];
                if (!nextClosestSpawned)
                {
                    martiniSpawned[nextClosestPos] = true;
                    SpawnMartiniBomb(nextClosestPos);
                }
            }

            if (isGrounded)
            {
                velocity.Y = 0;
                if (fell)
                {
                    CurrentState = GatsbyState.Idle;
                }
            }
            if (!(collisionResult.Normal.Y < 0))
            {
                fell = true;
            }
            if(collisionResult.Normal.X != 0)
            {
                velocity.X = 0f;
            }
        }
        #endregion

        #region Hit
        const float hitStunTime = 0.75f;
        float hitStunTimer = 0f;

        void Hit_Enter()
        {
            //turn off hitbox
            hitStunTimer = 0f;
            hurtBox.Enabled = false;
        }
        void Hit_Tick()
        {
            PlayAnim("hit", SpriteAnimator.LoopMode.Once);
            hitStunTimer += Time.DeltaTime;
            if(hitStunTimer > hitStunTime)
            {
                CurrentState = GatsbyState.JumpUp;
            }
        }
        #endregion

        #region die

        float minExplosionTime = 0.05f;
        float maxExplosionTime = 0.15f;
        float explosionTimer = 0f;

        float deathTime = 4.0f;
        float deathTimer = 0;

        float dieOffsetMax = 16f;

        bool wiping = false;

        void Die_Enter()
        {
            deathTimer = deathTime;
            PlayAnim("hit", SpriteAnimator.LoopMode.Once);
        }

        void Die_Tick()
        {
            //spawn explosions a bunch from a timer
            explosionTimer -= Time.DeltaTime;
            if(explosionTimer < 0f)
            {
                //spawn Explosion

                var splodeAnim = Aseprite.AespriteLoader.LoadSpriteAnimatorFromAesprite("img/mmboom", Entity.Scene.Content);
                var splode = new Explosion(splodeAnim);
                var offsetX = Nez.Random.Range(-dieOffsetMax, dieOffsetMax);
                var offsetY = Nez.Random.Range(-dieOffsetMax, dieOffsetMax);
                splode.Position = Entity.Position + new Vector2(offsetX, offsetY);
                Entity.Scene.AddEntity(splode);

                //reset timer
                explosionTimer = Nez.Random.Range(minExplosionTime, maxExplosionTime);
            }

            deathTimer -= Time.DeltaTime;
            if(deathTimer < 0f && !wiping)
            {
                wiping = true;
                //wipe screen white
                var e = new Entity();
                var wipe = new PrototypeSpriteRenderer(NezGame.designWidth * 2f, NezGame.designHeight * 2f);
                wipe.RenderLayer = (int)Data.RenderLayer.Foreground;
                wipe.Color = Color.White;
                wipe.Color.A = 0;
                e.AddComponent(wipe);
                e.Position = Entity.Scene.Camera.Position;
                Entity.Scene.AddEntity(e);
                wipe.TweenColorTo(Color.White, 3.0f)
                    .SetEaseType(Nez.Tweens.EaseType.CircInOut)
                    .SetCompletionHandler(t => Core.Schedule(1.0f, t2 => Core.Scene = new Scenes.SplashScreenScene(Scenes.SplashType.GBJAM)))
                    .Start();
            }
        }
        #endregion
    }
}
