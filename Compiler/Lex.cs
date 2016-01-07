using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//import
using System.IO;

namespace Compiler
{
    public class Lex
    {
        public int OK;    //状态信息
        public int ERROR;
        public int SPACE;
        public int RETURN;
        public int ENTER;
        public int TABLE;
        public string filePath;  //打开文件的位置信息
        public StreamReader myFile;   //打开文件流
        public Token myToken = new Token();   
        public static char cACharacter = Convert.ToChar(32);  //当前读到的单词
        public static int cAC_int = 32;

        public Lex()
        {
            OK = 1;
            ERROR = -1;
            SPACE = 32;
            RETURN = 10;
            ENTER = 13;
            TABLE = 9;
            filePath = "";
        }

        //从文件中读取一个字符然后返回
        //该函数被GetAWord()反复调用
        public int GetACharacterFromFile()
        {
            int word = myFile.Read();
            //从文件中读取一个字符
            if (word == RETURN) //如果是回车符
            {
                myToken.g_nLineNo++; //源代码行数加
                myToken.g_nColumnNo = 1; //回到列首
            }
            return word; //返回从文件中读取的一个字符
        }

        public int GetAWord() //词法分析，获取一个单词
        {
            char[] nNumberAddition = new char[5]; //E,e的附加成份
            int nNumberOfAddition; //E,e的附加成份个数
            int nValueOfAddition; //E,e的附加成份值
            int nNumberOfFloat; //小数的个数
            int i;
            string szAWord = "";
            //一个用于临时存放识别出字符的数组
            double nNumberValue; //数的值

            myToken.judgecomment = false;

            //忽略空格、换行和TAB
            //如果是第一次调用则一定为SPACE，之后调用要么就是上次完全匹配为SPACE，要么并不完全匹配为一个字符
            while ((cAC_int == SPACE || cAC_int == RETURN || cAC_int == TABLE || cAC_int == ENTER) && (cAC_int != -1))
            {
                cAC_int = GetACharacterFromFile(); //继续读下一个字符
                if(cAC_int != -1)
                    cACharacter = Convert.ToChar(cAC_int);
            }

            if (cAC_int != -1) //如果不是文件末尾
            {
                if ((cACharacter >= 'a' && cACharacter <= 'z') || (cACharacter >= 'A' && cACharacter <= 'Z')) //如果当前字符是字母
                {//标识符或保留字以a~z,A~Z开头
                    do
                    {
                        szAWord = szAWord + cACharacter; //把当前字符放进单词数组里，单词数组下标加1
                        cAC_int = GetACharacterFromFile(); //继续读下一个字符
                        if (cAC_int != -1)
                            cACharacter = Convert.ToChar(cAC_int);
                    } while (((cACharacter >= 'a' && cACharacter <= 'z') || (cACharacter >= '0' && cACharacter <= '9') || (cACharacter >= 'A' && cACharacter <= 'Z')) && (cAC_int != -1));
                    //只要后面继续跟a~z或0~9，就要继续看后面的字符
                    //当前字符不再是a~z或0~9
                    if (!myFile.EndOfStream) //如果不是文件末尾
                    {
                        //从保留字表中查询当前单词字符串是否为某一保留字字符串
                        for (i = 0; i < Token.NUMBER_OF_RESERVED_WORDS; i++)
                            if (String.Equals(szAWord, myToken.ReservedWordNameVsTypeTable[i].szName))
                            {
                                //如果是保留字，则赋当前单词类型为相应保留字的单词类型枚举值
                                myToken.g_Words[myToken.g_nWordsIndex].eType = myToken.ReservedWordNameVsTypeTable[i].eType;
                                myToken.g_Words[myToken.g_nWordsIndex].szName = szAWord; //识别出的单词放进单词队列g_Words中

                                myToken.g_Words[myToken.g_nWordsIndex].nLineNo = myToken.g_nLineNo; //在源代码文件中单词所在的行数
                                myToken.g_Words[myToken.g_nWordsIndex].nColumnNo = myToken.g_nColumnNo;
                                myToken.g_nWordsIndex++; //识别出来的单词个数加1
                                myToken.g_nColumnNo++;	//全部列号加一
                                myToken.g_PreWord.eType = myToken.ReservedWordNameVsTypeTable[i].eType; //记录这个单词
                                break;
                            }
                        if (i >= Token.NUMBER_OF_RESERVED_WORDS)
                        {
                            //如果在保留字表中查询不出，则当前单词是标识符，单词类型为单词类型枚举值IDENTIFIER
                            myToken.g_Words[myToken.g_nWordsIndex].eType = WORD_TYPE_ENUM.IDENTIFIER;
                            myToken.g_Words[myToken.g_nWordsIndex].szName = szAWord; //识别出的单词放进单词队列g_Words中

                            myToken.g_Words[myToken.g_nWordsIndex].nLineNo = myToken.g_nLineNo; //在源代码文件中单词所在的行数
                            myToken.g_Words[myToken.g_nWordsIndex].nColumnNo = myToken.g_nColumnNo;
                            myToken.g_nWordsIndex++; //识别出来的单词个数加1
                            myToken.g_nColumnNo++;	//全部列号加一
                            myToken.g_PreWord.eType = WORD_TYPE_ENUM.IDENTIFIER; //记录这个单词
                        }
                        return OK; //是保留字或标识符
                    }
                    else
                        return ERROR;
                }

                else if (cACharacter >= '0' && cACharacter <= '9')
                {
                    //数字是以0~9开头
                    nNumberValue = 0; //数单词的值
                    nNumberOfAddition = 0; //E，e的附加成份个数
                    nNumberOfFloat = 0;
                    nValueOfAddition = 0;
                    //初始化 nNumberAddition[]
                    for (int j = 0; j < 5; j++)
                    {
                        nNumberAddition[j] = '*';
                    }

                    do
                    {
                        szAWord = szAWord + cACharacter;
                        nNumberValue = 10 * nNumberValue + cACharacter - '0'; //计算数单词的值

                        cAC_int = GetACharacterFromFile(); //继续读下一个字符
                        if (cAC_int != -1)
                            cACharacter = Convert.ToChar(cAC_int);
                    } while (cACharacter >= '0' && cACharacter <= '9' && !myFile.EndOfStream); //如果是0~9，则继续拼装数单词

                    //如果不是0~9则数单词暂且结束，可能是int类型
                    if (!myFile.EndOfStream && cACharacter != '.' && cACharacter != 'E' && cACharacter != 'e')
                    {
                        myToken.g_Words[myToken.g_nWordsIndex].eType = WORD_TYPE_ENUM.INTNUMBER; //单词类型为单词类型枚举值INTNUMBER
                        myToken.g_Words[myToken.g_nWordsIndex].szName = szAWord; //识别出的单词放进单词队列
                        myToken.g_Words[myToken.g_nWordsIndex].nNumberValue = nNumberValue; //同时将数的值赋给单词的nNumberValue
                        myToken.g_Words[myToken.g_nWordsIndex].nLineNo = myToken.g_nLineNo; //在源代码文件中单词所在的行数
                        myToken.g_Words[myToken.g_nWordsIndex].nColumnNo = myToken.g_nColumnNo;
                        myToken.g_nWordsIndex++; //识别出的单词加一
                        myToken.g_nColumnNo++;	//全部列号加一
                        myToken.g_PreWord.eType = WORD_TYPE_ENUM.INTNUMBER; //记录这个单词
                        return OK;
                    }
                    //如果不是0~9则数单词暂且结束，可能是real类型
                    else if (!myFile.EndOfStream && (cACharacter == '.' || cACharacter == 'E' || cACharacter == 'e'))
                    {
                        //不含小数点的浮点数
                        if (cACharacter == 'E' || cACharacter == 'e')
                        {
                            cAC_int = GetACharacterFromFile(); //继续读下一个字符
                            if (cAC_int != -1)
                                cACharacter = Convert.ToChar(cAC_int);
                            //如果为-，要缩小
                            if (cACharacter == '-')
                            {
                                cAC_int = GetACharacterFromFile(); //继续读下一个字符
                                if (cAC_int != -1)
                                    cACharacter = Convert.ToChar(cAC_int);
                                do
                                {
                                    nNumberAddition[nNumberOfAddition++] = cACharacter; //
                                    cAC_int = GetACharacterFromFile(); //继续读下一个字符
                                    if (cAC_int != -1)
                                        cACharacter = Convert.ToChar(cAC_int);
                                } while (cACharacter >= '0' && cACharacter <= '9');

                                //计算E，e后的具体数值
                                for (i = 0; i < nNumberOfAddition; i++)
                                {
                                    nValueOfAddition = 10 * nValueOfAddition + (nNumberAddition[i] - '0');
                                }

                                //开始将该单词放进符号表
                                szAWord = szAWord + 'E';
                                szAWord = szAWord + '-';
                                for (i = 0; i < nNumberOfAddition; i++)
                                {
                                    szAWord = szAWord + nNumberAddition[i];
                                }

                                myToken.g_Words[myToken.g_nWordsIndex].eType = WORD_TYPE_ENUM.REALNUMBER; //单词类型为单词类型枚举值REAL
                                myToken.g_Words[myToken.g_nWordsIndex].szName = szAWord; //识别出的单词放进单词队列
                                nNumberValue = nNumberValue / Math.Pow(10, nNumberOfFloat++); //缩小nNumberValue
                                myToken.g_Words[myToken.g_nWordsIndex].nNumberValue = nNumberValue; //同时将数的值赋给单词的nNumberValue
                                myToken.g_Words[myToken.g_nWordsIndex].nLineNo = myToken.g_nLineNo; //在源代码文件中单词所在的行数
                                myToken.g_Words[myToken.g_nWordsIndex].nColumnNo = myToken.g_nColumnNo;
                                myToken.g_nWordsIndex++; //识别出的单词加一
                                myToken.g_nColumnNo++;	//全部列号加一
                                myToken.g_PreWord.eType = WORD_TYPE_ENUM.REALNUMBER; //记录这个单词
                                return OK;
                            }
                            //如果为+，要放大
                            else if (cACharacter == '+')
                            {
                                cAC_int = GetACharacterFromFile(); //继续读下一个字符
                                if (cAC_int != -1)
                                    cACharacter = Convert.ToChar(cAC_int);
                                do
                                {
                                    nNumberAddition[nNumberOfAddition++] = cACharacter; //
                                    cAC_int = GetACharacterFromFile(); //继续读下一个字符
                                    if (cAC_int != -1)
                                        cACharacter = Convert.ToChar(cAC_int);
                                } while (cACharacter >= '0' && cACharacter <= '9');

                                //计算E，e后的具体数值
                                for (i = 0; i < nNumberOfAddition; i++)
                                {
                                    nValueOfAddition = 10 * nValueOfAddition + (nNumberAddition[i] - '0');
                                }

                                //开始将该单词放进符号表
                                szAWord = szAWord + 'E';
                                szAWord = szAWord + '+';
                                for (i = 0; i < nNumberOfAddition; i++)
                                {
                                    szAWord = szAWord + nNumberAddition[i];
                                }

                                myToken.g_Words[myToken.g_nWordsIndex].eType = WORD_TYPE_ENUM.REALNUMBER; //单词类型为单词类型枚举值REAL
                                myToken.g_Words[myToken.g_nWordsIndex].szName = szAWord; //识别出的单词放进单词队列
                                nNumberValue = nNumberValue * Math.Pow(10, nValueOfAddition); //放大nNumberValue
                                myToken.g_Words[myToken.g_nWordsIndex].nNumberValue = nNumberValue; //同时将数的值赋给单词的nNumberValue
                                myToken.g_Words[myToken.g_nWordsIndex].nLineNo = myToken.g_nLineNo; //在源代码文件中单词所在的行数
                                myToken.g_Words[myToken.g_nWordsIndex].nColumnNo = myToken.g_nColumnNo;
                                myToken.g_nWordsIndex++; //识别出的单词加一
                                myToken.g_nColumnNo++;	//全部列号加一
                                myToken.g_PreWord.eType = WORD_TYPE_ENUM.REALNUMBER; //记录这个单词
                                return OK;
                            }
                            //如果为空，要放大
                            else
                            {
                                cAC_int = GetACharacterFromFile(); //继续读下一个字符
                                if (cAC_int != -1)
                                    cACharacter = Convert.ToChar(cAC_int);
                                do
                                {
                                    nNumberAddition[nNumberOfAddition++] = cACharacter; //
                                    cAC_int = GetACharacterFromFile(); //继续读下一个字符
                                    if (cAC_int != -1)
                                        cACharacter = Convert.ToChar(cAC_int);
                                } while (cACharacter >= '0' && cACharacter <= '9');

                                //计算E，e后的具体数值
                                for (i = 0; i < nNumberOfAddition; i++)
                                {
                                    nValueOfAddition = 10 * nValueOfAddition + (nNumberAddition[i] - '0');
                                }

                                //开始将该单词放进符号表
                                szAWord = szAWord + 'E';
                                for (i = 0; i < nNumberOfAddition; i++)
                                {
                                    szAWord = szAWord + nNumberAddition[i];
                                }

                                myToken.g_Words[myToken.g_nWordsIndex].eType = WORD_TYPE_ENUM.REALNUMBER; //单词类型为单词类型枚举值REAL
                                myToken.g_Words[myToken.g_nWordsIndex].szName = szAWord; //识别出的单词放进单词队列
                                nNumberValue = nNumberValue * Math.Pow(10, nValueOfAddition); //放大nNumberValue
                                myToken.g_Words[myToken.g_nWordsIndex].nNumberValue = nNumberValue; //同时将数的值赋给单词的nNumberValue
                                myToken.g_Words[myToken.g_nWordsIndex].nLineNo = myToken.g_nLineNo; //在源代码文件中单词所在的行数
                                myToken.g_Words[myToken.g_nWordsIndex].nColumnNo = myToken.g_nColumnNo;
                                myToken.g_nWordsIndex++; //识别出的单词加一
                                myToken.g_nColumnNo++;	//全部列号加一
                                myToken.g_PreWord.eType = WORD_TYPE_ENUM.REALNUMBER; //记录这个单词
                                return OK;
                            }
                        }
                        else//含小数点的浮点数
                        {
                            szAWord = szAWord + '.'; //将.放入数组
                            cAC_int = GetACharacterFromFile(); //继续读下一个字符
                            if (cAC_int != -1)
                                cACharacter = Convert.ToChar(cAC_int);
                            do
                            {
                                szAWord = szAWord + cACharacter;
                                nNumberOfFloat++; //小数个数加一
                                nNumberValue = 10 * nNumberValue + cACharacter - '0'; //计算数单词的值

                                cAC_int = GetACharacterFromFile(); //继续读下一个字符
                                if (cAC_int != -1)
                                    cACharacter = Convert.ToChar(cAC_int);
                            } while (cACharacter >= '0' && cACharacter <= '9' && !myFile.EndOfStream); //如果是0~9，则继续拼装数单词

                            nNumberValue = nNumberValue / Math.Pow(10, nNumberOfFloat++);

                            //是否有E，e
                            if (cACharacter == 'E' || cACharacter == 'e')
                            {
                                cAC_int = GetACharacterFromFile(); //继续读下一个字符
                                if (cAC_int != -1)
                                    cACharacter = Convert.ToChar(cAC_int);
                                //如果为-，要缩小
                                if (cACharacter == '-')
                                {
                                    cAC_int = GetACharacterFromFile(); //继续读下一个字符
                                    if (cAC_int != -1)
                                        cACharacter = Convert.ToChar(cAC_int);
                                    do
                                    {
                                        nNumberAddition[nNumberOfAddition++] = cACharacter; //
                                        cAC_int = GetACharacterFromFile(); //继续读下一个字符
                                        if (cAC_int != -1)
                                            cACharacter = Convert.ToChar(cAC_int);
                                    } while (cACharacter >= '0' && cACharacter <= '9');

                                    //计算E，e后的具体数值
                                    for (i = 0; i < nNumberOfAddition; i++)
                                    {
                                        nValueOfAddition = 10 * nValueOfAddition + (nNumberAddition[i] - '0');
                                    }

                                    //开始将该单词放进符号表
                                    szAWord = szAWord + 'E';
                                    szAWord = szAWord + '-';
                                    for (i = 0; i < nNumberOfAddition; i++)
                                    {
                                        szAWord = szAWord + nNumberAddition[i];
                                    }

                                    myToken.g_Words[myToken.g_nWordsIndex].eType = WORD_TYPE_ENUM.REALNUMBER; //单词类型为单词类型枚举值REAL
                                    myToken.g_Words[myToken.g_nWordsIndex].szName = szAWord; //识别出的单词放进单词队列
                                    nNumberValue = nNumberValue / Math.Pow(10, nNumberOfFloat++); //缩小nNumberValue
                                    myToken.g_Words[myToken.g_nWordsIndex].nNumberValue = nNumberValue; //同时将数的值赋给单词的nNumberValue
                                    myToken.g_Words[myToken.g_nWordsIndex].nLineNo = myToken.g_nLineNo; //在源代码文件中单词所在的行数
                                    myToken.g_Words[myToken.g_nWordsIndex].nColumnNo = myToken.g_nColumnNo;
                                    myToken.g_nWordsIndex++; //识别出的单词加一
                                    myToken.g_nColumnNo++;	//全部列号加一
                                    myToken.g_PreWord.eType = WORD_TYPE_ENUM.REALNUMBER; //记录这个单词
                                    return OK;

                                }
                                //如果为+，要放大
                                else if (cACharacter == '+')
                                {
                                    cAC_int = GetACharacterFromFile(); //继续读下一个字符
                                    if (cAC_int != -1)
                                        cACharacter = Convert.ToChar(cAC_int);
                                    do
                                    {
                                        nNumberAddition[nNumberOfAddition++] = cACharacter; //
                                        cAC_int = GetACharacterFromFile(); //继续读下一个字符
                                        if (cAC_int != -1)
                                            cACharacter = Convert.ToChar(cAC_int);
                                    } while (cACharacter >= '0' && cACharacter <= '9');

                                    //计算E，e后的具体数值
                                    for (i = 0; i < nNumberOfAddition; i++)
                                    {
                                        nValueOfAddition = 10 * nValueOfAddition + (nNumberAddition[i] - '0');
                                    }

                                    //开始将该单词放进符号表
                                    szAWord = szAWord + 'E';
                                    szAWord = szAWord + '+';
                                    for (i = 0; i < nNumberOfAddition; i++)
                                    {
                                        szAWord = szAWord + nNumberAddition[i];
                                    }

                                    myToken.g_Words[myToken.g_nWordsIndex].eType = WORD_TYPE_ENUM.REALNUMBER; //单词类型为单词类型枚举值REAL
                                    myToken.g_Words[myToken.g_nWordsIndex].szName = szAWord; //识别出的单词放进单词队列
                                    nNumberValue = nNumberValue * Math.Pow(10, nValueOfAddition); //放大nNumberValue
                                    myToken.g_Words[myToken.g_nWordsIndex].nNumberValue = nNumberValue; //同时将数的值赋给单词的nNumberValue
                                    myToken.g_Words[myToken.g_nWordsIndex].nLineNo = myToken.g_nLineNo; //在源代码文件中单词所在的行数
                                    myToken.g_Words[myToken.g_nWordsIndex].nColumnNo = myToken.g_nColumnNo;
                                    myToken.g_nWordsIndex++; //识别出的单词加一
                                    myToken.g_nColumnNo++;	//全部列号加一
                                    myToken.g_PreWord.eType = WORD_TYPE_ENUM.REALNUMBER; //记录这个单词
                                    return OK;
                                }
                                //如果为空，要放大
                                else
                                {
                                    do
                                    {
                                        nNumberAddition[nNumberOfAddition++] = cACharacter; //
                                        cAC_int = GetACharacterFromFile(); //继续读下一个字符
                                        if (cAC_int != -1)
                                            cACharacter = Convert.ToChar(cAC_int);
                                    } while (cACharacter >= '0' && cACharacter <= '9');

                                    //计算E，e后的具体数值
                                    for (i = 0; i < nNumberOfAddition; i++)
                                    {
                                        nValueOfAddition = 10 * nValueOfAddition + (nNumberAddition[i] - '0');
                                    }

                                    //开始将该单词放进符号表
                                    szAWord = szAWord + 'E';
                                    for (i = 0; i < nNumberOfAddition; i++)
                                    {
                                        szAWord = szAWord + nNumberAddition[i];
                                    }

                                    myToken.g_Words[myToken.g_nWordsIndex].eType = WORD_TYPE_ENUM.REALNUMBER; //单词类型为单词类型枚举值REAL
                                    myToken.g_Words[myToken.g_nWordsIndex].szName = szAWord; //识别出的单词放进单词队列
                                    nNumberValue = nNumberValue * Math.Pow(10, nValueOfAddition); //放大nNumberValue
                                    myToken.g_Words[myToken.g_nWordsIndex].nNumberValue = nNumberValue; //同时将数的值赋给单词的nNumberValue
                                    myToken.g_Words[myToken.g_nWordsIndex].nLineNo = myToken.g_nLineNo; //在源代码文件中单词所在的行数
                                    myToken.g_Words[myToken.g_nWordsIndex].nColumnNo = myToken.g_nColumnNo;
                                    myToken.g_nWordsIndex++; //识别出的单词加一
                                    myToken.g_nColumnNo++;	//全部列号加一
                                    myToken.g_PreWord.eType = WORD_TYPE_ENUM.REALNUMBER; //记录这个单词
                                    return OK;
                                }
                            }
                            else //即不含E，e
                            {
                                myToken.g_Words[myToken.g_nWordsIndex].eType = WORD_TYPE_ENUM.REALNUMBER; //单词类型为单词类型枚举值REALNUMBER
                                myToken.g_Words[myToken.g_nWordsIndex].szName = szAWord; //识别出的单词放进单词队列
                                myToken.g_Words[myToken.g_nWordsIndex].nNumberValue = nNumberValue; //同时将数的值赋给单词的nNumberValue
                                myToken.g_Words[myToken.g_nWordsIndex].nLineNo = myToken.g_nLineNo; //在源代码文件中单词所在的行数
                                myToken.g_Words[myToken.g_nWordsIndex].nColumnNo = myToken.g_nColumnNo;
                                myToken.g_nWordsIndex++; //识别出的单词加一
                                myToken.g_nColumnNo++;	//全部列号加一
                                myToken.g_PreWord.eType = WORD_TYPE_ENUM.REALNUMBER; //记录这个单词
                                return OK;
                            }
                        }
                    }
                    else
                        return ERROR;
                }

                else if (cACharacter == '+' || cACharacter == '-')
                {//此时可能是数也可能是单符号
                    char positiveOrNegetive = cACharacter; //记录加减号
                    char tempcACharacter = cACharacter;
                    cAC_int = GetACharacterFromFile(); //继续读下一个字符
                    if (cAC_int != -1)
                        cACharacter = Convert.ToChar(cAC_int);
                    if (myToken.g_PreWord.eType == WORD_TYPE_ENUM.ASSIGN || myToken.g_PreWord.eType == WORD_TYPE_ENUM.DIVIDE ||
                         myToken.g_PreWord.eType == WORD_TYPE_ENUM.EQL || myToken.g_PreWord.eType == WORD_TYPE_ENUM.GEQ ||
                        myToken.g_PreWord.eType == WORD_TYPE_ENUM.GTR || myToken.g_PreWord.eType == WORD_TYPE_ENUM.LEFT_PARENTHESIS ||
                        myToken.g_PreWord.eType == WORD_TYPE_ENUM.LEQ || myToken.g_PreWord.eType == WORD_TYPE_ENUM.LES ||
                        myToken.g_PreWord.eType == WORD_TYPE_ENUM.MINUS || myToken.g_PreWord.eType == WORD_TYPE_ENUM.MULTIPLY ||
                        myToken.g_PreWord.eType == WORD_TYPE_ENUM.NEQ || myToken.g_PreWord.eType == WORD_TYPE_ENUM.PLUS)
                    {
                        if (cACharacter >= '0' && cACharacter <= '9')
                        {
                            //数字是以0~9开头
                            nNumberValue = 0; //数单词的值
                            nNumberOfAddition = 0; //E，e的附加成份个数
                            nNumberOfFloat = 0;
                            nValueOfAddition = 0;
                            //初始化 nNumberAddition[]
                            for (int j = 0; j < 5; j++)
                            {
                                nNumberAddition[j] = '*';
                            }
                            szAWord = szAWord + tempcACharacter;
                            do
                            {
                                szAWord = szAWord + cACharacter;
                                nNumberValue = 10 * nNumberValue + cACharacter - '0'; //计算数单词的值

                                cAC_int = GetACharacterFromFile(); //继续读下一个字符
                                if (cAC_int != -1)
                                    cACharacter = Convert.ToChar(cAC_int);

                            } while (cACharacter >= '0' && cACharacter <= '9' && !myFile.EndOfStream); //如果是0~9，则继续拼装数单词

                            //如果不是0~9则数单词暂且结束，可能是int类型
                            if (!myFile.EndOfStream && cACharacter != '.' && cACharacter != 'E' && cACharacter != 'e')
                            {
                                myToken.g_Words[myToken.g_nWordsIndex].eType = WORD_TYPE_ENUM.INTNUMBER; //单词类型为单词类型枚举值INTNUMBER
                                myToken.g_Words[myToken.g_nWordsIndex].szName = szAWord; //识别出的单词放进单词队列
                                if (positiveOrNegetive == '+')
                                    myToken.g_Words[myToken.g_nWordsIndex].nNumberValue = nNumberValue; //同时将数的值赋给单词的nNumberValue
                                else
                                    myToken.g_Words[myToken.g_nWordsIndex].nNumberValue = -nNumberValue;
                                myToken.g_Words[myToken.g_nWordsIndex].nLineNo = myToken.g_nLineNo; //在源代码文件中单词所在的行数
                                myToken.g_Words[myToken.g_nWordsIndex].nColumnNo = myToken.g_nColumnNo;
                                myToken.g_nWordsIndex++; //识别出的单词加一
                                myToken.g_nColumnNo++;	//全部列号加一
                                myToken.g_PreWord.eType = WORD_TYPE_ENUM.INTNUMBER; //记录这个单词
                                return OK;
                            }
                            //如果不是0~9则数单词暂且结束，可能是real类型
                            else if (!myFile.EndOfStream && (cACharacter == '.' || cACharacter == 'E' || cACharacter == 'e'))
                            {
                                //不含小数点的浮点数
                                if (cACharacter == 'E' || cACharacter == 'e')
                                {
                                    cAC_int = GetACharacterFromFile(); //继续读下一个字符
                                    if (cAC_int != -1)
                                        cACharacter = Convert.ToChar(cAC_int);

                                    //如果为-，要缩小
                                    if (cACharacter == '-')
                                    {
                                        cAC_int = GetACharacterFromFile(); //继续读下一个字符
                                        if (cAC_int != -1)
                                            cACharacter = Convert.ToChar(cAC_int);
                                        do
                                        {
                                            nNumberAddition[nNumberOfAddition++] = cACharacter; //
                                            cAC_int = GetACharacterFromFile(); //继续读下一个字符
                                            if (cAC_int != -1)
                                                cACharacter = Convert.ToChar(cAC_int);
                                        } while (cACharacter >= '0' && cACharacter <= '9');

                                        //计算E，e后的具体数值
                                        for (i = 0; i < nNumberOfAddition; i++)
                                        {
                                            nValueOfAddition = 10 * nValueOfAddition + (nNumberAddition[i] - '0');
                                        }

                                        //开始将该单词放进符号表
                                        szAWord = szAWord + 'E';
                                        szAWord = szAWord + '-';
                                        for (i = 0; i < nNumberOfAddition; i++)
                                        {
                                            szAWord = szAWord + nNumberAddition[i];
                                        }

                                        myToken.g_Words[myToken.g_nWordsIndex].eType = WORD_TYPE_ENUM.REALNUMBER; //单词类型为单词类型枚举值REAL
                                        myToken.g_Words[myToken.g_nWordsIndex].szName = szAWord; //识别出的单词放进单词队列
                                        nNumberValue = nNumberValue / Math.Pow(10, nNumberOfFloat++); //缩小nNumberValue
                                        if (positiveOrNegetive == '+')
                                            myToken.g_Words[myToken.g_nWordsIndex].nNumberValue = nNumberValue; //同时将数的值赋给单词的nNumberValue
                                        else
                                            myToken.g_Words[myToken.g_nWordsIndex].nNumberValue = -nNumberValue;
                                        myToken.g_Words[myToken.g_nWordsIndex].nLineNo = myToken.g_nLineNo; //在源代码文件中单词所在的行数
                                        myToken.g_Words[myToken.g_nWordsIndex].nColumnNo = myToken.g_nColumnNo;
                                        myToken.g_nWordsIndex++; //识别出的单词加一
                                        myToken.g_nColumnNo++;	//全部列号加一
                                        myToken.g_PreWord.eType = WORD_TYPE_ENUM.REALNUMBER; //记录这个单词
                                        return OK;

                                    }
                                    //如果为+，要放大
                                    else if (cACharacter == '+')
                                    {
                                        cAC_int = GetACharacterFromFile(); //继续读下一个字符
                                        if (cAC_int != -1)
                                            cACharacter = Convert.ToChar(cAC_int);
                                        do
                                        {
                                            nNumberAddition[nNumberOfAddition++] = cACharacter; //
                                            cAC_int = GetACharacterFromFile(); //继续读下一个字符
                                            if (cAC_int != -1)
                                                cACharacter = Convert.ToChar(cAC_int);
                                        } while (cACharacter >= '0' && cACharacter <= '9');

                                        //计算E，e后的具体数值
                                        for (i = 0; i < nNumberOfAddition; i++)
                                        {
                                            nValueOfAddition = 10 * nValueOfAddition + (nNumberAddition[i] - '0');
                                        }

                                        //开始将该单词放进符号表
                                        szAWord = szAWord + 'E';
                                        szAWord = szAWord + '+';
                                        for (i = 0; i < nNumberOfAddition; i++)
                                        {
                                            szAWord = szAWord + nNumberAddition[i];
                                        }

                                        myToken.g_Words[myToken.g_nWordsIndex].eType = WORD_TYPE_ENUM.REALNUMBER; //单词类型为单词类型枚举值REAL
                                        myToken.g_Words[myToken.g_nWordsIndex].szName = szAWord; //识别出的单词放进单词队列
                                        nNumberValue = nNumberValue * Math.Pow(10, nValueOfAddition); //放大nNumberValue
                                        if (positiveOrNegetive == '+')
                                            myToken.g_Words[myToken.g_nWordsIndex].nNumberValue = nNumberValue; //同时将数的值赋给单词的nNumberValue
                                        else
                                            myToken.g_Words[myToken.g_nWordsIndex].nNumberValue = -nNumberValue;
                                        myToken.g_Words[myToken.g_nWordsIndex].nLineNo = myToken.g_nLineNo; //在源代码文件中单词所在的行数
                                        myToken.g_Words[myToken.g_nWordsIndex].nColumnNo = myToken.g_nColumnNo;
                                        myToken.g_nWordsIndex++; //识别出的单词加一
                                        myToken.g_nColumnNo++;	//全部列号加一
                                        myToken.g_PreWord.eType = WORD_TYPE_ENUM.REALNUMBER; //记录这个单词
                                        return OK;
                                    }
                                    //如果为空，要放大
                                    else
                                    {
                                        cAC_int = GetACharacterFromFile(); //继续读下一个字符
                                        if (cAC_int != -1)
                                            cACharacter = Convert.ToChar(cAC_int);
                                        do
                                        {
                                            nNumberAddition[nNumberOfAddition++] = cACharacter; //
                                            cAC_int = GetACharacterFromFile(); //继续读下一个字符
                                            if (cAC_int != -1)
                                                cACharacter = Convert.ToChar(cAC_int);
                                        } while (cACharacter >= '0' && cACharacter <= '9');

                                        //计算E，e后的具体数值
                                        for (i = 0; i < nNumberOfAddition; i++)
                                        {
                                            nValueOfAddition = 10 * nValueOfAddition + (nNumberAddition[i] - '0');
                                        }

                                        //开始将该单词放进符号表
                                        szAWord = szAWord + 'E';
                                        for (i = 0; i < nNumberOfAddition; i++)
                                        {
                                            szAWord = szAWord + nNumberAddition[i];
                                        }

                                        myToken.g_Words[myToken.g_nWordsIndex].eType = WORD_TYPE_ENUM.REALNUMBER; //单词类型为单词类型枚举值REAL
                                        myToken.g_Words[myToken.g_nWordsIndex].szName = szAWord; //识别出的单词放进单词队列
                                        nNumberValue = nNumberValue * Math.Pow(10, nValueOfAddition); //放大nNumberValue
                                        if (positiveOrNegetive == '+')
                                            myToken.g_Words[myToken.g_nWordsIndex].nNumberValue = nNumberValue; //同时将数的值赋给单词的nNumberValue
                                        else
                                            myToken.g_Words[myToken.g_nWordsIndex].nNumberValue = -nNumberValue;
                                        myToken.g_Words[myToken.g_nWordsIndex].nLineNo = myToken.g_nLineNo; //在源代码文件中单词所在的行数
                                        myToken.g_Words[myToken.g_nWordsIndex].nColumnNo = myToken.g_nColumnNo;
                                        myToken.g_nWordsIndex++; //识别出的单词加一
                                        myToken.g_nColumnNo++;	//全部列号加一
                                        myToken.g_PreWord.eType = WORD_TYPE_ENUM.REALNUMBER; //记录这个单词
                                        return OK;
                                    }
                                }
                                else//含小数点的浮点数
                                {
                                    szAWord = szAWord + '.'; //将.放入数组
                                    cAC_int = GetACharacterFromFile(); //继续读下一个字符
                                    if (cAC_int != -1)
                                        cACharacter = Convert.ToChar(cAC_int);
                                    do
                                    {
                                        szAWord = szAWord + cACharacter;
                                        nNumberOfFloat++; //小数个数加一
                                        nNumberValue = 10 * nNumberValue + cACharacter - '0'; //计算数单词的值

                                        cAC_int = GetACharacterFromFile(); //继续读下一个字符
                                        if (cAC_int != -1)
                                            cACharacter = Convert.ToChar(cAC_int);
                                    } while (cACharacter >= '0' && cACharacter <= '9' && !myFile.EndOfStream); //如果是0~9，则继续拼装数单词

                                    nNumberValue = nNumberValue / Math.Pow(10, nNumberOfFloat++);

                                    //是否有E，e
                                    if (cACharacter == 'E' || cACharacter == 'e')
                                    {
                                        cAC_int = GetACharacterFromFile(); //继续读下一个字符
                                        if (cAC_int != -1)
                                            cACharacter = Convert.ToChar(cAC_int);
                                        //如果为-，要缩小
                                        if (cACharacter == '-')
                                        {
                                            cAC_int = GetACharacterFromFile(); //继续读下一个字符
                                            if (cAC_int != -1)
                                                cACharacter = Convert.ToChar(cAC_int);
                                            do
                                            {
                                                nNumberAddition[nNumberOfAddition++] = cACharacter; //
                                                cAC_int = GetACharacterFromFile(); //继续读下一个字符
                                                if (cAC_int != -1)
                                                    cACharacter = Convert.ToChar(cAC_int);
                                            } while (cACharacter >= '0' && cACharacter <= '9');

                                            //计算E，e后的具体数值
                                            for (i = 0; i < nNumberOfAddition; i++)
                                            {
                                                nValueOfAddition = 10 * nValueOfAddition + (nNumberAddition[i] - '0');
                                            }

                                            //开始将该单词放进符号表
                                            szAWord = szAWord + 'E';
                                            szAWord = szAWord + '-';
                                            for (i = 0; i < nNumberOfAddition; i++)
                                            {
                                                szAWord = szAWord + nNumberAddition[i];
                                            }

                                            myToken.g_Words[myToken.g_nWordsIndex].eType = WORD_TYPE_ENUM.REALNUMBER; //单词类型为单词类型枚举值REAL
                                            myToken.g_Words[myToken.g_nWordsIndex].szName = szAWord; //识别出的单词放进单词队列
                                            nNumberValue = nNumberValue / Math.Pow(10, nNumberOfFloat++); //缩小nNumberValue
                                            if (positiveOrNegetive == '+')
                                                myToken.g_Words[myToken.g_nWordsIndex].nNumberValue = nNumberValue; //同时将数的值赋给单词的nNumberValue
                                            else
                                                myToken.g_Words[myToken.g_nWordsIndex].nNumberValue = -nNumberValue;
                                            myToken.g_Words[myToken.g_nWordsIndex].nLineNo = myToken.g_nLineNo; //在源代码文件中单词所在的行数
                                            myToken.g_Words[myToken.g_nWordsIndex].nColumnNo = myToken.g_nColumnNo;
                                            myToken.g_nWordsIndex++; //识别出的单词加一
                                            myToken.g_nColumnNo++;	//全部列号加一
                                            myToken.g_PreWord.eType = WORD_TYPE_ENUM.REALNUMBER; //记录这个单词
                                            return OK;

                                        }
                                        //如果为+，要放大
                                        else if (cACharacter == '+')
                                        {
                                            cAC_int = GetACharacterFromFile(); //继续读下一个字符
                                            if (cAC_int != -1)
                                                cACharacter = Convert.ToChar(cAC_int);
                                            do
                                            {
                                                nNumberAddition[nNumberOfAddition++] = cACharacter; //
                                                cAC_int = GetACharacterFromFile(); //继续读下一个字符
                                                if (cAC_int != -1)
                                                    cACharacter = Convert.ToChar(cAC_int);
                                            } while (cACharacter >= '0' && cACharacter <= '9');

                                            //计算E，e后的具体数值
                                            for (i = 0; i < nNumberOfAddition; i++)
                                            {
                                                nValueOfAddition = 10 * nValueOfAddition + (nNumberAddition[i] - '0');
                                            }

                                            //开始将该单词放进符号表
                                            szAWord = szAWord + 'E';
                                            szAWord = szAWord + '+';
                                            for (i = 0; i < nNumberOfAddition; i++)
                                            {
                                                szAWord = szAWord + nNumberAddition[i];
                                            }

                                            myToken.g_Words[myToken.g_nWordsIndex].eType = WORD_TYPE_ENUM.REALNUMBER; //单词类型为单词类型枚举值REAL
                                            myToken.g_Words[myToken.g_nWordsIndex].szName = szAWord; //识别出的单词放进单词队列
                                            nNumberValue = nNumberValue * Math.Pow(10, nValueOfAddition); //放大nNumberValue
                                            if (positiveOrNegetive == '+')
                                                myToken.g_Words[myToken.g_nWordsIndex].nNumberValue = nNumberValue; //同时将数的值赋给单词的nNumberValue
                                            else
                                                myToken.g_Words[myToken.g_nWordsIndex].nNumberValue = -nNumberValue;
                                            myToken.g_Words[myToken.g_nWordsIndex].nLineNo = myToken.g_nLineNo; //在源代码文件中单词所在的行数
                                            myToken.g_Words[myToken.g_nWordsIndex].nColumnNo = myToken.g_nColumnNo;
                                            myToken.g_nWordsIndex++; //识别出的单词加一
                                            myToken.g_nColumnNo++;	//全部列号加一
                                            myToken.g_PreWord.eType = WORD_TYPE_ENUM.REALNUMBER; //记录这个单词
                                            return OK;
                                        }
                                        //如果为空，要放大
                                        else
                                        {
                                            do
                                            {
                                                nNumberAddition[nNumberOfAddition++] = cACharacter; //
                                                cAC_int = GetACharacterFromFile(); //继续读下一个字符
                                                if (cAC_int != -1)
                                                    cACharacter = Convert.ToChar(cAC_int);
                                            } while (cACharacter >= '0' && cACharacter <= '9');

                                            //计算E，e后的具体数值
                                            for (i = 0; i < nNumberOfAddition; i++)
                                            {
                                                nValueOfAddition = 10 * nValueOfAddition + (nNumberAddition[i] - '0');
                                            }

                                            //开始将该单词放进符号表
                                            szAWord = szAWord + 'E';
                                            for (i = 0; i < nNumberOfAddition; i++)
                                            {
                                                szAWord = szAWord + nNumberAddition[i];
                                            }

                                            myToken.g_Words[myToken.g_nWordsIndex].eType = WORD_TYPE_ENUM.REALNUMBER; //单词类型为单词类型枚举值REAL
                                            myToken.g_Words[myToken.g_nWordsIndex].szName = szAWord; //识别出的单词放进单词队列
                                            nNumberValue = nNumberValue * Math.Pow(10, nValueOfAddition); //放大nNumberValue
                                            if (positiveOrNegetive == '+')
                                                myToken.g_Words[myToken.g_nWordsIndex].nNumberValue = nNumberValue; //同时将数的值赋给单词的nNumberValue
                                            else
                                                myToken.g_Words[myToken.g_nWordsIndex].nNumberValue = -nNumberValue;
                                            myToken.g_Words[myToken.g_nWordsIndex].nLineNo = myToken.g_nLineNo; //在源代码文件中单词所在的行数
                                            myToken.g_Words[myToken.g_nWordsIndex].nColumnNo = myToken.g_nColumnNo;
                                            myToken.g_nWordsIndex++; //识别出的单词加一
                                            myToken.g_nColumnNo++;	//全部列号加一
                                            myToken.g_PreWord.eType = WORD_TYPE_ENUM.REALNUMBER; //记录这个单词
                                            return OK;
                                        }
                                    }
                                    else //即不含E，e
                                    {
                                        myToken.g_Words[myToken.g_nWordsIndex].eType = WORD_TYPE_ENUM.REALNUMBER; //单词类型为单词类型枚举值REAL
                                        myToken.g_Words[myToken.g_nWordsIndex].szName = szAWord; //识别出的单词放进单词队列
                                        if (positiveOrNegetive == '+')
                                            myToken.g_Words[myToken.g_nWordsIndex].nNumberValue = nNumberValue; //同时将数的值赋给单词的nNumberValue
                                        else
                                            myToken.g_Words[myToken.g_nWordsIndex].nNumberValue = -nNumberValue;
                                        myToken.g_Words[myToken.g_nWordsIndex].nLineNo = myToken.g_nLineNo; //在源代码文件中单词所在的行数
                                        myToken.g_Words[myToken.g_nWordsIndex].nColumnNo = myToken.g_nColumnNo;
                                        myToken.g_nWordsIndex++; //识别出的单词加一
                                        myToken.g_nColumnNo++;	//全部列号加一
                                        myToken.g_PreWord.eType = WORD_TYPE_ENUM.REALNUMBER; //记录这个单词
                                        return OK;
                                    }
                                }
                            }
                        }
                    }
                    else //如果不是则。。。
                    {
                        if (tempcACharacter == '+')
                        {//单词是"+"
                            myToken.g_Words[myToken.g_nWordsIndex].eType = WORD_TYPE_ENUM.PLUS; //单词类型为单词类型枚举值PLUS
                            myToken.g_Words[myToken.g_nWordsIndex].szName = "+"; //识别出的单词放进单词队列
                            myToken.g_Words[myToken.g_nWordsIndex].nLineNo = myToken.g_nLineNo; //在源代码文件中单词所在行数
                            myToken.g_Words[myToken.g_nWordsIndex].nColumnNo = myToken.g_nColumnNo; //						
                            myToken.g_nWordsIndex++; //识别出的单词个数加一
                            myToken.g_nColumnNo++;	//全部列号加一
                            myToken.g_PreWord.eType = WORD_TYPE_ENUM.PLUS; //记录这个单词
                            //此处不可再取一个符号
                            return OK;
                        }
                        else if (tempcACharacter == '-')
                        {//单词是"-"
                            myToken.g_Words[myToken.g_nWordsIndex].eType = WORD_TYPE_ENUM.MINUS; //单词类型为单词类型枚举值MINUS
                            myToken.g_Words[myToken.g_nWordsIndex].szName = "-"; //识别出的单词放进单词队列
                            myToken.g_Words[myToken.g_nWordsIndex].nLineNo = myToken.g_nLineNo; //在源代码文件中单词所在行数
                            myToken.g_Words[myToken.g_nWordsIndex].nColumnNo = myToken.g_nColumnNo; //
                            myToken.g_nWordsIndex++; //识别出的单词个数加一
                            myToken.g_nColumnNo++;	//全部列号加一
                            myToken.g_PreWord.eType = WORD_TYPE_ENUM.MINUS; //记录这个单词
                            //此处不可再取一个符号
                            return OK;
                        }
                    }
                }

                else if (cACharacter == '<') //检测是"<"还是"<="单词
                {
                    cAC_int = GetACharacterFromFile(); //继续读下一个字符
                    if (cAC_int != -1)
                        cACharacter = Convert.ToChar(cAC_int);
                    if (cACharacter == '=')
                    {//单词是"<="
                        myToken.g_Words[myToken.g_nWordsIndex].eType = WORD_TYPE_ENUM.LEQ; //单词类型为单词类型枚举值LEQ
                        myToken.g_Words[myToken.g_nWordsIndex].szName = "<="; //识别出的单词放进单词队列
                        myToken.g_Words[myToken.g_nWordsIndex].nLineNo = myToken.g_nLineNo; //在源代码文件中单词所在行数
                        myToken.g_Words[myToken.g_nWordsIndex].nColumnNo = myToken.g_nColumnNo; //
                        myToken.g_nWordsIndex++; //识别出的单词个数加一
                        myToken.g_nColumnNo++;	//全部列号加一
                        myToken.g_PreWord.eType = WORD_TYPE_ENUM.LEQ; //记录这个单词
                        cAC_int = GetACharacterFromFile(); //继续读下一个字符
                        if (cAC_int != -1)
                            cACharacter = Convert.ToChar(cAC_int);
                        return OK;
                    }
                    else
                    {//单词是"<"
                        myToken.g_Words[myToken.g_nWordsIndex].eType = WORD_TYPE_ENUM.LES; //单词类型为单词类型枚举值LES
                        myToken.g_Words[myToken.g_nWordsIndex].szName = "<"; //识别出的单词放进单词队列
                        myToken.g_Words[myToken.g_nWordsIndex].nLineNo = myToken.g_nLineNo; //在源代码文件中单词所在行数
                        myToken.g_Words[myToken.g_nWordsIndex].nColumnNo = myToken.g_nColumnNo; //
                        myToken.g_nWordsIndex++; //识别出的单词个数加一
                        myToken.g_nColumnNo++;	//全部列号加一
                        myToken.g_PreWord.eType = WORD_TYPE_ENUM.LES; //记录这个单词
                        return OK;
                    }
                }

                else if (cACharacter == '>') //检测是">"还是">="单词
                {
                    cAC_int = GetACharacterFromFile(); //继续读下一个字符
                    if (cAC_int != -1)
                        cACharacter = Convert.ToChar(cAC_int);
                    if (cACharacter == '=')
                    {//单词是">="
                        myToken.g_Words[myToken.g_nWordsIndex].eType = WORD_TYPE_ENUM.GEQ; //单词类型为单词类型枚举值GEQ
                        myToken.g_Words[myToken.g_nWordsIndex].szName = ">="; //识别出的单词放进单词队列
                        myToken.g_Words[myToken.g_nWordsIndex].nLineNo = myToken.g_nLineNo; //在源代码文件中单词所在行数
                        myToken.g_Words[myToken.g_nWordsIndex].nColumnNo = myToken.g_nColumnNo; //
                        myToken.g_nWordsIndex++; //识别出的单词个数加一
                        myToken.g_nColumnNo++;	//全部列号加一
                        myToken.g_PreWord.eType = WORD_TYPE_ENUM.GEQ; //记录这个单词
                        cAC_int = GetACharacterFromFile(); //继续读下一个字符
                        if (cAC_int != -1)
                            cACharacter = Convert.ToChar(cAC_int);
                        return OK;
                    }
                    else
                    {//单词是">"
                        myToken.g_Words[myToken.g_nWordsIndex].eType = WORD_TYPE_ENUM.GTR; //单词类型为单词类型枚举值GTR
                        myToken.g_Words[myToken.g_nWordsIndex].szName = ">"; //识别出的单词放进单词队列
                        myToken.g_Words[myToken.g_nWordsIndex].nLineNo = myToken.g_nLineNo; //在源代码文件中单词所在行数
                        myToken.g_Words[myToken.g_nWordsIndex].nColumnNo = myToken.g_nColumnNo; //
                        myToken.g_nWordsIndex++; //识别出的单词个数加一
                        myToken.g_nColumnNo++;	//全部列号加一
                        myToken.g_PreWord.eType = WORD_TYPE_ENUM.GTR; //记录这个单词
                        return OK;
                    }
                }

                else if (cACharacter == '=') //检测是"="还是"=="单词
                {
                    cAC_int = GetACharacterFromFile(); //继续读下一个字符
                    if (cAC_int != -1)
                        cACharacter = Convert.ToChar(cAC_int);
                    if (cACharacter == '=')
                    {//单词是"=="
                        myToken.g_Words[myToken.g_nWordsIndex].eType = WORD_TYPE_ENUM.EQL; //单词类型为单词类型枚举值EQL
                        myToken.g_Words[myToken.g_nWordsIndex].szName = "=="; //识别出的单词放进单词队列
                        myToken.g_Words[myToken.g_nWordsIndex].nLineNo = myToken.g_nLineNo; //在源代码文件中单词所在行数
                        myToken.g_Words[myToken.g_nWordsIndex].nColumnNo = myToken.g_nColumnNo; //
                        myToken.g_nWordsIndex++; //识别出的单词个数加一
                        myToken.g_nColumnNo++;	//全部列号加一
                        myToken.g_PreWord.eType = WORD_TYPE_ENUM.EQL; //记录这个单词
                        cAC_int = GetACharacterFromFile(); //继续读下一个字符
                        if (cAC_int != -1)
                            cACharacter = Convert.ToChar(cAC_int);
                        return OK;
                    }
                    else
                    {//单词是"="
                        myToken.g_Words[myToken.g_nWordsIndex].eType = WORD_TYPE_ENUM.ASSIGN; //单词类型为单词类型枚举值ASSIGN
                        myToken.g_Words[myToken.g_nWordsIndex].szName = "="; //识别出的单词放进单词队列
                        myToken.g_Words[myToken.g_nWordsIndex].nLineNo = myToken.g_nLineNo; //在源代码文件中单词所在行数
                        myToken.g_Words[myToken.g_nWordsIndex].nColumnNo = myToken.g_nColumnNo; //
                        myToken.g_nWordsIndex++; //识别出的单词个数加一
                        myToken.g_nColumnNo++;	//全部列号加一
                        myToken.g_PreWord.eType = WORD_TYPE_ENUM.ASSIGN; //记录这个单词
                        return OK;
                    }
                }

                else if (cACharacter == '!') //检测是"!="单词
                {
                    cAC_int = GetACharacterFromFile(); //继续读下一个字符
                    if (cAC_int != -1)
                        cACharacter = Convert.ToChar(cAC_int);
                    if (cACharacter == '=')
                    {//单词是"!="
                        myToken.g_Words[myToken.g_nWordsIndex].eType = WORD_TYPE_ENUM.NEQ; //单词类型为单词类型枚举值NEQ
                        myToken.g_Words[myToken.g_nWordsIndex].szName = "!="; //识别出的单词放进单词队列
                        myToken.g_Words[myToken.g_nWordsIndex].nLineNo = myToken.g_nLineNo; //在源代码文件中单词所在行数
                        myToken.g_Words[myToken.g_nWordsIndex].nColumnNo = myToken.g_nColumnNo; //
                        myToken.g_nWordsIndex++; //识别出的单词个数加一
                        myToken.g_nColumnNo++;	//全部列号加一
                        myToken.g_PreWord.eType = WORD_TYPE_ENUM.NEQ; //记录这个单词
                        cAC_int = GetACharacterFromFile(); //继续读下一个字符
                        if (cAC_int != -1)
                            cACharacter = Convert.ToChar(cAC_int);
                        return OK;
                    }
                    else
                        return ERROR;
                }

                else if (cACharacter == '/') //检测是"/"还是"//"单词
                {
                    cAC_int = GetACharacterFromFile(); //继续读下一个字符
                    if (cAC_int != -1)
                        cACharacter = Convert.ToChar(cAC_int);
                    if (cACharacter == '/')
                    {//单词是"//"
                        myToken.judgecomment = true;
                        while (cACharacter != RETURN)
                        {
                            cAC_int = GetACharacterFromFile(); //读完该行
                            if (cAC_int != -1)
                                cACharacter = Convert.ToChar(cAC_int);
                        }
                        cAC_int = GetACharacterFromFile(); //继续读下一个字符
                        if (cAC_int != -1)
                            cACharacter = Convert.ToChar(cAC_int);
                        return OK;
                    }
                    else
                    {//单词是"/"
                        myToken.g_Words[myToken.g_nWordsIndex].eType = WORD_TYPE_ENUM.DIVIDE; //单词类型为单词类型枚举值DIVIDE
                        myToken.g_Words[myToken.g_nWordsIndex].szName = "/"; //识别出的单词放进单词队列
                        myToken.g_Words[myToken.g_nWordsIndex].nLineNo = myToken.g_nLineNo; //在源代码文件中单词所在行数
                        myToken.g_Words[myToken.g_nWordsIndex].nColumnNo = myToken.g_nColumnNo; //
                        myToken.g_nWordsIndex++; //识别出的单词个数加一
                        myToken.g_nColumnNo++;	//全部列号加一
                        myToken.g_PreWord.eType = WORD_TYPE_ENUM.DIVIDE; //记录这个单词
                        return OK;
                    }
                }

                else
                {	//当不满足上述条件是，则是单字符
                    //通过查表寻找单字符的单词类型枚举值
                    myToken.g_Words[myToken.g_nWordsIndex].eType = myToken.SingleCharacterWordTypeTable[cACharacter];
                    myToken.g_Words[myToken.g_nWordsIndex].szName = myToken.g_Words[myToken.g_nWordsIndex].szName + cACharacter; //单词放进单词队列g_Words中
                    myToken.g_Words[myToken.g_nWordsIndex].nLineNo = myToken.g_nLineNo; //
                    myToken.g_Words[myToken.g_nWordsIndex].nColumnNo = myToken.g_nColumnNo;
                    myToken.g_nWordsIndex++; //
                    myToken.g_nColumnNo++;	//全部列号加一
                    myToken.g_PreWord.eType = myToken.SingleCharacterWordTypeTable[cACharacter]; //记录这个单词

                    cAC_int = GetACharacterFromFile(); //继续读下一个字符
                    if (cAC_int != -1)
                        cACharacter = Convert.ToChar(cAC_int);
                    return OK;
                }
            }
            return ERROR;	//cACharacter==EOF
        }

