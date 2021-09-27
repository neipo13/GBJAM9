using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Data
{
    public enum RenderLayer
    {
        UI,
        Foreground,
        Object,
        Tiles,
        Background,
        FurtherBackground
    }
    public static class PhysicsLayers
    {
        public const int tiles = 1 << 1;
        public const int move = 1 << 2;
        public const int player_hit = 1 << 3;
        public const int player_trigger = 1 << 4;
        public const int player_shoot = 1 << 5;
        public const int enemy_hit = 1 << 6;
        public const int enemy_trigger = 1 << 7;
        public const int enemy_shoot = 1 << 8;
        public const int camera_activator = 1 << 9;
        public const int checkpoint = 1 << 10;

    }
}
