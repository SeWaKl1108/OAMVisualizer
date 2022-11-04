//#define INITVALUE

using ScottPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timers = System.Timers;
using System.Diagnostics;
using ScottPlot.Plottable;
using LabJack.LabJackUD;
using System.Globalization;
using System.IO;


namespace plotLabjack
{


    public partial class Form1 : Form
    {



        Timers.Timer aTimer = null;
        Timers.Timer bTimer = null;

        fileWriter write = null;
        aws awsStream = null;
        labJack labjack = null;

        LinesPlot lines = null;
        Values adVal = null;

        int NextPointIndex = 0;

        Random rand = new Random(0);

        SignalPlot SignalPlot1 = null;
        SignalPlot SignalPlot2 = null;
        SignalPlot SignalPlot3 = null;
        SignalPlot SignalPlot4 = null;

        bool plotSignal1 = true;
        bool plotSignal2 = true;
        bool plotSignal3 = true;
        bool plotSignal4 = true;

        public IPlottable Plottable;
        public Form1()
        {
            InitializeComponent();

            labjack = new labJack();
            awsStream = new aws();
            write = new fileWriter(@"abd", @"dfc");

            SetTimer();

            lines = new LinesPlot();
#if INITVALUE
            adVal = new Values(100);
#else
            adVal = new Values();
#endif
            
            defaultSignalPlot();

            formsPlot1.RightClicked -= formsPlot1.DefaultRightClickEvent;
            formsPlot1.RightClicked += new EventHandler((sender, e) => CustomRightClickEvent(sender, e));

            formsPlot1.Configuration.RightClickDragZoom = false;
            formsPlot1.Configuration.ScrollWheelZoom = false;

            errorBox.Text = LJUD.GetDriverVersion().ToString();

            // Experiment enabled
            startExperiment.Enabled = true;
            stopExperiment.Enabled = true;

            //awsStream.Connect("labjackDevice");

        }

        private void defaultSignalPlot()
        {
            formsPlot1.Plot.Title("4-Channel-Plot");
            SignalPlot1 = formsPlot1.Plot.AddSignal(adVal.AdValues1);
            SignalPlot2 = formsPlot1.Plot.AddSignal(adVal.AdValues2);
            SignalPlot3 = formsPlot1.Plot.AddSignal(adVal.AdValues3);
            SignalPlot4 = formsPlot1.Plot.AddSignal(adVal.AdValues4);
#if INITVALUE
            formsPlot1.Plot.SetAxisLimits(0, 100, 0, 10);
#else
            formsPlot1.Plot.SetAxisLimits(0, 100, 0, 1);
#endif

            formsPlot1.Render();
        }

        private void CustomRightClickEvent(object sender, EventArgs e)
        {

            ContextMenuStrip customMenu = new ContextMenuStrip();
            customMenu.Items.Add(new ToolStripMenuItem("Add/ Disable Channel 1", null, new EventHandler(toggleChannel1)));
            customMenu.Items.Add(new ToolStripMenuItem("Add/ Disable Channel 2", null, new EventHandler(toggleChannel2)));
            customMenu.Items.Add(new ToolStripMenuItem("Add/ Disable Channel 3", null, new EventHandler(toggleChannel3)));
            customMenu.Items.Add(new ToolStripMenuItem("Add/ Disable Channel 4", null, new EventHandler(toggleChannel4)));
            //customMenu.Items.Add(new ToolStripMenuItem("Clear Plot", null, new EventHandler(clearPlot)));
            customMenu.Show(Form1.MousePosition.X, Form1.MousePosition.Y);
        }

        private void toggleChannel1(object sender, EventArgs e)
        {

            if (plotSignal1 == true)
            {
                SignalPlot1.IsVisible = false;
                plotSignal1 = false;
            }
            else
            {
                SignalPlot1.IsVisible = true;
                plotSignal1 = true;
            }

            //formsPlot1.Plot.Clear();
            //formsPlot1.Plot.AxisAuto();
            formsPlot1.Render();
            
        }



        private void toggleChannel2(object sender, EventArgs e)
        {
            if (plotSignal2 == true)
            {
                SignalPlot2.IsVisible = false;
                plotSignal2 = false;
            }
            else
            {
                SignalPlot2.IsVisible = true;
                plotSignal2 = true;
            }

            //formsPlot1.Plot.Clear();
            //formsPlot1.Plot.AxisAuto();
            formsPlot1.Render();
        }

        private void toggleChannel3(object sender, EventArgs e)
        {
            if (plotSignal3 == true)
            {
                SignalPlot3.IsVisible = false;
                plotSignal3 = false;
            }
            else
            {
                SignalPlot3.IsVisible = true;
                plotSignal3 = true;
            }

            //formsPlot1.Plot.Clear();
            //formsPlot1.Plot.AxisAuto();
            formsPlot1.Render();
        }

        private void toggleChannel4(object sender, EventArgs e)
        {
            if (plotSignal4 == true)
            {
                SignalPlot4.IsVisible = false;
                plotSignal4 = false;
            }
            else
            {
                SignalPlot4.IsVisible = true;
                plotSignal4 = true;
            }

            //formsPlot1.Plot.Clear();
            //formsPlot1.Plot.AxisAuto();
            formsPlot1.Render();
        }

