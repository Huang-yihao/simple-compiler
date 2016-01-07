using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//
using System.IO;
using System.Collections; 

namespace Compiler
{
    public partial class Compiler : Form
    {
        Lex myLex;  //词法分析类
        GrammarSheet gr;    //语法符号表
        Translation tr;//语义分析
        int nResult;    //用于判定是否终止
        string[] myString;  //用于传递单词属性
        string[] syntaxString; //传到语法单词属性
        bool fileChange;  //用于判定是否修改了代码
        bool fileChick;   //用于判定是否选中过代码
        bool fileTemp;    //用于判定是否移开了代码框
        bool filePress;
        bool fileLock;  //用于防止文件死锁
        string myfileText;  //用于记录原本代码内容
        string tmpFile;     //用于记录临时文件的地址
        Hashtable keywords1; //用于记录代码高亮中的关键字 int real
        Hashtable keywords2; //用于记录代码高亮中的关键字 if then else while
        int show = 0;
        Form2 f2;

        public Compiler()
        {
            InitializeComponent();  
        }

        private void Compiler_Load(object sender, EventArgs e)
        {
            myLex = new Lex(); //初始化
            gr = new GrammarSheet();
            tr = new Translation();
            nResult = 0;
            myString = new String[6];
            syntaxString = new String[6];
            fileChange = false;
            fileChick = false;
            filePress = false;
            fileLock = false;
            fileTemp = false;
            thisStatus.Text = "  程序加载成功";
            //高亮
            keywords1 = new Hashtable();
            keywords2 = new Hashtable();
            keywords1.Add("int", '1');
            keywords1.Add("real", '1');
            keywords2.Add("if", '1');
            keywords2.Add("then", '1');
            keywords2.Add("else", '1');
            keywords2.Add("while", '1');
        }

        //listView：显示一个单词详细信息
        private void PrintInLexisToListView(string[] myString)
        {
            this.listView_lex.BeginUpdate();

            this.listView_lex.Items.Add(myString[0],myString[0],0);
            this.listView_lex.Items[myString[0]].SubItems.Add(myString[1]);
            this.listView_lex.Items[myString[0]].SubItems.Add(myString[2]);
            if (String.Equals(myString[2], Convert.ToString(WORD_TYPE_ENUM.INTNUMBER)) || String.Equals(myString[2], Convert.ToString(WORD_TYPE_ENUM.REALNUMBER)))
                this.listView_lex.Items[myString[0]].SubItems.Add(myString[3]);
            else
                this.listView_lex.Items[myString[0]].SubItems.Add("");
            this.listView_lex.Items[myString[0]].SubItems.Add(myString[4]);
            this.listView_lex.Items[myString[0]].SubItems.Add(myString[5]);

            this.listView_lex.EndUpdate();
        }

        //listView：显示语法分析过程
        private void PrintInSyntaxToSyntax()
        {
            string tmp = Convert.ToString(myLex.myToken.g_nWordsIndex - 1);
            listView_syntax.Items.Clear();//清空显示界面中的listView_syntax中的数据

            for (int i = 0; gr.process[i] != null; i++)//把所得数据按序输出
            {
                this.listView_syntax.Items.Add(Convert.ToString(i), Convert.ToString(i), 0);
                this.listView_syntax.Items[Convert.ToString(i)].SubItems.Add(gr.input[i]);
                this.listView_syntax.Items[Convert.ToString(i)].SubItems.Add(gr.process[i]);
                this.listView_syntax.Items[Convert.ToString(i)].SubItems.Add(gr.action[i]);
            }
        }

        //listView: 显示符号表
        private void PrintSymbolToListView()
        {
            int i = 0; 
            while(i < tr.symbolTableCount)
            {
                string count = Convert.ToString(i);
                this.listView_symbol.Items.Add(count,count,0);//序号
                this.listView_symbol.Items[count].SubItems.Add(tr.symbolTableName[i]); //名称
                this.listView_symbol.Items[count].SubItems.Add(tr.symbolTableType[i]); //类型
                this.listView_symbol.Items[count].SubItems.Add(tr.symbolTableValue[i]); //数值
                this.listView_symbol.Items[count].SubItems.Add("第" + tr.symbolTableLineNo[i] + "行" + "  第" + tr.symbolTableColumnNo[i] + "列");
                i++;
            }
        
        }

