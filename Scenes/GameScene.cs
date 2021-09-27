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
using GBJAM9.Enemies;

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

        TmxMap map;
        Entity mapEntity;

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
            paletteSwapPostProcessor.SetColors(Data.Settings.Instance.currentPalette);
            input = InputManager.Instance.GetInput(0);
            Data.Settings.Instance.currentCheckpoint = 0;
            checkpoints = new Dictionary<int, Data.Checkpoint>();
        }

        public override void OnStart()
        {
            base.OnStart();

            //var effect = Content.Load<Effect>("effects/background_scroll");
            //var texture = Content.Load<Texture2D>("img/repeatingBgCircle");
            //var material = new MenuBackgroundScrollMat(effect, new Vector2(texture.Width, texture.Height), -0.75f);
            //var sprite = new Sprite(texture);
            //var spriteRenderer = new SpriteRenderer(sprite);
            //spriteRenderer.Material = material;
            //spriteRenderer.RenderLayer = (int)Data.RenderLayer.Background;
            //scrollingSpriteEntity = new Entity("sprite-scroll");
            //scrollingSpriteEntity.Scale = new Vector2(NezGame.designWidth / texture.Width, NezGame.designHeight / texture.Height);
            //scrollingSpriteEntity.AddComponent(spriteRenderer);
            //scrollingSpriteEntity.Position = new Vector2(NezGame.designWidth / 2f, NezGame.designHeight / 2f);
            //AddEntity(scrollingSpriteEntity);

            map = Content.LoadTiledMap($"Content/tiled/{mapName}.tmx");
            var playerGroup = map.ObjectGroups["player"];
            var spawn = playerGroup.Objects["spawn"];
            Camera.Position = new Vector2(spawn.X, spawn.Y);
            mapEntity = CreateEntity("map");
            var mapRenderer = new TiledMapRenderer(map, "collision");
            mapRenderer.PhysicsLayer = Data.PhysicsLayers.tiles;
            mapRenderer.SetLayersToRender(new string[] { "collision" });
            mapRenderer.RenderLayer = (int)Data.RenderLayer.Tiles;
            mapEntity.AddComponent(mapRenderer);

            var bgRenderer = new TiledMapRenderer(map);
            bgRenderer.SetLayersToRender(new string[] { "background" });
            bgRenderer.RenderLayer = (int)Data.RenderLayer.Background;
            mapEntity.AddComponent(bgRenderer);

            var playerAnim = Aseprite.AespriteLoader.LoadSpriteAnimatorFromAesprite("img/mrpeanut", Content);
            playerAnim.RenderLayer = (int)Data.RenderLayer.Object;
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
            var follow = new BoundedFollowCamera(player.aimPoint, currentBounds, cameraBounds.ToArray());
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

            var martiniPositions = new List<Vector2>();
            if (map.ObjectGroups.Contains("martiniBombSpawns"))
            {
                var martiniLayer = map.ObjectGroups["martiniBombSpawns"];
                foreach (TmxObject o in martiniLayer.Objects)
                {
                    martiniPositions.Add(new Vector2(o.X, o.Y));
                }
            }


            var enemiesLayer = map.ObjectGroups["enemies"];
            foreach (TmxObject o in enemiesLayer.Objects.OrderBy(o => o.Name))
            {
                var type = o.Type.ToLower();
                switch (type)
                {
                    case "guy":
                        var guyFlashEffect = Content.Load<Effect>("effects/white_flash");
                        var guyFlash = new WhiteFlashMaterial(guyFlashEffect, new Vector2(24));
                        var guyAnim = Aseprite.AespriteLoader.LoadSpriteAnimatorFromAesprite("img/richgunguy", Content);
                        guyAnim.RenderLayer = (int)Data.RenderLayer.Object;
                        var enemy = new Enemies.BasicRichGuy.RichGuy("test", guyFlash, guyAnim);
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
                    case "gatsby":
                        var gatsFlashEffect = Content.Load<Effect>("effects/white_flash");
                        var gatsFlash = new WhiteFlashMaterial(gatsFlashEffect, new Vector2(32));                        
                        var gatsAnim = Aseprite.AespriteLoader.LoadSpriteAnimatorFromAesprite("img/gatsby", Content);
                        gatsAnim.RenderLayer = (int)Data.RenderLayer.Object;
                        var gats = new Enemies.Gatsby.GatsbyEntity(gatsAnim, gatsFlash, martiniPositions);
                        gats.Position = new Vector2(o.X, o.Y);
                        AddEntity(gats);
                        break;
                }
            }

            var bossRoomLayer = map.ObjectGroups["bossRoom"];
            foreach (TmxObject o in bossRoomLayer.Objects)
            {
                var name = o.Name.ToLower();
                if(name == "trigger")
                {
                    var rect = new Rectangle((int)o.X, (int)o.Y, (int)o.Width, (int)o.Height);
                    var e = new Entity("bossTrigger");
                    var triggerCol = new BoxCollider(rect);
                    triggerCol.IsTrigger = true;
                    triggerCol.PhysicsLayer = Data.PhysicsLayers.checkpoint;
                    triggerCol.CollidesWithLayers = Data.PhysicsLayers.player_trigger;
                    e.AddComponent(triggerCol);
                    var wallTrigger = new BossWallTriggerListener(SpawnBossWall);
                    e.AddComponent(wallTrigger);
                    AddEntity(e);
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

        public void SpawnBossWall()
        {
            var bossRoomMapRenderer = new TiledMapRenderer(map, "bosswall");
            bossRoomMapRenderer.PhysicsLayer = Data.PhysicsLayers.tiles;
            bossRoomMapRenderer.SetLayersToRender(new string[] { "bosswall" });
            bossRoomMapRenderer.RenderLayer = (int)Data.RenderLayer.Tiles;
            mapEntity.AddComponent(bossRoomMapRenderer);
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
