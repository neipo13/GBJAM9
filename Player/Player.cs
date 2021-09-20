using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Player
{
    public class Player : Entity
    {
        public Player() : base("player")
        {
            var sprite = new PrototypeSpriteRenderer(16, 24);
            sprite.Color = Color.Black;
            AddComponent(sprite);

            var moveCollider = new BoxCollider(16,24)
            {
                PhysicsLayer = Data.PhysicsLayers.move,
                CollidesWithLayers = Data.PhysicsLayers.tiles
            };
            AddComponent(moveCollider);

            var mover = new Mover();
            AddComponent(mover);


            AddComponent(new PlayerController());
        }
    }
}
