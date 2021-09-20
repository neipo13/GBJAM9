using Microsoft.Xna.Framework;
using Nez.Sprites;
using System.Linq;

namespace GBJAM9.Aseprite
{
    public class AespriteAnimator
    {
        public string[] animationNames { get; set; }
        public SpriteAnimator spriteAnimator { get; set; }
    }
    public class AespriteSheet
    {
        public Frame[] frames { get; set; }
        //public MetaData meta { get; set; }
    }

    public class Frame
    {
        public string filename { get; set; }
        public string animationName
        {
            get
            {
                return filename.Split('|').Skip(1).First();
            }
        }
        public int animationFrame
        {
            get
            {
                return int.Parse(filename.Split('|').Last());
            }
        }
        public Dimensions frame { get; set; }
        public bool rotated { get; set; }
        public bool trimmed { get; set; }
        public Dimensions spriteSourceSize { get; set; }
        public Dimensions sourceSize { get; set; }
        public int duration { get; set; }
    }

    public class Dimensions
    {
        public int x { get; set; }
        public int y { get; set; }
        public int w { get; set; }
        public int h { get; set; }

        public Rectangle GetRectangle()
        {
            return new Rectangle(x, y, w, h);
        }
    }

    public class MetaData
    {
        public string app { get; set; }
        public string version { get; set; }
        public string image { get; set; }
        public string format { get; set; }
        public string scale { get; set; }
        public AnimationInfo frameTags { get; set; }

    }

    public class AnimationInfo
    {
        public string name { get; set; }
        public int from { get; set; }
        public int to { get; set; }
        public string direction { get; set; }
    }
}