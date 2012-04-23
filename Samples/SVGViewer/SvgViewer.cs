using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Svg;
using Svg.Transforms;

namespace SVGViewer
{
    public partial class SVGViewer : Form
    {
        private string currentFile;

        public SVGViewer()
        {
            InitializeComponent();
            currentFile = null;
        }

        private void openFile(string fileName)
        {
            var displaySize = imageBox.Size;
            
            SvgDocument svgDoc = SvgDocument.Open(openSvgFile.FileName);
            var svgSize = svgDoc.GetDimensions();

            if (svgSize.Width == 0)
            {
                throw new Exception("SVG does not have size specified. Cannot work with it.");
            }

            var displayProportion = (displaySize.Height * 1.0f) / displaySize.Width;
            var svgProportion = svgSize.Height / svgSize.Width;

            float scalingFactor = 0f;
            int padding = 10; // screen pixels

            // if display is proportionally narrower than svg
            if (displayProportion > svgProportion)
            {
                // we pick the width of display as max and compute the scaling against that.
                scalingFactor = (( displaySize.Width - padding * 2 )  * 1.0f) / svgSize.Width;
            }
            else
            {
                // we pick the height of display as max and compute the scaling against that.
                scalingFactor = ((displaySize.Height - padding * 2) * 1.0f) / svgSize.Height;
            }

            if (scalingFactor < 0)
            {
                throw new Exception("Viewing area is too small to render the image");
            }
            
            svgDoc.Transforms = new SvgTransformCollection();
            svgDoc.Transforms.Add(new SvgScale(scalingFactor));
            svgDoc.Transforms.Add(new SvgTranslate(padding, padding));

            //svgDoc.Width = new SvgUnit(svgDoc.Width.Type, svgDoc.Width * factor);
            //svgDoc.Height = new SvgUnit(svgDoc.Height.Type, svgDoc.Height * factor);

            imageBox.Image = svgDoc.Draw();
        }


        private void open_Click(object sender, EventArgs e)
        {
            if (openSvgFile.ShowDialog() == DialogResult.OK)
            {
                currentFile = openSvgFile.FileName;
                openFile(currentFile);
            }
        }

        private void imageBox_SizeChanged(object sender, EventArgs e)
        {
            if (currentFile != null)
            {
                openFile(currentFile);
            }
        }
    }
}
