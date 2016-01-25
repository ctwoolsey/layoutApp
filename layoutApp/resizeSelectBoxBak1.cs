using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace layoutApp
{
    public class resizeSelectBox
    {
        public enum resizeDirection { nW, nE, sW, sE, none };
        Color resizeBoxColor = Color.White;
        Pen resizeBoxPen;
        SolidBrush resizeBoxBrush;

        placeableItem _container = null;
        Rectangle _rectangle;
        Rectangle nW;
        Rectangle nE;
        Rectangle sW;
        Rectangle sE;
        Rectangle[] grabberRects;
        Rectangle bounding;

        Size grabberSize = new Size(10, 10);
        
        private resizeDirection _direction;

        public enum boxTypes { resize, selected }
        private boxTypes _boxType;

        public resizeSelectBox(Object containerItem, boxTypes boxType)
        {
            if (containerItem is placeableItem)
                _container = (placeableItem)containerItem;
            else
                if (containerItem is Rectangle)
                    _rectangle = (Rectangle)containerItem;

            _boxType = boxType;
            resizeBoxPen = new Pen(resizeBoxColor, 3.0f);
            resizeBoxBrush = new SolidBrush(resizeBoxColor);
        }

        private void createRectangles()
        {
            if (_container != null)
                bounding = _container.bounds;
            else
                bounding = _rectangle;

            if (_boxType == boxTypes.resize)
            {
                nW = new Rectangle(new Point(bounding.X, bounding.Y), grabberSize);
                nE = new Rectangle(new Point(bounding.X + (bounding.Width - grabberSize.Width), bounding.Y), grabberSize);
                sW = new Rectangle(new Point(bounding.X, bounding.Y + (bounding.Height - grabberSize.Height)), grabberSize);
                sE = new Rectangle(new Point(bounding.X + (bounding.Width - grabberSize.Width), bounding.Y + (bounding.Height - grabberSize.Height)), grabberSize);
                grabberRects = new Rectangle[] { nW, nE, sW, sE };
            }
        }

      
        public Boolean hover(Point mouseLocation)
        {
            Boolean isHovering = false;
            _direction = resizeDirection.none;

            if (nW.Contains(mouseLocation))
            {
                isHovering = true;
                _direction = resizeDirection.nW;
            }
            else if (nE.Contains(mouseLocation))
            {
                isHovering = true;
                _direction = resizeDirection.nE;
            }
            else if (sW.Contains(mouseLocation))
            {
                isHovering = true;
                _direction = resizeDirection.sW;
            }
            else if (sE.Contains(mouseLocation))
            {
                isHovering = true;
                _direction = resizeDirection.sE;
            }

      
            return (isHovering);
        }

        public Cursor getCursor()//getCursor(Point mouseLocation)
        {
            Cursor returnCursor = Cursors.Default;
            
            if ((_direction == resizeDirection.nW) || (_direction == resizeDirection.sE))
                returnCursor = Cursors.SizeNWSE;
            if ((_direction == resizeDirection.nE) || (_direction == resizeDirection.sW))
                returnCursor = Cursors.SizeNESW;

            return (returnCursor);
        }

        public resizeDirection direction
        {
            get { return _direction; }
        }

        public void draw(Graphics gfx)
        {
            createRectangles();
            if (_container != null)
                gfx.DrawRectangle(resizeBoxPen, _container.bounds);
            else
                gfx.DrawRectangle(resizeBoxPen, _rectangle);
         
            if (_boxType == boxTypes.resize)
                gfx.FillRectangles(resizeBoxBrush,grabberRects);
        }


    }
}
