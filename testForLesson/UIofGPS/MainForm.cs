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
        //待解决：用DateTime优化
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
            chart.AxisX.Maximum = 600;
            //chart.AxisY.Minimum = 1*9.52;            
            //chart.AxisY.Maximum = 15*9.52;
            chart.AxisX.Interval = 10;
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
            double mark = 0.5;//设置横坐标变量，待改动
            double psr, adr;
            double pc=0; //记录偏差值
            string line1,line2;//获取行值作暂时储存
            string[] stringArray1 , old1;//储存分割后的行值
            string[] stringArray2, old2;
            string[] news, olds;
            char[] charArray = new char[] { ',' };//用于分割的标记
            line1 = sr.ReadLine();
            line2 = sr2.ReadLine();
            //先读一次新建Old的内容
            old1 = line1.Split(charArray);//储存上一次存入的数据，用于与新的比较
            old2 = line2.Split(charArray);
            olds = old1[0].Split(':');
            olds[2] = MakeSame(old1[0], old2[0], sr, sr2);
            //psr= (Convert.ToDouble(old2[2]) - Convert.ToDouble(old1[2]))*9.52;
            //adr = (Convert.ToDouble(old2[3]) / 120 - Convert.ToDouble(old1[3]) / 154) * 279.2;
            //data.Points.AddXY(0.5, psr);
            //data2.Points.AddXY(0.5, adr);
            while (mark<600)//待改
            {
                line1 = sr.ReadLine();
                line2 = sr2.ReadLine();
                if (line1 == null || line2 == null)
                {
                    break;
                }
                stringArray1 = line1.Split(charArray);
                stringArray2 = line2.Split(charArray);
                news = stringArray1[0].Split(':');
                if (stringArray1[0] != stringArray2[0])
                {
                    olds[2]=MakeSame(stringArray1[0], stringArray2[0], sr, sr2);
                }
                //if (mark == 490)
                //{

                //}
                if (Convert.ToInt32(news[2]) == AddThirty(olds[2]))
                {
                    mark += 0.5;
                    psr = (Convert.ToDouble(stringArray2[2]) - Convert.ToDouble(stringArray1[2])) * 9.52;
                    adr = (Convert.ToDouble(stringArray2[3]) / 120 - Convert.ToDouble(stringArray1[3]) / 154) * 279.2;
                    pc += (psr - adr);
                    data.Points.AddXY(mark, psr);
                    data2.Points.AddXY(mark, adr);
                    old1 = stringArray1;
                    old2 = stringArray2;
                    olds = old1[0].Split(':');
                }                
            }
            pc = pc / (mark*2);
            double tmp;
            foreach(DataPoint add in data2.Points)
            {
                tmp = add.YValues[0];
                add.SetValueY(tmp + pc);
            }
        }
        //加时器
        private int AddThirty(string old)
        {
            int oldt;
            oldt = Convert.ToInt32(old) + 20;
            if (oldt >= 60)
            {
                oldt = oldt - 60;
            }
            return oldt;
        }
        //使时间同步
        private string MakeSame(string time1,string time2,StreamReader sr1,StreamReader sr2)
        {
            DateTime timeA = Convert.ToDateTime(time1);
            DateTime timeB = Convert.ToDateTime(time2);
            TimeSpan n;//差值
            if (timeA > timeB)
            {
                n = timeA - timeB;
                double cha = n.TotalSeconds;
                for (int m = 0; m < cha; m++)
                {
                    sr2.ReadLine();
                }
                return timeA.Second.ToString();
            }
            else
            {
                n = timeB - timeA;
                double cha = n.TotalSeconds;
                for (int m = 0; m < cha; m++)
                {
                    sr1.ReadLine();
                }
                return timeB.Second.ToString();
            }
            //if(timeA==0 && timeB > timeA && minA!=minB)
            //{
            //    for (int m = 0; m < 60-timeB; m++)
            //    {
            //        sr2.ReadLine();
            //    }
            //    return timeA.ToString();
            //}
            //else if(timeB==0 && timeA > timeB && minA!=minB)
            //{
            //    for (int m = 0; m < 60 - timeA; m++)
            //    {
            //        sr1.ReadLine();
            //    }
            //    return timeB.ToString();
            //}
            //else if (timeA > timeB)
            //{
            //    n = timeA - timeB;
            //    for(int m = 0; m < n; m++)
            //    {
            //        sr2.ReadLine();
            //    }
            //    return timeA.ToString();
            //}
            //else 
            //{
            //    n = timeB - timeA;
            //    for (int m = 0; m < n; m++)
            //    {
            //        sr1.ReadLine();
            //    }
            //    return timeB.ToString();
            //}
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
            FileStream fs;
            fs = new FileStream("data.gps", FileMode.Open, FileAccess.Read);
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
    }
}

