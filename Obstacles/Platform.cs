using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GBJAM9.Obstacles
{
    public class Platform : Entity
    {
        PlatformController controller;
        public Platform(float width, float height, Vector2 moveToPoint, float speed) : base("platform")
        {
            var sprite = new PrototypeSpriteRenderer(width, height);
            sprite.Color = Color.Black;
            AddComponent(sprite);

            var collider = new BoxCollider(width, height);
            collider.PhysicsLayer = Data.PhysicsLayers.tiles;
            AddComponent(collider);

            AddComponent(new Mover());
            controller = new PlatformController(moveToPoint, speed);
            AddComponent(controller);
        }

    }

    public class PlatformController : Component
    {
        Vector2 moveToPoint;

        float speed;
        bool moves;

        Player.PlayerController controller;
        BoxCollider platformCollider;
        BoxCollider playerCollider;

        public PlatformController(Vector2 moveToPoint, float speed, bool moves = true)
        {
            this.moveToPoint = moveToPoint;
            this.speed = speed;
            this.moves = moves;
        }
        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            if (moves)
            {
                Entity
                    .TweenPositionTo(moveToPoint, speed)
                    .SetEaseType(Nez.Tweens.EaseType.SineInOut)
                    .SetLoops(Nez.Tweens.LoopType.PingPong, -1, 0f)
                    .SetUpdateAction(UpdateRiders)
                    .Start();
            }
            var player = Entity.Scene.FindEntity("player") as Player.Player;
            controller = player.GetComponent<Player.PlayerController>();
            playerCollider = player.GetComponents<BoxCollider>().SingleOrDefault(c => c.PhysicsLayer.IsFlagSet(Data.PhysicsLayers.move));
            platformCollider = Entity.GetComponent<BoxCollider>();
        }

        public void UpdateRiders(Vector2 movement)
        {
            if (HasPlayerRider() || HasPlayerRider(movement.Y))
            {
                movement.Y += 1f; // force further into ground to keep us on the falling thing
                controller.Move(movement);
            }
        }

        bool HasPlayerRider(float moveOffset = 0f)
        {
            if (!controller.isGrounded) return false;

            var player = controller.Entity;
            var adjBounds = playerCollider.Bounds;
            adjBounds.Y += 1f;
            adjBounds.Y += moveOffset;
            var neighbors = Physics.BoxcastBroadphaseExcludingSelf(playerCollider, ref adjBounds, Data.PhysicsLayers.tiles);
            var collidesWithMe = neighbors.Contains(platformCollider);

            return collidesWithMe;
        }
    }
}
