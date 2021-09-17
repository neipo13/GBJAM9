using GBJAM9.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using GBJAM9.Input;

namespace GBJAM9.Scenes
{
    public class Test : Scene
    {
        PaletteSwapPostProcessor paletteSwapPostProcessor;
        InputHandler input;

        public override void Initialize()
        {
            base.Initialize();
            ClearColor = Color.CornflowerBlue;
            AddRenderer<DefaultRenderer>(new DefaultRenderer());
            var effect = Content.Load<Effect>("effects/paletteswap");
            paletteSwapPostProcessor = AddPostProcessor(new PaletteSwapPostProcessor(999, effect));
            paletteSwapPostProcessor.SetColors(Palette.GameGuy);
            input = InputManager.Instance.GetInput(0);


        }

        public override void OnStart()
        {
            base.OnStart();

            var effect = Content.Load<Effect>("effects/background_scroll");
            var texture = Content.Load<Texture2D>("img/repeatingBgCircle");
            var material = new MenuBackgroundScrollMat(effect, new Vector2(texture.Width, texture.Height), -0.75f);
            var sprite = new Sprite(texture);
            var spriteRenderer = new SpriteRenderer(sprite);
            spriteRenderer.Material = material;
            spriteRenderer.RenderLayer = (int)Data.RenderLayer.Background;
            var e = new Entity("testObj");
            e.Position = new Vector2(NezGame.designWidth / 2f, NezGame.designHeight / 2f);
            e.Scale = new Vector2(NezGame.designWidth/texture.Width, NezGame.designHeight/texture.Height);
            e.AddComponent(spriteRenderer);
            AddEntity(e);
        }

        public override void Update()
        {
            base.Update();
            if (input.SelectButton.IsPressed)
            {
                var pal = Nez.Random.NextInt(Enum.GetValues(typeof(Palette)).Length);
                Data.Settings.Instance.currentPalette = (Palette)pal;
                paletteSwapPostProcessor.SetColors(Data.Settings.Instance.currentPalette);
            }
        }
    }
}