        //button：打开文件
        private void toolStripButton_open_Click(object sender, EventArgs e)
        {
            //加载文件选择对话框
            if (this.openFileDialog_button.ShowDialog() == DialogResult.OK)
            {
                myLex.filePath = this.openFileDialog_button.FileName;
                myLex.myFile = new StreamReader(myLex.filePath, Encoding.Default);
                //将文件内容写到richTextBox_code中
                RichHighlight(-1);
                //此时在回到文件起始处，为其他操作服务
                myLex.myFile.Close();
                myLex.myFile = new StreamReader(myLex.filePath, Encoding.Default);

                //此时不是tmp文件
                fileTemp = false;

                //g_nWordIndex是已识别单词的序号
                myLex.myToken.g_nWordsIndex = 0;
                //文件结束标记回位
                nResult = 32;
                //清除几个显示框中数据
                this.listView_lex.Items.Clear();
                this.listView_syntax.Items.Clear();
                this.listView_symbol.Items.Clear();
                this.textBox_error.Clear();
                //新建Token空间
                myLex.rebuiltToken();
                //新建gr空间
                gr.rebuiltcompiler();
                tr.symboltable_rebuilt();
                //更新状态
                thisStatus.Text = "  文件打开成功";
            }
            else
                //更新状态
                thisStatus.Text = "  文件打开失败";
        }

        //button：保存文件
        private void toolStripButton_save_Click(object sender, EventArgs e)
        {
            //加载文件选择对话框
            if (this.saveFileDialog_button.ShowDialog() == DialogResult.OK)
            {
                this.richTextBox_code.SaveFile(this.saveFileDialog_button.FileName, RichTextBoxStreamType.PlainText);
                //更新状态
                thisStatus.Text = "  文件保存成功";
            }
            else
                //更新状态
                thisStatus.Text = "  文件保存失败";
        }
        
