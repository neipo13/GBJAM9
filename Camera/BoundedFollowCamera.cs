using GBJAM9.Player;
using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GBJAM9.Camera
{
    public enum CameraStyle
    {
        BoundedFollow,
        RoomTransition
    }
    public class BoundedFollowCamera : Nez.AI.FSM.SimpleStateMachine<CameraStyle>
    {
        public enum CameraStyle
        {
            LockOn,
            CameraWindow
        }

        public Nez.Camera camera;

        /// <summary>
        /// how fast the camera closes the distance to the target position
        /// </summary>
        public float followLerp = 0.1f;

        /// <summary>
        /// when in CameraWindow mode the width/height is used as a bounding box to allow movement within it without moving the camera.
        /// when in LockOn mode only the deadzone x/y values are used. This is set to sensible defaults when you call follow but you are
        /// free to override it to get a custom deadzone directly or via the helper setCenteredDeadzone.
        /// </summary>
        public RectangleF deadzone;

        /// <summary>
        /// offset from the screen center that the camera will focus on
        /// </summary>
        public Vector2 focusOffset;

        /// <summary>
        /// If true, the camera position will not got out of the map rectangle (0,0, mapwidth, mapheight)
        /// </summary>
        public bool mapLockEnabled;

        /// <summary>
        /// Contains the width and height of the current map.
        /// </summary>
        public Vector2 mapSize;

        Entity _targetEntity;
        Collider _targetCollider;
        Vector2 _desiredPositionDelta;
        CameraStyle _cameraStyle;
        RectangleF _worldSpaceDeadzone;
        public CameraShake cameraShake = null;

        public Vector2 min, max;
        public Vector2 transitionStart, transitionEnd, targetTransitionEnd;
        public CameraArea[] cameraBounds;
        public int boundaryWidth = 12; // how close to the boundary must the player be before the camera changes

        public PlayerController playerController;

        public BoundedFollowCamera(Entity targetEntity, Nez.Camera camera, CameraArea current, CameraArea[] bounds)
        {
            _targetEntity = targetEntity;
            this.camera = camera;
            cameraBounds = (CameraArea[])bounds.Clone();
            SetBounds(current.id);
        }


        public BoundedFollowCamera(Entity targetEntity, CameraArea current, CameraArea[] bounds) : this(targetEntity, null, current, bounds)
        { }


        public override void OnAddedToEntity()
        {
            if (camera == null)
                camera = Entity.Scene.Camera;

            follow(_targetEntity, _cameraStyle);

            playerController = _targetEntity.GetComponent<PlayerController>();
            InitialState = Camera.CameraStyle.BoundedFollow;

            // listen for changes in screen size so we can keep our deadzone properly positioned
            Core.Emitter.AddObserver(CoreEvents.GraphicsDeviceReset, onGraphicsDeviceReset);
        }


        public override void OnRemovedFromEntity()
        {
            Core.Emitter.RemoveObserver(CoreEvents.GraphicsDeviceReset, onGraphicsDeviceReset);
        }


        public override void Update()
        {
            base.Update();
        }

        void BoundedFollow_Tick()
        {
            if (cameraShake != null && cameraShake.Enabled) return;
            // translate the deadzone to be in world space
            var halfScreen = Entity.Scene.SceneRenderTargetSize.ToVector2() * 0.5f;
            _worldSpaceDeadzone.X = camera.Position.X - halfScreen.X + deadzone.X + focusOffset.X;
            _worldSpaceDeadzone.Y = camera.Position.Y - halfScreen.Y + deadzone.Y + focusOffset.Y;
            _worldSpaceDeadzone.Width = deadzone.Width;
            _worldSpaceDeadzone.Height = deadzone.Height;

            if (_targetEntity != null)
                updateFollow();

            camera.Position = Vector2.Lerp(camera.Position, camera.Position + _desiredPositionDelta, followLerp);
            camera.Entity.Transform.RoundPosition();

            if (mapLockEnabled)
            {
                camera.Position = clampToMapSize(camera.Position);
                camera.Entity.Transform.RoundPosition();
            }

            var cameraBounds = Entity.Scene.Camera.Bounds;

            //ease back into place?
            if (cameraBounds.Top < min.Y)
                Entity.Scene.Camera.Position += new Vector2(0, min.Y - cameraBounds.Top);

            if (cameraBounds.Left < min.X)
                Entity.Scene.Camera.Position += new Vector2(min.X - cameraBounds.Left, 0);

            if (cameraBounds.Bottom > max.Y)
                Entity.Scene.Camera.Position += new Vector2(0, max.Y - cameraBounds.Bottom);

            if (cameraBounds.Right > max.X)
                Entity.Scene.Camera.Position += new Vector2(max.X - cameraBounds.Right, 0);

            MoveCamera(CheckTargetExit());
        }

        void RoomTransition_Enter()
        {
            // set player controller to RoomTransitionState & setup tweens to handle the movement
            this.Transform.Position = transitionStart;
            playerController.StartRoomTransition();
            var roomTransitionTimer = 0.5f;
            this.Transform.TweenPositionTo(transitionEnd, roomTransitionTimer).SetCompletionHandler(t => CurrentState = Camera.CameraStyle.BoundedFollow).Start();
            _targetEntity.Transform.TweenPositionTo(targetTransitionEnd, roomTransitionTimer).Start();
        }

        void RoomTransition_Tick()
        {
            // translate the position (acutally being handled by the tween)

        }

        void RoomTransition_Exit()
        {
            //set player controller to normal state
            playerController.EndRoomTransition();
        }

        /// <summary>
        /// Clamps the camera so it never leaves the visible area of the map.
        /// </summary>
        /// <returns>The to map size.</returns>
        /// <param name="position">Position.</param>
        Vector2 clampToMapSize(Vector2 position)
        {
            var halfScreen = new Vector2(camera.Bounds.Width, camera.Bounds.Height) * 0.5f;
            var cameraMax = new Vector2(mapSize.X - halfScreen.X, mapSize.Y - halfScreen.Y);

            return Vector2.Clamp(position, halfScreen, cameraMax);
        }


        public override void DebugRender(Batcher batcher)
        {
            if (_cameraStyle == CameraStyle.LockOn)
                batcher.DrawHollowRect(_worldSpaceDeadzone.X - 5, _worldSpaceDeadzone.Y - 5, _worldSpaceDeadzone.Width, _worldSpaceDeadzone.Height, Color.DarkRed);
            else
                batcher.DrawHollowRect(_worldSpaceDeadzone, Color.DarkRed);
        }


        void onGraphicsDeviceReset()
        {
            // we need this to occur on the next frame so the camera bounds are updated
            Core.Schedule(0f, this, t =>
            {
                var self = t.Context as BoundedFollowCamera;
                self.follow(self._targetEntity, self._cameraStyle);
            });
        }


        void updateFollow()
        {
            _desiredPositionDelta.X = _desiredPositionDelta.Y = 0;

            if (_cameraStyle == CameraStyle.LockOn)
            {
                var targetX = _targetEntity.Transform.Position.X;
                var targetY = _targetEntity.Transform.Position.Y;

                // x-axis
                if (_worldSpaceDeadzone.X > targetX)
                    _desiredPositionDelta.X = targetX - _worldSpaceDeadzone.X;
                else if (_worldSpaceDeadzone.X < targetX)
                    _desiredPositionDelta.X = targetX - _worldSpaceDeadzone.X;

                // y-axis
                if (_worldSpaceDeadzone.Y < targetY)
                    _desiredPositionDelta.Y = targetY - _worldSpaceDeadzone.Y;
                else if (_worldSpaceDeadzone.Y > targetY)
                    _desiredPositionDelta.Y = targetY - _worldSpaceDeadzone.Y;
            }
            else
            {
                // make sure we have a targetCollider for CameraWindow. If we dont bail out.
                if (_targetCollider == null)
                {
                    _targetCollider = _targetEntity.GetComponent<Collider>();
                    if (_targetCollider == null)
                        return;
                }

                var targetBounds = _targetEntity.GetComponent<Collider>().Bounds;
                if (!_worldSpaceDeadzone.Contains(targetBounds))
                {
                    // x-axis
                    if (_worldSpaceDeadzone.Left > targetBounds.Left)
                        _desiredPositionDelta.X = targetBounds.Left - _worldSpaceDeadzone.Left;
                    else if (_worldSpaceDeadzone.Right < targetBounds.Right)
                        _desiredPositionDelta.X = targetBounds.Right - _worldSpaceDeadzone.Right;

                    // y-axis
                    if (_worldSpaceDeadzone.Bottom < targetBounds.Bottom)
                        _desiredPositionDelta.Y = targetBounds.Bottom - _worldSpaceDeadzone.Bottom;
                    else if (_worldSpaceDeadzone.Top > targetBounds.Top)
                        _desiredPositionDelta.Y = targetBounds.Top - _worldSpaceDeadzone.Top;
                }
            }
        }


        public void follow(Entity targetEntity, CameraStyle cameraStyle = CameraStyle.CameraWindow)
        {
            _targetEntity = targetEntity;
            _cameraStyle = cameraStyle;
            var cameraBounds = camera.Bounds;

            switch (_cameraStyle)
            {
                case CameraStyle.CameraWindow:
                    var w = (cameraBounds.Width / 6);
                    var h = (cameraBounds.Height / 3);
                    deadzone = new RectangleF((cameraBounds.Width - w) / 2, (cameraBounds.Height - h) / 2, w, h);
                    break;
                case CameraStyle.LockOn:
                    deadzone = new RectangleF(cameraBounds.Width / 2, cameraBounds.Height / 2, 10, 10);
                    break;
            }
        }


        /// <summary>
        /// sets up the deadzone centered in the current cameras bounds with the given size
        /// </summary>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public void setCenteredDeadzone(int width, int height)
        {
            var cameraBounds = camera.Bounds;
            deadzone = new RectangleF((cameraBounds.Width - width) / 2, (cameraBounds.Height - height) / 2, width, height);
        }

        public void SetBounds(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;
        }

        public void SetBounds(int id)
        {
            var area = cameraBounds.Single(a => a.id == id);
            this.min = new Vector2(area.bounds.Left, area.bounds.Top);
            this.max = new Vector2(area.bounds.Right, area.bounds.Bottom);
        }
        public void SetBounds(Rectangle rect)
        {
            this.min = new Vector2(rect.Left, rect.Top);
            this.max = new Vector2(rect.Right, rect.Bottom);
        }
        Vector2 CheckTargetExit()
        {
            Vector2 direction = Vector2.Zero;

            if (_targetEntity.Position.X > (max.X - boundaryWidth))
                direction.X = 1;
            else if (_targetEntity.Position.X < (min.X + boundaryWidth))
                direction.X = -1;
            else if (_targetEntity.Position.Y > (max.Y - boundaryWidth))
                direction.Y = 1;
            else if (_targetEntity.Position.Y < (min.Y + boundaryWidth))
                direction.Y = -1;

            return direction;
        }

        public CameraArea FindBoundsByPosition(Vector2 pos)
        {
            return cameraBounds.SingleOrDefault(b => b.bounds.Contains(pos));
        }

        void MoveCamera(Vector2 direction)
        {
            if (direction.X > 0)
            {
                transitionStart = this.Transform.Position;
                targetTransitionEnd = _targetEntity.Position;
                targetTransitionEnd.X += boundaryWidth * 2 + 2; //fake moving to next screen
                transitionEnd = transitionStart + new Vector2(NezGame.designWidth, 0);
                var newBounds = FindBoundsByPosition(targetTransitionEnd);
                if (newBounds != null)
                {
                    SetBounds(newBounds.id);
                }
                this.CurrentState = Camera.CameraStyle.RoomTransition;
            }
            else if (direction.Y > 0)
            {
                transitionStart = this.Transform.Position;
                targetTransitionEnd = _targetEntity.Position;
                targetTransitionEnd.Y += boundaryWidth * 2 + 2; //fake moving to next screen
                transitionEnd = transitionStart + new Vector2(0, NezGame.designHeight);
                var newBounds = FindBoundsByPosition(targetTransitionEnd);
                if (newBounds != null)
                {
                    SetBounds(newBounds.id);
                }
                this.CurrentState = Camera.CameraStyle.RoomTransition;
            }
            else if (direction.Y < 0)
            {
                transitionStart = this.Transform.Position;
                targetTransitionEnd = _targetEntity.Position;
                targetTransitionEnd.Y -= boundaryWidth * 2 + 2; //fake moving to next screen
                transitionEnd = transitionStart + new Vector2(0, -NezGame.designHeight);
                var newBounds = FindBoundsByPosition(targetTransitionEnd);
                if (newBounds != null)
                {
                    SetBounds(newBounds.id);
                }
                this.CurrentState = Camera.CameraStyle.RoomTransition;
            }
            else if (direction.X < 0)
            {
                transitionStart = this.Transform.Position;
                targetTransitionEnd = _targetEntity.Position;
                targetTransitionEnd.X -= boundaryWidth * 2 + 2; //fake moving to next screen
                transitionEnd = transitionStart + new Vector2(-NezGame.designWidth, 0);
                var newBounds = FindBoundsByPosition(targetTransitionEnd);
                if (newBounds != null)
                {
                    SetBounds(newBounds.id);
                }
                this.CurrentState = Camera.CameraStyle.RoomTransition;
            }

        }
    }
}
