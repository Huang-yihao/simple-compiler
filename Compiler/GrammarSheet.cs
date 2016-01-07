using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    //语法表
    //语法表行为终结符，即二维表上边部分
    //语法表列为非终结符，即二维表左边部分
    //语法表单元为产生式，即二维表中间部分


    //node类型存放节点用于构造树
    public class Node
    {
        public String index;    //该节点储存的token的序号
        public String name;     //该节点储存的token的名字
        public String type;     //该节点储存的token的类型
        public String value;    //该节点储存的token的值
        public String lineNo;   //该节点储存的token的行号
        public String columnNo; //该节点储存的token的列号
        public String symboltype;
        public String symbolvalue;
        public String symbolname;

        public int positionX = 0;//节点在画图时x坐标
        public int positionY = 0;//节点在画图时y坐标
        public int layercount = 0;//节点在画图时所在层的位置
        public int leafcount = 0;//每个叶子节点在总叶子数中的位置

        public int layer = 0;    //在画图时该结点层级
        public int layerheight = 0;//计算该节点所在树的高
        private List<Node> childs = null;    //保存该结点的孩子
        public Node(String name, String type, String value, String lineNo, String columnNo)
        {
            this.name = name;
            this.type = type;
            this.value = value;
            this.lineNo = lineNo;
            this.columnNo = columnNo;
            this.symbolname = name;
            this.symboltype = type;
            this.symbolvalue = value;
        }
        //加子节点
        public void Add(Node n)
        {
            if (childs == null)
                childs = new List<Node>();
            childs.Add(n);
        }
        public bool hasChild()
        {
            return childs == null ? false : true;
        }
        public List<Node> getChilds()
        {
            return childs;
        }

    }


    public class GrammarSheet
    {
        //判断代码是否有错,有错就不做语义和符号表
        public bool programright = true;
        //成员变量
        public Stack<string> symbol_stack = new Stack<string>();     //符号栈
        public Stack<string[]> input_stack = new Stack<string[]>();      //输入栈

        public Stack<Node> treestack = new Stack<Node>();//treestack和a相同,就多个指针
        //初始化list和stack所用的Node
        public Node headfore = new Node("", "$", "", "", "");
        public Node treeroot = new Node("", "program", "", "", "");

        public string[] process = new string[1000];//用于输出到过程框
        public string[] input = new string[1000];//用于输出到输入框
        public string[] action = new string[1000];//用于输出到动作框
        public string errorshow = "";//错误显示
        int outcount = 0;//计算process,input,action的序号

        string[] grammarSheetRow = new string[25];           //语法表25列终结符
        string[] grammarSheetColumn = new string[18];        //语法表18行非终结符
        Production[,] grammarSheetUnit = new Production[18, 25];

        //构造函数
        public GrammarSheet()
        {
            Grammarsheet_Initialize();
            //每次打开文件都会调用rebuilt()，所以栈不用初始化
        }

        //语法表初始化
        public void Grammarsheet_Initialize()
        {

            Production[] productionRule = new Production[36];			 //语法表达式
            //语法表达式初始化
            Production.Production_Initialize(productionRule);


            //初始化表格,先把表格中所有单元设为字符串error
            for (int m = 0; m < 18; m++)
                for (int n = 0; n < 25; n++)
                {
                    grammarSheetUnit[m, n] = new Production();
                    grammarSheetUnit[m, n].productionSubstring[0] = "error";
                }

            //初始化列
            grammarSheetColumn[0] = "program";
            grammarSheetColumn[1] = "stmt";
            grammarSheetColumn[2] = "compoundstmt";
            grammarSheetColumn[3] = "stmts";
            grammarSheetColumn[4] = "ifstmt";
            grammarSheetColumn[5] = "whilestmt";
            grammarSheetColumn[6] = "assgstmt";
            grammarSheetColumn[7] = "decl";
            grammarSheetColumn[8] = "type";
            grammarSheetColumn[9] = "list";
            grammarSheetColumn[10] = "list1";
            grammarSheetColumn[11] = "boolexpr";
            grammarSheetColumn[12] = "boolop";
            grammarSheetColumn[13] = "arithexpr";
            grammarSheetColumn[14] = "arithexprprime";
            grammarSheetColumn[15] = "multexpr";
            grammarSheetColumn[16] = "multexprprime";
            grammarSheetColumn[17] = "simpleexpr";
            //初始化行
            grammarSheetRow[0] = "{";
            grammarSheetRow[1] = "}";
            grammarSheetRow[2] = "(";
            grammarSheetRow[3] = ")";
            grammarSheetRow[4] = "if";
            grammarSheetRow[5] = "then";
            grammarSheetRow[6] = "else";
            grammarSheetRow[7] = "while";
            grammarSheetRow[8] = "int";
            grammarSheetRow[9] = "real";
            grammarSheetRow[10] = "ID";
            grammarSheetRow[11] = "NUM";
            grammarSheetRow[12] = ",";
            grammarSheetRow[13] = ";";
            grammarSheetRow[14] = "+";
            grammarSheetRow[15] = "-";
            grammarSheetRow[16] = "*";
            grammarSheetRow[17] = "/";
            grammarSheetRow[18] = "=";
            grammarSheetRow[19] = "<";
            grammarSheetRow[20] = ">";
            grammarSheetRow[21] = "<=";
            grammarSheetRow[22] = ">=";
            grammarSheetRow[23] = "==";
            grammarSheetRow[24] = "$";

            //把生成式填入其中
            //-------------------------------------第一行
            grammarSheetUnit[0, 0] = productionRule[1];
            grammarSheetUnit[0, 24] = productionRule[0];
            //-------------------------------------第二行
            grammarSheetUnit[1, 0] = productionRule[6];
            grammarSheetUnit[1, 4] = productionRule[3];
            grammarSheetUnit[1, 7] = productionRule[4];
            grammarSheetUnit[1, 8] = productionRule[2];
            grammarSheetUnit[1, 9] = productionRule[2];
            grammarSheetUnit[1, 10] = productionRule[5];
            grammarSheetUnit[1, 1] = productionRule[0];
            grammarSheetUnit[1, 6] = productionRule[0];
            //-------------------------------------第三行
            grammarSheetUnit[2, 0] = productionRule[7];
            grammarSheetUnit[2, 1] = productionRule[0];
            grammarSheetUnit[2, 4] = productionRule[0];
            grammarSheetUnit[2, 6] = productionRule[0];
            grammarSheetUnit[2, 7] = productionRule[0];
            grammarSheetUnit[2, 8] = productionRule[0];
            grammarSheetUnit[2, 9] = productionRule[0];
            grammarSheetUnit[2, 10] = productionRule[0];
            grammarSheetUnit[2, 24] = productionRule[0];
            //-------------------------------------第四行
            grammarSheetUnit[3, 0] = productionRule[8];
            grammarSheetUnit[3, 1] = productionRule[9];
            grammarSheetUnit[3, 4] = productionRule[8];
            grammarSheetUnit[3, 7] = productionRule[8];
            grammarSheetUnit[3, 8] = productionRule[8];
            grammarSheetUnit[3, 9] = productionRule[8];
            grammarSheetUnit[3, 10] = productionRule[8];
            //-------------------------------------第五行
            grammarSheetUnit[4, 4] = productionRule[10];
            grammarSheetUnit[4, 0] = productionRule[0];
            grammarSheetUnit[4, 1] = productionRule[0];
            grammarSheetUnit[4, 6] = productionRule[0];
            grammarSheetUnit[4, 7] = productionRule[0];
            grammarSheetUnit[4, 8] = productionRule[0];
            grammarSheetUnit[4, 9] = productionRule[0];
            grammarSheetUnit[4, 10] = productionRule[0];
            //-------------------------------------第六行
            grammarSheetUnit[5, 7] = productionRule[11];
            grammarSheetUnit[5, 0] = productionRule[0];
            grammarSheetUnit[5, 1] = productionRule[0];
            grammarSheetUnit[5, 4] = productionRule[0];
            grammarSheetUnit[5, 6] = productionRule[0];
            grammarSheetUnit[5, 8] = productionRule[0];
            grammarSheetUnit[5, 9] = productionRule[0];
            grammarSheetUnit[5, 10] = productionRule[0];
            //-------------------------------------第七行
            grammarSheetUnit[6, 10] = productionRule[12];
            grammarSheetUnit[6, 0] = productionRule[0];
            grammarSheetUnit[6, 1] = productionRule[0];
            grammarSheetUnit[6, 4] = productionRule[0];
            grammarSheetUnit[6, 6] = productionRule[0];
            grammarSheetUnit[6, 7] = productionRule[0];
            grammarSheetUnit[6, 8] = productionRule[0];
            grammarSheetUnit[6, 9] = productionRule[0];
            //-------------------------------------第八行
            grammarSheetUnit[7, 8] = productionRule[13];
            grammarSheetUnit[7, 9] = productionRule[13];
            grammarSheetUnit[7, 0] = productionRule[0];
            grammarSheetUnit[7, 1] = productionRule[0];
            grammarSheetUnit[7, 4] = productionRule[0];
            grammarSheetUnit[7, 6] = productionRule[0];
            grammarSheetUnit[7, 7] = productionRule[0];
            grammarSheetUnit[7, 10] = productionRule[0];
            //-------------------------------------第九行
            grammarSheetUnit[8, 8] = productionRule[14];
            grammarSheetUnit[8, 9] = productionRule[15];
            grammarSheetUnit[8, 10] = productionRule[0];
            //-------------------------------------第十行
            grammarSheetUnit[9, 10] = productionRule[16];
            grammarSheetUnit[9, 13] = productionRule[0];
            //-------------------------------------第十一行
            grammarSheetUnit[10, 12] = productionRule[17];
            grammarSheetUnit[10, 13] = productionRule[18];
            //-------------------------------------第十二行
            grammarSheetUnit[11, 2] = productionRule[19];
            grammarSheetUnit[11, 10] = productionRule[19];
            grammarSheetUnit[11, 11] = productionRule[19];
            grammarSheetUnit[11, 1] = productionRule[0];
            //-------------------------------------第十三行
            grammarSheetUnit[12, 19] = productionRule[20];
            grammarSheetUnit[12, 20] = productionRule[21];
            grammarSheetUnit[12, 21] = productionRule[22];
            grammarSheetUnit[12, 22] = productionRule[23];
            grammarSheetUnit[12, 23] = productionRule[24];
            grammarSheetUnit[12, 2] = productionRule[0];
            grammarSheetUnit[12, 10] = productionRule[0];
            grammarSheetUnit[12, 11] = productionRule[0];
            //-------------------------------------第十四行
            grammarSheetUnit[13, 2] = productionRule[25];
            grammarSheetUnit[13, 10] = productionRule[25];
            grammarSheetUnit[13, 11] = productionRule[25];
            grammarSheetUnit[13, 19] = productionRule[0];
            grammarSheetUnit[13, 20] = productionRule[0];
            grammarSheetUnit[13, 21] = productionRule[0];
            grammarSheetUnit[13, 22] = productionRule[0];
            grammarSheetUnit[13, 23] = productionRule[0];
            //-------------------------------------第十五行
            grammarSheetUnit[14, 3] = productionRule[28];
            grammarSheetUnit[14, 13] = productionRule[28];
            grammarSheetUnit[14, 14] = productionRule[26];
            grammarSheetUnit[14, 15] = productionRule[27];
            grammarSheetUnit[14, 19] = productionRule[28];
            grammarSheetUnit[14, 20] = productionRule[28];
            grammarSheetUnit[14, 21] = productionRule[28];
            grammarSheetUnit[14, 22] = productionRule[28];
            grammarSheetUnit[14, 23] = productionRule[28];
            //-------------------------------------第十六行
            grammarSheetUnit[15, 2] = productionRule[29];
            grammarSheetUnit[15, 10] = productionRule[29];
            grammarSheetUnit[15, 11] = productionRule[29];
            grammarSheetUnit[15, 3] = productionRule[0];
            grammarSheetUnit[15, 13] = productionRule[0];
            grammarSheetUnit[15, 14] = productionRule[0];
            grammarSheetUnit[15, 15] = productionRule[0];
            grammarSheetUnit[15, 19] = productionRule[0];
            grammarSheetUnit[15, 20] = productionRule[0];
            grammarSheetUnit[15, 21] = productionRule[0];
            grammarSheetUnit[15, 22] = productionRule[0];
            grammarSheetUnit[15, 23] = productionRule[0];
            //-------------------------------------第十七行
            grammarSheetUnit[16, 3] = productionRule[32];
            grammarSheetUnit[16, 13] = productionRule[32];
            grammarSheetUnit[16, 14] = productionRule[32];
            grammarSheetUnit[16, 15] = productionRule[32];
            grammarSheetUnit[16, 16] = productionRule[30];
            grammarSheetUnit[16, 17] = productionRule[31];
            grammarSheetUnit[16, 19] = productionRule[32];
            grammarSheetUnit[16, 20] = productionRule[32];
            grammarSheetUnit[16, 21] = productionRule[32];
            grammarSheetUnit[16, 22] = productionRule[32];
            grammarSheetUnit[16, 23] = productionRule[32];
            //-------------------------------------第十八行
            grammarSheetUnit[17, 2] = productionRule[35];
            grammarSheetUnit[17, 10] = productionRule[33];
            grammarSheetUnit[17, 11] = productionRule[34];
            grammarSheetUnit[17, 3] = productionRule[0];
            grammarSheetUnit[17, 13] = productionRule[0];
            grammarSheetUnit[17, 14] = productionRule[0];
            grammarSheetUnit[17, 15] = productionRule[0];
            grammarSheetUnit[17, 16] = productionRule[0];
            grammarSheetUnit[17, 17] = productionRule[0];
            grammarSheetUnit[17, 19] = productionRule[0];
            grammarSheetUnit[17, 20] = productionRule[0];
            grammarSheetUnit[17, 21] = productionRule[0];
            grammarSheetUnit[17, 22] = productionRule[0];
            grammarSheetUnit[17, 23] = productionRule[0];
        }

        //语法分析表输出,输入为两个字符串,分别为左右栈的栈顶字符串,输出为Production类型的生成式的表达式
        public string Grammarsheet_Return(string a, string b, GrammarSheet c)
        {
            string prostring = "";
            for (int m = 0; m < 18; m++)
            {
                for (int n = 0; n < 25; n++)
                {
                    if (c.grammarSheetColumn[m] == a && c.grammarSheetRow[n] == b)
                    {//如果匹配成功，输出产生式
                        for (int i = 0; i < 10; i++)
                        {
                            if (c.grammarSheetUnit[m, n].productionSubstring[i] != null)
                                prostring = prostring + c.grammarSheetUnit[m, n].productionSubstring[i] + " ";
                        }
                    }
                }
            }
            return prostring;
        }

        //语法分析表输出,输入为两个字符串,分别为左右栈的栈顶字符串,输出为Production类型的生成式
        Production Grammarsheet_Return_Production(string a, string b, GrammarSheet c)
        {
            for (int m = 0; m < 18; m++)
            {
                for (int n = 0; n < 25; n++)
                {
                    if (c.grammarSheetColumn[m] == a && c.grammarSheetRow[n] == b)
                    {
                        return c.grammarSheetUnit[m, n];
                    }
                }
            }
            return c.grammarSheetUnit[10, 0];//default情况,返回
        }

        //初始化过程栈
        public void Process_Stack_Initialize(ref Stack<Node> a)
        {
            //每次进入match函数,树根都要初始化
            headfore = new Node("", "$", "", "", "");
            treeroot = new Node("", "program", "", "", "");
            headfore.Add(treeroot);//list初始化
            //stack初始化
            a.Push(headfore);
            a.Push(treeroot);
        }

        //初始化输入栈
        public void Input_Stack_Initialize(ref Stack<string[]> a)
        {
            string[] m = new string[6];
            m[2] = "$";
            a.Push(m);
        }

        //一次压一个token
        public void Input_Stack_Push(string[] syntaxString)
        {
            input_stack.Push(syntaxString);
        }

        //过程栈和输入栈进行怼的过程,treestack为过程栈,b为输入栈
        public void match(Stack<string[]> b, GrammarSheet c, int nResult/*判定是否是文件结束*/)
        {
            int[] errorshowrow = new int[100];//显示错误所在的行
            c.outcount = -1;//每次进入match函数,序号初始化为-1


            //p,q用于输出两个栈的内容
            Stack<Node> p = new Stack<Node>();
            Stack<string[]> q = new Stack<string[]>();
            Stack<Node> r = new Stack<Node>();//用于反向输出的保存



            while (!(treestack.Count == 0))
            {
                c.outcount++;//process,inout,action的计数+1
                //清空process,input,action使得每多读一个token都不是在原来的输出中写内容
                c.process[outcount] = "";
                c.input[outcount] = "";
                c.action[outcount] = "";
                //p,q用于输出两个栈的内容
                p = new Stack<Node>(treestack);
                q = new Stack<string[]>(b);

                while (!(p.Count == 0))
                {
                    if (p.Peek().type != "empty")
                        r.Push(p.Peek());
                    p.Pop();
                }
                while (!(r.Count == 0))
                {
                    if (r.Peek().type != "")
                        c.process[c.outcount] = c.process[c.outcount] + r.Peek().type + " ";//用字符串m获取过程栈中字符串
                    r.Pop();
                }
                c.process[c.outcount] = c.process[c.outcount];//空一行
                //	printf("%-30s", m.c_str());
                while (!(q.Count == 0))
                {
                    if (q.Peek()[2] != "empty")
                        c.input[c.outcount] = c.input[c.outcount] + q.Peek()[2] + " ";//用字符串n获取输入栈中字符串
                    q.Pop();
                }
                c.input[c.outcount] = c.input[c.outcount];
                //	printf("%-30s", n.c_str());
                //把empty符号删除,因为empty表示空

                if (treestack.Peek().type == "empty")
                {
                    treestack.Pop();
                }
                if (b.Peek()[2] == "empty")
                {
                    b.Pop();
                }
                //如果某个栈只剩$了，而另一个不可能变成$,那么就结束并报错
                if ((treestack.Peek().type == "$" && b.Peek()[2] != "$") || (treestack.Peek().type != "$" && b.Peek()[2] == "$"))
                {
                    c.action[c.outcount] = c.action[c.outcount] + "该语句出错";
                    c.programright = false;
                    break;
                }
                //当两个栈的栈顶不同时
                if (treestack.Peek().type != b.Peek()[2])
                {

                    Production k = Grammarsheet_Return_Production(treestack.Peek().type, b.Peek()[2], c);

                    //假如能在符号表中找到生成式,说明分析过程还是正确的
                    if (k.productionSubstring[0] != "error" && k.productionSubstring[0] != "synch")
                    {
                        //构建临时list临时存放生成式的node，用于反向放入treestack中
                        List<Node> temp = new List<Node>();
                        //输出动作
                        c.action[c.outcount] = c.action[c.outcount] + "输出 ";
                        c.action[c.outcount] = c.action[c.outcount] + Grammarsheet_Return(treestack.Peek().type, b.Peek()[2], c);
                        //构建list
                        Production currentproduction = Grammarsheet_Return_Production(treestack.Peek().type, b.Peek()[2], c);
                        for (int i = 2; i < 10; i++)
                        {
                            if (currentproduction.productionSubstring[i] != "" && currentproduction.productionSubstring[i] != null)
                            {
                                //获取当前孩子节点中的某个
                                Node currentnode = new Node("", currentproduction.productionSubstring[i], "", "", "");
                                temp.Add(currentnode);//
                                //取栈顶作为父节点来加入他的孩子
                                Node tmpnode = treestack.Peek();
                                tmpnode.Add(currentnode);
                                //计算当前节点所在的层
                                currentnode.layer = tmpnode.layer + 1;
                                //计算list的高
                                if (currentnode.layer >= treeroot.layerheight)
                                    treeroot.layerheight = currentnode.layer;
                            }
                        }
                        treestack.Pop();
                        //treestack放入生成式节点
                        for (int i = temp.Count - 1; i >= 0; i--)
                        {
                            if (k.productionSubstring[i] != "" && k.productionSubstring[i] != null)
                            {
                                treestack.Push(temp[i]);
                            }
                        }

                        //清空临时list
                        temp.Clear();
                    }

                    //如果没有对应的生成式,则弹出输入栈中最上方的字符并报错
                    else if (k.productionSubstring[0] == "error")
                    {
                        //输出动作	
                        c.action[c.outcount] = c.action[c.outcount] + "出错";
                        c.errorshow = c.errorshow + "第" + b.Peek()[4] + "行 " + "缺少或多余" +found(b.Peek()[2],c)+ "\r\n" + "\r\n";
                        c.programright = false;
                        b.Pop();
                    }

                    //如果对应的生成式为错误处理,则弹出过程栈中最上方的非终结符并报错
                    else if (k.productionSubstring[0] == "synch")
                    {
                        //输出动作		
                        c.action[c.outcount] = c.action[c.outcount] + "出错";
                        c.errorshow = c.errorshow + "第" + b.Peek()[4] + "行 " + "缺少或多余" +found(b.Peek()[2],c)+ "\r\n" + "\r\n";
                        c.programright = false;
                        treestack.Pop();
                    }
                }

                //两个栈的栈顶相同
                else if (treestack.Peek().type == b.Peek()[2])
                {
                    treestack.Peek().name = b.Peek()[1];
                    treestack.Peek().symbolname = b.Peek()[1];
                    treestack.Peek().value = b.Peek()[3];
                    treestack.Peek().symbolvalue = b.Peek()[3];
                    treestack.Peek().lineNo = b.Peek()[4];
                    treestack.Peek().columnNo = b.Peek()[5];
                    if (treestack.Peek().type == "}")
                    {
                        c.action[c.outcount] = c.action[c.outcount] + "匹配 " + treestack.Peek().type + "该句子正确";
                    }
                    else
                    {
                        c.action[c.outcount] = c.action[c.outcount] + "匹配 " + treestack.Peek().type;
                    }

                    treestack.Pop();
                    b.Pop();
                }
            }
        }
        String found(String a,GrammarSheet c)
        {
            bool unendoccur = false;
            string less="";
            for (int m = 0; m < 18; m++)
            {
                for (int n = 0; n < 25; n++)
                {
                    if (c.grammarSheetColumn[m] == a )
                    {
                        unendoccur = true;
                        if (c.grammarSheetUnit[m, n].productionSubstring[0] != "error" && c.grammarSheetUnit[m, n].productionSubstring[0] != "synch")
                            less = less + c.grammarSheetUnit[m, n] + ",";
                    }
                }
            }
            if (!unendoccur)
                less = less + a;
            return less;
        }

        //common：释放gr，每次重新运行时都需要
        public void rebuilt()
        {
            symbol_stack.Clear();
            input_stack.Clear();
            treestack = new Stack<Node>();
            input_stack = new Stack<string[]>();
            Process_Stack_Initialize(ref treestack);
            Input_Stack_Initialize(ref input_stack);
        }

        public void rebuiltcompiler()
        {
            errorshow = "";
            process = new string[1000];//用于输出到过程框
            input = new string[1000];//用于输出到输入框
            action = new string[1000];//用于输出到动作框
            symbol_stack.Clear();
            input_stack.Clear();
            treestack = new Stack<Node>();
            input_stack = new Stack<string[]>();
            Process_Stack_Initialize(ref treestack);
            Input_Stack_Initialize(ref input_stack);
            programright = true;
        }
    }
}
