using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace layoutApp
{
    abstract class basePageMatFrameArea : Panel
    {
        public enum _pageMatFrameType { page, mat, frame };
        protected SizeF _completePageMatFrameSizeF; //includes the bleed
        
        protected float _bleed = 0.0f;
        protected float _margin = 0.125f;
        protected List<baseImage> _images = new List<baseImage>();
        protected _pageMatFrameType _type;
        protected baseLayoutArea _attachedToLayoutArea = null;
        
        public basePageMatFrameArea(baseLayoutArea layoutArea)
        {
            this._attachedToLayoutArea = layoutArea;
            _completePageMatFrameSizeF = new SizeF(24.0f, 12.0f);
            this.BackColor = Color.Red;
            _setPageMatFrameType();
        }

        protected abstract void _setPageMatFrameType();

        public void setSize(float scale)
        {
            int width = Convert.ToInt16(this.completeSizeF.Width * scale);
            int height = Convert.ToInt16(this.completeSizeF.Height * scale);
            this.Size = new Size(width, height);

            foreach (baseImage image in _images)
            {
                image.setScale(scale);
            }
        }

        public SizeF completeSizeF
        {
            get { return (_completePageMatFrameSizeF); }
            set { _completePageMatFrameSizeF = value; }
        }

        public void imageFileDropped(String[] fileNames, Point mouseLocation)
        {
            //mouseLocation = PointToClient(mouseLocation);
           Console.WriteLine(String.Concat("Drop Location: (",mouseLocation.X,",",mouseLocation.Y,")"));
             this._dropNewImages(fileNames, mouseLocation);
        }
        protected abstract void _dropNewImages(String[] fileNames, Point mouseLocation);

        public List<baseImage> images
        {
            get { return (_images); }
        }

    }
}
