using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Player.Weapons
{
    public interface IProjectile
    {
        string SpriteTexturePath { get; }
        Vector2 direction { get; set; }
        float speed { get; set; }
        SpriteAnimator animator { get; set; }
        BoxCollider collider { get; }
        int PhysicsLayer { get; set; }
        bool active { get; set; }

        Vector2 position { get; set; }

        void TurnOnOff(bool on);
    }
}
