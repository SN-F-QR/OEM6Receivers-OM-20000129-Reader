using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using GPS;

namespace UIofGPS
{
    public partial class MainForm : Form
    {
        private string name;
        public MainForm()
        {
            InitializeComponent();
            radioButton2.Select();
        }

        private void 输出选择_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void Button3_Click(object sender, EventArgs e)
        {
            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            string path1 = @"C:\Users\DELL\Desktop\GPSData\"+name+"RangeData00.txt";
            string path2 = @"C:\Users\DELL\Desktop\GPSData\"+name+"RangeData09.txt";
            //button3.Enabled = false;
            var chart = chart1.ChartAreas[0];
            chart1.Titles[0].Text = "TECA与TECR拟合 GPS" + name;
            //var chart2 = chart1.ChartAreas.Add("TECR");
            ///一区
            chart.AxisX.IntervalType = DateTimeIntervalType.Number;
            chart.AxisX.LabelStyle.Format = "";
            chart.AxisY.LabelStyle.Format = "";
            chart.AxisY.LabelStyle.IsEndLabelVisible = true;
            chart.AxisX.Minimum = 0;
            chart.AxisX.Maximum = 700;
            //chart.AxisY.Minimum = 1*9.52;            
            //chart.AxisY.Maximum = 15*9.52;
            chart.AxisX.Interval = 20;
            //chart.AxisY.Interval = 9.52;
            var data = chart1.Series[0];
            data.ChartType = SeriesChartType.Spline;
            data.IsVisibleInLegend = false;
            /////二区
            //chart2.AxisX.IntervalType = DateTimeIntervalType.Number;
            //chart2.AxisX.LabelStyle.Format = "";
            //chart2.AxisY.LabelStyle.Format = "";
            //chart2.AxisY.LabelStyle.IsEndLabelVisible = true;
            //chart2.AxisX.Minimum = 0;
            //chart2.AxisX.Maximum = 500;
            //chart2.AxisY.Minimum = -0.6*279.2;           
            //chart2.AxisY.Maximum = -0.1*279.2;
            //chart2.AxisX.Interval = 10;
            //chart2.AxisY.Interval = 0.1*279.2;
            //var data2 = chart1.Series.Add("Series2");
            var data2 = chart1.Series[1];
            data2.BorderWidth = 2;
            data2.ChartArea = "ChartArea1";
            data2.ChartType = SeriesChartType.Spline;
            data2.IsVisibleInLegend = false;
            //待整理
            FileStream fs = new FileStream(path1, FileMode.Open, FileAccess.Read);
            FileStream fs2 = new FileStream(path2, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);//L1
            StreamReader sr2 = new StreamReader(fs2);//L2
            List<double> allMark = new List<double>();//记录断点处的标记
            List<double> allPc = new List<double>();//记录断点处的pc
            double mark = 0.5;//设置横坐标变量，待改动
            double psr, adr;
            double pc=0; //记录偏差值
            string line1,line2;//获取行值作暂时储存
            string[] stringArray1 , old1;//储存分割后的行值
            string[] stringArray2, old2;
            DateTime newtime, oldtime;
            char[] charArray = new char[] { ',' };//用于分割的标记
            line1 = sr.ReadLine();
            line2 = sr2.ReadLine();
            //先读一次新建Old的内容
            old1 = line1.Split(charArray);//储存上一次存入的数据，用于与新的比较
            old2 = line2.Split(charArray);
            oldtime = Convert.ToDateTime(old1[0]);
            oldtime = MakeSame(old1[0], old2[0], sr, sr2);
            //psr= (Convert.ToDouble(old2[2]) - Convert.ToDouble(old1[2]))*9.52;
            //adr = (Convert.ToDouble(old2[3]) / 120 - Convert.ToDouble(old1[3]) / 154) * 279.2;
            //data.Points.AddXY(0.5, psr);
            //data2.Points.AddXY(0.5, adr);
            while (mark<700)//待改
            {
                line1 = sr.ReadLine();
                line2 = sr2.ReadLine();
                if (line1 == null || line2 == null)
                {
                    break;
                }
                stringArray1 = line1.Split(charArray);
                stringArray2 = line2.Split(charArray);
                if (IfTogether(stringArray1[0], old1[0]))
                {
                    allMark.Add(mark);
                    allPc.Add(pc);
                    pc = 0;
                    mark += 25;//下面还有个25
                    old1 = stringArray1;
                    oldtime = Convert.ToDateTime(old1[0]);
                }
                newtime = Convert.ToDateTime(stringArray1[0]);
                if (stringArray1[0] != stringArray2[0])
                {
                    oldtime = MakeSame(stringArray1[0], stringArray2[0], sr, sr2);
                }
                //if (mark == 176)
                //{

                //}
                if (newtime.Second == oldtime.AddSeconds(20).Second)
                {
                    
                    psr = (Convert.ToDouble(stringArray2[2]) - Convert.ToDouble(stringArray1[2])) * 9.52;
                    adr = (Convert.ToDouble(stringArray2[3]) / 120 - Convert.ToDouble(stringArray1[3]) / 154) * 279.2;
                    if((psr<10000 && psr > -10000) || (adr < 10000 && adr > -10000))//有一种很特殊的情况导致错误，这里用了简单解决方法
                    {
                        mark += 0.5;
                        pc += (psr - adr);
                        data.Points.AddXY(mark, psr);
                        data2.Points.AddXY(mark, adr);
                        old1 = stringArray1;
                        old2 = stringArray2;
                        oldtime = Convert.ToDateTime(old1[0]);
                    }
                }                
            }
            allMark.Add(mark);
            allPc.Add(pc);
            textBox1.Text = "卫星过境次数:" + allMark.Count+"\r\n";
            textBox1.AppendText("分别结束在横坐标:");
            foreach(var k in allMark)
            {
                textBox1.AppendText(k + "  ");
            }
            double tmp;
            int start = 0;
            for(int i=0;i<allMark.Count;i++)//allMark[]为每轮结尾值,i+1为轮数
            {
                if (i == 0)//作各段均值运算
                    pc = allPc[i] / (allMark[0] * 2);
                else
                {
                    pc = allPc[i] / ((allMark[i] - allMark[i - 1] - 25) * 2);
                }
                for ( ; start < (allMark[i] - 25 * i )* 2-1; start ++)//作加均值运算
                {
                    tmp = data2.Points[start].YValues[0];
                    data2.Points[start].SetValueY(tmp + pc);
                }
            }
        }
        //使时间同步
        private DateTime MakeSame(string time1,string time2,StreamReader sr1,StreamReader sr2)
        {
            DateTime timeA = Convert.ToDateTime(time1);
            DateTime timeB = Convert.ToDateTime(time2);
            string line;
           // TimeSpan n;//差值
            if (timeA > timeB)
            {
                for(; ; )
                {
                    if (timeB < timeA)
                    {
                        line=sr2.ReadLine();
                        timeB = Convert.ToDateTime(line.Split(',')[0]);
                    }
                    else
                    {
                        break;
                    }
                }
                return timeA;
            }
            else
            {
                for (; ; )
                {
                    if (timeB > timeA)
                    {
                        line = sr1.ReadLine();
                        timeA = Convert.ToDateTime(line.Split(',')[0]);
                    }
                    else
                    {
                        break;
                    }
                }
                return timeB;
            }
        }
        /// <summary>
        /// 判断时间是否跳跃
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <returns></returns>
        private bool IfTogether(string time1, string time2)
        {
            DateTime timeA = Convert.ToDateTime(time1);
            DateTime timeB = Convert.ToDateTime(time2);
            if (timeA.Hour != timeB.Hour && timeA.Hour != timeB.AddHours(1).Hour)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            name = (comboBox1.SelectedIndex+1).ToString();
        }

