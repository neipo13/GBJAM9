using Nez;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Enemies
{
    public class BossWallTriggerListener: Component, ITriggerListener
    {
        Action bossWallTriggerCall;
        public BossWallTriggerListener(Action triggerCall)
        {
            bossWallTriggerCall = triggerCall;
        }

        public void OnTriggerEnter(Collider other, Collider local)
        {
            if (other.PhysicsLayer.IsFlagSet(Data.PhysicsLayers.player_trigger))
            {
                bossWallTriggerCall?.Invoke();
            }
        }

        public void OnTriggerExit(Collider other, Collider local)
        {
        }
    }
}
