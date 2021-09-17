using GBJAM9.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Scenes
{
    public class Test : Scene
    {
        PaletteSwapPostProcessor paletteSwapPostProcessor;

        public override void Initialize()
        {
            base.Initialize();
            ClearColor = Color.CornflowerBlue;
            AddRenderer<DefaultRenderer>(new DefaultRenderer());
            paletteSwapPostProcessor = AddPostProcessor(new PaletteSwapPostProcessor(999));
            paletteSwapPostProcessor.SetColors(Palette.GameGuy);


        }

        public override void OnStart()
        {
            base.OnStart();

            var texture = Content.Load<Texture2D>("img/repeatingBgCircle");
            var material = new MenuBackgroundScrollMat(this, new Vector2(texture.Width/4f, texture.Height/4f), -0.75f);
            var sprite = new Sprite(texture);
            var spriteRenderer = new SpriteRenderer(sprite);
            spriteRenderer.Material = material;
            var e = new Entity("testObj");
            e.Position = new Vector2(16, 16);
            e.Scale = new Vector2(10);
            e.AddComponent(spriteRenderer);
            AddEntity(e);
        }
    }
}
