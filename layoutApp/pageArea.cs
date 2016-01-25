using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace layoutApp
{
    class pageArea : basePageMatFrameArea
    {

        private float _maxDroppedImageSize_inches = 6.0f;

        public pageArea(float scale): base(scale)
        {
           
        }

        public pageArea(Panel parentPanel, float scale, pageMatFrameSaveClass pmfSC)
            : base(parentPanel,scale, pmfSC)
        {

        }

        protected override void _setPageMatFrameType()
        {
            _type = pageMatFrameType.page;
        }

        protected override void _placeImage(float scale, imageSaveClass iSC)
        {
            baseImage placedImage = new baseImage(this.location, scale, _maxDroppedImageSize_inches, iSC);
            _placedItems.Add(placedImage);
            placedImage.add(this._parent, _placedItems.Count());
        }
        
        protected override void _dropNewImages(String[] fileNames, Point mouseLocation, float scale)
        {
            Point dropLocation = mouseLocation;
            if (fileNames != null)
            {
                for (int count = 0; count < fileNames.Length; count++)
                {
                    baseImage droppedImage = new baseImage(fileNames[count], dropLocation, this.location, scale,_maxDroppedImageSize_inches);
                    droppedImage.location = dropLocation;
                    //if multiple images are dropped separate them by 50 pixels
                    dropLocation = new Point(dropLocation.X + 50, dropLocation.Y + 10);
                    _placedItems.Add(droppedImage);
                    droppedImage.add(this._parent,_placedItems.Count());
                 }
            }
        }

        
    }
}
