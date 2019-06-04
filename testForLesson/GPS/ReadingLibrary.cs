using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
//采用先判断头文件(只读取一部分），再进行读取
namespace GPS
{
    //未收录数据有需要直接加
    public struct BP    //BestPos ID:42  Length:H+72(已包括CRC)
    {
        public double lat;
        public double lon;
        public double hgt;
    }
    public struct RA   //Range ID:43 Length:H+4+(#obs x 44)+4(已包括CRC)
    {
        public UInt32 obs;
        public List<double> psr;
        public List<double> adr;
        public List<UInt32> ch_tr_status;
        public List<uint> system;
        public List<uint> s_type;
    }
    public struct SA   //SATVIS2 ID:1043 Length:H+16+(#sat x 40)+4(已包括CRC)
    {
        public UInt32 sat;
        public List<double> elev;
        public List<double> az;
    }
    public struct Head
    {
        public byte[] sync;
        public byte HeaderLgth;//uchar
        public UInt16 MessageID;
        public UInt16 week;
        public int gpss;
        public DateTime UTC;
        //byte le = br.ReadByte();//转化uchar为int
        //string le2 = le.ToString();
        //int.TryParse(le2, out int le3);
    }

    public class ReadingLibrary
    {
        //读头文件并打印，返回ID
        public static Head ReadHead(FileStream fs,BinaryReader br)  
        {
            //string[] check = new string[3] { "170", "68", "18" }; //用于检查异常，只要有非法部分，下一次必定不是这个
            Head myHead;
            myHead.sync= br.ReadBytes(3);
            myHead.HeaderLgth = br.ReadByte();//表给的uchar，直接读byte
            myHead.MessageID = br.ReadUInt16();//到这里已经读了6byte
            Console.Write("Mark:");
            for (int i = 0; i < myHead.sync.Length; i++)
            {
                Console.Write("{0}"+" ", myHead.sync[i]);
                //}
                //else
                //{
                //    throw new Exception("nimadeweishenme?");
                //}
            }
            Console.WriteLine();
            Console.WriteLine("HeadLgth "+myHead.HeaderLgth.ToString());
            Console.WriteLine("ID "+myHead.MessageID.ToString());
            fs.Seek(8, SeekOrigin.Current); //指到time部分
            myHead.week = br.ReadUInt16();
            myHead.gpss = br.ReadInt32();
            myHead.UTC = GpstToUTC(myHead.week, myHead.gpss);
            Console.WriteLine(myHead.UTC);
            fs.Seek(8, SeekOrigin.Current);
            return myHead;
        }
        //读取BestPos 确保指针位置
        public static BP ReadBestPos(FileStream fs,BinaryReader br,BP bestPos)
        {
            fs.Seek(8, SeekOrigin.Current);//直接到3个数据前面
            bestPos.lat = br.ReadDouble();
            bestPos.lon = br.ReadDouble();
            bestPos.hgt = br.ReadDouble();
            Console.WriteLine("lat:"+bestPos.lat);
            Console.WriteLine("lon"+bestPos.lon);
            Console.WriteLine("hgt"+bestPos.hgt);
            fs.Seek(44, SeekOrigin.Current);
            return bestPos;
        }
        public static RA  ReadObs(BinaryReader br) //读取前置数据，同时初始化结构体
        {
            RA back = new RA
            {
                adr = new List<double>(),
                psr = new List<double>(),
                system = new List<uint>(),
                s_type = new List<uint>(),
                ch_tr_status = new List<UInt32>()
            };
            back.obs = br.ReadUInt32();
            return back;
        }
        public static RA ReadRange(FileStream fs, BinaryReader br,RA range)//原理类似
        {
            fs.Seek(4, SeekOrigin.Current);
            range.psr.Add(br.ReadDouble());
            fs.Seek(4, SeekOrigin.Current);
            range.adr.Add(br.ReadDouble());
            fs.Seek(16, SeekOrigin.Current);
            range.ch_tr_status.Add(br.ReadUInt32());
            Console.WriteLine(range.psr.Last());
            Console.WriteLine(range.adr.Last());
            Console.WriteLine(range.ch_tr_status.Last());
            range.system.Add(DoSys(range.ch_tr_status.Last()));
            range.s_type.Add(DoStype(range.ch_tr_status.Last()));
            return range;
        }
        public static SA ReadSat(FileStream fs,BinaryReader br)//读取前置数据，同时初始化结构体
        {
            SA back = new SA
            {
                elev = new List<double>(),
                az = new List<double>(),
            };
            fs.Seek(12, SeekOrigin.Current);
            back.sat = br.ReadUInt32();
            return back;
        }
        public static SA ReadRange(FileStream fs, BinaryReader br, SA sat)//原理类似
        {
            fs.Seek(8, SeekOrigin.Current);
            sat.elev.Add(br.ReadDouble());
            sat.az.Add(br.ReadDouble());
            fs.Seek(16, SeekOrigin.Current);
            Console.WriteLine(sat.elev.Last());
            Console.WriteLine(sat.az.Last());
            return sat;
        }
        public static void FindNextHead(FileStream fs, BinaryReader br)
        {
            string[] check = new string[3] { "170", "68", "18" };
            byte[] sync = new byte[3];
            for (int flag = 0;flag!=3 ; )
            {
                flag = 0;
                sync = br.ReadBytes(3);
                for(int i = 0; i < 3; i++)
                {
                    if (check[i] == sync[i].ToString())
                        flag++;
                }               
            }
            fs.Seek(-3, SeekOrigin.Current);
        }
        private static uint DoSys(uint chtr)
        {
            UInt32 get = 0x00070000;
            chtr = chtr & get; //读取16-18
            chtr >>= 16;
            Console.WriteLine(chtr);
            return chtr;
        }
        private static uint DoStype(uint chtr)
        {
            uint get = 0x03e00000;//取对应16进制
            chtr = chtr & get;
            chtr >>= 21; //平移对应位数
            Console.WriteLine(chtr);
            return chtr;
        }
        //Gps周和周内秒转换为UTC
        private static DateTime GpstToUTC(UInt16 week,int gpss)
        {
            int difFromBegin = week * 604800 + gpss/1000;
            DateTime gpsBeginTime = new DateTime(1980, 1, 6, 0, 0, 0);
            gpsBeginTime = gpsBeginTime.AddSeconds(difFromBegin);
            return gpsBeginTime.AddSeconds(-18.0);
        }
    }
}
