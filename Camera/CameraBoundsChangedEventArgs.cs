using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Camera
{
    public class CameraBoundsChangedEventArgs : EventArgs
    {
        public CameraArea cameraArea;
        public CameraArea[] previouslyVisited;

        public CameraBoundsChangedEventArgs(CameraArea cameraArea, CameraArea[] previouslyVisited)
        {
            this.cameraArea = cameraArea;
            //this.previouslyVisited = (CameraArea[])previouslyVisited.Clone();
        }
    }
}
