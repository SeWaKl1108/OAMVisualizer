using LabJack.LabJackUD;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace plotLabjack
{
    internal class labJack
    {
        private U3 u3 = null;
        double dblValue = 0;

        //public IList<double> values = new List<double>();

        public labJack()
        {
            try
            {
                u3 = new U3(LJUD.CONNECTION.USB, "0", true);
                LJUD.ePut(u3.ljhandle, LJUD.IO.PIN_CONFIGURATION_RESET, 0, 0, 0);

            }
            catch (LabJackUDException exc)
            {
                throw new LabJackUDException(exc.LJUDError);
            }
        }

        public string getHardwareVersion()
        {
            //change the , to .

            LJUD.eGet(u3.ljhandle, LJUD.IO.GET_CONFIG, LJUD.CHANNEL.HARDWARE_VERSION, ref dblValue, 0);
            return dblValue.ToString(CultureInfo.InvariantCulture);
        }

        public string getDriverVersion()
        {
            dblValue = LJUD.GetDriverVersion();
            return dblValue.ToString(CultureInfo.InvariantCulture);
        }

        public string getFirmwareVersion()
        {
            LJUD.eGet(u3.ljhandle, LJUD.IO.GET_CONFIG, LJUD.CHANNEL.FIRMWARE_VERSION, ref dblValue, 0);
            return dblValue.ToString(CultureInfo.InvariantCulture);
        }

        public IList<double> readValue()
        {

            //#if DEBUG
            //            Stopwatch timer = new Stopwatch();
            //            timer.Start();
            //#endif

            IList<double> values = new List<double>();

            LJUD.IO ioType = 0;
            LJUD.CHANNEL channel = 0;
            
            double dblValue = 0;

            int dummyInt = 0;
            double dummyDouble = 0;

            bool finish = false;

            try
            {
                LJUD.AddRequest(u3.ljhandle, LJUD.IO.GET_AIN_DIFF, 0, 0, 32, 0);
                LJUD.AddRequest(u3.ljhandle, LJUD.IO.GET_AIN_DIFF, 1, 0, 32, 0);
                LJUD.AddRequest(u3.ljhandle, LJUD.IO.GET_AIN_DIFF, 2, 0, 32, 0);
                LJUD.AddRequest(u3.ljhandle, LJUD.IO.GET_AIN_DIFF, 3, 0, 32, 0);
                LJUD.AddRequest(u3.ljhandle, LJUD.IO.GET_AIN, 30, 0, 0, 0);

                LJUD.GoOne(u3.ljhandle);

                LJUD.GetFirstResult(u3.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble);
            }
            catch (LabJackUDException exc)
            {
                throw new LabJackUDException(exc.LJUDError);
            }


            //Debug.WriteLine("Ch0 " + dblValue.ToString());
            values.Add(dblValue);

            while (!finish)
            {
                switch ((int)channel)
                {
                    case 1:
                        //Debug.WriteLine("Ch1 " + dblValue.ToString());
                        values.Add(dblValue);
                        break;
                    case 2:
                        //Debug.WriteLine("Ch2 " + dblValue.ToString());
                        values.Add(dblValue);
                        break ;
                    case 3:
                        //Debug.WriteLine("Ch3 " + dblValue.ToString());
                        values.Add(dblValue);
                        break;
                    case 30:
                        //Debug.WriteLine("Ch3 " + dblValue.ToString());
                        Debug.WriteLine("Ch30 " + dblValue.ToString());
                        values.Add(dblValue);
                        break;
                }

                try
                {
                    LJUD.GetNextResult(u3.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble);
                }
                catch (LabJackUDException exc)
                {
                    if (exc.LJUDError == U3.LJUDERROR.NO_MORE_DATA_AVAILABLE)
                        finish = true;
                    else
                        MessageBox.Show(exc.ToString());
                }
            }


//#if DEBUG
//            timer.Stop();
//            Debug.WriteLine("Read Time: " + timer.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
//#endif
        return values;
        }

    }
}
