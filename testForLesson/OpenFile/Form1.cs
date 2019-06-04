using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace OpenFile
{
    public partial class Form1 : Form
    {
        private const string defaultText="请输入...";
        static string lu;
        public static string name;
        public Form1()
        {          
            InitializeComponent();
            SetDefaultText();
            textBox1.LostFocus += new EventHandler(txt_LostFocus);
            textBox1.GotFocus += new EventHandler(txt_GotFocus);
            
        }
        private void SetDefaultText()
        {
            textBox1.Text = defaultText;
            textBox1.ForeColor = Color.Gray;
        }
        private void txt_GotFocus(object sender,EventArgs e)
        {
            if (textBox1.Text == defaultText)
            {
                textBox1.Clear();
                textBox1.ForeColor = Color.Black;
            }
        }
        private void txt_LostFocus(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text))
            {
                SetDefaultText();
            }
        }

        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog path = new OpenFileDialog();
            path.ShowDialog();
            lu = path.FileName;
            FileStream fs = new FileStream(lu, FileMode.Open,FileAccess.ReadWrite);
            BinaryReader br = new BinaryReader(fs);
            //double num = br.ReadDouble();
            string num2 = br.ReadString();
            //char[] item = new char[3];
            //for(int i = 0; i <= 2; i++)
            //{
               // item[i] = br.ReadChar();
           // }
            //textBox1.AppendText(num.ToString());
            textBox1.AppendText(num2);
            //for (int i = 0; i <= 2; i++)
            //{
            //    textBox1.AppendText(item[i].ToString());
            //}
            br.Close();
            fs.Close();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            FileStream fs = new FileStream(lu, FileMode.Truncate, FileAccess.ReadWrite);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(textBox1.Text);
            MessageBox.Show("写入成功");
            bw.Close();
            fs.Close();
        }

        private void 创建ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            //f2.MdiParent = this;
            f2.ShowDialog();//先执行小窗口
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.ShowDialog();
            lu = path.SelectedPath + "\\" + name+".bin";
            FileStream fs = new FileStream(lu, FileMode.Create, FileAccess.ReadWrite);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write("Created in 2019");
            MessageBox.Show("创建成功");
            bw.Close();
            fs.Close();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
