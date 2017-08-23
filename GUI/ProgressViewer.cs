using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using EzShare.ModelLib;

namespace EzShare
{
    namespace GUI
    {
        /// <summary>
        /// Box for visualisation of progress of PartFile
        /// </summary>
        public partial class ProgressViewer : UserControl
        {
            /// <summary>
            /// Constructs new ProgressViewer for visualisation of specified PartFile
            /// </summary>
            /// <param name="file">The PartFile which progress to visualize</param>
            public ProgressViewer(PartFile file)
            {
                InitializeComponent();
                DoubleBuffered = true;
                this.Paint += ProgressViewer_Paint;
                this.file = file;

                penChoice = new Dictionary<PartFile.ePartStatus, Pen>();

                penChoice.Add(PartFile.ePartStatus.Available, availablePen);
                penChoice.Add(PartFile.ePartStatus.Missing, missingPen);
                penChoice.Add(PartFile.ePartStatus.Processing, processingPen);
            }

            Pen availablePen = Pens.Green;
            Pen missingPen = Pens.Red;
            Pen processingPen = Pens.Yellow;

            PartFile file;
            Dictionary<PartFile.ePartStatus, Pen> penChoice;

            /// <summary>
            /// Draws vertical line of proper color to visualise progress
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void ProgressViewer_Paint(object sender, PaintEventArgs e)
            {
                long count = file.NumberOfParts;
                float oneWidth =  (float)count / Size.Width;

                for (long i = 0; i < Size.Width; ++i)
                {
                    //approximately determines which part to show
                    e.Graphics.DrawLine(penChoice[file.PartStatus[(long)oneWidth*i]], i, 0, i, Size.Height);
                }
            }
        }


    }
}
