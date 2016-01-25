using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace layoutApp
{
    abstract class basePageMatFrameArea
    {
        public enum pageMatFrameType { page, mat, frame };
        protected SizeF _completePageMatFrameSizeF; //includes the bleed
        
        protected float _bleed = 0.0f;
        protected float _margin = 0.125f;
        protected float _scale = 1.0f;
        //protected List<baseImage> _images = new List<baseImage>();
        protected List<placeableItem> _placedItems = new List<placeableItem>();
        protected pageMatFrameType _type;
      
        //actual pixel sizes to mimic Rectangle
        protected Color _backColor = Color.LightGray;
        protected Point _location = new Point(0,0);
        protected Size _size = new Size(0,0);
        protected Panel _parent;
        
        public basePageMatFrameArea(float scale)
        {
            this._scale = scale;
            _completePageMatFrameSizeF = new SizeF(24.0f, 12.0f);
            _setPageMatFrameType();
        }

        public basePageMatFrameArea(Panel parentPanel, float scale, pageMatFrameSaveClass pmfSC)
        {
            this._parent = parentPanel;
            this._scale = scale;
            _completePageMatFrameSizeF = pmfSC.completedSize;
            _setPageMatFrameType();
            _backColor = Color.FromName(pmfSC.backColor);
            _bleed = pmfSC.bleed;
            _margin = pmfSC.margin;

           
        }

        public void loadItems(pageMatFrameSaveClass pmfSC)
        {
            foreach (itemSaveClass iSC in pmfSC.placeableItems)
            {
                switch ((placeableItem.itemTypes)(Enum.Parse(typeof(placeableItem.itemTypes), iSC.itemType)))
                {
                    case placeableItem.itemTypes.image:
                        _placeImage(_scale, (imageSaveClass)iSC);
                        break;
                }


            }
        }

        public void add(Panel panelToAddTo)
        {
            this._parent = panelToAddTo;
        }

        public Color backColor
        {
            get { return (this._backColor); }
            set
            {
                this._backColor = value;
                this.draw();
            }
        }
        protected abstract void _setPageMatFrameType();

        public Point location
        {
            get { return (this._location); }
            set { this._location = value; }
        }

        public Size size
        {
            get { return (this._size); }
        }

        public float scale
        {
            set
            {
                int width = Convert.ToInt16(this.completeSizeF.Width * value);
                int height = Convert.ToInt16(this.completeSizeF.Height * value);
                this._size = new Size(width, height);

                foreach (placeableItem placedItem in _placedItems)
                {
                    placedItem.scale(value, this.location);
                }

                this._parent.Refresh();
            }
        }
        /* 
                public Boolean pointIncluded(Point pointToTest)
                {
            
                    return (this.bounds().Contains(pointToTest));
                }

      
                public Point getRelativePoint(Point absolutePoint)
                {
                    int x = absolutePoint.X - this.location.X;
                    int y = absolutePoint.Y - this.location.Y;

                    return (new Point(x, y));
                }*/
       


        public SizeF completeSizeF
        {
            get { return (_completePageMatFrameSizeF); }
            set { _completePageMatFrameSizeF = value; }
        }

        public void imageFileDropped(String[] fileNames, Point mouseLocation, float scale)
        {
             this._dropNewImages(fileNames, mouseLocation, scale);
        }

        protected abstract void _placeImage(float scale, imageSaveClass iSC);

        protected abstract void _dropNewImages(String[] fileNames, Point mouseLocation,float scale);

        public virtual void removeItem(placeableItem itemToRemove)
        {
            int zIndexOfRemoved = itemToRemove.zOrder;
            placedItems.Remove(itemToRemove);
            foreach (placeableItem placedItem in this.placedItems)
            {
                if (placedItem.zOrder > zIndexOfRemoved)
                    placedItem.zOrder--;
            }
        }

        public List<placeableItem> placedItems
        {
            get { return (_placedItems); }
        }

        public Rectangle bounds()
        {
            return (new Rectangle(this._location,this._size));
        }

        public void outputSaveData()
        {

        }

        public void draw()
        {
            Graphics gfx = this._parent.CreateGraphics();
            this.draw(gfx);
        }

        public void draw(Graphics gfx)
        {
            gfx.FillRectangle(new SolidBrush(this._backColor), this.bounds());
            foreach (placeableItem placedItem in _placedItems)
            {
                placedItem.draw(gfx);
            }       
        }

        public virtual pageMatFrameSaveClass save()
        {
            pageMatFrameSaveClass pmfSC = new pageMatFrameSaveClass();
            pmfSC.pageMatFrameType = _type.ToString();
            pmfSC.completedSize = completeSizeF;
            pmfSC.bleed = _bleed;
            pmfSC.margin = _margin;
            pmfSC.backColor = _backColor.Name.ToString();

            foreach (placeableItem placedItem in _placedItems)
            {
                if (placedItem.itemType == placeableItem.itemTypes.image)  
                    pmfSC.placeableItems.Add((imageSaveClass)placedItem.save());
            }

            return (pmfSC);
        }
    }
}
