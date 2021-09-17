using GBJAM9.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Scenes
{
    public enum SplashType
    {
        GBJAM,
        US
    }
    public class SplashScreenScene : Scene
    {
        Entity Entity;
        readonly Color hidden = new Color(0, 0, 0, 0);
        readonly Color visible = new Color(255, 255, 255, 255);
        bool transitioning = false;
        SplashType type;
        bool tweenedIn = false;
        PaletteSwapPostProcessor paletteSwapPostProcessor;

        public SplashScreenScene(SplashType type)
        {
            this.type = type;
        }

        public override void Initialize()
        {
            base.Initialize();
            ClearColor = Color.White;
            AddRenderer(new DefaultRenderer());
            var effect = Content.Load<Effect>("effects/paletteswap");
            paletteSwapPostProcessor = AddPostProcessor(new PaletteSwapPostProcessor(999, effect));
            paletteSwapPostProcessor.SetColors(Data.Settings.Instance.currentPalette);
        }

        public override void OnStart()
        {
            base.OnStart();

            Entity = AddEntity(new Entity());
            Entity.Position = new Vector2(NezGame.designWidth / 2f, NezGame.designHeight / 2f);

            var logoPath = "";
            switch (type)
            {
                case SplashType.GBJAM:
                    logoPath = "jam-logo";
                    break;
                case SplashType.US:
                    break;
            }
            var logoTex = Content.Load<Texture2D>($"img/{logoPath}");
            var logoSprite = new SpriteRenderer(logoTex);
            logoSprite.Color = hidden;
            Entity.AddComponent(logoSprite);

            var fadeTime = 0.5f;
            var holdTime = 2f;
            logoSprite
                .TweenColorTo(visible, fadeTime)
                .SetCompletionHandler(c =>
                {
                    logoSprite
                    .TweenColorTo(hidden, fadeTime)
                    .SetDelay(holdTime)
                    .SetCompletionHandler(StartNewScene)
                    .Start();
                    tweenedIn = true;
                })
                .Start();
            transitioning = false;
        }

        public override void Update()
        {
            base.Update();
            if (Nez.Input.LeftMouseButtonPressed && tweenedIn)
            {
                StartNewScene(null);
            }
        }

        public void StartNewScene(Nez.Tweens.ITween<Color> c)
        {
            if (!transitioning)
            {
                transitioning = true;
                Scene nextScene = null;
                switch (type)
                {
                    case SplashType.GBJAM:
                        nextScene = new Scenes.Test();
                        break;
                }
                Core.StartSceneTransition(new FadeTransition(() => nextScene));
            }
        }

        public override void Unload()
        {
            Entity = null;
            base.Unload();
        }
    }
}
