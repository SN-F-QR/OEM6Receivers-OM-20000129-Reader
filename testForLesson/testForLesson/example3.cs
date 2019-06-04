using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using GPS;
//测试位操作 用于range
//最主要是参照表125 P589
//这里的数据是储存在ch_tr_status中的，其为32位无符号整型，也就是二进制有32位
//每一位对应的含义都在表里对应好了
//注意位次，表中0-31，意思是32位二进制中，从右0开始往左32位进行对应的
//运用与&运算符（运算符功能是将1相同的位提取出来），提取我们需要的那几位
//然后再进行平移到一端（这样才能得出我们想要的这几位对应的32位无符号整型）
//注意平移是用>>而且记得移的位数
//二进制转16进制方法自己找
namespace testForLesson
{
    class example3
    {
        static void Main(string[] args)
        {
            FileStream fs;
            fs = new FileStream("data.gps", FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            ReadingLibrary.ReadHead(fs, br);
            BP test1 = new BP();
            ReadingLibrary.ReadBestPos(fs, br, test1);
            RA test2 = new RA();
            ReadingLibrary.ReadHead(fs, br);
            test2 = ReadingLibrary.ReadObs(br);
            test2 = ReadingLibrary.ReadRange(fs, br, test2);
            UInt32 move = test2.ch_tr_status.Last();
            UInt32 get1 = 0x00070000;
            move = move & get1; //读取16-18
            move >>= 16;
            uint get2 = 0x03e00000;
            move = test2.ch_tr_status.Last();
            move = move & get2;
            move >>= 21;
        }
    }
}
