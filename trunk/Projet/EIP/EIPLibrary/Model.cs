using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EIPLibrary.DBUtility;
using System.Data;
using Npgsql;

namespace EIPLibrary
{
    public static class Model
    {
        
        public static bool IsDBUp()
        {
            NpgsqlConnection conn = new NpgsqlConnection(Context.Instance.ConnectStr);
            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }

        }

        public static Account GetAccount(long userID)
        {
            List<NpgsqlParameter> parms = new List<NpgsqlParameter>();
            StringBuilder cmdText = new StringBuilder();
            cmdText.Append(" SELECT * FROM account a ");
            cmdText.Append(" LEFT JOIN  accountfacebook f ON a.accountid=f.accountid ");
            cmdText.Append(" LEFT JOIN  accounttwitter t ON a.accountid=t.accountid ");
            cmdText.Append(" WHERE a.userid=@USERID");

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

        public static List<Account> GetAccountsByUserID(long userID)
        {
            List<NpgsqlParameter> parms = new List<NpgsqlParameter>();
            StringBuilder cmdText = new StringBuilder();
            cmdText.Append(" SELECT * FROM account a ");
            cmdText.Append(" LEFT JOIN  accountfacebook f ON a.accountid=f.accountid ");
            cmdText.Append(" LEFT JOIN  accounttwitter t ON a.accountid=t.accountid ");
            cmdText.Append(" WHERE a.groupid = (SELECT groupid FROM account WHERE userid=@USERID)");
            cmdText.Append(" ORDER BY a.name ");

            parms.Add(new NpgsqlParameter("@USERID", userID));

            DataTable dt = PgSqlHelper.ExecuteDataTable(CommandType.Text, cmdText.ToString(), parms);

            List<Account> listAccount = new List<Account>();
            using (dt)
            {
                foreach (DataRow dtr in dt.Rows)
                {
                    listAccount.Add(Populate(dtr));
                }
            }
            return listAccount;
        }

        public static List<Account> GetAccountsByGroupID(long groupID)
        {
            List<NpgsqlParameter> parms = new List<NpgsqlParameter>();
            StringBuilder cmdText = new StringBuilder();
            cmdText.Append(" SELECT * FROM account a ");
            cmdText.Append(" LEFT JOIN  accountfacebook f ON a.accountid=f.accountid ");
            cmdText.Append(" LEFT JOIN  accounttwitter t ON a.accountid=t.accountid ");
            cmdText.Append(" WHERE a.groupid = @GROUPID");
            cmdText.Append(" ORDER BY a.name ");

            parms.Add(new NpgsqlParameter("@GROUPID", groupID));

            DataTable dt = PgSqlHelper.ExecuteDataTable(CommandType.Text, cmdText.ToString(), parms);

            List<Account> listAccount = new List<Account>();
            using (dt)
            {
                foreach (DataRow dtr in dt.Rows)
                {
                    listAccount.Add(Populate(dtr));
                }
            }
            return listAccount;
        }


        public static bool AddAccount(Account newAccount)
        {
            if (newAccount != null)
            {
                List<NpgsqlParameter> parms = new List<NpgsqlParameter>();
                StringBuilder cmdText = new StringBuilder();
                cmdText.Append(" SELECT * FROM account a ");
                cmdText.Append(" WHERE a.userid=@USERID ");
                cmdText.Append(" AND a.type=@TYPE ");

                parms.Add(new NpgsqlParameter("@USERID", newAccount.userID));
                parms.Add(new NpgsqlParameter("@TYPE", newAccount.typeAccount.ToString()));

                DataTable dt = PgSqlHelper.ExecuteDataTable(CommandType.Text, cmdText.ToString(), parms);

                using (dt)
                {
                    if (dt.Rows.Count > 0)
                    {
                        return false;
                    }
                }

                List<NpgsqlParameter> parmsInsert = new List<NpgsqlParameter>();
                StringBuilder cmdTextInsert = new StringBuilder();
                cmdTextInsert.Append(" INSERT INTO account ");
                cmdTextInsert.Append(" (userid, name, type, groupid) ");
                cmdTextInsert.Append(" VALUES ");
                cmdTextInsert.Append(" (@USERID, @NAME, @TYPE, @GROUPID) ");

                parmsInsert.Add(new NpgsqlParameter("@USERID", newAccount.userID));
                //  parmsInsert.Add(new NpgsqlParameter("@ACCOUNTID", newAccount.accountID));
                parmsInsert.Add(new NpgsqlParameter("@NAME", newAccount.name));
                parmsInsert.Add(new NpgsqlParameter("@TYPE", newAccount.typeAccount.ToString()));
                parmsInsert.Add(new NpgsqlParameter("@GROUPID", newAccount.groupID));

                int accountID = PgSqlHelper.ExecuteScalar(CommandType.Text, cmdTextInsert.ToString(), parmsInsert, "account_accountid_seq");

                if (accountID > 0)
                {
                    parms = new List<NpgsqlParameter>();
                    cmdText = new StringBuilder();

                    switch (newAccount.typeAccount)
                    {
                        case Account.TypeAccount.Facebook:
                            cmdText.Append(" INSERT INTO accountfacebook ");
                            cmdText.Append(" (sessionkey, sessionsecret, sessionexpires, accountid) ");
                            cmdText.Append(" VALUES ");
                            cmdText.Append(" (@SESSIONKEY, @SESSIONSECRET, @SESSIONEXPIRES, @ACCOUNTID) ");

                            parms.Add(new NpgsqlParameter("@SESSIONKEY", ((AccountFacebook)newAccount).sessionKey));
                            parms.Add(new NpgsqlParameter("@SESSIONSECRET", ((AccountFacebook)newAccount).sessionSecret));
                            parms.Add(new NpgsqlParameter("@SESSIONEXPIRES", ((AccountFacebook)newAccount).sessionExpires));

                            parms.Add(new NpgsqlParameter("@ACCOUNTID", accountID));
                            break;
                        case Account.TypeAccount.Twitter:
                            cmdText.Append(" INSERT INTO accounttwitter ");
                            cmdText.Append(" (accountid, token, tokensecret) ");
                            cmdText.Append(" VALUES ");
                            cmdText.Append(" (@ACCOUNTID, @TOKEN, @TOKENSECRET) ");

                            parms.Add(new NpgsqlParameter("@TOKEN", ((AccountTwitter)newAccount).token));
                            parms.Add(new NpgsqlParameter("@TOKENSECRET", ((AccountTwitter)newAccount).tokenSecret));
                            parms.Add(new NpgsqlParameter("@ACCOUNTID", accountID));
                            break;
                        case Account.TypeAccount.Myspace:
                            break;
                        default:
                            break;
                    }

                    int result = PgSqlHelper.ExecuteNonQuery(CommandType.Text, cmdText.ToString(), parms);

                    if (result > 0)
                    {
                        return true;
                    }
                }

            }

            return false;
        }

        public static bool SaveAccount(Account newAccount)
        {

            List<NpgsqlParameter> parms = new List<NpgsqlParameter>();
            StringBuilder cmdText = new StringBuilder();
            cmdText.Append(" UPDATE account SET ");
            cmdText.Append(" name=@NAME ");
            cmdText.Append(" WHERE ");
            cmdText.Append(" accountid=@ACCOUNTID ");

            parms.Add(new NpgsqlParameter("@NAME", newAccount.name));

            int result = PgSqlHelper.ExecuteNonQuery(CommandType.Text, cmdText.ToString(), parms);

            
            parms = new List<NpgsqlParameter>();
            cmdText = new StringBuilder();
            switch (newAccount.typeAccount)
            {
                case Account.TypeAccount.Facebook:
                    cmdText.Append(" UPDATE accountfacebook SET ");
                    cmdText.Append(" sessionkey=@SESSIONKEY, sessionsecret=@SESSIONSECRET, sessionexpires=@SESSIONEXPIRES ");
                    cmdText.Append(" WHERE accountid=@ACCOUNTID ");

                    parms.Add(new NpgsqlParameter("@SESSIONKEY", ((AccountFacebook)newAccount).sessionKey));
                    parms.Add(new NpgsqlParameter("@SESSIONSECRET", ((AccountFacebook)newAccount).sessionSecret));
                    parms.Add(new NpgsqlParameter("@SESSIONEXPIRES", ((AccountFacebook)newAccount).sessionExpires));
                    parms.Add(new NpgsqlParameter("@ACCOUNTID", newAccount.accountID));
                    break;
                case Account.TypeAccount.Twitter:
                    cmdText.Append(" UPDATE accounttwitter SET ");
                    cmdText.Append(" token=@TOKEN, tokensecret=@TOKENSECRET ");
                        cmdText.Append(" WHERE accountid=@ACCOUNTID ");

                        parms.Add(new NpgsqlParameter("@TOKEN", ((AccountTwitter)newAccount).token));
                        parms.Add(new NpgsqlParameter("@TOKENSECRET", ((AccountTwitter)newAccount).tokenSecret));
                        parms.Add(new NpgsqlParameter("@ACCOUNTID", newAccount.accountID));
                    break;
                case Account.TypeAccount.Myspace:
                    break;
                default:
                    break;

            }
            result = PgSqlHelper.ExecuteNonQuery(CommandType.Text, cmdText.ToString(), parms);

            if (result > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Méthode permettant de peupler un object Account à partir d'une DataRow
        /// </summary>
        /// <param name="dtr">DataRow</param>
        /// <returns></returns>
        public static Account Populate(DataRow dtr)
        {
            Account account = new Account();
            if (DbUtil.HasFieldNotNull(dtr, "Type"))
            {
                switch ((Account.TypeAccount)Enum.Parse(typeof(Account.TypeAccount), dtr["Type"].ToString()))
                {
                    case Account.TypeAccount.Facebook:
                        account = new AccountFacebook();
                        break;
                    case Account.TypeAccount.Twitter:
                        account = new AccountTwitter();
                        break;
                    case Account.TypeAccount.Myspace:
                        break;
                    default:
                        break;
                }



                if (DbUtil.HasFieldNotNull(dtr, "Type"))
                    account.typeAccount = (Account.TypeAccount)Enum.Parse(typeof(Account.TypeAccount), dtr["Type"].ToString());

                if (DbUtil.HasFieldNotNull(dtr, "accountid"))
                    account.accountID = Convert.ToInt64(dtr["accountid"]);

                if (DbUtil.HasFieldNotNull(dtr, "groupid"))
                    account.groupID = Convert.ToInt64(dtr["groupid"]);

                if (DbUtil.HasFieldNotNull(dtr, "name"))
                    account.name = dtr["name"].ToString();

                if (DbUtil.HasFieldNotNull(dtr, "userid"))
                    account.userID = Convert.ToInt64(dtr["userid"]);

                switch (account.typeAccount)
                {
                    case Account.TypeAccount.Facebook:
                        if (DbUtil.HasFieldNotNull(dtr, "sessionexpires"))
                            ((AccountFacebook)account).sessionExpires = Convert.ToBoolean(dtr["sessionexpires"]);
                        if (DbUtil.HasFieldNotNull(dtr, "sessionkey"))
                            ((AccountFacebook)account).sessionKey = dtr["sessionkey"].ToString();
                        if (DbUtil.HasFieldNotNull(dtr, "sessionsecret"))
                            ((AccountFacebook)account).sessionSecret = dtr["sessionsecret"].ToString();
                        break;
                    case Account.TypeAccount.Twitter:
                        if (DbUtil.HasFieldNotNull(dtr, "token"))
                            ((AccountTwitter)account).token = dtr["token"].ToString();
                        if (DbUtil.HasFieldNotNull(dtr, "tokensecret"))
                            ((AccountTwitter)account).tokenSecret = dtr["tokensecret"].ToString();
                        break;
                    case Account.TypeAccount.Myspace:
                        break;
                    default:
                        break;
                }

            }
            return account;
        }




    }
}