        //private void clearChannel(object sender, EventArgs e)
        //{
        //    formsPlot1.Plot.Clear();
        //    formsPlot1.Plot.AxisAuto();
        //    formsPlot1.Render();
        //}

        //private void clearPlot(object sender, EventArgs e)
        //{
        //    formsPlot1.Plot.Clear();
        //    formsPlot1.Plot.AxisAuto();
        //    formsPlot1.Render();
        //}

        private void SetTimer()
        {
            aTimer = new Timers.Timer(100);
            aTimer.Elapsed += this.OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.SynchronizingObject = this;
            aTimer.Enabled = false;

            bTimer = new Timers.Timer(5000);
            bTimer.Elapsed += this.OnRender;
            bTimer.AutoReset = true;
            bTimer.SynchronizingObject = this;
            bTimer.Enabled = false;
        }


        private void OnTimedEvent(Object source, Timers.ElapsedEventArgs e)
        {
            IList<double> data = new List<double>();

            data = labjack.readValue();

            label1.Text = data.ElementAt(0).ToString("0.000", CultureInfo.InvariantCulture);
            label2.Text = data.ElementAt(1).ToString("0.000", CultureInfo.InvariantCulture);
            label3.Text = data.ElementAt(2).ToString("0.000", CultureInfo.InvariantCulture);
            label4.Text = data.ElementAt(3).ToString("0.000", CultureInfo.InvariantCulture);
        }


        private void OnRender(Object source, Timers.ElapsedEventArgs e)
        {
            
#if DEBUG
            Stopwatch timer = new Stopwatch();
            timer.Start();
#endif
            IList<double> data = new List<double>();
            double currentRightEdge = formsPlot1.Plot.GetAxisLimits().XMax;

            data = labjack.readValue();

            if (NextPointIndex == adVal.arrayLength())
            {
                //Array.Resize<double>(ref adValues1, adValues1.Length + 500_000);
                //Array.Resize<double>(ref adValues2, adValues2.Length + 500_000);
                //Array.Resize<double>(ref adValues3, adValues3.Length + 500_000);
                //Array.Resize<double>(ref adValues4, adValues4.Length + 500_000);

                formsPlot1.Plot.Clear();
                SignalPlot1 = formsPlot1.Plot.AddSignal(adVal.AdValues1);
                SignalPlot2 = formsPlot1.Plot.AddSignal(adVal.AdValues2);
                SignalPlot3 = formsPlot1.Plot.AddSignal(adVal.AdValues3);
                SignalPlot4 = formsPlot1.Plot.AddSignal(adVal.AdValues4);

            }

            adVal.writeValues(data.ElementAt(0), data.ElementAt(1), data.ElementAt(2), data.ElementAt(3), NextPointIndex);

            label1.Text = data.ElementAt(0).ToString("0.000", CultureInfo.InvariantCulture);
            label2.Text = data.ElementAt(1).ToString("0.000", CultureInfo.InvariantCulture);
            label3.Text = data.ElementAt(2).ToString("0.000", CultureInfo.InvariantCulture);
            label4.Text = data.ElementAt(3).ToString("0.000", CultureInfo.InvariantCulture);


            Task task = write.SimpleWriteAsync(Convert.ToUInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds).ToString() + @";" + data.ElementAt(0).ToString() + @";" + data.ElementAt(1).ToString() + @";" + data.ElementAt(2).ToString() + @";" + data.ElementAt(3).ToString() + Environment.NewLine);

            awsStream.sendMessage(adVal);
            //double currentRightEdge = formsPlot1.Plot.GetAxisLimits().XMax;

            //if (NextPointIndex == currentRightEdge)


            SignalPlot1.MaxRenderIndex = NextPointIndex;
            SignalPlot2.MaxRenderIndex = NextPointIndex;
            SignalPlot3.MaxRenderIndex = NextPointIndex;
            SignalPlot4.MaxRenderIndex = NextPointIndex;

            if (NextPointIndex > currentRightEdge)
                formsPlot1.Plot.SetAxisLimits(xMax: currentRightEdge + 100);

            formsPlot1.Plot.AxisAutoY(0.1, 0);
            formsPlot1.Render();

            NextPointIndex++;

#if DEBUG
            timer.Stop();
            Debug.WriteLine("Time Taken: " + timer.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
#endif
        }

        private void button2_Click(object sender, EventArgs e)
        {
            aTimer.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            aTimer.Stop();
        }

        private void fileDialog_Click(object sender, EventArgs e)
        {
            DateTimeOffset dto = new DateTimeOffset(DateTime.UtcNow);

            saveFileDialog1.Filter = "CSV files (*.csv)|*.csv";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.FileName = dto.ToUnixTimeSeconds().ToString();
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                startExperiment.Enabled = true;
                stopExperiment.Enabled = true;
            }
            else
            {
                startExperiment.Enabled = false;
                stopExperiment.Enabled = false;
            }
        }

        private void startExperiment_Click(object sender, EventArgs e)
        {
            bTimer.Start();
        }

        private void stopExperiment_Click(object sender, EventArgs e)
        {
            bTimer.Stop();
        }
    }
}