        //button：完成单个单词分析
        private void toolStripButton_next_Click(object sender, EventArgs e)
        {
            gr.rebuilt(); //每次都把堆栈清空
            if (myLex.filePath != "")
            {
                //词法分析
                nResult = myLex.GetAWord(); //词法分析，识别下一个单词
                if (nResult == myLex.OK && myLex.myToken.g_nWordsIndex >= 1)
                {
                    if (myLex.myToken.judgecomment == false)
                    {
                        myLex.getPrintInLexis(myLex.myToken.g_nWordsIndex - 1, myString); //准备打印一个单词
                        PrintInLexisToListView(myString); //词法显示
                        //语法工作
                        for (int i = 0; i < myLex.myToken.g_nWordsIndex; i++)//把所有的已读好的符号放入栈中
                        {
                            syntaxString = new string[6];
                            syntaxString[0] = Convert.ToString(myLex.myToken.g_nWordsIndex - (i+1));//序号
                            syntaxString[1] = myLex.myToken.g_Words[myLex.myToken.g_nWordsIndex - (i + 1)].szName;//名称     
                            syntaxString[2] = myLex.enumToString(myLex.myToken.g_nWordsIndex - (i+1));//类型
                            syntaxString[3] = Convert.ToString(myLex.myToken.g_Words[myLex.myToken.g_nWordsIndex - (i+1)].nNumberValue);
                            syntaxString[4] = Convert.ToString(myLex.myToken.g_Words[myLex.myToken.g_nWordsIndex - (i + 1)].nLineNo);
                            syntaxString[5] = Convert.ToString(myLex.myToken.g_Words[myLex.myToken.g_nWordsIndex - (i + 1)].nColumnNo);
                            gr.Input_Stack_Push(syntaxString);
                        }

                        //怼栈
                        gr.match(gr.input_stack, gr, nResult);
                        //语法显示
                        PrintInSyntaxToSyntax();
                        //错误输出
                        textBox_error.Text = gr.errorshow;
                        
                    }
                    thisStatus.Text = "  成功读入一个token";
                    //更新状态
                }     
                else
                    thisStatus.Text = "  读入失败或者文件编译结束";

            }
            else //没有导入文件
            {
                MessageBox.Show("代码框不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                thisStatus.Text = "  请选择待编译文件";
            }
        }

        //button：完成所有单词分析
        private void toolStripButton_all_Click(object sender, EventArgs e)
        {
            if (myLex.filePath != "")
            {
                //词法分析
                //词法分析前肯定要先打开文件，很多清零操作无需再做
                nResult = myLex.GetAWord(); //词法分析，识别下一个单词
                while (nResult == myLex.OK && myLex.myToken.g_nWordsIndex >= 1)
                {

                    if (myLex.myToken.judgecomment == false)
                    {
                        myLex.getPrintInLexis(myLex.myToken.g_nWordsIndex - 1, myString); //准备打印一个单词
                        PrintInLexisToListView(myString); //打印一个单词

                    }
                    nResult = myLex.GetAWord(); //词法分析，识别下一个单词
                }
                //语法工作
                gr.rebuilt(); //每次都把堆栈清空
                //语法工作
                for (int i = 0; i < myLex.myToken.g_nWordsIndex; i++)//把所有的已读好的符号放入栈中
                {
                    syntaxString = new string[6];
                    syntaxString[0] = Convert.ToString(myLex.myToken.g_nWordsIndex - (i + 1));//序号
                    syntaxString[1] = myLex.myToken.g_Words[myLex.myToken.g_nWordsIndex - (i + 1)].szName;//名称     
                    syntaxString[2] = myLex.enumToString(myLex.myToken.g_nWordsIndex - (i + 1));//类型
                    syntaxString[3] = Convert.ToString(myLex.myToken.g_Words[myLex.myToken.g_nWordsIndex - (i + 1)].nNumberValue);
                    /* if (String.Equals(myString[2], Convert.ToString(WORD_TYPE_ENUM.INTNUMBER)) || String.Equals(myString[2], Convert.ToString(WORD_TYPE_ENUM.REALNUMBER)))
                         syntaxString[3] = Convert.ToString(myLex.myToken.g_Words[myLex.myToken.g_nWordsIndex - (i + 1)].nNumberValue);
                     else
                         syntaxString[3] = "";*/
                    syntaxString[4] = Convert.ToString(myLex.myToken.g_Words[myLex.myToken.g_nWordsIndex - (i + 1)].nLineNo);
                    syntaxString[5] = Convert.ToString(myLex.myToken.g_Words[myLex.myToken.g_nWordsIndex - (i + 1)].nColumnNo);
                    gr.Input_Stack_Push(syntaxString);
                }
                //怼栈
                gr.match(gr.input_stack, gr, nResult);
                //显示
                PrintInSyntaxToSyntax();
                //错误输出
                textBox_error.Text = gr.errorshow;
                //更新状态
                thisStatus.Text = "  成功编译全部代码";
                if (gr.programright)
                {
                    //语义分析
                    tr.symboltable_rebuilt();
                    tr.symboltable(gr.treeroot);
                    //符号表显示
                    PrintSymbolToListView();
                    //三地址计算
                    tr.threeaddress(gr.treeroot);                   
                }

            }

            else //没有导入文件
            {
                MessageBox.Show("代码框不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //tSB_draw:画语法树
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (show == 0)
            {
                show++;
                f2 = new Form2(gr.treeroot, gr.treestack);
                f2.Show();
            }
            else
            {
                f2.Close();
                f2 = new Form2(gr.treeroot, gr.treestack);
                f2.Show();
            }
        }

        //tSB_generate:生成三地址
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //三地址显示
            Form1 f1 = new Form1(tr.threeaddressshow, tr.threeaddresscount);
            f1.Show();
        }
        //button：重新开始分析
        private void toolStripButton_restart_Click(object sender, EventArgs e)
        {
            if (myLex.filePath != "")
            {
                DialogResult dr = MessageBox.Show("请确定是否重新开始执行编译！", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                if (dr == DialogResult.OK)
                {
                    if (!fileTemp)
                    {//不是tmp文件
                        //此时在回到文件起始处
                        myLex.myFile.Close();
                        myLex.myFile = new StreamReader(myLex.filePath, Encoding.Default);
                        //g_nWordIndex是已识别单词的序号
                        myLex.myToken.g_nWordsIndex = 0;
                        //清除几个显示框中数据
                        this.listView_lex.Items.Clear();
                        this.listView_syntax.Items.Clear();
                        this.listView_symbol.Items.Clear();
                        this.textBox_error.Clear();
                        //文件结束标记回位
                        nResult = 32;
                        //新建Token空间
                        myLex.rebuiltToken();
                        //新建gr空间
                        gr.rebuiltcompiler();
                    }
                    else
                    {//是tmp文件
                        //关闭当前文件
                        myLex.myFile.Close();
                        //然后同打开文件操作
                        myLex.myFile = new StreamReader(tmpFile, Encoding.Default);

                        //g_nWordIndex是已识别单词的序号
                        myLex.myToken.g_nWordsIndex = 0;
                        //文件结束标记回位
                        nResult = 32;
                        //清除几个显示框中数据
                        this.listView_lex.Items.Clear();
                        this.listView_syntax.Items.Clear();
                        this.listView_symbol.Items.Clear();
                        //新建Token空间
                        myLex.rebuiltToken();
                        //新建gr空间
                        gr.rebuiltcompiler();
                    }
                    //更新状态
                    thisStatus.Text = "  成功重新开始编译文件";
                }
            }
            else //没有导入文件
            {
                MessageBox.Show("没有任何待编译代码！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                thisStatus.Text = "  请选择待编译文件";
            }
        }

        //richTextBox：修改代码，同步文件
        private void richTextBox_code_TextChanged(object sender, EventArgs e)
        {
            if (!String.Equals(myfileText, richTextBox_code.Text.ToString()))
                fileChange = true;
        }

        private void richTextBox_code_MouseClick(object sender, MouseEventArgs e)
        {
            fileChick = true;
        }

        private void richTextBox_code_KeyPress(object sender, KeyPressEventArgs e)
        {
            filePress = true;
        }

        private void richTextBox_code_MouseLeave(object sender, EventArgs e)
        {
            if (fileChange && fileChick && filePress)
            {// 代码已经修改
                DialogResult dr = MessageBox.Show("您已经修改了代码，是否放弃现在所有操作重新编译！", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                if (dr == DialogResult.OK)
                {
                    //关闭当前文件
                    myLex.myFile.Close();
                    if (!fileLock) {
                        tmpFile = "C:\\Users\\Void\\Documents\\Visual Studio 2013\\Projects\\src\\tmp.txt";
                        fileLock = true;
                    }
                    else
                    {
                        tmpFile = "C:\\Users\\Void\\Documents\\Visual Studio 2013\\Projects\\src\\tmp1.txt";                       
                        fileLock = false;
                    }
                    //将当前文件保存到临时文件
                    this.richTextBox_code.SaveFile(tmpFile, RichTextBoxStreamType.PlainText);
                    //然后同打开文件操作
                    myLex.myFile = new StreamReader(tmpFile, Encoding.Default);
                    //将文件内容写到richTextBox_code中
                    RichHighlight(-1);
                    //此时在回到文件起始处，为其他操作服务
                    myLex.myFile.Close();
                    myLex.myFile = new StreamReader(tmpFile, Encoding.Default);

                    //g_nWordIndex是已识别单词的序号
                    myLex.myToken.g_nWordsIndex = 0;
                    //文件结束标记回位
                    nResult = 32;
                    //清除几个显示框中数据
                    this.listView_lex.Items.Clear();
                    this.listView_syntax.Items.Clear();
                    this.listView_symbol.Items.Clear();
                    this.textBox_error.Clear();
                    //新建Token空间
                    myLex.rebuiltToken();
                    //新建gr空间
                    gr.rebuiltcompiler();

                    //此时编译的是tmp文件
                    fileTemp = true;
                }
                else
                {//放弃修改
                    MessageBox.Show("您已放弃修改内容，继续当前编译工作！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.richTextBox_code.Text = myfileText;
                }
                fileChange = false;
                fileChick = false;
                filePress = false;
                //焦点移出
                this.listView_lex.Focus();
                //更新状态
                thisStatus.Text = "  待编译代码已经修改";
            }
        }

        //richTextBox: 代码高亮
        private void RichHighlight(int start)
        {
            richTextBox_code.Text = myLex.myFile.ReadToEnd();  //换成文件打开
            myfileText = this.richTextBox_code.Text.ToString();
            string[] ln = richTextBox_code.Text.Split('\n'); //去回车
            int pos = 0; //目前读到的位置
            int lnum = 0;
            foreach (string lv in ln)
            {
                if (lnum >= start)
                {
                    string ts = lv.Replace("(", " ").Replace(")", " ");  //去 ( )
                    ts = ts.Replace("{", " ").Replace("}", " ");  //去 { }
                    ts = ts.Replace(".", " ").Replace("=", " ").Replace(";", " ");  //去 . = ;

                    if (lv.Trim().StartsWith("//"))
                    {//去掉注释
                        richTextBox_code.Select(pos, lv.Length); //这些都是在计算某处颜色，后面直接往里填
                        richTextBox_code.SelectionFont = new Font("Consolas", 11, (FontStyle.Regular));
                        richTextBox_code.SelectionColor = Color.Green;
                        pos += lv.Length + 1;
                        continue;
                    }

                    if (lv.Trim().StartsWith("#"))
                    {//去掉宏
                        richTextBox_code.Select(pos, lv.Length);
                        richTextBox_code.SelectionFont = new Font("Consolas", 11, (FontStyle.Regular));
                        richTextBox_code.SelectionColor = Color.Gray;
                        pos += lv.Length + 1;
                        continue;
                    }

                    ArrayList marks = new ArrayList();
                    string smark = "";
                    string last = "";
                    bool inmark = false;
                    for (int i = 0; i < ts.Length; i++)
                    {
                        if (ts.Substring(i, 1) == "\"" && last != "\\")
                        {
                            if (inmark)
                            {
                                marks.Add(smark + "," + i);
                                smark = "";
                                inmark = false;
                            }
                            else
                            {
                                smark += i;
                                inmark = true;
                            }
                        }
                        last = ts.Substring(i, 1);
                    }

                    if (inmark)
                    {
                        marks.Add(smark + "," + ts.Length);
                    }

                    string[] ta = ts.Split(' '); //此时ts已经去掉了一些符号
                    int x = 0;
                    foreach (string tv in ta)
                    {//针对每个字符串开始分析
                        if (tv.Length < 2)
                        {
                            x += tv.Length + 1;
                            continue;
                        }
                        else
                        {
                            bool find = false;
                            foreach (string px in marks)
                            {
                                string[] pa = px.Split(',');
                                if (x >= Int32.Parse(pa[0]) && x < Int32.Parse(pa[1]))
                                {
                                    find = true;
                                    break;
                                }
                            }
                            if (!find)
                            {
                                if (keywords1[tv] != null)
                                {
                                    richTextBox_code.Select(pos + x, tv.Length);
                                    richTextBox_code.SelectionFont = new Font("Consolas", 11, (FontStyle.Italic));
                                    richTextBox_code.SelectionColor = Color.SteelBlue;
                                }
                                else if (keywords2[tv] != null)
                                {
                                    richTextBox_code.Select(pos + x, tv.Length);
                                    richTextBox_code.SelectionFont = new Font("Consolas", 11, (FontStyle.Regular));
                                    richTextBox_code.SelectionColor = Color.Red;
                                }
                            }
                            x += tv.Length + 1;
                        }
                    }

                    foreach (string px in marks)
                    {
                        string[] pa = px.Split(',');
                        richTextBox_code.Select(pos + Int32.Parse(pa[0]), Int32.Parse(pa[1]) - Int32.Parse(pa[0]) + 1);
                        richTextBox_code.SelectionFont = new Font("Consolas", 11, (FontStyle.Regular));
                        richTextBox_code.SelectionColor = Color.DarkRed;
                    }
                }
                pos += lv.Length + 1;
                lnum++;
            }

            // 设置一下，才能恢复；后续正确！
            richTextBox_code.Select(0, 1);
            richTextBox_code.SelectionFont = new Font("Consolas", 11, (FontStyle.Regular));
            richTextBox_code.SelectionColor = Color.White;
        }


//
//菜单栏上功能
//
        private void 打开文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //加载文件选择对话框
            if (this.openFileDialog_button.ShowDialog() == DialogResult.OK)
            {
                myLex.filePath = this.openFileDialog_button.FileName;
                myLex.myFile = new StreamReader(myLex.filePath, Encoding.Default);
                //将文件内容写到richTextBox_code中
                RichHighlight(-1);
                //此时在回到文件起始处，为其他操作服务
                myLex.myFile.Close();
                myLex.myFile = new StreamReader(myLex.filePath, Encoding.Default);

                //此时不是tmp文件
                fileTemp = false;

                //g_nWordIndex是已识别单词的序号
                myLex.myToken.g_nWordsIndex = 0;
                //文件结束标记回位
                nResult = 32;
                //清除几个显示框中数据
                this.listView_lex.Items.Clear();
                this.listView_syntax.Items.Clear();
                //新建Token空间
                myLex.rebuiltToken();
                //新建gr空间
                gr.rebuilt();
            }         
        }

        private void 保存文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //加载文件选择对话框
            if (this.saveFileDialog_button.ShowDialog() == DialogResult.OK)
            {
                this.richTextBox_code.SaveFile(this.saveFileDialog_button.FileName, RichTextBoxStreamType.PlainText);
            }      
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 下一个tokenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gr.rebuilt(); //每次都把堆栈清空
            if (myLex.filePath != "")
            {
                //词法分析
                nResult = myLex.GetAWord(); //词法分析，识别下一个单词
                if (nResult == myLex.OK && myLex.myToken.g_nWordsIndex >= 1)
                {
                    if (myLex.myToken.judgecomment == false)
                    {
                        myLex.getPrintInLexis(myLex.myToken.g_nWordsIndex - 1, myString); //准备打印一个单词
                        PrintInLexisToListView(myString); //打印一个单词
                        //语法工作
                        //语法工作
                        for (int i = 0; i < myLex.myToken.g_nWordsIndex; i++)//把所有的已读好的符号放入栈中
                        {
                            syntaxString = new string[6];
                            syntaxString[0] = Convert.ToString(myLex.myToken.g_nWordsIndex - (i + 1));//序号
                            syntaxString[1] = myLex.myToken.g_Words[myLex.myToken.g_nWordsIndex - (i + 1)].szName;//名称     
                            syntaxString[2] = myLex.enumToString(myLex.myToken.g_nWordsIndex - (i + 1));//类型
                            syntaxString[3] = Convert.ToString(myLex.myToken.g_Words[myLex.myToken.g_nWordsIndex - (i + 1)].nNumberValue);
                            syntaxString[4] = Convert.ToString(myLex.myToken.g_Words[myLex.myToken.g_nWordsIndex - (i + 1)].nLineNo);
                            syntaxString[5] = Convert.ToString(myLex.myToken.g_Words[myLex.myToken.g_nWordsIndex - (i + 1)].nColumnNo);
                            gr.Input_Stack_Push(syntaxString);
                        }
                        //怼栈
                        gr.match( gr.input_stack, gr, nResult);
                        //显示
                        PrintInSyntaxToSyntax();
                        //错误输出
                        textBox_error.Text = gr.errorshow;
                    }
                }
            }
            else //没有导入文件
            {
                MessageBox.Show("代码框不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void 全部执行ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (myLex.filePath != "")
            {
                //词法分析
                //词法分析前肯定要先打开文件，很多清零操作无需再做
                nResult = myLex.GetAWord(); //词法分析，识别下一个单词
                while (nResult == myLex.OK && myLex.myToken.g_nWordsIndex >= 1)
                {

                    if (myLex.myToken.judgecomment == false)
                    {
                        myLex.getPrintInLexis(myLex.myToken.g_nWordsIndex - 1, myString); //准备打印一个单词
                        PrintInLexisToListView(myString); //打印一个单词

                    }
                    nResult = myLex.GetAWord(); //词法分析，识别下一个单词
                }
                //语法工作
                gr.rebuilt(); //每次都把堆栈清空
                //语法工作
                for (int i = 0; i < myLex.myToken.g_nWordsIndex; i++)//把所有的已读好的符号放入栈中
                {
                    syntaxString = new string[6];
                    syntaxString[0] = Convert.ToString(myLex.myToken.g_nWordsIndex - (i + 1));//序号
                    syntaxString[1] = myLex.myToken.g_Words[myLex.myToken.g_nWordsIndex - (i + 1)].szName;//名称     
                    syntaxString[2] = myLex.enumToString(myLex.myToken.g_nWordsIndex - (i + 1));//类型
                    syntaxString[3] = Convert.ToString(myLex.myToken.g_Words[myLex.myToken.g_nWordsIndex - (i + 1)].nNumberValue);
                   /* if (String.Equals(myString[2], Convert.ToString(WORD_TYPE_ENUM.INTNUMBER)) || String.Equals(myString[2], Convert.ToString(WORD_TYPE_ENUM.REALNUMBER)))
                        syntaxString[3] = Convert.ToString(myLex.myToken.g_Words[myLex.myToken.g_nWordsIndex - (i + 1)].nNumberValue);
                    else
                        syntaxString[3] = "";*/
                    syntaxString[4] = Convert.ToString(myLex.myToken.g_Words[myLex.myToken.g_nWordsIndex - (i + 1)].nLineNo);
                    syntaxString[5] = Convert.ToString(myLex.myToken.g_Words[myLex.myToken.g_nWordsIndex - (i + 1)].nColumnNo);
                    gr.Input_Stack_Push(syntaxString);
                }
                //怼栈
                gr.match( gr.input_stack, gr, nResult);
                //显示
                PrintInSyntaxToSyntax();
                //错误输出
                textBox_error.Text = gr.errorshow;
                //更新状态
                thisStatus.Text = "  成功编译全部代码";
                //语义分析
                tr.symboltable_rebuilt();
                tr.symboltable(gr.treeroot);
                //符号表显示
                PrintSymbolToListView();
            }

            else //没有导入文件
            {
                MessageBox.Show("代码框不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void 重新生成ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (myLex.filePath != "")
            {
                DialogResult dr = MessageBox.Show("请确定是否重新开始执行编译！", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                if (dr == DialogResult.OK)
                {
                    if (!fileTemp)
                    {//不是tmp文件
                        //此时在回到文件起始处
                        myLex.myFile.Close();
                        myLex.myFile = new StreamReader(myLex.filePath, Encoding.Default);
                        //g_nWordIndex是已识别单词的序号
                        myLex.myToken.g_nWordsIndex = 0;
                        //清除几个显示框中数据
                        this.listView_lex.Items.Clear();
                        this.listView_syntax.Items.Clear();
                        //文件结束标记回位
                        nResult = 32;
                        //新建Token空间
                        myLex.rebuiltToken();
                        //新建gr空间
                        gr.rebuiltcompiler();
                    }
                    else
                    {//是tmp文件
                        //关闭当前文件
                        myLex.myFile.Close();
                        //然后同打开文件操作
                        myLex.myFile = new StreamReader(tmpFile, Encoding.Default);

                        //g_nWordIndex是已识别单词的序号
                        myLex.myToken.g_nWordsIndex = 0;
                        //文件结束标记回位
                        nResult = 32;
                        //清除几个显示框中数据
                        this.listView_lex.Items.Clear();
                        this.listView_syntax.Items.Clear();
                        //新建Token空间
                        myLex.rebuiltToken();
                        //新建gr空间
                        gr.rebuiltcompiler();
                    }
                    //更新状态
                    thisStatus.Text = "  成功重新开始编译文件";
                }
            }
            else //没有导入文件
            {
                MessageBox.Show("没有任何待编译代码！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                thisStatus.Text = "  请选择待编译文件";
            }
        }

        private void 画语法树ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (show == 0)
            {
                show++;
                f2 = new Form2(gr.treeroot, gr.treestack);
                f2.Show();
            }
            else
            {
                f2.Close();
                f2 = new Form2(gr.treeroot, gr.treestack);
                f2.Show();
            }
        }






    }//类的括号
}//namespace的括号

