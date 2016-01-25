using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;

namespace layoutApp
{
    public partial class mainForm : Form
    {
        //private enum layoutProjectType { Album, Mat, Product };

        private String projectFile = null;
        private baseProduct _product = null;

        private Boolean _keyDownHandled = false;

        public mainForm()
        {
            InitializeComponent();
            this.ResizeRedraw = true;
        }

        private void _loadProject(baseProduct.layoutProjectType projectType)
        {
            switch (projectType)
            {
                case baseProduct.layoutProjectType.Album:
                    this._product = new albumProduct(this);    
                    break;
                case baseProduct.layoutProjectType.Mat:
                    break;
                case baseProduct.layoutProjectType.Product:
                    break;
                default:

                    break;
            }
        }

        private void originalAlbumToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this._loadProject(baseProduct.layoutProjectType.Album);
        }

        private void mainForm_Layout(object sender, LayoutEventArgs e)
        {
            if (this._product != null)
                this._product.draw();
        }

       

        private void mainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (this._product != null)
            {
                if (this._product.keyDownHandled(sender, e))
                    e.Handled = true;
            }
            
        }

        private void mainForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (this._product != null)
            {
                if (this._product.keyUpHandled(sender, e))
                    e.Handled = true;
            }
        }

        
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_product != null)
            {
                if (projectFile == null)
                {
                    SaveFileDialog sfD = new SaveFileDialog();
                    sfD.Filter = "layout app project|*.lpf";
                    sfD.Title = "Save Project";
                    sfD.OverwritePrompt = true;
                    sfD.ShowDialog();

                    if (sfD.FileName != "")
                    {
                        projectFile = sfD.FileName;
                        _writeXML(sfD.FileName);
                    }
                }
                else
                {
                    _writeXML(projectFile);
                }
            }
        }

        private void _writeXML(String fileName)
        {
            productSaveClass psc = _product.save();
            XmlSerializer x = new XmlSerializer(psc.GetType());
            TextWriter writer = new StreamWriter(fileName);
            x.Serialize(writer, psc);
            writer.Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private productSaveClass openProduct(productSaveClass psc)
        {
            productSaveClass returnValue = null;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.AddExtension = true;
            ofd.CheckFileExists = true;
            ofd.DefaultExt = "lpf";
            ofd.Filter = "layout app project|*.lpf";
            ofd.Title = "Open Project";

            ofd.ShowDialog();

            if (ofd.FileName != "")
            {              
                XmlSerializer x = new XmlSerializer(psc.GetType());
                FileStream myFileStream = new FileStream(ofd.FileName, FileMode.Open);
                psc = (productSaveClass)x.Deserialize(myFileStream);
                myFileStream.Close();
                projectFile = ofd.FileName;
                returnValue = psc;
            }

            return (returnValue);
        }

        private void albumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            albumProductSaveClass apsc = new albumProductSaveClass();
            var openedClass = openProduct(apsc);
            if (openedClass != null)
            {
                apsc = (albumProductSaveClass)openedClass;
                this._product = new albumProduct(this,apsc);
            }
            
        }


    }
}
