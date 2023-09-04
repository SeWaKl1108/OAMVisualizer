// #define INITVALUE
// #define TEST

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
using System.Configuration;
using ScottPlot.Control;
using System.IO.Ports;

namespace plotLabjack
{


    public partial class Form1 : Form
    {
        int constaTimer = 200;
        int constbTimer = 5000;

        bool isMouseHovering = false;

        Timers.Timer aTimer = null;
        Timers.Timer bTimer = null;

        fileWriter write = null;
        //aws awsStream = null;
        labJack myLabJack = null;

        LinesPlot lines = null;
        Values adVal = null;

        int NextPointIndex = 0;

        SignalPlot SignalPlot1 = null;
        SignalPlot SignalPlot2 = null;
        SignalPlot SignalPlot3 = null;
        SignalPlot SignalPlot4 = null;

        bool plotSignal1 = true;
        bool plotSignal2 = true;
        bool plotSignal3 = true;
        bool plotSignal4 = true;

        //write a function that generates 10 in numbers

        public Form1()
        {
            int timerValue;
            string timerValueString;

            InitializeComponent();

            Dictionary<string, string> timerValues = new Dictionary<string, string>
            {
                //add a timer to, "aTimer" the dictornary
                { "aTimer", "constaValue" },
                { "bTimer", "constbValue" }
            };


            foreach (KeyValuePair<string, string> dictionary in timerValues)
            {
                timerValues.TryGetValue(dictionary.Key, out string result);
                switch (result)
                {
                    case "constaValue":
                        timerValueString = ConfigurationManager.AppSettings[dictionary.Key];
                        if (int.TryParse(timerValueString, out timerValue))
                        {
                            constaTimer = timerValue;
                        }
                        else
                        {
                            MessageBox.Show("Something is wrong with App.Config, set init value");
                            constaTimer = 200;
                        }
                        Debug.Write(constaTimer + " " + constbTimer);
                        break;
                    case "constbValue":
                        timerValueString = ConfigurationManager.AppSettings[dictionary.Key];
                        if (int.TryParse(timerValueString, out timerValue))
                        {
                            constbTimer = timerValue;
                        }
                        else
                        {
                            MessageBox.Show("Something is wrong with App.Config, set init value");
                            constbTimer = 5000;
                        }
                        Debug.Write(constaTimer + " " + constbTimer);
                        break;
                    default:
                        MessageBox.Show("Something is wrong with App.Config, set init value");
                        constaTimer = 200;
                        constbTimer = 5000;
                        break;
                }

            }
 


            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            if (screenHeight < 1080)
            {
                // Assuming you want to scale the form to 80% of its original size as an example
                float scalePercentage = 0.8f;

                this.Width = (int)(this.Width * scalePercentage);
                this.Height = (int)(this.Height * scalePercentage);

                for (int i = 1; i <= 12; i++)
                {
                    Control control = FindControlByName(this, "label" + i);
                    if (control is Label label)
                    {
                        label.Font = new Font(label.Font.FontFamily, 15);
                    }
                }

            }

#if !TEST
            try
            {
                myLabJack = new labJack();
                label6.Text = myLabJack.getDriverVersion();
                label11.Text = myLabJack.getHardwareVersion();

                // Get the firmware version.
                label8.Text = myLabJack.getFirmwareVersion();
            }
            catch (LabJackUDException exc)
            {
                MessageBox.Show(exc.ToString(), "Restart App");
                // Experiment enabled
                this.Text = "Restart App, Labjack not found";
                button1.Enabled = false;
                button2.Enabled = false;
                fileDialog.Enabled = false;
                label6.Text = "Labjack not found";
                label11.Text = "Labjack not found";
                label8.Text = "Labjack not found";
            }
#else
            label6.Text = "Debug";
#endif

            DateTimeOffset dto = new DateTimeOffset(DateTime.UtcNow);
            write = new fileWriter(Environment.GetFolderPath(Environment.SpecialFolder.Personal), dto.ToString("yyyyMMdd_HHmmss"));


            SetTimer();

            lines = new LinesPlot();
#if INITVALUE
            adVal = new Values(100);
#else
            adVal = new Values();
#endif

            defaultSignalPlot();

            System.Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            label10.Text = version.ToString();

            pictureBox1.Paint += pictureBox1_Paint;

            this.Text = GetAssemblyTitle();

            // Experiment enabled
            startExperiment.Enabled = false;
            stopExperiment.Enabled = false;

        }

        private string GetAssemblyTitle()
        {
            // Get the current assembly
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            // Get the AssemblyTitle attribute value
            var titleAttribute = assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyTitleAttribute), false)
                                         .OfType<System.Reflection.AssemblyTitleAttribute>()
                                         .FirstOrDefault();

