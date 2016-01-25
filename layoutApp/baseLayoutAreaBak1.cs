using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace layoutApp
{
    public struct designAreaPaddingStruct
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        public int padding
        {
            set
            {
                this.left = value;
                this.top = value;
                this.right = value;
                this.bottom = value;
            }
        }

        public int xPadding
        {
            get { return (this.left + this.right); }
            set
            {
                this.left = value;
                this.right = value;
            }
        }

        public int yPadding
        {
            get { return (this.top + this.bottom); }
            set
            {
                this.top = value;
                this.bottom = value;
            }
        }
    }
    // the baseProduct Owns this, but it is attached to the main form
    class baseLayoutArea : Panel
    {
        public enum layoutActions { none, layoutAreaPan, itemMove, itemResize, groupResize, imagePan, imageCrop, imageResize };
        public layoutActions currentLayoutAction = layoutActions.none;
        public history layoutStateHistory;
        

        private Point mouseDownPosition = new Point();

        private designAreaPaddingStruct _padding = new designAreaPaddingStruct();

        private float _zoom = 1.0f;
        private float _initialScale = 1.0f;
        private float _zoomScale = 1.0f;  //this is the product of _zoom and _initialScale

        Size referenceSize;

        private designArea _layoutArea = new designArea();
        private basePageMatFrameArea _pageMatFrame;

        private placeableItem _selectedItem = null;

        private Boolean _showGroupingBox = false;
        private resizeSelectBox _GroupingBox = null;
        private Rectangle _groupingRectangle;
        //private Boolean _groupingBoxResize = false;

        Boolean _keyDownHandled = false;
        
        private resizeSelectBox.resizeDirection _groupingBoxResizeDirection = resizeSelectBox.resizeDirection.none;

        //private Boolean _autoFitImages = true;  //set in preferences or some check box; after sizing boxes the images will be fit
        public baseLayoutArea(int xOffset, int yOffset)
        {
            layoutStateHistory = new history(layoutActions.none.ToString());
            layoutStateHistory.layoutActionState = layoutActions.none.ToString();
            layoutStateHistory.layoutCursor = Cursor;

            this._padding.padding = 100;
            this.Location = new Point(xOffset, yOffset);
            
            _layoutArea.BackColor = Color.DarkGray;
            this.Controls.Add(_layoutArea);
            

            _layoutArea.Paint += _layoutArea_Paint;

            this.AutoScroll = true;

            this._layoutArea.AllowDrop = true;
            this._layoutArea.DragDrop += _layoutArea_DragDrop;
            this._layoutArea.DragEnter += _layoutArea_DragEnter;

            this._layoutArea.MouseMove += _layoutArea_MouseMove;
            this._layoutArea.MouseDown += _layoutArea_MouseDown;
            this._layoutArea.MouseUp += _layoutArea_MouseUp;

               
        }
        private void _setCurrentLayoutActionAndCursor()
        {
            this.currentLayoutAction = (layoutActions)Enum.Parse(typeof(layoutActions), layoutStateHistory.layoutActionState);
            Cursor = layoutStateHistory.layoutCursor;
        }

        private void _storeAndSetCurrentLayoutActionAndCursor(Cursor newCursor, layoutActions newAction)
        {
            layoutStateHistory.layoutCursor = Cursor;
            layoutStateHistory.layoutActionState = this.currentLayoutAction.ToString();
            this.currentLayoutAction = newAction;
            Cursor = newCursor;
        }

        public Boolean keyUpHandled(object sender, KeyEventArgs e)
        {
            Boolean handleKey = false;
            switch (e.KeyData)
            {
                case Keys.Menu:
                    if ((_selectedItem != null) && (_selectedItem.contains(PointToClient(MousePosition))))
                    {
                        handleKey = true;
                        _setCurrentLayoutActionAndCursor();
                    }
                    break;
                case Keys.Space:
                    if (this.currentLayoutAction == layoutActions.layoutAreaPan)
                    {
                        _setCurrentLayoutActionAndCursor();
                        handleKey = true;
                    }
                    break;
            }
            
            return (handleKey);
        }

        public Boolean keyDownHandled(object sender, KeyEventArgs e)
        {
            Boolean handleKey = false;
            switch (e.KeyCode)
            {
                case Keys.Menu:
                    if ((_selectedItem != null) && (_selectedItem.contains(PointToClient(MousePosition))))
                    {
                        handleKey = true;
                        _storeAndSetCurrentLayoutActionAndCursor(Cursors.Hand,layoutActions.imagePan);
                    }
                    break;
                case Keys.Space:
                    if (this.currentLayoutAction == layoutActions.none)
                    {
                        _storeAndSetCurrentLayoutActionAndCursor(Cursors.Hand, layoutActions.layoutAreaPan);
                        handleKey = true;
                    }
                    break;
            }

            return (handleKey);
        }
        

        void _layoutArea_MouseUp(object sender, MouseEventArgs e)
        {

            if (_selectedItem != null)
            {
                List<placeableItem> selectedItems = _getSelectedItems();
                foreach (placeableItem selectedItem in selectedItems)
                {
                    if (selectedItem.itemType == placeableItem.itemTypes.image)
                    {
                        selectedItem.mouseUp();
                        //((baseImage)selectedItem).mouseUp(_autoFitImages);
                    }
                    else
                    {
                        selectedItem.mouseUp();
                    }
                }
              
            }

            currentLayoutAction = layoutActions.none;
            //_groupingBoxResize = false;

            _layoutArea.Refresh();

        }

        void _layoutArea_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDownPosition = e.Location;

            if ((_showGroupingBox) && (_GroupingBox.hover(e.Location)))
            {
                this.currentLayoutAction = layoutActions.groupResize;
                //_groupingBoxResize = true;
                _groupingBoxResizeDirection = _GroupingBox.direction;
            }

            placeableItem previousSelectedItem = null;

            if (!_showGroupingBox)
            {
                if (_selectedItem != null)
                    previousSelectedItem = _selectedItem;
            }
           

            List<placeableItem> containingItems = new List<placeableItem>();
          
            foreach (placeableItem placedItem in _pageMatFrame.placedItems)
            {
                if (placedItem.contains(new Point(e.X, e.Y)))
                    containingItems.Add(placedItem);
            }
            if (containingItems.Count > 1)
            {
                containingItems.Sort(new zComparer());  //images can overlap, but the mouse click should only go to the top level image/graphic
            }
            if (containingItems.Count > 0)  //if this is not true than nothing was selected
            {
                _selectedItem = containingItems[0];
                if (Control.MouseButtons == System.Windows.Forms.MouseButtons.Right)
                {
                    //?????? should I put this here
                    _showGroupingBox = false;
                    /*if (_selectedItem.selected == false)
                    {
                        _unselectAll();
                        _selectedItem.selectItem(e, false);
                    }*/
                    if (_selectedItem.selected)
                        this.showContextMenu(_layoutArea.PointToScreen(e.Location));
                }
                else
                {
                   
                    if (Control.ModifierKeys == Keys.Control)
                    {
                        //selecting more than one image
                        /*
                     
                         * is selected image already selected? then un-select it
                         * else -> select the image and add it to a list of selected images
                 
                         * 
                         * need to check if a single items is group selected and if it is then change it so it is not if only one is left
                        */
                        if (_selectedItem.selected)
                        {
                            _showGroupingBox = false;
                            _selectedItem.unselectItem();
                            List<placeableItem> selectedItems = _getSelectedItems();
                            if (selectedItems.Count == 1)
                            {
                                _selectedItem = selectedItems[0];
                                _selectedItem.selectItem(e, false);
                            }
                        }
                        else
                        {
                            //if a previous item is selected then change the previous item to a group select
                            if ((previousSelectedItem != null) && (previousSelectedItem != _selectedItem))
                            {
                                if (previousSelectedItem.selected)
                                    previousSelectedItem.selectItem(e, true);
                                    
                            }
                            if (previousSelectedItem == null) // only one item is being control clicked
                            {
                                _showGroupingBox = false;
                                _selectedItem.selectItem(e, false); 
                            }
                            else
                            {
                                _selectedItem.selectItem(e, true);
                                _showGroupingBox = true;
                                _createGroupingBox();
                            }
                        }
                    } //end CNTL Modifier Key
                    else //not adding images via CNTL Modifier
                    {
                        if (_selectedItem.groupSelect == false)
                        {
                            _showGroupingBox = false;
                            if ((previousSelectedItem != null) && (previousSelectedItem != _selectedItem))
                            {
                                previousSelectedItem.unselectItem();
                            }
                            if (Control.ModifierKeys == Keys.Alt)
                            {

                            }
                            _selectedItem.selectItem(e, false);
                        }
                        else 
                        {
                            _showGroupingBox = true;
                            _updateGroupSelectedMousePosition(e);
                        }
                    }
                } //end not right click
            }
            else //the click was not on any image
            {
                if ((_GroupingBox != null) && (_groupingRectangle.Contains(e.Location)))
                {
                    _showGroupingBox = true;
                    _updateGroupSelectedMousePosition(e);
                }
                else
                {
                    _showGroupingBox = false;
                    _GroupingBox = null;
                    List<placeableItem> selectedItems = _getSelectedItems();
                    foreach (placeableItem selectedItem in selectedItems)
                    {
                        selectedItem.unselectItem();
                    }

                    _selectedItem = null;
                }
            }
            this.Refresh();
        }

        void _updateGroupSelectedMousePosition(MouseEventArgs e)
        {
            List<placeableItem> selectedItems = _getSelectedItems();
            foreach (placeableItem selectedItem in selectedItems)
            {
                    selectedItem.selectItem(e, true); //need to update all grouped items with current mouseClick
            }
        }
        public void showContextMenu(Point mouseLocation)
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.ShowCheckMargin = false;
            contextMenu.ShowImageMargin = false;
            ToolStripMenuItem bringToFront = new ToolStripMenuItem("Bring To Front");
            ToolStripMenuItem bringForward = new ToolStripMenuItem("Bring Forward");
            ToolStripMenuItem sendBackwards = new ToolStripMenuItem("Send Backwards");
            ToolStripMenuItem sendToBack = new ToolStripMenuItem("Send To Back");
            contextMenu.Items.Add(bringToFront);
            contextMenu.Items.Add(bringForward);
            contextMenu.Items.Add(sendBackwards);
            contextMenu.Items.Add(sendToBack);
            contextMenu.Show(mouseLocation);

            bringToFront.Click += bringToFront_Click;
            bringForward.Click += bringForward_Click;
            sendBackwards.Click += sendBackwards_Click;
            sendToBack.Click += sendToBack_Click;
        }

        List<placeableItem> _getSelectedItems()
        {
            List<placeableItem> selectedItems = new List<placeableItem>();

            foreach (placeableItem placedItem in _pageMatFrame.placedItems)
            {
                if (placedItem.selected)
                    selectedItems.Add(placedItem);
            }

            return (selectedItems);
        }

        void _unselectAll()
        {
            foreach (placeableItem placedItem in _pageMatFrame.placedItems)
            {
                if (placedItem.selected)
                    placedItem.unselectItem();
            }
        }

        void sendToBack_Click(object sender, EventArgs e)
        {
            List<placeableItem> selectedItems = _getSelectedItems();
            foreach (placeableItem selectedItem in selectedItems)
            {
                int listIndex = _pageMatFrame.placedItems.IndexOf(selectedItem);
                _pageMatFrame.placedItems.RemoveAt(listIndex);
                _pageMatFrame.placedItems.Insert(0, selectedItem);
            }

            _layoutArea.Refresh();
        }

        void sendBackwards_Click(object sender, EventArgs e)
        {
            List<placeableItem> selectedItems = _getSelectedItems();
            foreach (placeableItem selectedItem in selectedItems)
            {
                int listIndex = _pageMatFrame.placedItems.IndexOf(selectedItem);
                if (listIndex != 0)
                {
                    _pageMatFrame.placedItems.RemoveAt(listIndex);
                    _pageMatFrame.placedItems.Insert(listIndex-1, selectedItem);
                }
            }

            _layoutArea.Refresh();
        }

        void bringForward_Click(object sender, EventArgs e)
        {
            List<placeableItem> selectedItems = _getSelectedItems();
            foreach (placeableItem selectedItem in selectedItems)
            {
                int listIndex = _pageMatFrame.placedItems.IndexOf(selectedItem);
                if (listIndex != (_pageMatFrame.placedItems.Count -1))
                {
                    _pageMatFrame.placedItems.RemoveAt(listIndex);
                    _pageMatFrame.placedItems.Insert(listIndex + 1, selectedItem);
                }
            }

            _layoutArea.Refresh();
        }

        void bringToFront_Click(object sender, EventArgs e)
        {
            List<placeableItem> selectedItems = _getSelectedItems();
            foreach (placeableItem selectedItem in selectedItems)
            {
                int listIndex = _pageMatFrame.placedItems.IndexOf(selectedItem);
                _pageMatFrame.placedItems.RemoveAt(listIndex);
                _pageMatFrame.placedItems.Insert(_pageMatFrame.placedItems.Count(), selectedItem);
            }

            _layoutArea.Refresh();
        }

        void _layoutArea_Paint(object sender, PaintEventArgs e)
        {
            _pageMatFrame.draw(e.Graphics);
            if (_showGroupingBox)
            {
                _drawGroupingBox(e.Graphics);
            }
        }

        void _createGroupingBox()
        {
            List<placeableItem> selectedItems = _getSelectedItems();
            int minX = 0;
            int minY = 0;
            int maxX = 0;
            int maxY = 0;
            int loopCounter = 0;
            foreach (placeableItem selectedItem in selectedItems)
            {
                if (loopCounter == 0)
                {
                    minX = selectedItem.location.X;
                    minY = selectedItem.location.Y;
                    maxX = selectedItem.location.X + selectedItem.size.Width;
                    maxY = selectedItem.location.Y + selectedItem.size.Height;
                }
                else
                {
                    if (selectedItem.location.X < minX)
                        minX = selectedItem.location.X;
                    if (selectedItem.location.Y < minY)
                        minY = selectedItem.location.Y;

                    if ((selectedItem.location.X + selectedItem.size.Width) > (maxX))
                        maxX = selectedItem.location.X + selectedItem.size.Width;
                    if ((selectedItem.location.Y + selectedItem.size.Height) > (maxY))
                        maxY = selectedItem.location.Y + selectedItem.size.Height;
                }
                //Console.WriteLine(String.Concat("Item #", loopCounter, " size - w:", selectedItem.size.Width, " h:", selectedItem.size.Height));
                loopCounter++;
            }
            _groupingRectangle = new Rectangle(new Point(minX, minY), new Size(maxX - minX, maxY - minY));
            //Console.WriteLine(String.Concat("GBLoc - (",_groupingRectangle.Location.X,",",_groupingRectangle.Location.Y,") : GBSize - w:",_groupingRectangle.Width," h:",_groupingRectangle.Height));
            _GroupingBox = new resizeSelectBox(_groupingRectangle, resizeSelectBox.boxTypes.resize);
        }
        void _drawGroupingBox(Graphics gfx)
        {
            _GroupingBox.draw(gfx);
        }

        void _layoutArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.currentLayoutAction == layoutActions.layoutAreaPan)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    Boolean updateScrollLayout = false;
                    if (this.VerticalScroll.Visible)
                    {
                        int deltaY = this.mouseDownPosition.Y - e.Location.Y;
                        
                        int scrollYValue = this.VerticalScroll.Value + deltaY;
                        
                        if (scrollYValue < 0)
                            scrollYValue = 0;
                        else
                            if (scrollYValue > this.VerticalScroll.Maximum)
                                scrollYValue = this.VerticalScroll.Maximum;

                        this.VerticalScroll.Value = scrollYValue;
                        updateScrollLayout = true;
                    }
                    if (this.HorizontalScroll.Visible)
                    {
                        int deltaX = this.mouseDownPosition.X - e.Location.X;

                        int scrollXValue = this.HorizontalScroll.Value + deltaX;

                        if (scrollXValue < 0)
                            scrollXValue = 0;
                        else
                            if (scrollXValue > this.HorizontalScroll.Maximum)
                                scrollXValue = this.HorizontalScroll.Maximum;

                        this.HorizontalScroll.Value = scrollXValue;
                        updateScrollLayout = true;
                     }
                    if (updateScrollLayout)
                        this.PerformLayout();
                }
            }

            
            else
            if (_selectedItem != null)
            {
                if (Control.MouseButtons == System.Windows.Forms.MouseButtons.Left)
                {
                    float deltaXPerc = 0;
                    float deltaYPerc = 0;

                    if (currentLayoutAction == layoutActions.groupResize)
                    {
                      switch (_groupingBoxResizeDirection)
                        {
                            case resizeSelectBox.resizeDirection.sE:
                                deltaXPerc = (float)(e.X - _groupingRectangle.Location.X)/(float)_groupingRectangle.Width;
                                deltaYPerc = (float)(e.Y - _groupingRectangle.Location.Y) / (float)_groupingRectangle.Height;
                                break;
                            case resizeSelectBox.resizeDirection.sW:
                                deltaXPerc = (float)((_groupingRectangle.Location.X + _groupingRectangle.Width) - e.X) / (float)_groupingRectangle.Width;
                                deltaYPerc = (float)(e.Y - _groupingRectangle.Location.Y) / (float)_groupingRectangle.Height;
                                break;
                            case resizeSelectBox.resizeDirection.nE:
                                deltaXPerc = (float)(e.X - _groupingRectangle.Location.X) / (float)_groupingRectangle.Width;
                                deltaYPerc = (float)((_groupingRectangle.Location.Y + _groupingRectangle.Height) - e.Y) / (float)_groupingRectangle.Height;
                                break;
                            case resizeSelectBox.resizeDirection.nW:
                                deltaXPerc = (float)((_groupingRectangle.Location.X + _groupingRectangle.Width) - e.X)/ (float)_groupingRectangle.Width;
                                deltaYPerc = (float)((_groupingRectangle.Location.Y + _groupingRectangle.Height) - e.Y)/ (float)_groupingRectangle.Height;
                                break;
                        }
                        Console.WriteLine(String.Concat(deltaXPerc, ",", deltaYPerc)); 
                    }
               
                    Boolean keepAspect = (Control.ModifierKeys == Keys.Shift) ? true : false;
                    //moving Image or resizeImage
                    List<placeableItem> selectedItems = _getSelectedItems();
                    foreach (placeableItem selectedItem in selectedItems)
                    {
                        if (currentLayoutAction != layoutActions.groupResize)// !_groupingBoxResize)
                            selectedItem.moveOrResizeItem(e);
                        else
                            selectedItem.resizeItem(_groupingBoxResizeDirection, deltaXPerc, deltaYPerc, keepAspect);
                    }

                    if (_showGroupingBox)
                        _createGroupingBox();
                   
                   _layoutArea.Refresh();
                } //end left mouse button down
                else
                {
                    Boolean checkItem = true;
                    if (_showGroupingBox)
                    {
                        if (_GroupingBox.hover(e.Location))
                        {
                            _layoutArea.Cursor = _GroupingBox.getCursor();
                            checkItem = false;
                            //Console.WriteLine(_GroupingBox.direction);
                        }
                        else // mouse move was outside of grouping Box
                        {
                            _layoutArea.Cursor = Cursors.Default;
                        }
                    }

                   
                    //let selected image check if anything happened
                    if (checkItem)
                        _selectedItem.mouseMoved(e);
                }
            }
        }

        public float scale
        {
            get { return (_zoomScale); }
        }

        void _layoutArea_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        void _layoutArea_DragDrop(object sender, DragEventArgs e)
        {
            this._pageMatFrame.imageFileDropped((String[])e.Data.GetData("FileName"),PointToClient(new Point(e.X,e.Y)),scale);
            
        }

        

       // protected abstract void droppedOnMatting();

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
        }
        
        public void intitialPlacePageMatFrame(basePageMatFrameArea pageMatFrameItem)
        {
            _clearBaseDesignArea();
            _pageMatFrame = pageMatFrameItem;
            _setInitialScale();
            _centerAndSizePageMatFrame();
        }

        private void _centerAndSizePageMatFrame()
        {
            float availableDesignArea_width = ((float)_layoutArea.Width) - (float)this._padding.xPadding;
            float availableDesignArea_height = ((float)_layoutArea.Height) - (float)this._padding.yPadding;

            float pageMatFrameWidth = _pageMatFrame.completeSizeF.Width * _initialScale * _zoom;
            float pageMatFrameHeight = _pageMatFrame.completeSizeF.Height * _initialScale * _zoom;

            int xOffset = Convert.ToInt16((availableDesignArea_width - pageMatFrameWidth)/2.0f);
            int yOffset = Convert.ToInt16((availableDesignArea_height - pageMatFrameHeight) / 2.0f);

            //should I zoom the padding??
            _pageMatFrame.location = new Point(_padding.left + xOffset, _padding.top + yOffset);
            _setPageMatFrameSize();
        }

        private void _setPageMatFrameSize()
        {
            _pageMatFrame.scale = _zoomScale;
        }

        private void _clearBaseDesignArea()
        {
            _pageMatFrame = null;
            _layoutArea.Controls.Clear();
            _fitDesignArea();
            referenceSize = new Size(this.Size.Width,this.Size.Height);
        }

        public void _fitToScreen()
        {
            this._setInitialScale();
            _centerAndSizePageMatFrame();
            referenceSize = new Size(this.Size.Width, this.Size.Height);
        }

        private void _setInitialScale()
        {
            float availableDesignArea_width = (float)(_layoutArea.Width - this._padding.xPadding) * _zoom;
            float availableDesignArea_height = (float)(_layoutArea.Height - this._padding.yPadding) * _zoom;

            float xScale = availableDesignArea_width / _pageMatFrame.completeSizeF.Width;
            float yScale = availableDesignArea_height / _pageMatFrame.completeSizeF.Height;

            _initialScale = (xScale < yScale) ? xScale : yScale;

            _zoomScale = _zoom * _initialScale;

        }

        protected override void OnParentChanged(EventArgs e)
        {
            if (this.Parent != null)
                this.Parent.Resize += Parent_Resize;
            base.OnParentChanged(e);
        }

       
        void Parent_Resize(object sender, EventArgs e)
        {
            Size newUseableSize = new Size(this.Parent.ClientSize.Width - this.Location.X, this.Parent.ClientSize.Height - this.Location.Y);
            int newWidth = this._padding.xPadding + this._pageMatFrame.size.Width;
            int newHeight = this._padding.yPadding + this._pageMatFrame.size.Height;
            if (newWidth < newUseableSize.Width)
                newWidth = newUseableSize.Width;
            if (newHeight < newUseableSize.Height)
                newHeight = newUseableSize.Height;
         
            this.Size = newUseableSize;
            this._layoutArea.Size = new Size(newWidth, newHeight);
            this._centerAndSizePageMatFrame();


        }

       

        public void _fitDesignArea()
        {
            //sets basePanel Size to viewableArea, sets matting size to the same size
            this.Size = new Size(this.Parent.ClientSize.Width - this.Location.X, this.Parent.ClientSize.Height - this.Location.Y);
            _layoutArea.Size = new Size(this.Size.Width, this.Size.Height);
        }

        public void zoomChanged(float zoomValue)
        {
            //in reality the design area will only change when the window is resized so that the screen area with items overflows
            //the design area will not be less than the window size
            //the items attached to it will scale
            _zoom = zoomValue;
            //these equations make sure that the padding stays constant through Zoom
            int newWidth = Convert.ToInt16((float)(this.referenceSize.Width - this._padding.xPadding) * _zoom) + this._padding.xPadding;
            int newHeight = Convert.ToInt16((float)(this.referenceSize.Height - this._padding.yPadding) * _zoom) + this._padding.yPadding;
            Boolean centerX = false;
            Boolean centerY = false;
            if (newWidth < this.Width)
            {
                newWidth = this.Width;
                centerX = true;
            }
            if (newHeight < this.Height)
            {
                newHeight = this.Height;
                centerY = true;
            }
            _layoutArea.Size = new Size(newWidth, newHeight);
            
            _zoomScale = _zoom * _initialScale;
            
            if ((centerX == false) || (centerY == false))
               _setPageMatFrameSize();
            else
                _centerAndSizePageMatFrame();

            //_zoomScale = _zoom * _initialScale;

        }
        
    }
}
