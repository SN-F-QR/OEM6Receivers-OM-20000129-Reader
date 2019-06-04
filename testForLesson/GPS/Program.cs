using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
//用时约8min
//最新用时14min
namespace GPS
{
    class Program
    {
        static void Main(string[] args)
        {
            FileStream fs;
            fs = new FileStream("data.gps", FileMode.Open, FileAccess.Read);            
            BinaryReader br = new BinaryReader(fs);
            FileStream totxt00 = new FileStream("RangeData00.txt", FileMode.Create, FileAccess.Write);
            FileStream totxt09 = new FileStream("RangeData09.txt", FileMode.Create, FileAccess.Write);
            FileStream totxt10 = new FileStream("RangeData10.txt", FileMode.Create, FileAccess.Write);
            FileStream totxt15 = new FileStream("RangeData15.txt", FileMode.Create, FileAccess.Write);
            StreamWriter sw00 = new StreamWriter(totxt00);
            StreamWriter sw09 = new StreamWriter(totxt09);
            StreamWriter sw10 = new StreamWriter(totxt10);
            StreamWriter sw15 = new StreamWriter(totxt15);
            Head head;
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                head = ReadingLibrary.ReadHead(fs, br);
                if (head.MessageID == 42)
                {
                    BP test1 = new BP();
                    test1 = ReadingLibrary.ReadBestPos(fs, br, test1);
                }
                else if (head.MessageID == 43)
                {
                    RA test2 = new RA();
                    test2 = ReadingLibrary.ReadObs(br);
                    for (uint i = 0; i < test2.obs; i++)
                    {
                        Console.WriteLine("No." + i + ":");
                        test2 = ReadingLibrary.ReadRange(fs, br, test2);
                        //将数据分类写入txt中
                        switch (test2.system.Last()+test2.s_type.Last())
                        {
                            case 0:sw00.WriteLine(head.UTC+","+test2.psr.Last()+","+test2.adr.Last());break;
                            case 9:sw09.WriteLine(head.UTC + "," + test2.psr.Last() + "," + test2.adr.Last()); break;
                            case 1:sw10.WriteLine(head.UTC + "," + test2.psr.Last() + "," + test2.adr.Last()); break;
                            case 6:sw15.WriteLine(head.UTC + "," + test2.psr.Last() + "," + test2.adr.Last()); break;
                            default:throw new Exception("有未知量！！！");
                        }
                    }
                    fs.Seek(4, SeekOrigin.Current);
                }
                else if (head.MessageID == 1043)
                {
                    SA test3 = new SA();
                    test3 = ReadingLibrary.ReadSat(fs, br);
                    for (uint i = 0; i < test3.sat; i++)
                    {
                        Console.WriteLine("No." + i + ":");
                        test3 = ReadingLibrary.ReadRange(fs, br, test3);
                    }
                    fs.Seek(4, SeekOrigin.Current);
                }
                else
                {
                    ReadingLibrary.FindNextHead(fs,br);
                }
            }
            //ReadingLibrary.ReadHead(fs, br);
            //BP test=new BP();
            //test=ReadingLibrary.ReadBestPos(fs, br, test);
            //ReadingLibrary.ReadHead(fs, br);
            //RA test2 = new RA();
            //test2 = ReadingLibrary.ReadObs(br);
            //for(uint i = 0; i < test2.obs; i++)
            //{
            //    Console.WriteLine("No." + i+":");
            //    test2 = ReadingLibrary.ReadRange(fs, br,test2);
            //}
            //fs.Seek(4, SeekOrigin.Current);
            //ReadingLibrary.ReadHead(fs, br);
            //SA test3 = new SA();
            //test3 = ReadingLibrary.ReadSat(fs, br);
            //for (uint i = 0; i < test3.sat; i++)
            //{
            //    Console.WriteLine("No." + i + ":");
            //    test3 = ReadingLibrary.ReadRange(fs, br, test3);
            //}

            //byte[] data = br.ReadBytes(3);
            //for (int i = 0; i < data.Length; i++)
            //{
            //    Console.Write("{0}", data[i]);
            //}
            //Console.WriteLine();
            //byte le = br.ReadByte();//转化uchar为int
            //string le2 = le.ToString();
            //int.TryParse(le2, out int le3);
            //Console.WriteLine(le3);
            ////Console.WriteLine(br.ReadByte().ToString());
            //Console.WriteLine(br.ReadUInt16().ToString());
            //fs.Seek(30, SeekOrigin.Current);
            //Console.WriteLine(br.ReadDouble().ToString());
            //Console.WriteLine(br.ReadDouble().ToString());
            //Console.WriteLine(br.ReadDouble().ToString());
            ////data = br.ReadBytes(4);万一用得到的小技巧
            ////float sh = System.BitConverter.ToSingle(data, 0);
            //Console.WriteLine(br.ReadSingle().ToString());
            sw00.Close();
            sw09.Close();
            sw10.Close();
            sw15.Close();
            totxt00.Close();
            totxt09.Close();
            totxt10.Close();
            totxt15.Close();
            br.Close();
            fs.Close();
        }
    }
}
