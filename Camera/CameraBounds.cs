using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GBJAM9.Camera
{
    public class CameraBounds : Component, IUpdatable
    {
        public Vector2 min, max;
        public CameraArea[] cameraBounds;
        private Entity followTarget;
        private bool inTransition;
        public event EventHandler<CameraBoundsChangedEventArgs> BoundsChangedEvent;

        public int boundaryWidth = 12; // how close to the boundary must the player be before the camera changes

        public CameraBounds()
        {
            // make sure we run last so the camera is already moved before we evaluate its position
            SetUpdateOrder(int.MaxValue);
        }


        public CameraBounds(CameraArea current, CameraArea[] bounds, Entity follow) : this()
        {
            cameraBounds = (CameraArea[])bounds.Clone();
            SetBounds(current.id);
            followTarget = follow;
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
            BoundsChangedEvent?.Invoke(this, new CameraBoundsChangedEventArgs(area, null));
        }
        public void SetBounds(Rectangle rect)
        {
            this.min = new Vector2(rect.Left, rect.Top);
            this.max = new Vector2(rect.Right, rect.Bottom);
        }


        public override void OnAddedToEntity()
        {
            Entity.UpdateOrder = int.MaxValue;
        }


        void IUpdatable.Update()
        {
            var cameraBounds = Entity.Scene.Camera.Bounds;

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

        Vector2 CheckTargetExit()
        {
            Vector2 direction = Vector2.Zero;

            if (followTarget.Position.X > (max.X - boundaryWidth))
                direction.X = 1;
            else if (followTarget.Position.X < (min.X + boundaryWidth))
                direction.X = -1;
            else if (followTarget.Position.Y > (max.Y - boundaryWidth))
                direction.Y = 1;
            else if (followTarget.Position.Y < (min.Y + boundaryWidth))
                direction.Y = -1;

            return direction;
        }

        public CameraArea FindBoundsByPosition(Vector2 pos)
        {
            return cameraBounds.SingleOrDefault(b => b.bounds.Contains(pos));
        }

        void MoveCamera(Vector2 direction)
        {
            var pos = followTarget.Position;
            if (direction.X > 0)
            {
                pos.X += boundaryWidth * 2 + 2; //fake moving to next screen
                var newBounds = FindBoundsByPosition(pos);
                if (newBounds != null)
                {
                    SetBounds(newBounds.id);
                }
            }
            else if (direction.Y > 0)
            {
                pos.Y += boundaryWidth * 2 + 2; //fake moving to next screen
                var newBounds = FindBoundsByPosition(pos);
                if (newBounds != null)
                {
                    SetBounds(newBounds.id);
                }
            }
            else if (direction.Y < 0)
            {
                pos.Y -= boundaryWidth * 2 + 2; //fake moving to next screen
                var newBounds = FindBoundsByPosition(pos);
                if (newBounds != null)
                {
                    SetBounds(newBounds.id);
                }
            }
            else if (direction.X < 0)
            {
                pos.X -= boundaryWidth * 2 + 2; //fake moving to next screen
                var newBounds = FindBoundsByPosition(pos);
                if (newBounds != null)
                {
                    SetBounds(newBounds.id);
                }
            }
            followTarget.Position = pos;

        }
    }
}
