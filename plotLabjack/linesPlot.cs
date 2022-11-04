using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plotLabjack
{
    internal class LinesPlot
    {
        private bool showLine1 { get; set; }
        private bool showLine2 { get; set; }
        private bool showLine3 { get; set; }
        private bool showLine4 { get; set; }

        public LinesPlot()
        {
            showLine1 = true;
            showLine2= true;
            showLine3 = true;
            showLine4 = true;
        }

        public void disable()
        {
            showLine1 = false;
            showLine2 = false;
            showLine3 = false;
            showLine4 = false;
        }

        public bool ToggleSingleLine(int channel)
        {
            switch (channel)
            {
                case 1: showLine1 = !showLine1; return showLine1;
                case 2: showLine1 = !showLine2; return showLine2;
                case 3: showLine1 = !showLine3; return showLine3;
                case 4: showLine1 = !showLine4; return showLine4;
                default:
                    return false;
            }
        }
    }
}
