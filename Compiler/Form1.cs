using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compiler
{
    public partial class Form1 : Form
    {
        string[] threeaddress;
        int threeaddresscount;
        int count;
        public Form1(string[] a,int k)
        {
            InitializeComponent();
            threeaddress = a;
            threeaddresscount = k;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 1; i < threeaddresscount; i++)
                textBox1.Text = textBox1.Text + threeaddress[i] + "\r\n";    
        }
    }
}
