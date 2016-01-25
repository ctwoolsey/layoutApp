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
    public partial class albumControlBar : UserControl
    {
        private Form _callingForm = null;

        public albumControlBar(Form callingForm)
        {
            InitializeComponent();
            this._callingForm = callingForm;
            
        }

        
        private void _setLocation()
        {
            if (this._callingForm != null)
            {
                if (this._callingForm.Width < this.Width)
                    this.Location = new Point(0, this._callingForm.MainMenuStrip.Height);
                else
                {
                    int newX = (int)((Double)((Double)this._callingForm.Width / 2.00) - (Double)((Double)this.Width / 2.00));
                    this.Location = new Point(newX, this._callingForm.MainMenuStrip.Height);
                }
            }
        }

        private void newSinglePageBtn_Click(object sender, EventArgs e)
        {

        }

        private void newSpreadBtn_Click(object sender, EventArgs e)
        {
           // this._callingForm.addSpread();
        }

        private void albumControlBar_Paint(object sender, PaintEventArgs e)
        {
            this._setLocation();
        }

       
    }
}
