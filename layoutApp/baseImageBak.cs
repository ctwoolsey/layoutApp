using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace layoutApp
{
    class baseImage : Panel
    {
        private String _fileName;
        private Image _image = null;
        private SizeF _imageSize_units;
        private PointF _imageLocation_units;

        //private Point _mousePosition = new Point();

       // private Rectangle _imageBounds;

        private float _magnification = 1.0f;

        private float _maxInitialSize_units = 2.5f;

        private Boolean showResizeBox = false;

        private resizeBox _resizeBox = null;
        private Boolean _resizing = false;
     
        public baseImage(float scale)
        {
            initialization();
            this._magnification = scale;
        }

        public baseImage(String fileName)
        {
            initialization();
            this.fileName = fileName;
        }

        public baseImage(String fileName, float scale)
        {
            initialization();
            this._magnification = scale;
            this.fileName = fileName;
        }

        public baseImage(String fileName, float maxImageSize, float scale)
        {
            initialization();
            this._magnification = scale;
            this.fileName = fileName;
            this._maxInitialSize_units = maxImageSize;
        }

        private void initialization()
        {
            this.DoubleBuffered = true;
            this.Click += baseImage_Click;
            this.MouseMove += baseImage_MouseMove;
            this.MouseDown += baseImage_MouseDown;

            this.BackColor = Color.Violet;
        }

        void baseImage_MouseDown(object sender, MouseEventArgs e)
        {
            baseImage bI = (baseImage)sender;
            if ((bI.showResizeBox == true) && (bI._resizeBox.direction != resizeBox.resizeDirection.none))
            {
                _resizing = true;
               
            }
        }

        public Boolean resizing
        {
            get { return (_resizing); }
        }

        public resizeBox.resizeDirection resizeDirection
        {
            get { return (_resizeBox.direction); }
        }

        void baseImage_MouseMove(object sender, MouseEventArgs e)
        {
            //_mousePosition = new Point(e.X,e.Y);
            if (_resizeBox != null)
            {
                if (_resizing)
                {
                    
                }
                else
                {
                    if (_resizeBox.hover(new Point(e.X, e.Y)))
                    {
                        this.Cursor = _resizeBox.getCursor(new Point(e.X, e.Y));
                        if (e.Button == MouseButtons.Left)
                        {

                        }
                    }
                    else
                    {
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }

      

        void baseImage_Click(object sender, EventArgs e)
        {
            baseImage bI = (baseImage)sender;
            if (bI.showResizeBox == true)
            {
                
            }
            else
            {
                bI.showResizeBox = true;
            }
        }

       
        

        public void setLocation(Point location)
        {
            float originalLocation_unitsX = (float)location.X / this._magnification;
            float originalLocation_unitsY = (float)location.Y / this._magnification;
            _imageLocation_units = new PointF(originalLocation_unitsX, originalLocation_unitsY);
            _scaleLocation();
            //this._imageBounds = new Rectangle(_scaleLocation(), this._image.Size); ;
        }

        private Point _scaleLocation()
        {
            Point locationPoint = new Point(Convert.ToInt16(_imageLocation_units.X * _magnification), Convert.ToInt16(_imageLocation_units.Y * _magnification));
            this.Location = locationPoint;
            return locationPoint;
        }

        public String fileName
        {
            get { return (_fileName); }
            set {
                _fileName = value;
                _image = Image.FromFile(_fileName);
                _sizeImage(_maxInitialSize_units);
                this._drawImage(this.CreateGraphics());
                }
        }

        private void _sizeImage(float maxDimension)
        {
            float newWidth;
            float newHeight;

            if (_image.Width > _image.Height)
            {
                newWidth = maxDimension;
                newHeight = ((float)_image.Height / (float)_image.Width) * (float)newWidth;
            }
            else
            {
                newHeight = maxDimension;
                newWidth = ((float)newHeight * (float)_image.Width) / ((float)_image.Height);
            }
            this._imageSize_units = new SizeF(newWidth, newHeight);
            this.Size = new Size(Convert.ToInt16(newWidth*_magnification), Convert.ToInt16(newHeight*_magnification));
        }

        private void _scaleImage()
        {
            this.Size = new Size(Convert.ToInt16(_imageSize_units.Width * _magnification), Convert.ToInt16(_imageSize_units.Height * _magnification));
            this.Refresh();
        }

        public void setScale(float newScale)
        {
            _magnification = newScale;
            this._scaleImage();
            this._scaleLocation();
        }

        public Size newSize
        {
            set
            {
                this.Size = value;
            }
        }

        private void _drawResizeBox(Graphics gfx)
        {
            _resizeBox = new resizeBox(this);
            _resizeBox.draw(gfx);

       
        }

      
        private void _drawImage(Graphics gfx)
        {
            Rectangle imageRectangle = new Rectangle(new Point(0,0),new Size(Convert.ToInt16(_imageSize_units.Width * _magnification), Convert.ToInt16(_imageSize_units.Height * _magnification))); 
            gfx.DrawImage(this._image,imageRectangle);
            if (this.showResizeBox)
            {
                this._drawResizeBox(gfx);

            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
           base.OnPaint(e);
           this._drawImage(e.Graphics);
        }

        

    }
}
