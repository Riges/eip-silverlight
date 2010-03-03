using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;

namespace EIPLibrary.DBUtility
{
    public static class DbUtil
    {

        public static bool HasFieldNotNull(DataRow dbDataRow, string fieldName)
        {
            bool myReturn = false;

            if(dbDataRow.Table.Columns.Contains(fieldName))    
                if(!dbDataRow.IsNull(fieldName))
                    myReturn = true;
                
            return myReturn;
        }


    }
}
