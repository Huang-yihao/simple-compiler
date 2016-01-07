using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    //产生式
    public class Production
    {
        public string[] productionSubstring = new string[10];
        public static void Production_Initialize(Production[] Production)
        {
            for (int i = 0; i < 36; i++)
                Production[i] = new Production();// C#中基本都要new来实例化才能使用

            //0、synch
            Production[0].productionSubstring[0] = "synch";
            //1、program->compoundstmt
            Production[1].productionSubstring[0] = "program";
            Production[1].productionSubstring[1] = "->";
            Production[1].productionSubstring[2] = "compoundstmt";
            //2、stmt->decl
            Production[2].productionSubstring[0] = "stmt";
            Production[2].productionSubstring[1] = "->";
            Production[2].productionSubstring[2] = "decl";
            //3、stmt->ifstmt
            Production[3].productionSubstring[0] = "stmt";
            Production[3].productionSubstring[1] = "->";
            Production[3].productionSubstring[2] = "ifstmt";
            //4、stmt->whilestmt
            Production[4].productionSubstring[0] = "stmt";
            Production[4].productionSubstring[1] = "->";
            Production[4].productionSubstring[2] = "whilestmt";
            //5、stmt->assgstmt
            Production[5].productionSubstring[0] = "stmt";
            Production[5].productionSubstring[1] = "->";
            Production[5].productionSubstring[2] = "assgstmt";
            //6、stmt->compoundstmt
            Production[6].productionSubstring[0] = "stmt";
            Production[6].productionSubstring[1] = "->";
            Production[6].productionSubstring[2] = "compoundstmt";
            //7、compoundstmt->{ stmts }
            Production[7].productionSubstring[0] = "compoundstmt";
            Production[7].productionSubstring[1] = "->";
            Production[7].productionSubstring[2] = "{";
            Production[7].productionSubstring[3] = "stmts";
            Production[7].productionSubstring[4] = "}";
            //8、stmts->stmt stmts
            Production[8].productionSubstring[0] = "stmts";
            Production[8].productionSubstring[1] = "->";
            Production[8].productionSubstring[2] = "stmt";
            Production[8].productionSubstring[3] = "stmts";
            //9、stmts->empty
            Production[9].productionSubstring[0] = "stmts";
            Production[9].productionSubstring[1] = "->";
            Production[9].productionSubstring[2] = "empty";
            //10、ifstmt->if ( boolexpr ) then stmt else stmt
            Production[10].productionSubstring[0] = "ifstmt";
            Production[10].productionSubstring[1] = "->";
            Production[10].productionSubstring[2] = "if";
            Production[10].productionSubstring[3] = "(";
            Production[10].productionSubstring[4] = "boolexpr";
            Production[10].productionSubstring[5] = ")";
            Production[10].productionSubstring[6] = "then";
            Production[10].productionSubstring[7] = "stmt";
            Production[10].productionSubstring[8] = "else";
            Production[10].productionSubstring[9] = "stmt";
            //11、whilestmt->while ( boolexpr ) stmt
            Production[11].productionSubstring[0] = "whilestmt";
            Production[11].productionSubstring[1] = "->";
            Production[11].productionSubstring[2] = "while";
            Production[11].productionSubstring[3] = "(";
            Production[11].productionSubstring[4] = "boolexpr";
            Production[11].productionSubstring[5] = ")";
            Production[11].productionSubstring[6] = "stmt";
            //12、assgstmt->ID = arithexpr;
            Production[12].productionSubstring[0] = "assgstmt";
            Production[12].productionSubstring[1] = "->";
            Production[12].productionSubstring[2] = "ID";
            Production[12].productionSubstring[3] = "=";
            Production[12].productionSubstring[4] = "arithexpr";
            Production[12].productionSubstring[5] = ";";
            //13、decl->type list;
            Production[13].productionSubstring[0] = "decl";
            Production[13].productionSubstring[1] = "->";
            Production[13].productionSubstring[2] = "type";
            Production[13].productionSubstring[3] = "list";
            Production[13].productionSubstring[4] = ";";
            //14、type->int
            Production[14].productionSubstring[0] = "type";
            Production[14].productionSubstring[1] = "->";
            Production[14].productionSubstring[2] = "int";
            //15、type->real
            Production[15].productionSubstring[0] = "type";
            Production[15].productionSubstring[1] = "->";
            Production[15].productionSubstring[2] = "real";
            //16、list->ID list1
            Production[16].productionSubstring[0] = "list";
            Production[16].productionSubstring[1] = "->";
            Production[16].productionSubstring[2] = "ID";
            Production[16].productionSubstring[3] = "list1";
            //17、list1->, list 
            Production[17].productionSubstring[0] = "list1";
            Production[17].productionSubstring[1] = "->";
            Production[17].productionSubstring[2] = ",";
            Production[17].productionSubstring[3] = "list";
            //18、list1->empty 
            Production[18].productionSubstring[0] = "list1";
            Production[18].productionSubstring[1] = "->";
            Production[18].productionSubstring[2] = "empty";
            //19、boolexpr->arithexpr boolop arithexpr
            Production[19].productionSubstring[0] = "boolexpr";
            Production[19].productionSubstring[1] = "->";
            Production[19].productionSubstring[2] = "arithexpr";
            Production[19].productionSubstring[3] = "boolop";
            Production[19].productionSubstring[4] = "arithexpr";
            //20、boolop-><
            Production[20].productionSubstring[0] = "boolop";
            Production[20].productionSubstring[1] = "->";
            Production[20].productionSubstring[2] = "<";
            //21、boolop->> 
            Production[21].productionSubstring[0] = "boolop";
            Production[21].productionSubstring[1] = "->";
            Production[21].productionSubstring[2] = ">";
            //22、boolop-><=  
            Production[22].productionSubstring[0] = "boolop";
            Production[22].productionSubstring[1] = "->";
            Production[22].productionSubstring[2] = "<=";
            //23、boolop->>=  
            Production[23].productionSubstring[0] = "boolop";
            Production[23].productionSubstring[1] = "->";
            Production[23].productionSubstring[2] = ">=";
            //24、boolop==
            Production[24].productionSubstring[0] = "boolop";
            Production[24].productionSubstring[1] = "->";
            Production[24].productionSubstring[2] = "==";
            //25、arithexpr->multexpr arithexprprime
            Production[25].productionSubstring[0] = "arithexpr";
            Production[25].productionSubstring[1] = "->";
            Production[25].productionSubstring[2] = "multexpr";
            Production[25].productionSubstring[3] = "arithexprprime";
            //26、arithexprprime->+ multexpr arithexprprime 
            Production[26].productionSubstring[0] = "arithexprprime";
            Production[26].productionSubstring[1] = "->";
            Production[26].productionSubstring[2] = "+";
            Production[26].productionSubstring[3] = "multexpr";
            Production[26].productionSubstring[4] = "arithexprprime";
            //27、arithexprprime->- multexpr arithexprprime 
            Production[27].productionSubstring[0] = "arithexprprime";
            Production[27].productionSubstring[1] = "->";
            Production[27].productionSubstring[2] = "-";
            Production[27].productionSubstring[3] = "multexpr";
            Production[27].productionSubstring[4] = "arithexprprime";
            //28、arithexprprime->empty
            Production[28].productionSubstring[0] = "arithexprprime";
            Production[28].productionSubstring[1] = "->";
            Production[28].productionSubstring[2] = "empty";
            //29、multexpr->simpleexpr  multexprprime
            Production[29].productionSubstring[0] = "multexpr";
            Production[29].productionSubstring[1] = "->";
            Production[29].productionSubstring[2] = "simpleexpr";
            Production[29].productionSubstring[3] = "multexprprime";
            //30、multexprprime->* simpleexpr multexprprime 
            Production[30].productionSubstring[0] = "multexprprime";
            Production[30].productionSubstring[1] = "->";
            Production[30].productionSubstring[2] = "*";
            Production[30].productionSubstring[3] = "simpleexpr";
            Production[30].productionSubstring[4] = "multexprprime";
            //31、multexprprime->/ simpleexpr multexprprime 
            Production[31].productionSubstring[0] = "multexprprime";
            Production[31].productionSubstring[1] = "->";
            Production[31].productionSubstring[2] = "/";
            Production[31].productionSubstring[3] = "simpleexpr";
            Production[31].productionSubstring[4] = "multexprprime";
            //32、multexprprime->empty
            Production[32].productionSubstring[0] = "multexprprime";
            Production[32].productionSubstring[1] = "->";
            Production[32].productionSubstring[2] = "empty";
            //33、simpleexpr->ID 
            Production[33].productionSubstring[0] = "simpleexpr";
            Production[33].productionSubstring[1] = "->";
            Production[33].productionSubstring[2] = "ID";
            //34、simpleexpr->NUM 
            Production[34].productionSubstring[0] = "simpleexpr";
            Production[34].productionSubstring[1] = "->";
            Production[34].productionSubstring[2] = "NUM";
            //35、simpleexpr->( arithexpr )
            Production[35].productionSubstring[0] = "simpleexpr";
            Production[35].productionSubstring[1] = "->";
            Production[35].productionSubstring[2] = "(";
            Production[35].productionSubstring[3] = "arithexpr";
            Production[35].productionSubstring[4] = ")";
        }
    }
}
