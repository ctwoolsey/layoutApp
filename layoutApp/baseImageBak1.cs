using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace layoutApp
{
    public class baseImage : placeableItem
    {
        private String _fileName;
        private Image _image = null;
        private float _originalAspectRatio;
        private Rectangle _sourceRectangle;
        private Point _imageStartingPoint;

        private float _maxInitialSize_units = 2.5f;

        public baseImage(String fileName, Point dropLocation, Point pageOrigin, float scale) : base(placeableItem.itemTypes.image, dropLocation, pageOrigin, scale)
        {
            this.fileName = fileName;
        }

       
        public String fileName
        {
            get { return (_fileName); }
            set {
                _fileName = value;
                _image = Image.FromFile(_fileName);
                _originalAspectRatio = (float)_image.Width / (float)_image.Height;
                _sizeImage(_maxInitialSize_units);
                this.draw();
                }
        }

        private void _sizeImage(float maxDimension)
        {
            float newWidth;
            float newHeight;

            if (_image.Width > _image.Height)
            {
                newWidth = maxDimension;
                newHeight = (1.0f/_originalAspectRatio) * newWidth;
            }
            else
            {
                newHeight = maxDimension;
                newWidth = newHeight * _originalAspectRatio;
            }
            _itemAspectRatio = _originalAspectRatio;
            this._windowSize_units = new SizeF(newWidth, newHeight);
            this._size = new Size(Convert.ToInt16(newWidth*_magnification), Convert.ToInt16(newHeight*_magnification));
            _imageStartingPoint = new Point(0, 0);
            _sourceRectangle = new Rectangle(_imageStartingPoint, this._size);
        }

        private void _calculateSourceRectangle()
        {
            int sourceWidth;
            int sourceHeight;

            if (_originalAspectRatio > 1.0f) //width > height
            {
                if (_itemAspectRatio > 1.0f)
                {
                    sourceHeight = Convert.ToInt16((1.0f / (float)_itemAspectRatio) * (float)_image.Width);
                    sourceWidth = _image.Width;
                    if (sourceHeight > _image.Height)
                    {
                        sourceHeight = _image.Height;
                        sourceWidth = Convert.ToInt16((float)_image.Height * _itemAspectRatio);
                    }
                    _sourceRectangle = new Rectangle(new Point(0, 0), new Size(sourceWidth, sourceHeight));
             
                }
                else // the new stretch makes the height larger than the original height
                {
                    sourceWidth = Convert.ToInt16(_itemAspectRatio * (float)_image.Height);
                    sourceHeight = _image.Height;
                    if (sourceWidth > _image.Width)
                    {
                        sourceWidth = _image.Width;
                        sourceHeight = Convert.ToInt16((float)_image.Width * 1.0f / (float)_itemAspectRatio);
                    }
                    _sourceRectangle = new Rectangle(new Point(0, 0), new Size(sourceWidth, sourceHeight));
                }
            }
            else
            {
                if (_itemAspectRatio < 1.0f)
                {
                    sourceHeight = Convert.ToInt16((1.0f / (float)_itemAspectRatio) * (float)_image.Width);
                    sourceWidth = _image.Width;
                    if (sourceHeight > _image.Height)
                    {
                        sourceHeight = _image.Height;
                        sourceWidth = Convert.ToInt16((float)_image.Height * _itemAspectRatio);
                    }
                    _sourceRectangle = new Rectangle(new Point(0, 0), new Size(sourceWidth, sourceHeight));
             
                }
                else
                {
                    sourceWidth = Convert.ToInt16(_itemAspectRatio * (float)_image.Height);
                    sourceHeight = _image.Height;
                    if (sourceWidth > _image.Width)
                    {
                        sourceWidth = _image.Width;
                        sourceHeight = Convert.ToInt16((float)_image.Width * 1.0f / (float)_itemAspectRatio);
                    }
                    _sourceRectangle = new Rectangle(new Point(0, 0), new Size(sourceWidth, sourceHeight));
                }
            }
        }

        protected override void panItem(MouseEventArgs e)
        {
            Console.WriteLine("panning");
        }

        public override void draw(Graphics gfx)
        {
            
            Rectangle imageRectangle = new Rectangle(this.location, new Size(Convert.ToInt16(_windowSize_units.Width * _magnification), Convert.ToInt16(_windowSize_units.Height * _magnification)));
           
            //need some way of keeping existing location
            if (_autoSizeItem)
            {
                this._calculateSourceRectangle();
                gfx.DrawImage(this._image, imageRectangle, _sourceRectangle, GraphicsUnit.Pixel);

            }
            else //not autoFitting image
            {
                gfx.DrawImage(this._image, imageRectangle);
            }
            
            base.draw(gfx);
        }
       

        

        

    }
}
