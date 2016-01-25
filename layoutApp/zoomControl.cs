using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace layoutApp
{
    public partial class zoomControl : UserControl
    {
       public event zoomChangedEventHandler zoomChanged;
       public event fitToScreenEventHandler fitToScreen;

        int lastValidValue = 100;

        public zoomControl()
        {
            InitializeComponent();
            this._loadZoomValue(lastValidValue);
            //zoomBar.Value = lastValidValue;
            //this._setAndFormatZoomBarTBText(this.zoomBar.Value.ToString());
                        
        }

        private void _loadZoomValue(int zoomValueToLoad)
        {
            zoomBar.Value = zoomValueToLoad;
            this._setAndFormatZoomBarTBText(this.zoomBar.Value.ToString());
        }

        private void zoomTB_TextChanged(object sender, EventArgs e)
        {
            this._convertZoomTBValue();
        }

        private int _convertZoomTBValue()
        {
            String zoomTextNumber = zoomTB.Text.EndsWith("%") ? zoomTB.Text.Replace("%", "") : zoomTB.Text;
            int convertedValue;
            if (int.TryParse(zoomTextNumber, out convertedValue))
            {
                if ((convertedValue > zoomBar.Maximum) || (convertedValue < zoomBar.Minimum))
                    convertedValue = lastValidValue;
                else
                    lastValidValue = convertedValue;
            }
            else
            {
                convertedValue = lastValidValue;
            }

            return (convertedValue);
        }

        public float zoomValue
        {
            get 
            {
                return ((float)_convertZoomTBValue()/100.0f); 
            }
        }

        private void _setAndFormatZoomBarTBText(String numberToFormat)
        {
            this.zoomTB.Text = String.Concat(numberToFormat, "%");
        }

        private void zoomBar_Scroll(object sender, EventArgs e)
        {
            this._setAndFormatZoomBarTBText(this.zoomBar.Value.ToString());
            this._triggerExternalZoomChangedEvent();
        }

        private void zoomTB_Leave(object sender, EventArgs e)
        {
            zoomBar.Value = _convertZoomTBValue();
            this._setAndFormatZoomBarTBText(zoomBar.Value.ToString());
            this._triggerExternalZoomChangedEvent();
        }

        private void _triggerExternalZoomChangedEvent()
        {
            if (zoomChanged != null)
                zoomChanged(this, new zoomEventArgs(zoomBar.Value));
        }

        private void fitToScreenButton_Click(object sender, EventArgs e)
        {
            this._loadZoomValue(100);
            this._triggerExternalZoomChangedEvent();
            fitToScreen(this, new fitToScreenEventArgs());
        }

     }
}
