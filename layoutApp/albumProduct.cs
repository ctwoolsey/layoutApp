using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace layoutApp
{
    class albumProduct : baseProduct
    {
        private SizeF _CoverSize;
        
        public albumProduct(Form mainForm) : base(mainForm)
        {
            _initialize();
        }

        public albumProduct(Form mainForm, albumProductSaveClass apsc) : base(mainForm, apsc)
        {
            _initialize();
            _CoverSize = apsc.coverSize;
        }

        private void _initialize()
        {
            _type = layoutProjectType.Album;
        }

        protected override void _createProductControlBar()
        {
            //required function called by the baseClass
            this._productControlBar = new albumControlBar(this._mainForm);
        }

        protected override void _loadPageMatFrame(pageMatFrameSaveClass pmfSC)
        {
            basePageMatFrameArea pageMatFrameArea = new pageArea(_layoutArea,_layoutArea.scale, pmfSC);
           // pageMatFrameArea.add(_layoutArea);
            _pagesMatsFrames.Add(pageMatFrameArea);
           // pageMatFrameArea.loadItems(pmfSC);
        }

        protected override void _addNewPageMatFrame()
        {
            _currentPageMatFrame = new pageArea(_layoutArea.scale);
            _currentPageMatFrame.add(_layoutArea);
            _pagesMatsFrames.Add(_currentPageMatFrame); 
        }

        public override productSaveClass save()
        {
            _pSC = new albumProductSaveClass();
            //albumProductSaveClass psc = (albumProductSaveClass)base.save();
            ((albumProductSaveClass)_pSC).coverSize = _CoverSize;
            base.save();
            return (_pSC);
        }

      
    }
}
