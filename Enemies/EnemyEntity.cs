using GBJAM9.Effects;
using GBJAM9.SharedComponents;
using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Enemies
{
    public class EnemyEntity : Entity
    {
        public Health health;
        public SpriteAnimator animator;
        public BoxCollider hurtBox;
        public WhiteFlashMaterial whiteFlashMaterial;

        public EnemyEntity(string entityName) : base(entityName)
        {
            health = new Health(3);

            var prototypeSprite 
        }
    }
}
