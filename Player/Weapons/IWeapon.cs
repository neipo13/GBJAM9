using Nez;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Player.Weapons
{
    public enum WeaponType
    {
        Buster
    }
    public interface IWeapon
    {
        WeaponType type { get; }
        int MaxShots { get; }
        IProjectile[] Shots { get; }
        IProjectile Shoot(bool facingRight);
        IProjectile GetNextAvailableShot();
        Scene scene { get; }

    }
}
