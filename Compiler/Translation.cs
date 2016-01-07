using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class Translation
    {
        GrammarSheet gr = new GrammarSheet();
        public String[] symbolTableName = new String[100];   //符号表中的名字
        public String[] symbolTableType = new String[100];   //符号表中的类型
        public String[] symbolTableValue = new String[100];  //符号表中的值
        public String[] symbolTableLineNo = new String[100];   //符号表中的名字
        public String[] symbolTableColumnNo = new String[100];   //符号表中的类型
        public int symbolTableCount = 0;       //符号表的序号记录
        public int tempcount = 0;//临时变量下标
        public int thenstart = 0;//记录三地址中then判断语句开始的位置
        public int elsestart = 0;//记录三地址中else判断语句开始的位置
        public int elseend = 0;//记录三地址中else语句结束的位置
        public int whilestart = 0;//记录三地址中while判断语句开始的位置
        public int whileend = 0;//记录三地址中while语句结束的位置
        public String[] threeaddressshow = new String[100];//三地址输出
        public int threeaddresscount = 1;//三地址行标


        public void symboltable(Node n)
        {
            switch (n.type)
            {
                case "program":
                    symboltable(n.getChilds()[0]);
                    break;
                case "compoundstmt":
                    symboltable(n.getChilds()[1]);
                    break;
                case "stmts":
                    if (n.hasChild())
                    {
                        if (n.getChilds().Count == 2)
                        {
                            symboltable(n.getChilds()[0]);
                            symboltable(n.getChilds()[1]);
                        }
                        else if (n.getChilds().Count == 1)
                        {
                            symboltable(n.getChilds()[0]);
                        }
                    }
                    break;
                case "stmt":
                    symboltable(n.getChilds()[0]);
                    break;
                case "decl":
                    symboltable(n.getChilds()[0]);
                    n.symboltype = n.getChilds()[0].getChilds()[0].symboltype;
                    n.getChilds()[1].symboltype = n.symboltype;
                    symboltable(n.getChilds()[1]);
                    break;
                case "list":
                    if (n.getChilds().Count == 2)
                    {
                        n.getChilds()[0].symboltype = n.symboltype;
                        n.getChilds()[1].symboltype = n.symboltype;
                    }
                    else
                    {
                        n.getChilds()[0].symboltype = n.symboltype;
                    }
                    if (n.getChilds()[0].type == "ID")
                    {
                        bool redesign = false;//判断重复声明,重复为true
                        for (int m = 0; m < symbolTableCount; m++)
                        {
                            if (n.getChilds()[0].symbolname == symbolTableName[m])
                            {
                                redesign = true;
                                symbolTableType[m] = "重复声明";
                                symbolTableValue[m] = "";
                                symbolTableLineNo[m] = "";
                                symbolTableColumnNo[m] = "";
                            }
                        }
                        if (!redesign)
                        {
                            symbolTableName[symbolTableCount] = n.getChilds()[0].symbolname;
                            symbolTableType[symbolTableCount] = n.getChilds()[0].symboltype;
                            symbolTableValue[symbolTableCount] = "未赋值";
                            symbolTableLineNo[symbolTableCount] = n.getChilds()[0].lineNo;
                            symbolTableColumnNo[symbolTableCount] = n.getChilds()[0].columnNo;
                            symbolTableCount++;
                        }
                    }
                    symboltable(n.getChilds()[1]);
                    break;
                case "list1":
                    if (n.getChilds().Count == 2)
                    {
                        n.getChilds()[0].symboltype = n.symboltype;
                        n.getChilds()[1].symboltype = n.symboltype;
                        symboltable(n.getChilds()[1]);
                    }
                    else
                    {
                        n.getChilds()[0].symboltype = n.symboltype;
                        symboltable(n.getChilds()[0]);
                    }
                    if (n.getChilds()[0].type == "ID")
                    {
                        bool redesign = false;   //判断重复声明,重复为true
                        for (int m = 0; m < symbolTableCount; m++)
                        {
                            if (n.getChilds()[0].symbolname == symbolTableName[m])
                            {
                                redesign = true;
                                symbolTableType[m] = "重复声明";
                                symbolTableValue[m] = "";
                                symbolTableLineNo[m] = "";
                                symbolTableColumnNo[m] = "";
                            }
                        }
                        if (!redesign)
                        {
                            symbolTableName[symbolTableCount] = n.getChilds()[0].symbolname;
                            symbolTableType[symbolTableCount] = n.getChilds()[0].symboltype;
                            symbolTableValue[symbolTableCount] = "未赋值";
                            symbolTableLineNo[symbolTableCount] = n.getChilds()[0].lineNo;
                            symbolTableColumnNo[symbolTableCount] = n.getChilds()[0].columnNo;
                            symbolTableCount++;
                        }
                    }
                    break;
                case "assgstmt":
                    symboltable(n.getChilds()[2]);
                    n.getChilds()[0].symbolvalue = n.getChilds()[2].symbolvalue;
                    bool occur = false;
                    //判断是否为赋值但没声明
                    for (int m = 0; m < symbolTableCount; m++)
                    {
                        if (n.getChilds()[0].symbolname == symbolTableName[m])
                            occur = true;
                    }
                    if (!occur)
                    {
                        symbolTableName[symbolTableCount] = n.getChilds()[0].symbolname;
                        symbolTableType[symbolTableCount] = "未声明";
                        symbolTableValue[symbolTableCount] = "";
                        symbolTableLineNo[symbolTableCount] = "";
                        symbolTableColumnNo[symbolTableCount] = "";
                        symbolTableCount++;
                    }
                    else
                    {
                        //判断赋值类型是否一致
                        bool judge = false;//判断是否值为小数,小数为true
                        for (int l = 0; l < n.getChilds()[0].symbolvalue.Length; l++)
                            if (n.getChilds()[0].symbolvalue[l] == '.')
                                judge = true;
                        if (n.getChilds()[2].symboltype == "real")
                            judge = true;
                        if (judge)
                        {
                            for (int m = 0; m < symbolTableCount; m++)
                            {
                                if (n.getChilds()[0].symbolname == symbolTableName[m] && symbolTableType[m] == "int")
                                {
                                    symbolTableType[m] = "类型不匹配";
                                    symbolTableValue[m] = "";
                                    symbolTableLineNo[m] = "";
                                    symbolTableColumnNo[m] = "";
                                }
                                if (n.getChilds()[0].symbolname == symbolTableName[m] && symbolTableType[m] == "real")
                                {
                                    symbolTableValue[m] = n.getChilds()[0].symbolvalue;
                                }
                            }
                        }
                        else
                        {
                            for (int m = 0; m < symbolTableCount; m++)
                            {
                                if (n.getChilds()[0].symbolname == symbolTableName[m])
                                {
                                    symbolTableValue[m] = n.getChilds()[0].symbolvalue;
                                }
                            }
                        }
                    }
                    break;
                case "arithexpr":
                    //对直接赋值和运算赋值分别讨论
                    symboltable(n.getChilds()[0]);
                    symboltable(n.getChilds()[1]);
                    if (n.hasChild())
                    {
                        if (n.getChilds()[1].getChilds().Count == 1)
                        {
                            n.symboltype = n.getChilds()[0].symboltype;
                            n.symbolvalue = n.getChilds()[0].symbolvalue;
                        }
                        else if (n.getChilds()[1].getChilds().Count == 3)
                        {
                            bool occurleft=false;
                            bool occurright=false;
                            for (int m = 0; m < symbolTableCount; m++)
                            {
                                if (n.getChilds()[0].symbolname == symbolTableName[m])
                              {
                                  n.getChilds()[0].symboltype = symbolTableType[m];
                                  occurleft=true;
                              }
                                if (n.getChilds()[1].symbolname == symbolTableName[m])
                              {
                                  n.getChilds()[1].symboltype = symbolTableType[m];
                                  occurright=true;
                              }
                            }
                            if( occurleft&&occurright)
                            {
                                if (n.getChilds()[0].symboltype == "int" && n.getChilds()[1].symboltype == "int")
                                    n.symboltype = "int";
                                if (n.getChilds()[0].symboltype == "int" && n.getChilds()[1].symboltype == "real")
                                    n.symboltype = "real";
                                if (n.getChilds()[0].symboltype == "real" && n.getChilds()[1].symboltype == "int")
                                    n.symboltype = "real";
                                if (n.getChilds()[0].symboltype == "real" && n.getChilds()[1].symboltype == "real")
                                    n.symboltype = "real";
                            }
                        }
                    }
                    break;
                case "multexpr":
                    symboltable(n.getChilds()[0]);
                    n.symbolname = n.getChilds()[0].symbolname;
                    n.symboltype = n.getChilds()[0].symboltype;
                    n.symbolvalue = n.getChilds()[0].symbolvalue;
                    break;
                case "simpleexpr":
                    symboltable(n.getChilds()[0]);
                    n.symbolname = n.getChilds()[0].symbolname;
                    n.symboltype = n.getChilds()[0].symboltype;
                    n.symbolvalue = n.getChilds()[0].symbolvalue;
                    break;
                case "arithexprprime":
                    if (n.getChilds().Count == 3)
                    {
                        symboltable(n.getChilds()[1]);
                        symboltable(n.getChilds()[2]);
                        n.symbolname = n.getChilds()[1].symbolname;
                    }
                    break;

                default:
                    break;
            }
        }

        public void threeaddress(Node n)
        {
            switch (n.type)
            {
                case "program":
                    threeaddress(n.getChilds()[0]);
                    threeaddressshow[threeaddresscount] = Convert.ToString(threeaddresscount) + ":";
                    n.name = threeaddressshow[threeaddresscount];
                    threeaddresscount++;
                    break;
                case "compoundstmt":
                    threeaddress(n.getChilds()[1]);
                    break;
                case "stmts":
                    if (n.getChilds().Count == 2)
                    {
                        threeaddress(n.getChilds()[0]);
                        threeaddress(n.getChilds()[1]);
                    }
                    else
                    {
                        threeaddress(n.getChilds()[0]);
                    }
                    break;
                case "stmt":
                    threeaddress(n.getChilds()[0]);
                    tempcount = 0;
                    break;
                case "assgstmt":
                    threeaddress(n.getChilds()[2]);
                    threeaddressshow[threeaddresscount] = Convert.ToString(threeaddresscount) + ":" + " " + "MOV" + " " + n.getChilds()[0].name + ","  + "," + n.getChilds()[2].value;
                    n.name = threeaddressshow[threeaddresscount];
                    threeaddresscount++;
                    break;
                case "arithexpr":
                    threeaddress(n.getChilds()[0]);
                    threeaddress(n.getChilds()[1]);
                    if (n.getChilds()[1].getChilds()[0].type=="empty")
                    {
                        n.value = n.getChilds()[0].value;
                    }
                    else
                    {
                        n.value = temppro();
                        tempcount++;
                    }
                    if (n.getChilds()[1].getChilds()[0].name == "+")
                    {
                        threeaddressshow[threeaddresscount] = Convert.ToString(threeaddresscount) + ":" + " " + "ADD" + " " + n.value + "," + n.getChilds()[0].name + "," + n.getChilds()[1].value;
                        n.name = threeaddressshow[threeaddresscount];
                        threeaddresscount++;
                    }
                    else if (n.getChilds()[1].getChilds()[0].name == "-")
                    {
                        threeaddressshow[threeaddresscount] = Convert.ToString(threeaddresscount) + ":" + " " + "SUB" + " " + n.value + "," + n.getChilds()[0].name + "," + n.getChilds()[1].value;
                        n.name = threeaddressshow[threeaddresscount];
                        threeaddresscount++;
                    }
                    break;
                case "multexpr":
                    threeaddress(n.getChilds()[0]);
                    threeaddress(n.getChilds()[1]);
                    if (n.getChilds()[1].getChilds().Count == 1)
                    {
                        n.value = n.getChilds()[0].value;
                        n.name = n.getChilds()[0].name;
                    }
                    if (n.getChilds()[1].getChilds().Count == 3)
                    {
                        n.value = temppro();
                        tempcount++;
                        if (n.getChilds()[1].getChilds()[0].name == "/")
                        {
                            threeaddressshow[threeaddresscount] = Convert.ToString(threeaddresscount) + ":" + " " + "DIV" + " " + n.value + "," + n.getChilds()[0].name + "," + n.getChilds()[1].value;
                            n.name = threeaddressshow[threeaddresscount];
                            threeaddresscount++;
                        }
                        else if (n.getChilds()[1].getChilds()[0].name == "*")
                        {
                            threeaddressshow[threeaddresscount] = Convert.ToString(threeaddresscount) + ":" + " " + "MUL" + " " + n.value + "," + n.getChilds()[0].name + "," + n.getChilds()[1].value;
                            n.name = threeaddressshow[threeaddresscount];
                            threeaddresscount++;
                        }
                      
                    }
                    break;
                case "arithexprprime":
                    if (n.getChilds().Count == 3)
                    {
                        threeaddress(n.getChilds()[1]);
                        threeaddress(n.getChilds()[2]);
                        n.value = n.getChilds()[1].value;
                        n.name = n.getChilds()[1].name;
                    }
                    else
                    {
                        threeaddress(n.getChilds()[0]);
                    }
                    break;
                case "simpleexpr":
                    if (n.getChilds().Count == 3)
                    {
                        threeaddress(n.getChilds()[1]);
                        n.value = n.getChilds()[1].value;
                    }
                    else
                    {
                        threeaddress(n.getChilds()[0]);
                        n.value = n.getChilds()[0].name;
                        n.name = n.getChilds()[0].name;
                        
                    }
                    break;
                case "multexprprime":
                    if (n.getChilds().Count == 3)
                    {
                        threeaddress(n.getChilds()[1]);
                        threeaddress(n.getChilds()[2]);
                        n.name = n.getChilds()[1].name;
                        n.value = n.getChilds()[1].value;
                    }
                    else
                    {
                        threeaddress(n.getChilds()[0]);
                    }
                    break;
                case "ifstmt":
                    threeaddress(n.getChilds()[2]);
                    threeaddress(n.getChilds()[5]);
                    threeaddress(n.getChilds()[6]);
                    threeaddress(n.getChilds()[7]);
                    elseend = threeaddresscount;
                    threeaddressshow[elsestart] = threeaddressshow[elsestart] + Convert.ToString(elseend);
                    break;
                case "whilestmt":
                    threeaddress(n.getChilds()[2]);
                    threeaddress(n.getChilds()[4]);
                    threeaddressshow[threeaddresscount] = Convert.ToString(threeaddresscount) + ":" + " " + "JMP" + " "  + "," + ","+Convert.ToString(whilestart);
                    n.name = threeaddressshow[threeaddresscount];
                    threeaddresscount++;
                    threeaddressshow[(whilestart + 1)] = threeaddressshow[whilestart + 1] + Convert.ToString(threeaddresscount);
                    break;
                case "decl":
                    threeaddress(n.getChilds()[0]);
                    threeaddress(n.getChilds()[1]);
                    break;
                case "type":
                    threeaddress(n.getChilds()[0]);
                    break;
                case "list":
                    threeaddress(n.getChilds()[1]);
                    break;
                case "list1":
                    if (n.getChilds().Count == 2)
                    {
                        threeaddress(n.getChilds()[1]);
                    }
                    else
                    {
                        threeaddress(n.getChilds()[0]);
                    }
                    break;
                case "boolexpr":
                    threeaddress(n.getChilds()[0]);
                    threeaddress(n.getChilds()[1]);
                    threeaddress(n.getChilds()[2]);
                    if(n.getChilds()[1].getChilds()[0].type=="<")
                    {
                        n.value = temppro();
                        tempcount++;
                        whilestart = threeaddresscount; 
                        threeaddressshow[threeaddresscount] = Convert.ToString(threeaddresscount) + ":" + " " + "LES" + " " + n.value + "," + n.getChilds()[0].value + "," + n.getChilds()[2].value;
                        n.name = threeaddressshow[threeaddresscount];
                        threeaddresscount++;
                        thenstart = threeaddresscount;
                        threeaddressshow[threeaddresscount] = Convert.ToString(threeaddresscount) + ":" + " " + "JMPF" + " " + n.value + "," + ",";
                        n.name = threeaddressshow[threeaddresscount];
                        threeaddresscount++;

                    }
                    else if (n.getChilds()[1].getChilds()[0].type == ">")
                    {
                        n.value = temppro();
                        tempcount++;
                        whilestart = threeaddresscount; 
                        threeaddressshow[threeaddresscount] = Convert.ToString(threeaddresscount) + ":" + " " + "GTR" + " " + n.value + "," + n.getChilds()[0].value + "," + n.getChilds()[2].value;
                        n.name = threeaddressshow[threeaddresscount];
                        threeaddresscount++;
                        thenstart = threeaddresscount;
                        threeaddressshow[threeaddresscount] = Convert.ToString(threeaddresscount) + ":" + " " + "JMPF" + " " + n.value + "," + ",";
                        n.name = threeaddressshow[threeaddresscount];
                        threeaddresscount++;
                    }
                    else if (n.getChilds()[1].getChilds()[0].type == "<=")
                    {
                        n.value = temppro();
                        tempcount++;
                        whilestart = threeaddresscount; 
                        threeaddressshow[threeaddresscount] = Convert.ToString(threeaddresscount) + ":" + " " + "LEQ" + " " + n.value + "," + n.getChilds()[0].value + "," + n.getChilds()[2].value;
                        n.name = threeaddressshow[threeaddresscount];
                        threeaddresscount++;
                        thenstart = threeaddresscount;
                        threeaddressshow[threeaddresscount] = Convert.ToString(threeaddresscount) + ":" + " " + "JMPF" + " " + n.value + "," + ",";
                        n.name = threeaddressshow[threeaddresscount];
                        threeaddresscount++;
                    }
                    else if (n.getChilds()[1].getChilds()[0].type == ">=")
                    {
                        n.value = temppro();
                        tempcount++;
                        whilestart = threeaddresscount; 
                        threeaddressshow[threeaddresscount] = Convert.ToString(threeaddresscount) + ":" + " " + "GEQ" + " " + n.value + "," + n.getChilds()[0].value + "," + n.getChilds()[2].value;
                        n.name = threeaddressshow[threeaddresscount];
                        threeaddresscount++;
                        thenstart = threeaddresscount;
                        threeaddressshow[threeaddresscount] = Convert.ToString(threeaddresscount) + ":" + " " + "JMPF" + " " + n.value + "," + ",";
                        n.name = threeaddressshow[threeaddresscount];
                        threeaddresscount++;
                    }
                    else if (n.getChilds()[1].getChilds()[0].type == "==")
                    {
                        n.value = temppro();
                        tempcount++;
                        whilestart = threeaddresscount; 
                        threeaddressshow[threeaddresscount] = Convert.ToString(threeaddresscount) + ":" + " " + "EQL" + " " + n.value + "," + n.getChilds()[0].value + "," + n.getChilds()[2].value;
                        n.name = threeaddressshow[threeaddresscount];
                        threeaddresscount++;
                        thenstart = threeaddresscount;
                        threeaddressshow[threeaddresscount] = Convert.ToString(threeaddresscount) + ":" + " " + "JMPF" + " " + n.value + "," + ",";
                        n.name = threeaddressshow[threeaddresscount];
                        threeaddresscount++;
                    }
                    break;
                case "boolop":
                    threeaddress(n.getChilds()[0]);
                    break;
                case "else":
                    threeaddressshow[threeaddresscount] = Convert.ToString(threeaddresscount) + ":" + " " + "JMP" + " "  + "," + ",";
                    n.name = threeaddressshow[threeaddresscount];
                    elsestart = threeaddresscount;
                    threeaddresscount++;
                    threeaddressshow[thenstart] = threeaddressshow[thenstart] + Convert.ToString(threeaddresscount);
                    break;
                default:
                    break;
            }
        }

        public string temppro()
        {
            return "t" + Convert.ToString(tempcount);
        }


        //每次运行结束后初始化
        public void symboltable_rebuilt()
        {
            symbolTableCount = 0;
            tempcount = 0;
            threeaddresscount = 1;
            symbolTableName = new String[100];
            symbolTableType = new String[100];
            symbolTableValue = new String[100];
            symbolTableLineNo = new String[100];
            symbolTableColumnNo = new String[100];
            threeaddressshow = new String[100];
        }
    }




}



