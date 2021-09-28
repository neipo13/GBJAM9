using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.ImGuiTools;

namespace GBJAM9
{
    public class NezGame : Core
    {
        public const int designWidth = 160;
        public const int designHeight = 144;

        ImGuiManager imGuiManager;

        public NezGame() : base(windowTitle: "GBJAM9", isFullScreen: false, width: 160, height: 144)
        {
            ExitOnEscapeKeypress = true;

            Nez.Input.MaxSupportedGamePads = 1;
        }
        protected override void Initialize()
        {
            base.Initialize();
            var policy = Scene.SceneResolutionPolicy.BestFit;
            Scene.SetDefaultDesignResolution(designWidth, designHeight, policy, 0, 0);
            Screen.SetSize(designWidth * 2, designHeight * 2);
            Screen.ApplyChanges();
            Scene = new Scenes.SplashScreenScene(Scenes.SplashType.GBJAM);
            //Scene = new Scenes.GameScene("gatsby");
        }
    }
}
