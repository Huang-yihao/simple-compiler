using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Compiler;

namespace Compiler
{
    public partial class Form2 : Form
    {
        public Node tree;
        public Stack<Node> treestack;
        public Form2(Node treeroot, Stack<Node> stack)
        {
            InitializeComponent();
            tree = treeroot;
            treestack = stack;
        }


        private void form_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {

            Graphics g = this.CreateGraphics();
            g.Clear(Color.White);
            //画出最上方那个点,即根节点
            tree.positionX = this.Width / 2;
            tree.positionY = 0;
            g.DrawString(Convert.ToString(tree.layer),
                 new Font("Arial", 10), System.Drawing.Brushes.Blue, new Point(0, tree.positionY + 5));
            //画program
            /*g.DrawString(tree.type,
                  new Font("Arial", 12), System.Drawing.Brushes.Purple, new Point(tree.positionX, tree.positionY - 5));*/
            //计算叶子个数
            int totalleafcount = 0;
            drawLeafFirstCal(ref totalleafcount, tree, g);
            //叶子画树计算位置
            drawLeafFirstLocation(totalleafcount, tree, g);
            //画树
            drawLeafFirst(tree, g);

            //画树
            //drawTreeDivide(tree, g); 

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            //一打开界面就显示树形图
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.form_Paint);

        }
        /*
                public int[] layercal(Node n)
                {
                    //用数组k[]来记录每层的节点个数并算出每个节点在该层的位置
                    int[] k = new int[n.layerheight + 1];
                    k[0] = 1;
                    //初始化k[]
                    for (int m = 1; m < n.layerheight + 1; m++)
                    {
                        k[m] = 0;
                    }
                    Queue<Node> queue = new Queue<Node>(200);
                    queue.Enqueue(n);
                    //广度遍历树
                    while (queue.Count != 0)
                    {
                        Node tmp1 = queue.Dequeue();
                        if (tmp1.hasChild())
                        {
                            for (int i = 0; i < tmp1.getChilds().Count; i++)
                            {
                                //该层节点数加1
                                k[tmp1.getChilds()[i].layer]++;
                                //每个节点在该层的位置
                                tmp1.getChilds()[i].layercount = k[tmp1.getChilds()[i].layer];
                                queue.Enqueue(tmp1.getChilds()[i]);
                            }
                        }
                    }
                    return k;
                }

                //等分画树
                public void drawTreeDivide(Node n, Graphics g)
                {
                    int[] k = layercal(tree);

                    //------------画树
                    if (n.hasChild())
                    {
                        int size = n.getChilds().Count;
                        for (int i = 0; i < size; i++)
                        {
                            //获得每层的间隔
                            int x = this.Width / (k[n.getChilds()[i].layer] + 1);
                            int y = this.Height / (tree.layerheight + 1) * (n.getChilds()[i].layer);
                            n.getChilds()[i].positionX = x * n.getChilds()[i].layercount;
                            n.getChilds()[i].positionY = y;
                            g.DrawString(Convert.ToString(n.getChilds()[i].layer),
                            new Font("Arial", 10), System.Drawing.Brushes.Blue, new Point(0, y));
                            g.DrawString(n.getChilds()[i].type,
                            new Font("Arial", 12), System.Drawing.Brushes.Purple, new Point(x * n.getChilds()[i].layercount, y - 10));
                            drawTreeDivide(n.getChilds()[i], g);
                            g.DrawLine(Pens.Blue, new Point(n.positionX, n.positionY), new Point(x * n.getChilds()[i].layercount, y));

                        }

                    }
                }
        */
        //先计算叶子节点个数
        public void drawLeafFirstCal(ref int totalleafcount, Node n, Graphics g)
        {
            if (n.hasChild())
            {
                int size = n.getChilds().Count;
                for (int i = 0; i < size; i++)
                {
                    drawLeafFirstCal(ref totalleafcount, n.getChilds()[i], g);
                }
            }
            else
            {
                totalleafcount++;
                n.leafcount = totalleafcount;
            }
        }






        public void drawLeafFirstLocation(int totalleafcount, Node n, Graphics g)
        {
            if (n.hasChild())
            {
                int ntotalX = 0;
                int size = n.getChilds().Count;
                for (int i = 0; i < size; i++)
                {
                    drawLeafFirstLocation(totalleafcount, n.getChilds()[i], g);
                    ntotalX = ntotalX + n.getChilds()[i].positionX;
                }
                n.positionX = ntotalX / n.getChilds().Count;
                n.positionY = this.Height / (tree.layerheight + 1) * n.layer;
            }
            else
            {
                n.positionX = this.Width / (totalleafcount + 1) * n.leafcount;
                n.positionY = this.Height / (tree.layerheight + 1) * n.layer;
            }
        }


        //先画叶子节点的方法画树
        public void drawLeafFirst(Node n, Graphics g)
        {
            if (n.hasChild())
            {
                int size = n.getChilds().Count;
                for (int i = 0; i < size; i++)
                {
                    int m = judge(n.getChilds()[i], treestack);
                    if (m == 1)
                    {
                        drawLeafFirst(n.getChilds()[i], g);
                        //行号
                        g.DrawString(Convert.ToString(n.getChilds()[i].layer),
                        new Font("Arial", 10), System.Drawing.Brushes.Blue, new Point(0, n.getChilds()[i].positionY));
                        //字符串
                        g.DrawString(n.getChilds()[i].name + "," + n.getChilds()[i].type + "," + n.getChilds()[i].value,
                        new Font("Arial", 12), System.Drawing.Brushes.Purple, new Point(n.getChilds()[i].positionX, n.getChilds()[i].positionY - 10));
                        //线
                        g.DrawLine(Pens.Blue, new Point(n.positionX, n.positionY), new Point(n.getChilds()[i].positionX, n.getChilds()[i].positionY));
                    }
                    else
                    {
                        drawLeafFirst(n.getChilds()[i], g);
                        //行号
                        g.DrawString(Convert.ToString(n.getChilds()[i].layer),
                        new Font("Arial", 10), System.Drawing.Brushes.Blue, new Point(0, n.getChilds()[i].positionY));
                        //字符串
                 /*       g.DrawString(n.getChilds()[i].symbolname + "," + n.getChilds()[i].symboltype + "," + n.getChilds()[i].symbolvalue,// + "," + n.getChilds()[i].lineNo + "," + n.getChilds()[i].columnNo + "," + n.getChilds()[i].symboltype,
                          new Font("Arial", 12), System.Drawing.Brushes.Green, new Point(n.getChilds()[i].positionX, n.getChilds()[i].positionY - 10));*/
                        //线
                        g.DrawLine(Pens.Orange, new Point(n.positionX, n.positionY), new Point(n.getChilds()[i].positionX, n.getChilds()[i].positionY));
                    }

                }
            }
        }

        int judge(Node n, Stack<Node> treestack)
        {
            int m = 0;
            Stack<Node> temp = new Stack<Node>();
            int k = treestack.Count;
            for (int i = 0; i < k; i++)
            {
                if (n == treestack.Peek())
                    m = 1;
                temp.Push(treestack.Pop());
            }
            for (int i = 0; i < k; i++)
            {
                treestack.Push(temp.Pop());
            }
            return m;
        }






    }
}
