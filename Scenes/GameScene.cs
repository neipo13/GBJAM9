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
using Nez.Tiled;
using GBJAM9.Camera;

namespace GBJAM9.Scenes
{
    public class GameScene : Scene
    {
        PaletteSwapPostProcessor paletteSwapPostProcessor;
        InputHandler input;
        Entity scrollingSpriteEntity;
        CameraShake cameraShake;
        BoundedFollowCamera followCamera;
        CameraBounds camBounds;

        public Dictionary<int, Data.Checkpoint> checkpoints;

        string mapName;

        public GameScene(string mapName)
        {
            this.mapName = mapName;
        }

        public override void Initialize()
        {
            base.Initialize();
            ClearColor = Color.White;
            AddRenderer<DefaultRenderer>(new DefaultRenderer());
            var effect = Content.Load<Effect>("effects/paletteswap");
            paletteSwapPostProcessor = AddPostProcessor(new PaletteSwapPostProcessor(999, effect));
            paletteSwapPostProcessor.SetColors(Palette.GameGuy);
            input = InputManager.Instance.GetInput(0);
            Data.Settings.Instance.currentCheckpoint = 0;
            checkpoints = new Dictionary<int, Data.Checkpoint>();
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
            scrollingSpriteEntity = new Entity("sprite-scroll");
            scrollingSpriteEntity.Scale = new Vector2(NezGame.designWidth / texture.Width, NezGame.designHeight / texture.Height);
            scrollingSpriteEntity.AddComponent(spriteRenderer);
            scrollingSpriteEntity.Position = new Vector2(NezGame.designWidth / 2f, NezGame.designHeight / 2f);
            AddEntity(scrollingSpriteEntity);

            var map = Content.LoadTiledMap($"Content/tiled/{mapName}.tmx");
            var playerGroup = map.ObjectGroups["player"];
            var spawn = playerGroup.Objects["spawn"];
            Camera.Position = new Vector2(spawn.X, spawn.Y);
            var mapEntity = CreateEntity("map");
            var mapRenderer = new TiledMapRenderer(map, "collision");
            mapRenderer.PhysicsLayer = Data.PhysicsLayers.tiles;
            mapRenderer.SetLayersToRender(new string[] { "collision" });
            mapEntity.AddComponent(mapRenderer);



            var playerAnim = Aseprite.AespriteLoader.LoadSpriteAnimatorFromAesprite("img/mrpeanut", Content);
            var player = new Player.Player(playerAnim);
            player.Position = new Vector2(spawn.X, spawn.Y - 10);
            AddEntity(player);

            var cameraBoundsLayer = map.ObjectGroups["camera_areas"];
            var cameraBounds = new List<CameraArea>();
            CameraArea currentBounds = null;
            foreach (TmxObject o in cameraBoundsLayer.Objects.OrderBy(o => o.Name))
            {
                //make this parse more robust
                var id = int.Parse(o.Name);
                var rect = new Rectangle((int)o.X, (int)o.Y, (int)o.Width, (int)o.Height);
                var area = new CameraArea(id, rect);
                if (rect.Contains(player.Position.X, player.Position.Y))
                {
                    currentBounds = area;
                }
                cameraBounds.Add(area);
            }
            //var bounds = new CameraBounds(currentBounds, cameraBounds.ToArray(), player);
            //camera.addComponent(bounds);
            var follow = new BoundedFollowCamera(player, currentBounds, cameraBounds.ToArray());
            Camera.AddComponent(follow);
            cameraShake = Camera.AddComponent(new CameraShake());
            cameraShake.Enabled = false;
            follow.cameraShake = cameraShake;

            var cameraActivatorBox = new BoxCollider(NezGame.designWidth, NezGame.designHeight);
            cameraActivatorBox.PhysicsLayer = Data.PhysicsLayers.camera_activator;
            cameraActivatorBox.CollidesWithLayers = Data.PhysicsLayers.enemy_trigger;
            cameraActivatorBox.IsTrigger = true;
            Camera.AddComponent(cameraActivatorBox);
            var camTriggerHelper = new CameraTriggerHelper();
            Camera.AddComponent(camTriggerHelper);

            //var camFollow = new FollowCamera(player);
            //Camera.AddComponent(camFollow);


            var enemiesLayer = map.ObjectGroups["enemies"];
            foreach (TmxObject o in enemiesLayer.Objects.OrderBy(o => o.Name))
            {
                var type = o.Type.ToLower();
                switch (type)
                {
                    case "guy":
                        var flashEffect = Content.Load<Effect>("effects/white_flash");
                        var flashMat = new WhiteFlashMaterial(flashEffect, new Vector2(24));
                        var animator = Aseprite.AespriteLoader.LoadSpriteAnimatorFromAesprite("img/richgunguy", Content);
                        var enemy = new Enemies.BasicRichGuy.RichGuy("test", flashMat, animator);
                        enemy.Position = new Vector2(o.X, o.Y);
                        AddEntity(enemy);
                        break;
                    case "chandelier":
                        var chandTex = Content.Load<Texture2D>("img/chandelier");
                        var chandSprite = new Sprite(chandTex);
                        var chandelier = new Obstacles.Chandelier(chandSprite);
                        chandelier.Position = new Vector2(o.X, o.Y);
                        AddEntity(chandelier);
                        break;
                }
            }

            var checkpointLayer = map.ObjectGroups["checkpoints"];
            foreach (TmxObject o in checkpointLayer.Objects.OrderBy(o => o.Name))
            {
                int ID = int.Parse(o.Name);
                var checkpoint = new Data.Checkpoint(ID, new Vector2(o.Width, o.Height));
                checkpoint.Position = new Vector2(o.X + (o.Width / 2f), o.Y + (o.Height / 2f));
                checkpoints.Add(ID, checkpoint);
                AddEntity(checkpoint);
            }
        }


        public void OnCameraBoundChangedTransition(CameraBoundsChangedEventArgs args)
        {

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
