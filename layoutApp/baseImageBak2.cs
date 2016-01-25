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

        private float _maxInitialSize_units;

        public baseImage(String fileName, Point dropLocation, Point pageOrigin, float scale, float maxImageDimensionUnits) : base(placeableItem.itemTypes.image, dropLocation, pageOrigin, scale)
        {
            _maxInitialSize_units = maxImageDimensionUnits;
            this.fileName = fileName;
        }

       
        public String fileName
        {
            get { return (_fileName); }
            set {
                _fileName = value;
                _image = Image.FromFile(_fileName);
                _originalAspectRatio = (float)_image.Width / (float)_image.Height;
                _sizeImage();
                this.draw();
                }
        }

        private void _sizeImage()
        {
            float newWidth;
            float newHeight;

            if (_image.Width > _image.Height)
            {
                newWidth = _maxInitialSize_units;
                newHeight = (1.0f/_originalAspectRatio) * newWidth;
            }
            else
            {
                newHeight = _maxInitialSize_units;
                newWidth = newHeight * _originalAspectRatio;
            }
            _windowItemAspectRatio = _originalAspectRatio;
            _containedItemAspectRatio = _originalAspectRatio;
            this._windowSize_units = new SizeF(newWidth, newHeight);
            this._size = new Size(Convert.ToInt16(newWidth*_magnification), Convert.ToInt16(newHeight*_magnification));
           _imageStartingPoint = new Point(0, 0);
            _sourceRectangle = new Rectangle(_imageStartingPoint, this._size);
        }

        private Rectangle _getImageRectangle()
        {
            Rectangle dummy = new Rectangle();
            return (dummy);
        }

        private void _calculateSourceRectangle()
        {
            int sourceWidth;
            int sourceHeight;
            
            if (_originalAspectRatio > 1.0f) //width > height
            {
                if (_windowItemAspectRatio > 1.0f)
                {
                    sourceHeight = Convert.ToInt16((1.0f / (float)_windowItemAspectRatio) * (float)_image.Width);
                    sourceWidth = _image.Width;
                    if (sourceHeight > _image.Height)
                    {
                        sourceHeight = _image.Height;
                        sourceWidth = Convert.ToInt16((float)_image.Height * _windowItemAspectRatio);
                    }
                    _sourceRectangle = new Rectangle(new Point(0, 0), new Size(sourceWidth, sourceHeight));
             
                }
                else // the new stretch makes the height larger than the original height
                {
                    sourceWidth = Convert.ToInt16(_windowItemAspectRatio * (float)_image.Height);
                    sourceHeight = _image.Height;
                    if (sourceWidth > _image.Width)
                    {
                        sourceWidth = _image.Width;
                        sourceHeight = Convert.ToInt16((float)_image.Width * 1.0f / (float)_windowItemAspectRatio);
                    }
                    _sourceRectangle = new Rectangle(new Point(0, 0), new Size(sourceWidth, sourceHeight));
                }
            }
            else
            {
                if (_windowItemAspectRatio < 1.0f)
                {
                    sourceHeight = Convert.ToInt16((1.0f / (float)_windowItemAspectRatio) * (float)_image.Width);
                    sourceWidth = _image.Width;
                    if (sourceHeight > _image.Height)
                    {
                        sourceHeight = _image.Height;
                        sourceWidth = Convert.ToInt16((float)_image.Height * _windowItemAspectRatio);
                    }
                    _sourceRectangle = new Rectangle(new Point(0, 0), new Size(sourceWidth, sourceHeight));
             
                }
                else
                {
                    sourceWidth = Convert.ToInt16(_windowItemAspectRatio * (float)_image.Height);
                    sourceHeight = _image.Height;
                    if (sourceWidth > _image.Width)
                    {
                        sourceWidth = _image.Width;
                        sourceHeight = Convert.ToInt16((float)_image.Width * 1.0f / (float)_windowItemAspectRatio);
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
            
            Rectangle imageRectangle = _getImageRectangle();

            if (_showContainedItem)
            {
                _containedItemResizeBox = new resizeSelectBox(imageRectangle, resizeSelectBox.boxTypes.resize, Color.Blue);
                _containedItemResizeBox.draw(gfx);
            }

            Rectangle destRectangle = new Rectangle(this.location, new Size(Convert.ToInt16(_windowSize_units.Width * _magnification), Convert.ToInt16(_windowSize_units.Height * _magnification)));

            //need some way of keeping existing location
            if (_autoSizeItem)
            {
                this._calculateSourceRectangle();
                gfx.DrawImage(this._image, destRectangle, _sourceRectangle, GraphicsUnit.Pixel);

            }
            else //not autoFitting image
            {
                gfx.DrawImage(this._image, destRectangle);
            }

            base.draw(gfx);
        }

       

        

        

    }
}
