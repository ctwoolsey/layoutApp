using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace layoutApp
{
    public class baseImage : placeableItem
    {
        private String _fileName;
        private Image _image = null;
        private float _originalAspectRatio = 0.66667f;
        private Rectangle _sourceRectangle;
      
        private float _maxInitialSize_units;

        public baseImage(String fileName, Point dropLocation, Point pageOrigin, float scale, float maxImageDimensionUnits) : base(placeableItem.itemTypes.image, dropLocation, pageOrigin, scale)
        {
            _maxInitialSize_units = maxImageDimensionUnits;
            this.fileName = fileName;
        }

        public baseImage(Point pageOrigin, float scale, float maxImageDimensionUnits, imageSaveClass iSC)
            : base(placeableItem.itemTypes.image, pageOrigin, scale, iSC)
        {
            _maxInitialSize_units = maxImageDimensionUnits;
            this._originalAspectRatio = iSC.originalAspectRatio;
            this.fileName = iSC.fileName;
        }
       
        public String fileName
        {
            get { return (_fileName); }
            set {
                    _fileName = value;
                    if (File.Exists(_fileName))
                    {
                        _image = Image.FromFile(_fileName);
                        _originalAspectRatio = (float)_image.Width / (float)_image.Height;
                        _sizeImage();
                        //this.draw();
                    }
                    else
                    {
                        _image = new Bitmap(_itemSize.Width, _itemSize.Height);
                        Image linkImage = new Bitmap(Properties.Resources.picture_link_icon);
                        Graphics g = Graphics.FromImage(_image);
                        g.DrawImage(linkImage, new PointF((float)_image.Width / 2.0f - (float)linkImage.Width / 2.0f, (float)_image.Height / 2.0f - (float)linkImage.Height / 2.0f));
                        _sizeImage();
                        //this.draw();
                    }
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
            _windowAspectRatio = _originalAspectRatio;
            _itemAspectRatio = _originalAspectRatio;
            this._windowSize_units = new SizeF(newWidth, newHeight);
            this._itemSize_units = new SizeF(newWidth, newHeight);
            _windowSize = new Size(Convert.ToInt16(newWidth*_magnification), Convert.ToInt16(newHeight*_magnification));
            _itemSize = new Size(Convert.ToInt16(newWidth * _magnification), Convert.ToInt16(newHeight * _magnification));
            

            //_sourceRectangle = new Rectangle(new Point(0,0),_windowSize);
        }

        private Rectangle _getImageRectangle()
        {
            Rectangle imageRectangle = new Rectangle(_itemLocation, _itemSize);
            return (imageRectangle);
        }

        private void _calculateSourceRectangle()
        {
            Point windowOffset = new Point(_windowLocation.X - _itemLocation.X, _windowLocation.Y - _itemLocation.Y);
            windowOffset.Y = Convert.ToInt16((float) windowOffset.Y * (float)_image.Height / (float)_itemSize.Height);
            windowOffset.X = Convert.ToInt16((float)windowOffset.X * (float)_image.Width / (float)_itemSize.Width);
            Size displaySize = new Size();
            displaySize.Height = Convert.ToInt16((float)((float)_windowSize.Height / (float)_itemSize.Height) * (float)_image.Height);
            displaySize.Width = Convert.ToInt16((float)((float)_windowSize.Width / (float)_itemSize.Width) * (float)_image.Width);
            _sourceRectangle = new Rectangle(windowOffset, displaySize);
           }

        /*private void _calculateSourceRectangle()
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
        } */

        public Image SetImageOpacity(Image image, float opacity)
        {
                //create a Bitmap the size of the image provided  
                Bitmap bmp = new Bitmap(image.Width, image.Height);

                //create a graphics object from the image  
                using (Graphics gfx = Graphics.FromImage(bmp))
                {

                    //create a color matrix object  
                    ColorMatrix matrix = new ColorMatrix();

                    //set the opacity  
                    matrix.Matrix33 = opacity;

                    //create image attributes  
                    ImageAttributes attributes = new ImageAttributes();

                    //set the color(opacity) of the image  
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    //now draw the image  
                    gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                }
                return bmp;
        } 

        public override void draw(Graphics gfx)
        {
            
            Rectangle imageRectangle = _getImageRectangle();

            if (_itemSelected)
            {
                Image transparentImage = SetImageOpacity(_image, 0.5f);
                gfx.DrawImage(transparentImage, _getImageRectangle());
                transparentImage.Dispose();
            }

            Rectangle destRectangle = new Rectangle(_windowLocation, new Size(Convert.ToInt16(_windowSize_units.Width * _magnification), Convert.ToInt16(_windowSize_units.Height * _magnification)));

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


        public override itemSaveClass save()
        {
            _iSC = new itemSaveClass();
            imageSaveClass _imageSaveClass = new imageSaveClass();
            _imageSaveClass.fileName = _fileName;
            _imageSaveClass.originalAspectRatio = _originalAspectRatio;
            _iSC = _imageSaveClass;
            base.save();

         
            return (_iSC);
        }

        

    }
}
