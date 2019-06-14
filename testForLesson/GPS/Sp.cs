using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS
{
    class Sp
    {
        static void Main(string[] args)
        {
            Console.ReadLine();
            for(int n = 1; n < 33; n++)
            {
                if (n == 1 || n == 26)
                {
                    continue;
                }
                ReadingLibrary.SpRange09(n.ToString());//还是要改函数
            }
            //ReadingLibrary.FindNew("1");
        }
    }
}
