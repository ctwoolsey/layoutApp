using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace layoutApp
{
    public delegate void zoomChangedEventHandler(object sender, zoomEventArgs e);

    public class zoomEventArgs : EventArgs
    {
        public float zoom = 0f;
        public zoomEventArgs(float zoomValue)
            : base()
        {
            zoom = zoomValue;
        }

        public zoomEventArgs(int zoomValue)
            : base()
        {
            zoom = (float)zoomValue / 100.0f;
        }
    }

    public delegate void fitToScreenEventHandler(Object sender, fitToScreenEventArgs e);

    public class fitToScreenEventArgs : EventArgs
    {
        public fitToScreenEventArgs() : base()
        {
        }
    }

}
