using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace layoutApp
{
    // this is the base class for all mats, frames, albums as a whole product
   abstract class baseProduct
    {
        public enum layoutProjectType { Album, Mat, Product };

        protected layoutProjectType _type;

        protected productSaveClass _pSC = new productSaveClass();

        protected UserControl _productControlBar = new UserControl();
        protected zoomControl _zoomControlBar = new zoomControl();
        protected String _productName;
        protected List<basePageMatFrameArea> _pagesMatsFrames = new List<basePageMatFrameArea>();

        protected baseLayoutArea _layoutArea = null;
        protected Form _mainForm = null;

        protected basePageMatFrameArea _currentPageMatFrame;

        public baseProduct(Form mainForm)
        {
            _initialize(mainForm);

            _addNewPageMatFrame();
            _layoutArea.intitialPlacePageMatFrame(_currentPageMatFrame);
        }

        public baseProduct(Form mainForm, productSaveClass psc)
        {
            _initialize(mainForm);
            foreach (pageMatFrameSaveClass content in psc.contents)
            {
                _loadPageMatFrame(content);
                //temp lines
                _currentPageMatFrame = _pagesMatsFrames[0];
                _layoutArea.intitialPlacePageMatFrame(_currentPageMatFrame);
                //need to somehow load items without haveing to display them until the page is active
                _currentPageMatFrame.loadItems(content);
            }
            if (_pagesMatsFrames.Count > 0)
            {
                _currentPageMatFrame = _pagesMatsFrames[0];
                _layoutArea.intitialPlacePageMatFrame(_currentPageMatFrame);
                _layoutArea.Refresh();
            }
        }

        private void _initialize(Form mainForm)
        {
            this._mainForm = mainForm;
            this._addProductControlBar();
            this._addZoomControlBar();
            this._addLayoutArea();
            this._zoomControlBar.zoomChanged += _zoomControlBar_zoomChanged;
            this._zoomControlBar.fitToScreen += _zoomControlBar_fitToScreen;
        }

        protected abstract void _loadPageMatFrame(pageMatFrameSaveClass pmfSC);
        protected abstract void _addNewPageMatFrame();

        void _zoomControlBar_fitToScreen(object sender, fitToScreenEventArgs e)
        {
            this._layoutArea._fitToScreen();
        }

        public Boolean keyUpHandled(object sender, KeyEventArgs e)
        {
            Boolean handleKey = this._layoutArea.keyUpHandled(sender, e);

            return (handleKey);
        }
        public Boolean keyDownHandled(object sender, KeyEventArgs e)
        {
            Boolean handleKey = this._layoutArea.keyDownHandled(sender, e);

            return (handleKey);
        }
        
        void _zoomControlBar_zoomChanged(object sender, zoomEventArgs e)
        {
            this._layoutArea.zoomChanged(e.zoom);
        }

        public int xOffset
        {
            get { return (this._calculateFormXOffset()); }
        }

        protected virtual int _calculateFormXOffset()
        {
            return (this._zoomControlBar.Width);
        }

        public int yOffset
        {
            get { return (this._calculateFormYOffset()); }
        }

        protected virtual int _calculateFormYOffset()
        {
            return (this._mainForm.MainMenuStrip.Height + this._productControlBar.Height);
        }

        protected abstract void _createProductControlBar();

        private void _addZoomControlBar()
        {
            this._zoomControlBar.Location = new Point(0, yOffset);
            this._mainForm.Controls.Add(this._zoomControlBar);
        }

        private void _addProductControlBar()
        {
            this._createProductControlBar();
            this._mainForm.Controls.Add(this._productControlBar);
        }
       
        private void _addLayoutArea()
        {
            this._layoutArea = new baseLayoutArea(xOffset,yOffset);
            this._mainForm.Controls.Add(this._layoutArea);
            this._layoutArea._fitDesignArea();
        }

        public virtual void draw()
        {
           this._productControlBar.Refresh();
           this._layoutArea.Refresh();
        }

        public Form mainForm
        {
            get { return (this._mainForm); }
        }

        public virtual productSaveClass save()
        {
            _pSC.projectName = _productName;
            _pSC.projectType = _type.ToString(); ;

            foreach (basePageMatFrameArea pageMatFrame in _pagesMatsFrames)
            {
                _pSC.contents.Add(pageMatFrame.save());
            }

            return (_pSC);
        }

    }
}
