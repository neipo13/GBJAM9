using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Effects
{
    public class WhiteFlashMaterial : Material
    {
        EffectParameter spriteSize;
        EffectParameter outlineColor;
        EffectParameter insideColor;

        public WhiteFlashMaterial(Effect effect, Vector2 spriteSize) : base(effect)
        {
            this.spriteSize = Effect.Parameters["sprite_size"];
            outlineColor = Effect.Parameters["outline_color"];
            insideColor = Effect.Parameters["inside_color"];
            this.spriteSize.SetValue(spriteSize);
            this.outlineColor.SetValue(new Vector4(0.75f, 0.75f, 0.75f, 1f));
            this.insideColor.SetValue(Color.White.ToVector4());
        }
    }
}
