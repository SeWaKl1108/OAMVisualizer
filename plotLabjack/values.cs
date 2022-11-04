using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plotLabjack
{
    internal class Values
    {
        private double[] adValues1;
        private double[] adValues2;
        private double[] adValues3;
        private double[] adValues4;

        public ref double[] AdValues1 => ref adValues1;
        public ref double[] AdValues2 => ref adValues2;
        public ref double[] AdValues3 => ref adValues3;
        public ref double[] AdValues4 => ref adValues4;

        public Values()
        {
            adValues1 = new double[500_000];
            adValues2 = new double[500_000];
            adValues3 = new double[500_000];
            adValues4 = new double[500_000];
        }

        public Values(int arrayLenth)
        {
            adValues1 = new double[arrayLenth];
            adValues2 = new double[arrayLenth];
            adValues3 = new double[arrayLenth];
            adValues4 = new double[arrayLenth];

            Random rnd = new Random();

            for (int i = 0; i < arrayLenth; i++)
            {

                adValues1[i] = rnd.NextDouble() * 10;
                adValues2[i] = rnd.NextDouble() * 10;
                adValues3[i] = rnd.NextDouble() * 10;
                adValues4[i] = rnd.NextDouble() * 10;
            }
        }

        public void writeValues(double value1, double value2, double value3, double value4, int position)
        {
            adValues1[(int)position] = value1;
            adValues2[(int)position] = value2;
            adValues3[(int)position] = value3;
            adValues4[(int)position] = value4;
        }

        public int arrayLength()
        {
            return adValues1.Length;
        }

        public double readLastValueChannel1()
        {
            return adValues1[adValues1.Length - 1];
        }

        public double readLastValueChannel2()
        {
            return adValues2[adValues2.Length - 1];
        }

        public double readLastValueChannel3()
        {
            return adValues3[adValues3.Length - 1];
        }

        public double readLastValueChannel4()
        {
            return adValues4[adValues4.Length - 1];
        }

    }
}
