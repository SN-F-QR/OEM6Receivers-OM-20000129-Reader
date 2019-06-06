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

namespace UIofGPS
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void 输出选择_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            var chart = chart1.ChartAreas[0];
            //var chart2 = chart1.ChartAreas.Add("TECR");
            ///一区
            chart.AxisX.IntervalType = DateTimeIntervalType.Number;
            chart.AxisX.LabelStyle.Format = "";
            chart.AxisY.LabelStyle.Format = "";
            chart.AxisY.LabelStyle.IsEndLabelVisible = true;
            chart.AxisX.Minimum = 0;
            chart.AxisX.Maximum = 500;
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
            var data2 = chart1.Series.Add("Series2");
            data2.ChartArea = "ChartArea1";
            data2.ChartType = SeriesChartType.Spline;
            data2.IsVisibleInLegend = false;
            //待整理
            FileStream fs = new FileStream(@"C:\Users\DELL\Desktop\GPSData\26RangeData00.txt", FileMode.Open, FileAccess.Read);
            FileStream fs2 = new FileStream(@"C:\Users\DELL\Desktop\GPSData\26RangeData09.txt", FileMode.Open, FileAccess.Read);
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
            psr= (Convert.ToDouble(old2[2]) - Convert.ToDouble(old1[2]))*9.52;
            adr = (Convert.ToDouble(old2[3]) / 120 - Convert.ToDouble(old1[3]) / 154) * 279.2;
            data.Points.AddXY(0.5, psr);
            data2.Points.AddXY(0.5, adr);
            while (mark<600)//待改
            {
                line1 = sr.ReadLine();
                line2 = sr2.ReadLine();
                if (line1 == null)
                {
                    break;
                }
                stringArray1 = line1.Split(charArray);
                stringArray2 = line2.Split(charArray);
                news = stringArray1[0].Split(':');
                if (stringArray1[0] != stringArray2[0])
                {
                    string[] tb = stringArray2[0].Split(':');
                    olds[2]=MakeSame(news, tb, sr, sr2);                   
                }
                //if (mark == 490)
                //{

                //}
                if (Convert.ToInt32(news[2]) == AddThirty(olds[2]) || news[2]==olds[2])
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
        //使得时间统一（无法解决快了1min）
        private string MakeSame(string[] time1,string[] time2,StreamReader sr1,StreamReader sr2)
        {
            int minA = Convert.ToInt32(time1[1]);
            int minB = Convert.ToInt32(time2[1]);
            int timeA = Convert.ToInt32(time1[2]);
            int timeB = Convert.ToInt32(time2[2]);
            int n;//差值
            if(timeA==0 && timeB > timeA && minA!=minB)
            {
                for (int m = 0; m < 60-timeB; m++)
                {
                    sr2.ReadLine();
                }
                return timeA.ToString();
            }
            else if(timeB==0 && timeA > timeB && minA!=minB)
            {
                for (int m = 0; m < 60 - timeA; m++)
                {
                    sr1.ReadLine();
                }
                return timeB.ToString();
            }
            else if (timeA > timeB)
            {
                n = timeA - timeB;
                for(int m = 0; m < n; m++)
                {
                    sr2.ReadLine();
                }
                return timeA.ToString();
            }
            else 
            {
                n = timeB - timeA;
                for (int m = 0; m < n; m++)
                {
                    sr1.ReadLine();
                }
                return timeB.ToString();
            }
        }
    }
}
