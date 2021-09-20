using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Camera
{
    public class CameraArea
    {
        public Rectangle bounds;
        public int id;

        public CameraArea(int id, Rectangle bounds)
        {
            this.id = id;
            this.bounds = bounds;
        }
    }
}
