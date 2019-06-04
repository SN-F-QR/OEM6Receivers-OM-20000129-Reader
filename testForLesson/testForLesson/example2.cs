using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace testForLesson
{
    class example2
    {
        static void Main(string[] args)
        {
            FileStream fs;
            fs = new FileStream("E:\\test.bin", FileMode.Truncate, FileAccess.ReadWrite);
            BinaryWriter bw = new BinaryWriter(fs);
            //Encoder e = Encoding.UTF8.GetEncoder();
            //准备不同类型的数据
            double aDouble = 12.5;
            int aInt = 34;
            char[] aCharArray = { 'A', 'B', 'C' };
            //byte[] byteData = new byte[aCharArray.Length];
            //利用Write 方法的多种重载形式写入数据           
            bw.Write(aDouble);
            bw.Write(aInt);
            bw.Write(aCharArray);
            int length = Convert.ToInt32(bw.BaseStream.Length);
            fs.Close();
            bw.Close();


            //读取并输出数据
            fs = new FileStream("E:\\test.bin", FileMode.OpenOrCreate, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            Console.WriteLine(br.ReadDouble().ToString());
            Console.WriteLine(br.ReadInt32().ToString());
            char[] data = br.ReadChars(length);
            for (int i = 0; i < data.Length; i++)
            {
                Console.WriteLine("{0}", data[i]);
            }
            fs.Close();
            br.Close();
            Console.ReadLine();
        }
    }
}
