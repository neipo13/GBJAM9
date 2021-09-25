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

        public NezGame() : base(windowTitle: "GBJAM9", isFullScreen: false, width: 640, height: 576)
        {
            ExitOnEscapeKeypress = false;
            Window.AllowUserResizing = true;

            Nez.Input.MaxSupportedGamePads = 1;
        }
        protected override void Initialize()
        {
            base.Initialize(); 
            //imGuiManager = new ImGuiManager();
            //Core.RegisterGlobalManager(imGuiManager);
            var policy = Scene.SceneResolutionPolicy.BestFit;
            Scene.SetDefaultDesignResolution(designWidth, designHeight, policy, 0, 0);
            //Scene = new Scenes.SplashScreenScene(Scenes.SplashType.GBJAM);
            Scene = new Scenes.GameScene("test");
        }
    }
}
