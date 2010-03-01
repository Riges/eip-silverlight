using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EIPLibrary.DBUtility;
using System.Data;
using Npgsql;

namespace EIPLibrary
{
    public class Model
    {




        public Account GetAccount(long userID)
        {
            List<NpgsqlParameter> parms = new List<NpgsqlParameter>();
            StringBuilder cmdText = new StringBuilder();
            cmdText.Append(" SELECT * FROM Account a ");
            cmdText.Append(" LEFT JOIN  AccountFacebook f ON a.AccountID=f.AccountID ");
            cmdText.Append(" LEFT JOIN  AccountTwitter t ON a.AccountID=t.AccountID ");
            cmdText.Append(" WHERE a.UserID=@USERID");

            parms.Add(new NpgsqlParameter("@USERID", userID));

            DataTable dt = PgSqlHelper.ExecuteDataTable(CommandType.Text, cmdText.ToString(), parms);
            
            using (dt)
            {
                if (dt.Rows.Count > 0)
                {
                    return Populate(dt.Rows[0]);
                }
            }
            return null;
        }


        public Account Populate(DataRow dtr)
        {
            Account account = new Account();
            account.typeAccount = (Account.TypeAccount)Enum.Parse(typeof(Account.TypeAccount), dtr["Type"].ToString());

            account.accountID = Convert.ToInt32(dtr["AccountID"]);
            account.name = dtr["Name"].ToString();
            account.userID = Convert.ToInt32(dtr["UserID"]);

            switch (account.typeAccount)
            {
                case Account.TypeAccount.Facebook:
                    ((AccountFacebook)account).sessionExpires = Convert.ToBoolean(dtr["SessionExpires"]);
                    ((AccountFacebook)account).sessionKey = dtr["SessionKey"].ToString();
                    ((AccountFacebook)account).sessionSecret = dtr["SessionSecret"].ToString();
                    break;
                case Account.TypeAccount.Twitter:
                    ((AccountTwitter)account).token = dtr["Token"].ToString();
                    ((AccountTwitter)account).tokenSecret = dtr["TokenSecret"].ToString();
                    break;
                case Account.TypeAccount.Myspace:
                    break;
                default:
                    break;
            }


            return account;
        }




    }
}