        public void getPrintInLexis(int nIndex, string[] myString) //打印单词队列中的每一个单词
        {
            myString[0] = Convert.ToString(nIndex);
            myString[1] = myToken.g_Words[nIndex].szName;
            myString[2] = Convert.ToString(myToken.g_Words[nIndex].eType);
            if (String.Equals(myString[2], Convert.ToString(WORD_TYPE_ENUM.INTNUMBER)))
                myString[3] = Convert.ToString(Convert.ToInt32(myToken.g_Words[nIndex].nNumberValue));
            if (String.Equals(myString[2], Convert.ToString(WORD_TYPE_ENUM.REALNUMBER)))
                myString[3] = Convert.ToString(myToken.g_Words[nIndex].nNumberValue);
            myString[4] = Convert.ToString(myToken.g_Words[nIndex].nLineNo);
            myString[5] = Convert.ToString(myToken.g_Words[nIndex].nColumnNo);
        }

        //common：释放Token，每次重新运行时都需要
        public void rebuiltToken()
        {
            cACharacter = Convert.ToChar(32);
            cAC_int = 32;
            this.myToken = new Token();
        }

        //将枚举类型变为字符串类型
        public string enumToString(int nIndex)
        {
            string myString = "";
            switch (myToken.g_Words[nIndex].eType)
            {
                case WORD_TYPE_ENUM.IDENTIFIER:
                    myString = "ID";
                    break;
                case WORD_TYPE_ENUM.INTNUMBER:
                    myString = "NUM";
                    break;
                case WORD_TYPE_ENUM.REALNUMBER:
                    myString = "NUM";
                    break;
                case WORD_TYPE_ENUM.INT:
                    myString = "int";
                    break;
                case WORD_TYPE_ENUM.REAL:
                    myString = "real";
                    break;
                case WORD_TYPE_ENUM.IF:
                    myString = "if";
                    break;
                case WORD_TYPE_ENUM.THEN:
                    myString = "then";
                    break;
                case WORD_TYPE_ENUM.ELSE:
                    myString = "else";
                    break;
                case WORD_TYPE_ENUM.WHILE:
                    myString = "while";
                    break;
                case WORD_TYPE_ENUM.LEFT_PARENTHESIS:
                    myString = "(";
                    break;
                case WORD_TYPE_ENUM.RIGHT_PARENTHESIS:
                    myString = ")";
                    break;
                case WORD_TYPE_ENUM.LEFT_BRACE:
                    myString = "{";
                    break;
                case WORD_TYPE_ENUM.RIGHT_BRACE:
                    myString = "}";
                    break;
                case WORD_TYPE_ENUM.COMMA:
                    myString = ",";
                    break;
                case WORD_TYPE_ENUM.SEMICOLON:
                    myString = ";";
                    break;
                case WORD_TYPE_ENUM.PERIOD:
                    myString = ".";
                    break;
                case WORD_TYPE_ENUM.PLUS:
                    myString = "+";
                    break;
                case WORD_TYPE_ENUM.MINUS:
                    myString = "-";
                    break;
                case WORD_TYPE_ENUM.MULTIPLY:
                    myString = "*";
                    break;
                case WORD_TYPE_ENUM.DIVIDE:
                    myString = "/";
                    break;
                case WORD_TYPE_ENUM.EQL:
                    myString = "==";
                    break;
                case WORD_TYPE_ENUM.NEQ:
                    myString = "!=";
                    break;
                case WORD_TYPE_ENUM.LES:
                    myString = "<";
                    break;
                case WORD_TYPE_ENUM.LEQ:
                    myString = "<=";
                    break;
                case WORD_TYPE_ENUM.GTR:
                    myString = ">";
                    break;
                case WORD_TYPE_ENUM.GEQ:
                    myString = ">=";
                    break;
                case WORD_TYPE_ENUM.ASSIGN:
                    myString = "=";
                    break;
            }
            return myString;
        }
    }
}
