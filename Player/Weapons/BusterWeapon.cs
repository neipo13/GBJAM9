using Nez;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Player.Weapons
{
    public class BusterWeapon : Component, IWeapon
    {

        public int MaxShots => 3;

        public IProjectile[] Shots { get; private set; }

        public Scene scene { get; private set; }

        public WeaponType type => WeaponType.Buster;

        public BusterWeapon(Scene scene)
        {
            this.scene = scene;
            Shots = new PeanutProjectile[MaxShots];
            for(int i = 0; i < MaxShots; i++)
            {
                var proj = new PeanutProjectile(scene);
                Shots[i] = proj;

                //move them offscreen
                proj.position = new Microsoft.Xna.Framework.Vector2(-100, -100);

                scene.AddEntity(proj);
            }
        }

        public IProjectile GetNextAvailableShot()
        {
            IProjectile shot = null;
            for(int i = 0; i < MaxShots; i++)
            {
                var tempShot = Shots[i];
                if (!tempShot.active)
                {
                    shot = tempShot;
                }
            }
            return shot;
        }

        public IProjectile Shoot(bool facingRight)
        {
            var nextShot = GetNextAvailableShot();
            if(nextShot != null)
            {
                nextShot.TurnOnOff(true);
                nextShot.position = this.Entity.Position;
                if (facingRight)
                {
                    nextShot.direction = new Microsoft.Xna.Framework.Vector2(1, 0);
                    nextShot.animator.FlipX = false;
                }
                else
                {
                    nextShot.direction = new Microsoft.Xna.Framework.Vector2(-1, 0);
                    nextShot.animator.FlipX = true;
                }
            }
            return nextShot;
        }
    }
}
