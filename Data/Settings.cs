using GBJAM9.Effects;
using GBJAM9.Player.Weapons;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Data
{
    public class Settings
    {
        /// <summary>
        /// Singleton to allow all pages to use these settings
        /// </summary>
        private static Settings instance;
        public static Settings Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new Settings();
                }
                return instance;
            }
        }
        /// <summary>
        /// Window size multiplier from gb res
        /// </summary>
        public int ScreenSizeMultiplier { get; set; } = 4;
        /// <summary>
        /// should window be fullscreen??
        /// </summary>
        public bool IsFullScreen { get; set; } = false;
        /// <summary>
        /// Pallete the game should be using now
        /// </summary>
        public Palette currentPalette { get; set; } = Palette.GameGuy;
        /// <summary>
        /// Holds list of completed Bosses
        /// </summary>
        public List<Bosses> bossesDefeated { get; set; } = new List<Bosses>();

        public List<WeaponType> weaponsAvailable { get; set; } = new List<WeaponType>() { WeaponType.Buster };
    }
}
