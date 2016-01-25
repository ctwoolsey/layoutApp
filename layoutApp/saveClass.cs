using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace layoutApp
{
   
    public class albumProductSaveClass : productSaveClass
    {
        public SizeF coverSize;
    }

    public class productSaveClass
    {
        public String projectName;
        public List<pageMatFrameSaveClass> contents = new List<pageMatFrameSaveClass>();
        public String projectType;
    }

    public class pageMatFrameSaveClass
    {
        public String pageMatFrameType;
        public SizeF completedSize;
        public float bleed;
        public float margin;
        public String backColor;


        public List<imageSaveClass> placeableItems = new List<imageSaveClass>();
    }

    public class imageSaveClass : itemSaveClass
    {
        public String fileName;
        public float originalAspectRatio;
    }
    
    public class itemSaveClass
    {
        public String itemType;
        public int zOrder;
        public SizeF windowSize;
        public PointF windowLocation;
        public SizeF itemSize;
        public PointF itemLocation;
    }
}
