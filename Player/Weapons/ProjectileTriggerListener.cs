﻿using Nez;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Player.Weapons
{
    public class ProjectileTriggerListener : Component, ITriggerListener
    {
        public IProjectile projectile;
        public void OnTriggerEnter(Collider other, Collider local)
        {
            projectile.TurnOnOff(false);
        }

        public void OnTriggerExit(Collider other, Collider local)
        {
        }
    }
}