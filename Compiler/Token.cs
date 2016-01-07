using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Compiler
{
    public enum WORD_TYPE_ENUM
    { //单词类型枚举值
        INVALID_WORD,
        IDENTIFIER,
        INTNUMBER,			//整数的属性
        REALNUMBER,			//实数的属性
        //保留字 开始
        INT,				//int保留字
        REAL,				//real保留字
        IF,
        THEN,
        ELSE,
        WHILE,
        //保留字 结束
        //单字符符号 开始
        LEFT_PARENTHESIS,	// (
        RIGHT_PARENTHESIS,	// )
        LEFT_BRACE,			// {
        RIGHT_BRACE,		// }
        COMMA,				// ,
        SEMICOLON,			// ;
        PERIOD,				// .
        PLUS,				// +
        MINUS,				// -
        MULTIPLY,			// *
        DIVIDE,				// /
        LES,				// <
        GTR,				// >
        EXCLAM,				// !
        ASSIGN,				// =

        //单字符符号 结束
        NEQ,				// !=
        LEQ,				// <=
        GEQ,				// >=
        EQL,				// ==
    };
    public class Token
    {
       //public static int MAX_LENGTH_OF_A_WORD = 64;   //一个单词的最多字符个数
       public static int MAX_NUMBER_OF_WORDS = 200;  //可识别的最多单词个数
       public static int NUMBER_OF_RESERVED_WORDS = 6; //保留字个数,保留字:int real if then else while

       //保留字的名字字符串和类型对照表结构
       public struct RESERVED_WORD_NAME_VS_TYPE_STRUCT
       {
           public string szName ; //保留字的名字字符串
	       public WORD_TYPE_ENUM eType; //保留字的单词类型枚举值
       };
      
       public struct WORD_STRUCT
       {  //一个单词的数据结构
           public string szName;  //单词名字的字符串
           public WORD_TYPE_ENUM eType;  //单词类型枚举值
           public double nNumberValue;  //数单词的值
           public int nLineNo;  //在源代码文件中单词所在的行数
           //打印调试信息时表示这个单词在源程序中的行号
	       public int nColumnNo; //添加变量用于记录列号
        };
        
        //
        //全局变量
        //
        public RESERVED_WORD_NAME_VS_TYPE_STRUCT[] ReservedWordNameVsTypeTable = new RESERVED_WORD_NAME_VS_TYPE_STRUCT[NUMBER_OF_RESERVED_WORDS];
        //保留字的名字字符串和类型对照表

        public WORD_TYPE_ENUM[] SingleCharacterWordTypeTable = new WORD_TYPE_ENUM[256]; //单字符单词的字符和类型对照表
        public WORD_STRUCT[] g_Words = new WORD_STRUCT[MAX_NUMBER_OF_WORDS]; //已识别出的单词队列
        public WORD_STRUCT g_PreWord = new WORD_STRUCT();  //存储前一个单词，用于区分+和+6.1这种情况
        //
        //
        //!!!这个可以用文件保存
        //后面可以知道，g_Words只放IDENTIFIER类型的字符串
        public int g_nWordsIndex = 0; //已识别出的单词的个数或序号
        public int g_nLineNo = 1; //文件中源代码的行数
        public int g_nColumnNo = 1;//文件中该单词所在这行的第几个位置上

        public bool judgecomment; //判断注释输出变量

        public void InitializeReservedWordTable() //设置保留字单词的名字字符串和相应类型的对照表
        {
            ReservedWordNameVsTypeTable[0].szName = "int";
            ReservedWordNameVsTypeTable[0].eType = WORD_TYPE_ENUM.INT;
            ReservedWordNameVsTypeTable[1].szName = "real";
            ReservedWordNameVsTypeTable[1].eType = WORD_TYPE_ENUM.REAL;
            ReservedWordNameVsTypeTable[2].szName = "if";
            ReservedWordNameVsTypeTable[2].eType = WORD_TYPE_ENUM.IF;
            ReservedWordNameVsTypeTable[3].szName = "then";
            ReservedWordNameVsTypeTable[3].eType = WORD_TYPE_ENUM.THEN;
            ReservedWordNameVsTypeTable[4].szName = "else";
            ReservedWordNameVsTypeTable[4].eType = WORD_TYPE_ENUM.ELSE;
            ReservedWordNameVsTypeTable[5].szName = "while";
            ReservedWordNameVsTypeTable[5].eType = WORD_TYPE_ENUM.WHILE;
        }

        public void InitializeSingleCharacterTable() //设置单字符单词的字符和相应类型的对照表
        {
            int i;
            for (i = 0; i <= 255; i++)
            {
                SingleCharacterWordTypeTable[i] = WORD_TYPE_ENUM.INVALID_WORD;
            }
            //operators
            SingleCharacterWordTypeTable['+'] = WORD_TYPE_ENUM.PLUS;
            SingleCharacterWordTypeTable['-'] = WORD_TYPE_ENUM.MINUS;
            SingleCharacterWordTypeTable['*'] = WORD_TYPE_ENUM.MULTIPLY;
            SingleCharacterWordTypeTable['/'] = WORD_TYPE_ENUM.DIVIDE;
            SingleCharacterWordTypeTable['<'] = WORD_TYPE_ENUM.LES;
            SingleCharacterWordTypeTable['>'] = WORD_TYPE_ENUM.GTR;
            SingleCharacterWordTypeTable['!'] = WORD_TYPE_ENUM.EXCLAM;
            SingleCharacterWordTypeTable['='] = WORD_TYPE_ENUM.ASSIGN;
            //delimiters
            SingleCharacterWordTypeTable['('] = WORD_TYPE_ENUM.LEFT_PARENTHESIS;
            SingleCharacterWordTypeTable[')'] = WORD_TYPE_ENUM.RIGHT_PARENTHESIS;
            SingleCharacterWordTypeTable['{'] = WORD_TYPE_ENUM.LEFT_BRACE;
            SingleCharacterWordTypeTable['}'] = WORD_TYPE_ENUM.RIGHT_BRACE;
            SingleCharacterWordTypeTable[','] = WORD_TYPE_ENUM.COMMA;
            SingleCharacterWordTypeTable['.'] = WORD_TYPE_ENUM.PERIOD;
            SingleCharacterWordTypeTable[';'] = WORD_TYPE_ENUM.SEMICOLON;
        }

        public Token(){
           InitializeReservedWordTable(); //设置保留字单词的名字字符串和相应类型的对照表
           InitializeSingleCharacterTable(); //设置单字符单词的字符和相应类型的对照表
        }
    }
}
