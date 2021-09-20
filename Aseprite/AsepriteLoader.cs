using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Nez.Sprites;
using Nez.Textures;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GBJAM9.Aseprite
{
    public class AespriteLoader
    {
        public static SpriteAnimator LoadSpriteAnimatorFromAesprite(string path, Nez.Systems.NezContentManager contentManager)
        {
            AespriteSheet sheet;
            using (StreamReader reader = new StreamReader($"Content/{path}.json"))
            {
                string json = reader.ReadToEnd();
                sheet = JsonConvert.DeserializeObject<AespriteSheet>(json);
            }

            //load texture
            var texture = contentManager.Load<Texture2D>(path);

            //create sprites associated with AespriteFrames
            var sprites = new Dictionary<string, Sprite[]>();
            //populate dictionary with sprite lists for each animation
            var frameGroups = sheet.frames.GroupBy(f => f.animationName).ToArray();
            for (int i = 0; i < frameGroups.Length; i++)
            {
                var frames = frameGroups[i].OrderBy(f => f.animationFrame).ToArray();
                var animSpriteList = new List<Sprite>();
                for (int j = 0; j < frames.Length; j++)
                {
                    var frame = frames[j];
                    var sprite = new Sprite(texture, frame.frame.GetRectangle());
                    animSpriteList.Add(sprite);
                }
                sprites.Add(frames[0].animationName, animSpriteList.ToArray());
            }

            // load into sprite animator via animationName & frame
            SpriteAnimator spriteAnimator = new SpriteAnimator();
            var spriteKeys = sprites.Keys.ToArray();

            for (int k = 0; k < spriteKeys.Length; k++)
            {
                var name = spriteKeys[k];
                spriteAnimator.AddAnimation(name, sprites[name]);
            }

            return spriteAnimator;
        }
    }
}