            return titleAttribute?.Title ?? string.Empty;
        }

        public Control FindControlByName(Control parent, string name)
        {
            // If the control name matches, return the control
            if (parent.Name == name)
                return parent;

            // Search within the child controls
            foreach (Control child in parent.Controls)
            {
                Control result = FindControlByName(child, name);
                if (result != null)
                    return result;
            }

            // If no control found, return null
            return null;
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
            formsPlot1.RightClicked -= formsPlot1.DefaultRightClickEvent;
            formsPlot1.RightClicked += new EventHandler((sender, e) => CustomRightClickEvent(sender, e));


            formsPlot1.Configuration.ScrollWheelZoom = false;
            formsPlot1.Configuration.RightClickDragZoom = false;
            formsPlot1.Configuration.LeftClickDragPan = false;

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

            bTimer = new Timers.Timer(constbTimer);
            bTimer.Elapsed += this.OnRender;
            bTimer.AutoReset = true;
            bTimer.SynchronizingObject = this;
            bTimer.Enabled = false;
        }


        private void OnTimedEvent(Object source, Timers.ElapsedEventArgs e)
        { 

            try
            {
                IList<double> data = myLabJack.readValue();
                label1.Text = data.ElementAt(0).ToString("0.000", CultureInfo.InvariantCulture);
                label2.Text = data.ElementAt(1).ToString("0.000", CultureInfo.InvariantCulture);
                label3.Text = data.ElementAt(2).ToString("0.000", CultureInfo.InvariantCulture);
                label4.Text = data.ElementAt(3).ToString("0.000", CultureInfo.InvariantCulture);
            }
            catch (LabJackUDException exc)
            {
                MessageBox.Show(exc.Message);
                this.Text = "Restart App, can't read values, Timer stopped";
                aTimer.Enabled = false;
                bTimer.Enabled = false;
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Data index is out of range.");
                this.Text = "Data issue, Timer stopped";
                aTimer.Enabled = false;
                bTimer.Enabled = false;
            }
        }


        private void OnRender(Object source, Timers.ElapsedEventArgs e)
        {

#if DEBUG
            Stopwatch timer = new Stopwatch();
            timer.Start();
#endif
            IList<double> data = new List<double>();
            double currentRightEdge = formsPlot1.Plot.GetAxisLimits().XMax;

            try
            {
                data = myLabJack.readValue();
            }
            catch (LabJackUDException exc)
            {
                MessageBox.Show(exc.Message);
                this.Text = "Restart App, can't read values, Timer stopped";
                aTimer.Enabled = false;
                bTimer.Enabled = false;
            }

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

            //awsStream.sendMessage(adVal);
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
            saveFileDialog1.FileName = dto.ToString("yyyyMMdd_HHmmss");
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                startExperiment.Enabled = true;
                stopExperiment.Enabled = true;
                write.updateFolder(saveFileDialog1.FileName);
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
            if (aTimer.Enabled == true)
            {
                MessageBox.Show("The comparison will be disabled");
                aTimer.Stop();
            }

        }

        private void stopExperiment_Click(object sender, EventArgs e)
        {
            bTimer.Stop();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.youtube.com/@elektrotechnik2go");
        }



        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            isMouseHovering = true;
            pictureBox1.Invalidate();  // Forces the picture box to be redrawn
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            isMouseHovering = false;
            pictureBox1.Invalidate();  // Forces the picture box to be redrawn
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (isMouseHovering)
            {
                pictureBox1.Image = Properties.Resources.clickForHelp;
            }
            else
            {
                pictureBox1.Image = Properties.Resources.jsLogo;
            }
        }


        private void controlTab_Selected(object sender, TabControlEventArgs e)
        {

            if (e.TabPage == tabPage4)
            {
                // Center pictureBox3 within its parent TabPage
                int centerX = (pictureBox3.Parent.Width - pictureBox3.Width) / 2;
                int centerY = (pictureBox3.Parent.Height - pictureBox3.Height) / 2;
                pictureBox3.Location = new Point(centerX, centerY);
            }
        }

        private void pictureBox3_MouseEnter(object sender, EventArgs e)
        {
            int newWidth = (int)(pictureBox3.Width * 1.25);
            int newHeight = (int)(pictureBox3.Height * 1.25);

            // Create a new bitmap with the scaled size
            Bitmap scaledBitmap = new Bitmap(newWidth, newHeight);

            // Draw the original image onto the scaled bitmap
            using (Graphics g = Graphics.FromImage(scaledBitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(pictureBox3.Image, 0, 0, newWidth, newHeight);
            }

            // Dispose of the old image to free memory
            System.Drawing.Image oldImage = pictureBox3.Image;
            pictureBox3.Image = scaledBitmap;
            oldImage?.Dispose();

            // Resize the PictureBox control itself
            pictureBox3.Size = new Size(newWidth, newHeight);

            // Center pictureBox3 within its parent TabPage
            int centerX = (pictureBox3.Parent.Width - pictureBox3.Width) / 2;
            int centerY = (pictureBox3.Parent.Height - pictureBox3.Height) / 2;
            pictureBox3.Location = new Point(centerX, centerY);
        }



        private void pictureBox3_MouseLeave(object sender, EventArgs e)
        {
            int newWidth = (int)(pictureBox3.Width * 0.8);
            int newHeight = (int)(pictureBox3.Height * 0.8);

            // Create a new bitmap with the scaled size
            Bitmap scaledBitmap = new Bitmap(newWidth, newHeight);

            // Draw the original image onto the scaled bitmap
            using (Graphics g = Graphics.FromImage(scaledBitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(pictureBox3.Image, 0, 0, newWidth, newHeight);
            }

            // Dispose of the old image to free memory
            System.Drawing.Image oldImage = pictureBox3.Image;
            pictureBox3.Image = scaledBitmap;
            oldImage?.Dispose();

            // Resize the PictureBox control itself
            pictureBox3.Size = new Size(newWidth, newHeight);

            // Center pictureBox3 within its parent TabPage
            int centerX = (pictureBox3.Parent.Width - pictureBox3.Width) / 2;
            int centerY = (pictureBox3.Parent.Height - pictureBox3.Height) / 2;
            pictureBox3.Location = new Point(centerX, centerY);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.schmid-johann.de");
        }
    }
}
