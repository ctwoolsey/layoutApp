using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace layoutApp
{
    public class placeableItem
    {
        public enum itemTypes { none, image, graphic, shape };
        protected itemTypes _itemType;

        protected itemSaveClass _iSC = new itemSaveClass();

        private int _zOrder = -1;
        protected Panel _parent = null;

        protected Point _pageOrigin;
        protected float _magnification = 1.0f;

        protected Boolean _groupSelect = false;

        protected Point _mouseClickOffsetToWindow = new Point();

        //For Window
        protected Boolean _windowSelected = false;
        protected SizeF _windowSize_units;
        protected resizeSelectBox _windowBox = null;
        protected float _windowAspectRatio;
        protected Point _windowLocation = new Point(0, 0);
        protected Size _windowSize = new Size(0, 0);
        protected PointF _windowLocationPMF_units = new PointF();
        
        
        //For ContainedItem
        protected Boolean _itemSelected = false;
        protected SizeF _itemSize_units;
        protected resizeSelectBox _itemBox = null;
        protected float _itemAspectRatio;
        protected Point _itemLocation = new Point(0, 0);
        protected Size _itemSize = new Size(0, 0);
        protected PointF _itemLocationPMF_units = new PointF();

        
        
        
        
        
        /*
        protected SizeF _containedItemSize_units;
        protected Point _containedItemLocation = new Point(0, 0);
        protected Boolean _showContainedItem = false;
        protected resizeSelectBox _containedItemResizeBox = null;
        protected Boolean _containedItemResizing = false;
        protected Boolean _containedItemResizingStarted = false;
        protected resizeSelectBox.resizeDirection _containedItemResizingDirection = resizeSelectBox.resizeDirection.none;
        */
        

       // protected Boolean _selected = false;
       
        //protected resizeSelectBox _resizeBox = null;
        protected Boolean _resizing = false;
        protected Boolean _resizingStarted = false;
        protected resizeSelectBox.resizeDirection _resizeDirection = resizeSelectBox.resizeDirection.none;
        protected PointF _resizingInitialLocationPFM_units;

        //protected Point _mouseClickOffset = new Point();
        //protected PointF _pageMatFrameLocation_units = new PointF();
        //protected PointF _resizingInitialPageMatFrameLocation;

        protected Boolean _autoSizeItem = true;
        
       // protected Point _location = new Point(0, 0);
       // protected Size _size = new Size(0, 0);
        
        public placeableItem(itemTypes placedType, Point dropLocation, Point pageOrigin, float scale)
        {
            _itemType = placedType;
            _pageOrigin = new Point(pageOrigin.X, pageOrigin.Y);
            this._windowLocationPMF_units.X = ((float)dropLocation.X - (float)pageOrigin.X) / scale;
            this._windowLocationPMF_units.Y = ((float)dropLocation.Y - (float)pageOrigin.Y) / scale;
            this._itemLocationPMF_units.X = this._windowLocationPMF_units.X;
            this._itemLocationPMF_units.Y = this._windowLocationPMF_units.Y;
            this._magnification = scale;
            this._windowLocation = dropLocation;
            this._itemLocation = dropLocation;
        }

        public placeableItem(itemTypes placedType, Point pageOrigin, float scale, itemSaveClass iSC)
        {
            _itemType = placedType;
            _pageOrigin = new Point(pageOrigin.X, pageOrigin.Y);
            this._windowLocationPMF_units.X = iSC.windowLocation.X;
            this._windowLocationPMF_units.Y = iSC.windowLocation.Y;
            this._itemLocationPMF_units.X = iSC.itemLocation.X;
            this._itemLocationPMF_units.Y = iSC.itemLocation.Y;
            this._magnification = scale;
            this._windowLocation = new Point(Convert.ToInt16(this._windowLocationPMF_units.X * scale), Convert.ToInt16(this._windowLocationPMF_units.Y * scale));
            this._itemLocation = new Point(Convert.ToInt16(this._itemLocationPMF_units.X * scale), Convert.ToInt16(this._itemLocationPMF_units.Y * scale));
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

        public void add(Panel panelToAddTo, int zIndex)
        {
            this._parent = panelToAddTo;
            this._zOrder = zIndex;
            this._parent.Refresh();
        }

        /*
        public void remove()
        {

            this._parent.Refresh();
        }

        public Boolean resizing
        {
            get { return (_resizing); }
        }
         */

        public Point location
        {
            get { return (this._windowLocation); }
            set
            {
                this._windowLocation = value;
            }
        }

        private void _scaleLocation(Point pageOrigin)
        {
            _pageOrigin = new Point(pageOrigin.X, pageOrigin.Y);
            this._windowLocation = new Point(Convert.ToInt16((_windowLocationPMF_units.X * _magnification) + (float)pageOrigin.X), Convert.ToInt16((_windowLocationPMF_units.Y * _magnification) + (float)pageOrigin.Y));
            this._itemLocation = new Point(Convert.ToInt16((_itemLocationPMF_units.X * _magnification) + (float)pageOrigin.X), Convert.ToInt16((_itemLocationPMF_units.Y * _magnification) + (float)pageOrigin.Y));
        }

        public void scale(float magnification, Point pageOrigin)
        {
            _magnification = magnification;
            this._windowSize = new Size(Convert.ToInt16(_windowSize_units.Width * _magnification), Convert.ToInt16(_windowSize_units.Height * _magnification));
            this._itemSize = new Size(Convert.ToInt16(_itemSize_units.Width * _magnification), Convert.ToInt16(_itemSize_units.Height * _magnification));
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

       
        
        protected void _drawSelectedBox(Graphics gfx)
        {
            _windowBox = new resizeSelectBox(this, resizeSelectBox.boxTypes.selected, Color.White);
            _windowBox.draw(gfx);
        }

        protected void _drawResizeBox(Graphics gfx)
        {
            if (_windowSelected)
            {
                _windowBox = new resizeSelectBox(this, resizeSelectBox.boxTypes.resize, Color.White);
                _windowBox.draw(gfx);
            }
            else
            {
                _itemBox = new resizeSelectBox(new Rectangle(_itemLocation, _itemSize), resizeSelectBox.boxTypes.resize, Color.Blue);
                _itemBox.draw(gfx);
                //_drawSelectedBox(gfx); //this will draw the window ontop of the item box
            }
        }

        private void _drawBorder(Graphics gfx)
        {
            gfx.DrawRectangle(new Pen(Color.Violet), this.bounds);
        }

        public resizeSelectBox.resizeDirection resizeDirection
        {
            get 
            {
                resizeSelectBox.resizeDirection direction = resizeSelectBox.resizeDirection.none;
                if (_windowSelected)
                    direction = _windowBox.direction;
                else
                {
                    if (_itemSelected)
                        direction = _itemBox.direction;
                }
               return (direction); 
            }
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
            Boolean returnValue = false;
                
           if (_itemSelected)
           {
               returnValue = _itemBox.contains(pointToCheck);
           }
           else
           {
               returnValue = this.bounds.Contains(pointToCheck);
           }

            return (returnValue);
        }

        public Boolean windowContains(Point pointToCheck)
        {
            return (this.bounds.Contains(pointToCheck));
        }

        public Boolean itemContains(Point pointToCheck)
        {
            Boolean returnValue = false;

            if (_itemSelected)
                returnValue = _itemBox.contains(pointToCheck);

            return (returnValue);
        }

        public Boolean groupSelect
        {
            get { return (this._groupSelect); }
        }

        public Boolean delete()
        {
            Boolean deleteExternally = false;
            if (_windowSelected)
            {
                deleteExternally = true;
            }
            if (_itemSelected)
            {
                //check if the item can be deleted separately like deleting an image in a template box or mat/frame
                //if it can then return false
                //if it can't then return true = let the main program remove it entirely
            }

            return (deleteExternally);
        }

        public void selectItem(Point mousePoint, Boolean groupSelect)
        {
            
            if (groupSelect)
            {
                _windowSelected = true;
                _itemSelected = false;
                _groupSelect = true;
            
            }
            else
            {
                if (Control.ModifierKeys == Keys.Alt)
                {
                    _itemSelected = true;
                    _windowSelected = false;
                    _groupSelect = false;
                }
                else
                {
                    //window has already been selected so set it up to resize
                    if (((_windowSelected == true) || (_itemSelected)) && (this._groupSelect == false))
                    {
                        Cursor grabCursor;
                        using (var memoryStream = new MemoryStream(Properties.Resources.Grabbed15_19))
                        {
                            grabCursor = new Cursor(memoryStream);
                        }
                        
                        if (_windowSelected)
                        {
                            if (_windowBox.direction != resizeSelectBox.resizeDirection.none)
                            {
                                _resizeDirection = _windowBox.direction;
                                _resizing = true;
                            }
                            else
                            {
                                _parent.Cursor = grabCursor;
                            }
                        }
                        if (_itemSelected)
                        {
                            if (_itemBox.direction != resizeSelectBox.resizeDirection.none)
                            {
                                _resizeDirection = _itemBox.direction;
                                _resizing = true;
                            }
                            else
                            {
                                _parent.Cursor = grabCursor;
                            }
                        }
                    }
                    else
                    {
                        if (this._windowSelected == false)
                        {
                            this._windowSelected = true;
                            this._itemSelected = false;
                            this._groupSelect = false;
                        }
                    }
                }
            }
            _mouseClickOffsetToWindow = new Point(mousePoint.X - _windowLocation.X, mousePoint.Y - _windowLocation.Y);
            if (_itemSelected)
                _mouseClickOffsetToWindow = new Point(mousePoint.X - _itemLocation.X, mousePoint.Y - _itemLocation.Y);
        }

        public void unselectItem()
        {
            _windowSelected = false;
            _itemSelected = false;
            _groupSelect = false;
            _mouseClickOffsetToWindow = new Point();
        }

        public Boolean selected
        {
            get { return (_windowSelected || _itemSelected); }
        }

        protected Point itemToWindowOffset()
        {
            Point itemToWindowOffsetPoint = new Point();
            itemToWindowOffsetPoint.X = _windowLocation.X - _itemLocation.X;
            itemToWindowOffsetPoint.Y = _windowLocation.Y - _itemLocation.Y;
            
            return (itemToWindowOffsetPoint);
        }

        public void moveOrResizeItem(MouseEventArgs e)
        {
            if (this._resizing)
            {
                this.mouseMoved(e);
            }
            else
            {
                Cursor grabCursor;
                using (var memoryStream = new MemoryStream(Properties.Resources.Grabbed15_19))
                {
                    grabCursor = new Cursor(memoryStream);
                }
                
                Point itemWindowOffset = itemToWindowOffset();
                if (_windowSelected)
                {
                    _parent.Cursor = grabCursor;
                    _windowLocation = new Point(e.X - _mouseClickOffsetToWindow.X, e.Y - _mouseClickOffsetToWindow.Y);
                    _windowLocationPMF_units.X = ((float)location.X - (float)_pageOrigin.X) / _magnification;
                    _windowLocationPMF_units.Y = ((float)location.Y - (float)_pageOrigin.Y) / _magnification;

                    _itemLocation = new Point(_windowLocation.X - itemWindowOffset.X, _windowLocation.Y - itemWindowOffset.Y);
                    _itemLocationPMF_units.X = ((float)_itemLocation.X - (float)_pageOrigin.X) / _magnification;
                    _itemLocationPMF_units.Y = ((float)_itemLocation.Y - (float)_pageOrigin.Y) / _magnification;
                }

                if (_itemSelected)
                {
                    _parent.Cursor = grabCursor;
                    _itemLocation = new Point(e.X - _mouseClickOffsetToWindow.X, e.Y - _mouseClickOffsetToWindow.Y);
                    _itemLocationPMF_units.X = ((float)_itemLocation.X - (float)_pageOrigin.X) / _magnification;
                    _itemLocationPMF_units.Y = ((float)_itemLocation.Y - (float)_pageOrigin.Y) / _magnification;
                }

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
            //this is only called when the groupingBox is resizing each element
            //so this only affects the windowSize

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
                    _windowLocation = new Point(newX, _windowLocation.Y);
                    _windowLocationPMF_units.X = (float)newX / _magnification;
                    break;
                case resizeSelectBox.resizeDirection.sE:
                     _windowSize_units.Height = _windowSize_units.Height * yPerc;
                    if (!keepAspect)
                        _windowSize_units.Width = _windowSize_units.Width * xPerc;
                    else
                        _windowSize_units.Width = aspectRatio * _windowSize_units.Height;
                    break;
                case resizeSelectBox.resizeDirection.nE:
                    fixedY = Convert.ToInt16((float)this.location.Y + (_windowSize_units.Height * _magnification));
                    _windowSize_units.Height = _windowSize_units.Height * yPerc;
                    if (!keepAspect)
                        _windowSize_units.Width = _windowSize_units.Width * xPerc;
                    else
                        _windowSize_units.Width = aspectRatio * _windowSize_units.Height;
                    newY = fixedY - Convert.ToInt16(_windowSize_units.Height * _magnification);
                    _windowLocationPMF_units.Y = (float)newY / _magnification;
                    _windowLocation = new Point(_windowLocation.X, newY);
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
                    _windowLocation = new Point(newX, newY);
                    _windowLocationPMF_units = new PointF((float)_windowLocation.X / _magnification, (float)_windowLocation.Y / _magnification);
                    break;
            }
            _windowSize = new Size(Convert.ToInt16(_windowSize_units.Width * _magnification),Convert.ToInt16(_windowSize_units.Height * _magnification));
            _windowAspectRatio = _windowSize_units.Width / _windowSize_units.Height;
        }

        public void mouseMoved(MouseEventArgs e)
        {
            if (_resizing) //resizing does not need to contain the point because the mouse may outpace the resize
            {
                SizeF boxSize_units = new SizeF();
                
                Boolean keepAspect = (Control.ModifierKeys == Keys.Shift) ? true : false;
                float aspectRatio;
                Point locationOrigin;
                

                PointF locationUnits;

                if (_windowSelected)
                {
                    aspectRatio = _windowAspectRatio;
                    locationOrigin = _windowLocation;
                    locationUnits = _windowLocationPMF_units;
                    boxSize_units = _windowSize_units;
                }
                else //_itemSelected = true
                {
                    aspectRatio = _itemAspectRatio;
                    locationOrigin = _itemLocation;
                    locationUnits = _itemLocationPMF_units;
                    boxSize_units = _itemSize_units;
                }
                
                int newX = 0;
                switch (_resizeDirection)
                {
                    case resizeSelectBox.resizeDirection.sE:
                        boxSize_units.Height = (float)(e.Y - locationOrigin.Y) / _magnification;
                        if (!keepAspect)
                            boxSize_units.Width = (float)(e.X - locationOrigin.X) / _magnification;
                        else
                            boxSize_units.Width = aspectRatio * boxSize_units.Height;

                        _resizingStarted = true;
                        break;
                    case resizeSelectBox.resizeDirection.sW:
                        if (_resizingStarted == false)
                            _resizingInitialLocationPFM_units = new PointF(locationUnits.X + boxSize_units.Width + (float)_pageOrigin.X / _magnification, locationUnits.Y + boxSize_units.Height + (float)_pageOrigin.Y / _magnification);
                        locationUnits.X = (float)(e.X - _pageOrigin.X) / _magnification;

                        boxSize_units.Height = (float)(e.Y - locationOrigin.Y) / _magnification;
                        if (!keepAspect)
                        {
                            boxSize_units.Width = _resizingInitialLocationPFM_units.X - (float)e.X / _magnification;
                            newX = e.X;
                        }
                        else
                        {
                            float wouldHaveBeenWidth = _resizingInitialLocationPFM_units.X - (float)e.X / _magnification;
                            boxSize_units.Width = aspectRatio * boxSize_units.Height;
                            newX = e.X + Convert.ToInt16((wouldHaveBeenWidth - boxSize_units.Width) * _magnification);
                        }
                        locationOrigin = new Point(newX, locationOrigin.Y);
                        _resizingStarted = true;
                        break;
                    case resizeSelectBox.resizeDirection.nE:
                        Console.WriteLine(String.Concat("resizeStarted: ",_resizingStarted.ToString()," Height: ",boxSize_units.Height.ToString()));
                        if (_resizingStarted == false)
                            _resizingInitialLocationPFM_units = new PointF(locationUnits.X + boxSize_units.Width + (float)_pageOrigin.X / _magnification, locationUnits.Y + boxSize_units.Height + (float)_pageOrigin.Y / _magnification);
                        locationUnits.Y = (float)(e.Y - _pageOrigin.Y) / _magnification;
                        boxSize_units.Height = _resizingInitialLocationPFM_units.Y - (float)e.Y / _magnification;
                        if (!keepAspect)
                            boxSize_units.Width = (float)(e.X - locationOrigin.X) / _magnification;
                        else
                            boxSize_units.Width = aspectRatio * boxSize_units.Height;
                        locationOrigin = new Point(locationOrigin.X, e.Y);
                        _resizingStarted = true;
                        break;
                    case resizeSelectBox.resizeDirection.nW:
                        if (_resizingStarted == false)
                            _resizingInitialLocationPFM_units = new PointF(locationUnits.X + boxSize_units.Width + (float)_pageOrigin.X / _magnification, _windowLocationPMF_units.Y + boxSize_units.Height + (float)_pageOrigin.Y / _magnification);
                        
                        locationUnits.Y = (float)(e.Y - _pageOrigin.Y) / _magnification;
                        locationUnits.X = (float)(e.X - _pageOrigin.X) / _magnification;
                        boxSize_units.Height = _resizingInitialLocationPFM_units.Y - (float)e.Y / _magnification;
                        if (!keepAspect)
                        {
                            boxSize_units.Width = _resizingInitialLocationPFM_units.X - (float)e.X / _magnification;
                            newX = e.X;
                        }
                        else
                        {
                            float wouldHaveBeenWidth = _resizingInitialLocationPFM_units.X - (float)e.X / _magnification;
                            boxSize_units.Width = aspectRatio * boxSize_units.Height;
                            newX = e.X + Convert.ToInt16((wouldHaveBeenWidth - boxSize_units.Width) * _magnification);
                        }
                        locationOrigin = new Point(newX, e.Y);
                        _resizingStarted = true;
                        break;
                };

                if (_windowSelected)
                {
                    _windowSize_units = boxSize_units;
                    _windowSize = new Size(Convert.ToInt16(_windowSize_units.Width * _magnification), Convert.ToInt16(_windowSize_units.Height * _magnification));
                    _windowAspectRatio = boxSize_units.Width / boxSize_units.Height;
                    _windowLocationPMF_units = locationUnits;
                    _windowLocation = locationOrigin;
                }
                if (_itemSelected)
                {
                    _itemSize_units = boxSize_units;
                    _itemSize = new Size(Convert.ToInt16(_itemSize_units.Width * _magnification), Convert.ToInt16(_itemSize_units.Height * _magnification));
                    _itemAspectRatio = boxSize_units.Width / boxSize_units.Height;
                    _itemLocationPMF_units = locationUnits;
                    _itemLocation = locationOrigin;
                }
                
            }
            else //not currently resizing
            {
                if (this.contains(e.Location))
                {
                   // if (Control.ModifierKeys == Keys.Alt)
                    //{
                      //  this.panItem(e); //I think this is taken care of in moveOrResizeItem
                    //}
                    //else
                    {
                        Cursor handCursor;
                        using (var memoryStream = new MemoryStream(Properties.Resources.hand15_19))
                        {
                            handCursor = new Cursor(memoryStream);
                        }
                        if (_windowSelected)
                        {
                            if (_windowBox.hover(e.Location))
                            {
                                if (_windowBox.direction == resizeSelectBox.resizeDirection.none)
                                    _parent.Cursor = handCursor;
                                else
                                    _parent.Cursor = _windowBox.getCursor();
                            }
                        }
                        if (_itemSelected)
                        {
                            if (_itemBox.hover(e.Location))
                            {
                                if (_itemBox.direction == resizeSelectBox.resizeDirection.none)
                                    _parent.Cursor = handCursor;
                                else
                                    _parent.Cursor = _itemBox.getCursor();
                            }
                        }
                    }
                }
                else
                {
                    _parent.Cursor = Cursors.Default;
                }
            }
        }

        public Cursor resizeHoverCursor(Point mousePoint)
        {
            Cursor returnCursor = Cursors.Default;
            if (_windowSelected)
            {
                _windowBox.hover(mousePoint);
                returnCursor = _windowBox.getCursor();
            }
            if (_itemSelected)
            {
                _itemBox.hover(mousePoint);
                returnCursor = _itemBox.getCursor();
            }
           
            return (returnCursor);
        }

        public virtual itemSaveClass save()
        {
            _iSC.itemType = _itemType.ToString();
            _iSC.zOrder = _zOrder;
            _iSC.windowSize = _windowSize_units;
            _iSC.windowLocation = _windowLocationPMF_units;
            _iSC.itemSize = _itemSize_units;
            _iSC.itemLocation = _itemLocationPMF_units;

            
            return (_iSC);
        }

        public virtual void load(itemSaveClass iSC)
        {
            _itemType = (itemTypes)Enum.Parse(typeof(itemTypes), iSC.itemType);
            _zOrder = iSC.zOrder;
            _windowSize_units = iSC.windowSize;
            _windowLocationPMF_units = iSC.windowLocation;
            _itemSize_units = iSC.itemSize;
            _itemLocationPMF_units = iSC.itemLocation;

        }

        public virtual void draw()
        {
            if (this._parent != null)
                this.draw(this._parent.CreateGraphics());
        }

        public virtual void draw(Graphics gfx)
        {
            if ((this._windowSelected) && (this._groupSelect))
            {
                this._drawSelectedBox(gfx);
            }else
            {
                if ((_windowSelected) || (_itemSelected))
                {
                    //this._drawSelectedBox(gfx);
                    //else
                        this._drawResizeBox(gfx);
                }
                else
                {
                    this._drawBorder(gfx);
                }
            }
        }


    }
}
