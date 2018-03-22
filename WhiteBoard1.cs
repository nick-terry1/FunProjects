using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string sample = "sample stringas ppp";
            bool[] barr = new bool[sample.Length];
            barr = UniqueVal(sample);
            for (int i = 0; i < barr.Length; i++)
            {
                Console.WriteLine(barr[i]);
            }

        }

        public static bool[] UniqueVal(string arg){
            int count = 0;
            bool[] barr = new bool[arg.Length];
            for (int i = 0; i < arg.Length; i++)
            {
                for (int j = 0; j < arg.Length; j++)
                {
                    if (arg[i] == arg[j])
                    {
                        count++;
                    }
                    if (i == j)
                    {
                        count--;
                    }
                }
                if (count > 0)
                {
                    barr[i] = true;
                }
                else { barr[i] = false; }
                count = 0;
            }
            return barr;
        }
    }
}
