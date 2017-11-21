using System;
using System.Collections.Generic;
using System.Drawing;
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
            private readonly Pen availablePen = Pens.Green;
            private readonly Pen missingPen = Pens.Red;
            private readonly Pen processingPen = Pens.Yellow;

            private readonly PartFile file;
            private readonly Dictionary<PartFile.EPartStatus, Pen> penChoice;

            /// <inheritdoc />
            /// <summary>
            /// Constructs new ProgressViewer for visualisation of specified PartFile
            /// </summary>
            /// <param name="file">The PartFile which progress to visualize</param>
            public ProgressViewer(PartFile file)
            {
                InitializeComponent();
                DoubleBuffered = true;
                Paint += ProgressViewer_Paint;
                this.file = file;

                penChoice = new Dictionary<PartFile.EPartStatus, Pen>
                {
                    { PartFile.EPartStatus.Available, availablePen },
                    { PartFile.EPartStatus.Missing, missingPen },
                    { PartFile.EPartStatus.Processing, processingPen }
                };
            }



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
