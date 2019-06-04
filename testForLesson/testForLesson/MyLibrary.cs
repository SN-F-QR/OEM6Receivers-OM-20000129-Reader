using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace testForLesson
{
    class Test
    {
        static void Main(string[] args)
        {
            MyLibrary.SayHello();
            //MyLibrary.AddBook();
            //MyLibrary.DisplayBook();
        }
    }
    class MyLibrary
    {
        public static void SayHello()
        {
            int i = 0;
            while (i == 0)
            {
                Console.WriteLine("This is a Library System!");
                Console.WriteLine("Please select what you want to do:");
                Console.WriteLine("01.Display all book");
                Console.WriteLine("02.Add one book");
                Console.WriteLine("03.Exit");
                string k = Console.ReadLine();
                if (k == "01" || k == "1")
                {
                    DisplayBook();
                    Console.WriteLine("=========================");
                }
                if (k == "02" || k == "2")
                {
                    AddBook();
                    Console.WriteLine("=========================");
                }
                if (k == "03" || k == "3")
                {
                    break;
                }
            }
        }
        private static List<Dictionary<string, string>> GetData(out List<string> columns)
        {
            string line;//获取行值作暂时储存
            string[] stringArray;//储存分割后的行值
            char[] charArray = new char[] { ',' };//用于分割的标记
            List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();
            columns = new List<string>();//储存第一行
            FileStream aFile = new FileStream("MyLibrary.txt", FileMode.Open, FileAccess.ReadWrite);
            StreamReader sr = new StreamReader(aFile);
            line = sr.ReadLine();
            stringArray = line.Split(charArray);//分割line
            for(int x = 0; x <= stringArray.GetUpperBound(0); x++)
            {
                columns.Add(stringArray[x]);//逐个添加进行头
            }
            line = sr.ReadLine();//继续读
            while (line != null)
            {
                stringArray = line.Split(charArray);//分割
                Dictionary<string, string> dataRow = new Dictionary<string, string>();
                for(int x = 0; x <= stringArray.GetUpperBound(0); x++)
                {
                    dataRow.Add(columns[x], stringArray[x]);//行数据，键是行头，值是现在的
                }
                data.Add(dataRow);//data就像表，datarow就是行，把行加到表里
                line = sr.ReadLine();
            }
            sr.Close();
            return data;
        }
        public static void DisplayBook()
        {
            List<string> columns;
            List<Dictionary<string, string>> myData = GetData(out columns);
            foreach(string column in columns)
            {
                Console.Write("{0,-20}", column);
            }
            Console.WriteLine();
            foreach(Dictionary<string,string> row in myData)//循环行
            {
                foreach(string column in columns)//循环关键字
                {
                    Console.Write("{0,-20}", row[column]);//打印关键字的值
                }
                Console.WriteLine();
            }
            Console.ReadKey();
        }
        public static void AddBook()
        {
            string message = Console.ReadLine();
            FileStream aFile = new FileStream("MyLibrary.txt", FileMode.Open, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(aFile);
            aFile.Seek(0, SeekOrigin.End);
            sw.WriteLine();
            sw.Write(message);
            sw.Close();
            aFile.Close();
        }

    }
}
