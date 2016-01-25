using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace layoutApp
{
    public class placeableItem
    {
        public enum itemTypes { none, image, graphic, shape };
        protected itemTypes _itemType;

        protected SizeF _windowSize_units;

        

        private int _zOrder = -1;
        private int _indexInList = -1;

        protected Panel _parent = null;

        protected Point _pageOrigin;
        protected float _magnification = 1.0f;

        protected Boolean _selected = false;
        protected Boolean _groupSelect = false;

        protected Boolean showResizeBox = false;

        protected resizeSelectBox _resizeBox = null;
        protected Boolean _resizing = false;
        protected Boolean _resizingStarted = false;
        protected resizeSelectBox.resizeDirection _resizeDirection = resizeSelectBox.resizeDirection.none;

        protected Point _mouseClickOffset = new Point();
        protected PointF _offsetLocation_units = new PointF();
        protected PointF _resizingInitialOffsetLoction;

        protected Boolean _autoSizeItem = true;
        protected float _itemAspectRatio;

        protected Point _location = new Point(0, 0);
        protected Size _size = new Size(0, 0);
        
        static List<placeableItem> _placedItems = new List<placeableItem>();

        public placeableItem(itemTypes placedType, Point dropLocation, Point pageOrigin, float scale)
        {
            _itemType = placedType;
            _pageOrigin = new Point(pageOrigin.X, pageOrigin.Y);
            this._offsetLocation_units.X = ((float)dropLocation.X - (float)pageOrigin.X) / scale;
            this._offsetLocation_units.Y = ((float)dropLocation.Y - (float)pageOrigin.Y) / scale;
            this._magnification = scale;
            this._location = dropLocation;
        }

        public itemTypes itemType
        {
            get { return (this._itemType); }
        }

        public int zOrder
        {
            get {return (_zOrder);}
            set {_zOrder = value;}
        }

        public int indexInList
        {
            get { return (_indexInList); }
            set { _indexInList = value; }
        }

        public void add(Panel panelToAddTo, int zIndex)
        {
            this._parent = panelToAddTo;
         //   _placedItems.Add(this);
            //_numItems++;
            this._zOrder = zIndex;// _placedItems.Count();
                //numItems;
            this._parent.Refresh();
        }

        public void remove()
        {
            //_numItems--;
            //_placedItems.Remove(this);
            this._parent.Refresh();
        }

        public Boolean resizing
        {
            get { return (_resizing); }
        }

        public Point location
        {
            get { return (this._location); }
            set
            {
                //  float originalLocation_unitsX = (float)location.X / this._magnification;
                // float originalLocation_unitsY = (float)location.Y / this._magnification;
                // _imageLocation_units = new PointF(originalLocation_unitsX, originalLocation_unitsY);
                //_scaleLocation();
                this._location = value;
            }
        }

        private Point _scaleLocation(Point pageOrigin)
        {
            _pageOrigin = new Point(pageOrigin.X, pageOrigin.Y);
            Point locationPoint = new Point(Convert.ToInt16((_offsetLocation_units.X * _magnification) + (float)pageOrigin.X), Convert.ToInt16((_offsetLocation_units.Y * _magnification) + (float)pageOrigin.Y));
            this._location = locationPoint;
            return locationPoint;
        }

        public void scale(float magnification, Point pageOrigin)
        {
            _magnification = magnification;
            this._size = new Size(Convert.ToInt16(_windowSize_units.Width * _magnification), Convert.ToInt16(_windowSize_units.Height * _magnification));
            this._scaleLocation(pageOrigin);
        }

        public float scale()
        {
            return (this._magnification); ;
        }

        public Size size
        {
            get { return (new Size(Convert.ToInt16(_windowSize_units.Width * _magnification), Convert.ToInt16(_windowSize_units.Height * _magnification))); }
        }

        public Size newSize
        {
            set
            {
                this._size = value;
            }
        }

        protected void _drawSelectedBox(Graphics gfx)
        {
            _resizeBox = new resizeSelectBox(this, resizeSelectBox.boxTypes.selected);
            _resizeBox.draw(gfx);
        }
        protected void _drawResizeBox(Graphics gfx)
        {
            _resizeBox = new resizeSelectBox(this, resizeSelectBox.boxTypes.resize);
            _resizeBox.draw(gfx);
        }

        public resizeSelectBox.resizeDirection resizeDirection
        {
            get { return (_resizeBox.direction); }
        }

        public Rectangle bounds
        {
            get
            {
                Rectangle imageRectangle = new Rectangle(this.location, new Size(Convert.ToInt16(_windowSize_units.Width * _magnification), Convert.ToInt16(_windowSize_units.Height * _magnification)));
                return (imageRectangle);
            }
        }

        public Boolean contains(Point pointToCheck)
        {
            return (this.bounds.Contains(pointToCheck));
        }

        public Boolean groupSelect
        {
            get { return (this._groupSelect); }
        }

        public void selectItem(MouseEventArgs e, Boolean groupSelect)
        {
            if (groupSelect)
            {
                _selected = true;
                _groupSelect = true;
               // this.showResizeBox = false;
               _mouseClickOffset = new Point(e.X - this.location.X, e.Y - this.location.Y);
            }
            else
            {
                if ((this._selected == true) && (this._groupSelect == false))//(this.showResizeBox == true))
                {
                    _mouseClickOffset = new Point(e.X - this.location.X, e.Y - this.location.Y);
                    if (_resizeBox.direction != resizeSelectBox.resizeDirection.none)
                    {
                        _resizeDirection = _resizeBox.direction;
                        _resizing = true;
                    }
                }
                else
                {
                    if (this._selected == false)
                    {
                        this._selected = true;
                        this._groupSelect = false;
                    }
                    
                   // this.showResizeBox = true;
                    _mouseClickOffset = new Point(e.X - this.location.X, e.Y - this.location.Y);
                }
            }
        }

        public void unselectItem()
        {
            _selected = false;
            //this.showResizeBox = false;
            _groupSelect = false;
            _mouseClickOffset = new Point();
        }

        public Boolean selected
        {
            get { return (_selected); }
        }
        
        public void moveOrResizeItem(MouseEventArgs e)
        {
            if (this._resizing)
            {
                this.mouseMoved(e);
            }
            else
            {
                this.location = new Point(e.X - _mouseClickOffset.X, e.Y - _mouseClickOffset.Y);
                this._offsetLocation_units.X = ((float)location.X - (float)_pageOrigin.X) / _magnification;
                this._offsetLocation_units.Y = ((float)location.Y - (float)_pageOrigin.Y) / _magnification;
            }
        }


        public Boolean autoSize
        {
            set { this._autoSizeItem = value; }
        }

        public virtual void mouseUp()
        {
            _resizing = false;
            _resizingStarted = false;
        }

        public void resizeItem(resizeSelectBox.resizeDirection resizeDirection, float xPerc, float yPerc, Boolean keepAspect)
        {
            float aspectRatio = _windowSize_units.Width / _windowSize_units.Height;
            int fixedX;
            int fixedY;
            int newX;
            int newY;
            switch (resizeDirection)
            {
                case resizeSelectBox.resizeDirection.sW:
                    fixedX = Convert.ToInt16((float)this.location.X + (_windowSize_units.Width * _magnification));
                    _windowSize_units.Height = _windowSize_units.Height * yPerc;
                    if (!keepAspect)
                        _windowSize_units.Width = _windowSize_units.Width * xPerc;
                    else
                        _windowSize_units.Width = aspectRatio * _windowSize_units.Height;

                    newX = fixedX - Convert.ToInt16(_windowSize_units.Width * _magnification);
                    this.location = new Point(newX, this.location.Y);
                    //    _resizingStarted = true;
                    break;
                case resizeSelectBox.resizeDirection.sE:
                     _windowSize_units.Height = _windowSize_units.Height * yPerc;
                    if (!keepAspect)
                        _windowSize_units.Width = _windowSize_units.Width * xPerc;
                    else
                        _windowSize_units.Width = aspectRatio * _windowSize_units.Height;
                    //_resizingStarted = true;
                    break;
                case resizeSelectBox.resizeDirection.nE:
                    fixedY = Convert.ToInt16((float)this.location.Y + (_windowSize_units.Height * _magnification));
                    _windowSize_units.Height = _windowSize_units.Height * yPerc;
                    if (!keepAspect)
                        _windowSize_units.Width = _windowSize_units.Width * xPerc;
                    else
                        _windowSize_units.Width = aspectRatio * _windowSize_units.Height;
                    newY = fixedY - Convert.ToInt16(_windowSize_units.Height * _magnification);
                    this.location = new Point(this.location.X, newY);
                    break;
                case resizeSelectBox.resizeDirection.nW:
                    fixedX = Convert.ToInt16((float)this.location.X + (_windowSize_units.Width * _magnification));
                    fixedY = Convert.ToInt16((float)this.location.Y + (_windowSize_units.Height * _magnification));
                    _windowSize_units.Height = _windowSize_units.Height * yPerc;
                    if (!keepAspect)
                        _windowSize_units.Width = _windowSize_units.Width * xPerc;
                    else
                        _windowSize_units.Width = aspectRatio * _windowSize_units.Height;
                    newX = fixedX - Convert.ToInt16(_windowSize_units.Width * _magnification);
                    newY = fixedY - Convert.ToInt16(_windowSize_units.Height * _magnification);
                    this.location = new Point(newX, newY);
                    break;
            }
            _itemAspectRatio = _windowSize_units.Width / _windowSize_units.Height;
        }

        public void mouseMoved(MouseEventArgs e)
        {
            if (_resizing)  //resizing does not need to contain the point because the mouse may outpace the resize
            {
                Boolean keepAspect = (Control.ModifierKeys == Keys.Shift) ? true : false;
                float aspectRatio = _windowSize_units.Width / _windowSize_units.Height;
                int newX = 0;
                switch (_resizeDirection)
                {
                    case resizeSelectBox.resizeDirection.sE:
                        _windowSize_units.Height = (float)(e.Y - this.location.Y) / _magnification;
                        if (!keepAspect)
                            _windowSize_units.Width = (float)(e.X - this.location.X) / _magnification;
                        else
                            _windowSize_units.Width = aspectRatio * _windowSize_units.Height;

                        _resizingStarted = true;
                        break;
                    case resizeSelectBox.resizeDirection.sW:
                        if (_resizingStarted == false)
                            _resizingInitialOffsetLoction = new PointF(_offsetLocation_units.X + _windowSize_units.Width + (float)_pageOrigin.X / _magnification, _offsetLocation_units.Y + _windowSize_units.Height + (float)_pageOrigin.Y / _magnification);
                        _offsetLocation_units.X = (float)(e.X - _pageOrigin.X) / _magnification;

                        _windowSize_units.Height = (float)(e.Y - this.location.Y) / _magnification;
                        if (!keepAspect)
                        {
                            _windowSize_units.Width = _resizingInitialOffsetLoction.X - (float)e.X / _magnification;
                            newX = e.X;
                        }
                        else
                        {
                            float wouldHaveBeenWidth = _resizingInitialOffsetLoction.X - (float)e.X / _magnification;
                            _windowSize_units.Width = aspectRatio * _windowSize_units.Height;
                            newX = e.X + Convert.ToInt16((wouldHaveBeenWidth - _windowSize_units.Width) * _magnification);
                        }
                        this.location = new Point(newX, this.location.Y);
                        _resizingStarted = true;
                        break;
                    case resizeSelectBox.resizeDirection.nE:
                        if (_resizingStarted == false)
                            _resizingInitialOffsetLoction = new PointF(_offsetLocation_units.X + _windowSize_units.Width + (float)_pageOrigin.X / _magnification, _offsetLocation_units.Y + _windowSize_units.Height + (float)_pageOrigin.Y / _magnification);
                        _offsetLocation_units.Y = (float)(e.Y - _pageOrigin.Y) / _magnification;
                        Console.WriteLine(String.Concat("rIOL.Y = ", _resizingInitialOffsetLoction.Y * _magnification, " e.Y = ", e.Y));
                        _windowSize_units.Height = _resizingInitialOffsetLoction.Y - (float)e.Y / _magnification;
                        if (!keepAspect)
                            _windowSize_units.Width = (float)(e.X - this.location.X) / _magnification;
                        else
                            _windowSize_units.Width = aspectRatio * _windowSize_units.Height;
                        this.location = new Point(this.location.X, e.Y);
                        _resizingStarted = true;
                        break;
                    case resizeSelectBox.resizeDirection.nW:
                        if (_resizingStarted == false)
                            _resizingInitialOffsetLoction = new PointF(_offsetLocation_units.X + _windowSize_units.Width + (float)_pageOrigin.X / _magnification, _offsetLocation_units.Y + _windowSize_units.Height + (float)_pageOrigin.Y / _magnification);
                        _offsetLocation_units.Y = (float)(e.Y - _pageOrigin.Y) / _magnification;
                        _offsetLocation_units.X = (float)(e.X - _pageOrigin.X) / _magnification;
                        _windowSize_units.Height = _resizingInitialOffsetLoction.Y - (float)e.Y / _magnification;
                        if (!keepAspect)
                        {
                            _windowSize_units.Width = _resizingInitialOffsetLoction.X - (float)e.X / _magnification;
                            newX = e.X;
                        }
                        else
                        {
                            float wouldHaveBeenWidth = _resizingInitialOffsetLoction.X - (float)e.X / _magnification;
                            _windowSize_units.Width = aspectRatio * _windowSize_units.Height;
                            newX = e.X + Convert.ToInt16((wouldHaveBeenWidth - _windowSize_units.Width) * _magnification);
                        }
                        this.location = new Point(newX, e.Y);
                        _resizingStarted = true;
                        break;
                };
                _itemAspectRatio = _windowSize_units.Width / _windowSize_units.Height;
            }
            else //not currently resizing
            {
                if (this.contains(e.Location))
                {
                    if (Control.ModifierKeys == Keys.Alt)
                    {
                        this.panItem(e);
                    }
                    else
                    {
                        _resizeBox.hover(e.Location);
                        _parent.Cursor = _resizeBox.getCursor();
                    }
                }
                else
                {
                    _parent.Cursor = Cursors.Default;
                }
            }
        }

        protected virtual void panItem(MouseEventArgs e)
        {

        }

        public virtual void draw()
        {
            if (this._parent != null)
                this.draw(this._parent.CreateGraphics());
        }

        public virtual void draw(Graphics gfx)
        {
            if ((this._selected) && (this._groupSelect))
            {
                this._drawSelectedBox(gfx);
            }else
            {
                if (this._selected)
                    this._drawResizeBox(gfx);
            }
        }


    }
}
