using Nez;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Camera
{
    public class CameraTriggerHelper : Component, IUpdatable
    {
        ColliderTriggerHelper triggerHelper;

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            triggerHelper = new ColliderTriggerHelper(this.Entity);
        }
        public void Update()
        {
            triggerHelper.Update();
        }
    }
}