        private void Up_Click(object sender, EventArgs e)
        {
            int n = Convert.ToInt32(name);
            if (n != 1 && n!=5)
            {
                n--;
                name = n.ToString();
                Button3_Click(this,new EventArgs());
            }
            else if(n==1)
            {
                MessageBox.Show("已经是第一个了~");
            }
            else
            {
                n = n - 2;
                name = n.ToString();
                Button3_Click(this, new EventArgs());
            }
        }

        private void Next_Click(object sender, EventArgs e)
        {
            int n = Convert.ToInt32(name);
            if (n != 32 && n != 3)
            {
                n++;
                name = n.ToString();
                Button3_Click(this, new EventArgs());
            }
            else if (n == 32)
            {
                MessageBox.Show("已经是最后一个了~");
            }
            else
            {
                n = n + 2;
                name = n.ToString();
                Button3_Click(this, new EventArgs());
            }
        }

        private void RadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                chart1.Visible = true;
            }
            else
            {
                chart1.Visible = false;
            }
        }

        private void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                textBox1.Visible = true;
            }
            else
            {
                textBox1.Visible = false;
            }

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog path = new OpenFileDialog();
            path.ShowDialog();
            string lu = path.FileName;
            FileStream fs;
            fs = new FileStream(lu, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            Head head;
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                head = ReadingLibrary.ReadHeadForUI(fs, br,textBox1);
                if (head.MessageID == 42)
                {
                    BP test1 = new BP();
                    test1 = ReadingLibrary.ReadBestPosForUI(fs, br, test1,textBox1);
                }
                else if (head.MessageID == 43)
                {
                    RA test2 = new RA();
                    test2 = ReadingLibrary.ReadObs(br);
                    for (uint i = 0; i < test2.obs; i++)
                    {
                        textBox1.AppendText("No." + i + ":"+"\r\n");
                        test2 = ReadingLibrary.ReadRangeForUI(fs, br, test2,textBox1);
                        //将数据分类写入txt中
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
                    ReadingLibrary.FindNextHead(fs, br);
                }
             }
             br.Close();
             fs.Close();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog path = new OpenFileDialog();
            path.ShowDialog();
            string lu = path.FileName;
            FileStream fs;
            fs = new FileStream(lu, FileMode.Open, FileAccess.Read);
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
                head = ReadingLibrary.ReadHeadForUI(fs, br, textBox1);
                if (head.MessageID == 42)
                {
                    BP test1 = new BP();
                    test1 = ReadingLibrary.ReadBestPosForUI(fs, br, test1, textBox1);
                }
                else if (head.MessageID == 43)
                {
                    RA test2 = new RA();
                    test2 = ReadingLibrary.ReadObs(br);
                    for (uint i = 0; i < test2.obs; i++)
                    {
                        textBox1.AppendText("No." + i + ":" + "\r\n");
                        test2 = ReadingLibrary.ReadRangeForUI(fs, br, test2, textBox1);
                        //将数据分类写入txt中
                        switch (test2.system.Last() + test2.s_type.Last())
                        {
                            case 0: sw00.WriteLine(head.UTC + "," + test2.PRN + "," + test2.psr.Last() + "," + test2.adr.Last()); break;
                            case 9: sw09.WriteLine(head.UTC + "," + test2.PRN + "," + test2.psr.Last() + "," + test2.adr.Last()); break;
                            case 1: sw10.WriteLine(head.UTC + "," + test2.PRN + "," + test2.psr.Last() + "," + test2.adr.Last()); break;
                            case 6: sw15.WriteLine(head.UTC + "," + test2.PRN + "," + test2.psr.Last() + "," + test2.adr.Last()); break;
                            default: throw new Exception("有未知量！！！");
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
                    ReadingLibrary.FindNextHead(fs, br);
                }
            }
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
            for (int n = 1; n < 33; n++)
            {
                ReadingLibrary.SpRange09(n.ToString());
                ReadingLibrary.SpRange00(n.ToString());
            }
            MessageBox.Show("输出成功！");
        }
    }
}

