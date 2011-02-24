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
            cmdText.Append(" LEFT JOIN  accountflickr r ON a.accountid=r.accountid ");
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

        public static Account GetAccountFlickr(string userID)
        {
            List<NpgsqlParameter> parms = new List<NpgsqlParameter>();
            StringBuilder cmdText = new StringBuilder();
            cmdText.Append(" SELECT * FROM account a, accountflickr r ");
            cmdText.Append(" WHERE a.accountid=r.accountid ");
            cmdText.Append(" AND r.useridstr=@USERID");

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
            cmdText.Append(" LEFT JOIN  accountflickr r ON a.accountid=r.accountid ");
            cmdText.Append(" WHERE a.groupid = (SELECT groupid FROM account WHERE userid=@USERID)");
            cmdText.Append(" ORDER BY a.name ");

            parms.Add(new NpgsqlParameter("@USERID", userID));

            DataTable dt = PgSqlHelper.ExecuteDataTable(CommandType.Text, cmdText.ToString(), parms);

            List<Account> listAccount = new List<Account>();
            using (dt)
            {
                foreach (DataRow dtr in dt.Rows)
                {
                    Account acc = Populate(dtr);
                    listAccount.Add(acc);
                    SaveAccount(acc, true);
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
            cmdText.Append(" LEFT JOIN  accountflickr r ON a.accountid=r.accountid ");
            cmdText.Append(" WHERE a.groupid = @GROUPID");
            cmdText.Append(" ORDER BY a.name ");

            parms.Add(new NpgsqlParameter("@GROUPID", groupID));

            DataTable dt = PgSqlHelper.ExecuteDataTable(CommandType.Text, cmdText.ToString(), parms);

            List<Account> listAccount = new List<Account>();
            using (dt)
            {
                foreach (DataRow dtr in dt.Rows)
                {
                    Account acc = Populate(dtr);
                    listAccount.Add(acc);
                    SaveAccount(acc, true);
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
                cmdTextInsert.Append(" (userid, name, type, groupid, added, lastconnexion) ");
                cmdTextInsert.Append(" VALUES ");
                cmdTextInsert.Append(" (@USERID, @NAME, @TYPE, @GROUPID, @ADDED, @LASTCONNEXION) ");

                parmsInsert.Add(new NpgsqlParameter("@USERID", newAccount.userID));
                //  parmsInsert.Add(new NpgsqlParameter("@ACCOUNTID", newAccount.accountID));
                parmsInsert.Add(new NpgsqlParameter("@NAME", newAccount.name));
                parmsInsert.Add(new NpgsqlParameter("@TYPE", newAccount.typeAccount.ToString()));
                parmsInsert.Add(new NpgsqlParameter("@GROUPID", newAccount.groupID));
                parmsInsert.Add(new NpgsqlParameter("@ADDED", DateTime.Now));
                parmsInsert.Add(new NpgsqlParameter("@LASTCONNEXION", DateTime.Now));

                int accountID = PgSqlHelper.ExecuteScalar(CommandType.Text, cmdTextInsert.ToString(), parmsInsert, "account_accountid_seq");

                if (accountID > 0)
                {
                    parms = new List<NpgsqlParameter>();
                    cmdText = new StringBuilder();

                    switch (newAccount.typeAccount)
                    {
                        case Account.TypeAccount.Facebook:
                            cmdText.Append(" INSERT INTO accountfacebook ");
                            cmdText.Append(" (sessionkey, sessionsecret, sessionexpires, accesstoken, accountid) ");
                            cmdText.Append(" VALUES ");
                            cmdText.Append(" (@SESSIONKEY, @SESSIONSECRET, @SESSIONEXPIRES, @ACCESSTOKEN, @ACCOUNTID) ");

                            parms.Add(new NpgsqlParameter("@SESSIONKEY", ((AccountFacebook)newAccount).sessionKey));
                            parms.Add(new NpgsqlParameter("@SESSIONSECRET", ((AccountFacebook)newAccount).sessionSecret));
                            parms.Add(new NpgsqlParameter("@SESSIONEXPIRES", ((AccountFacebook)newAccount).sessionExpires));
                            parms.Add(new NpgsqlParameter("@ACCESSTOKEN", ((AccountFacebook)newAccount).accessToken));

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
                        case Account.TypeAccount.Flickr:
                            cmdText.Append(" INSERT INTO accountflickr ");
                            cmdText.Append(" (accountid, tokenflickr, useridstr) ");
                            cmdText.Append(" VALUES ");
                            cmdText.Append(" (@ACCOUNTID, @TOKEN, @USERIDSTR) ");

                            parms.Add(new NpgsqlParameter("@TOKEN", ((AccountFlickr)newAccount).token));
                            parms.Add(new NpgsqlParameter("@USERIDSTR", ((AccountFlickr)newAccount).userIDstr));
                            parms.Add(new NpgsqlParameter("@ACCOUNTID", accountID));

                            break;
                        default:
                            break;
                    }

                    int result = 0;
                    if (cmdText.ToString() != "")
                        result = PgSqlHelper.ExecuteNonQuery(CommandType.Text, cmdText.ToString(), parms);

                    if (result > 0)
                    {
                        return true;
                    }
                }

            }
            return false;
        }

        public static bool SaveAccount(Account newAccount, bool justConnexion)
        {

            List<NpgsqlParameter> parms = new List<NpgsqlParameter>();
            StringBuilder cmdText = new StringBuilder();
            cmdText.Append(" UPDATE account SET ");
            cmdText.Append(" name=@NAME, lastconnexion=@LASTCONNEXION");
            cmdText.Append(" WHERE ");
            cmdText.Append(" accountid=@ACCOUNTID ");

            parms.Add(new NpgsqlParameter("@NAME", newAccount.name));
            parms.Add(new NpgsqlParameter("@ACCOUNTID", newAccount.accountID));
            parms.Add(new NpgsqlParameter("@LASTCONNEXION", DateTime.Now));

            int result = PgSqlHelper.ExecuteNonQuery(CommandType.Text, cmdText.ToString(), parms);

            if (!justConnexion)
            {
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
                    case Account.TypeAccount.Flickr:
                        cmdText.Append(" UPDATE accountflickr SET ");
                        cmdText.Append(" tokenflickr=@TOKEN, useridstr=@USERIDSTR ");
                        cmdText.Append(" WHERE accountid=@ACCOUNTID ");

                        parms.Add(new NpgsqlParameter("@TOKEN", ((AccountFlickr)newAccount).token));
                        parms.Add(new NpgsqlParameter("@USERIDSTR", ((AccountFlickr)newAccount).userIDstr));
                        parms.Add(new NpgsqlParameter("@ACCOUNTID", newAccount.accountID));
                        break;

                    default:
                        break;
                }
                result = PgSqlHelper.ExecuteNonQuery(CommandType.Text, cmdText.ToString(), parms);
            }
            if (result > 0)
            {
                return true;
            }
            return false;
        }

        public static long DeleteAccount(long accountID)
        {
            List<NpgsqlParameter> parms = new List<NpgsqlParameter>();
            StringBuilder cmdText = new StringBuilder();
            cmdText.Append(" DELETE FROM accountfacebook WHERE accountid=@ACCOUNTID;");
            cmdText.Append(" DELETE FROM accounttwitter WHERE accountid=@ACCOUNTID;");
            cmdText.Append(" DELETE FROM accountflickr WHERE accountid=@ACCOUNTID;");
            cmdText.Append(" DELETE FROM account WHERE accountid=@ACCOUNTID;");

            parms.Add(new NpgsqlParameter("@ACCOUNTID", accountID));

            int result = PgSqlHelper.ExecuteNonQuery(CommandType.Text, cmdText.ToString(), parms);

            if (result > 0)
            {
                return accountID;
            }
            return 0;
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
                    case Account.TypeAccount.Flickr:
                        account = new AccountFlickr();
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

                if (DbUtil.HasFieldNotNull(dtr, "added"))
                    account.added = Convert.ToDateTime(dtr["added"].ToString());

                if (DbUtil.HasFieldNotNull(dtr, "lastconnexion"))
                    account.lastConnexion = Convert.ToDateTime(dtr["lastconnexion"].ToString());

                switch (account.typeAccount)
                {
                    case Account.TypeAccount.Facebook:
                        if (DbUtil.HasFieldNotNull(dtr, "sessionexpires"))
                            ((AccountFacebook)account).sessionExpires = Convert.ToBoolean(dtr["sessionexpires"]);
                        if (DbUtil.HasFieldNotNull(dtr, "sessionkey"))
                            ((AccountFacebook)account).sessionKey = dtr["sessionkey"].ToString();
                        if (DbUtil.HasFieldNotNull(dtr, "sessionsecret"))
                            ((AccountFacebook)account).sessionSecret = dtr["sessionsecret"].ToString();
                        if (DbUtil.HasFieldNotNull(dtr, "accesstoken"))
                            ((AccountFacebook)account).accessToken = dtr["accesstoken"].ToString();
                        break;
                    case Account.TypeAccount.Twitter:
                        if (DbUtil.HasFieldNotNull(dtr, "token"))
                            ((AccountTwitter)account).token = dtr["token"].ToString();
                        if (DbUtil.HasFieldNotNull(dtr, "tokensecret"))
                            ((AccountTwitter)account).tokenSecret = dtr["tokensecret"].ToString();
                        break;
                    case Account.TypeAccount.Flickr:
                        if (DbUtil.HasFieldNotNull(dtr, "tokenflickr"))
                            ((AccountFlickr)account).token = dtr["tokenflickr"].ToString();
                        if (DbUtil.HasFieldNotNull(dtr, "useridstr"))
                            ((AccountFlickr)account).userIDstr = dtr["useridstr"].ToString();
                        break;
                  
                    default:
                        break;
                }
            }
            return account;
        }



        public static bool InsertError(long groupID, string stackTrace, string message)
        {
            List<NpgsqlParameter> parmsInsert = new List<NpgsqlParameter>();
            StringBuilder cmdTextInsert = new StringBuilder();
            cmdTextInsert.Append(" INSERT INTO errors ");
            cmdTextInsert.Append(" (message, stacktrace, dateerreur, groupid) ");
            cmdTextInsert.Append(" VALUES ");
            cmdTextInsert.Append(" (@MESSAGE, @STACKTRACE, @DATERREUR, @GROUPID) ");

            //  parmsInsert.Add(new NpgsqlParameter("@ACCOUNTID", newAccount.accountID));
            parmsInsert.Add(new NpgsqlParameter("@MESSAGE", message));
            parmsInsert.Add(new NpgsqlParameter("@STACKTRACE", stackTrace));
            parmsInsert.Add(new NpgsqlParameter("@DATERREUR", DateTime.Now));
            parmsInsert.Add(new NpgsqlParameter("@GROUPID", groupID));

            int result = PgSqlHelper.ExecuteNonQuery(CommandType.Text, cmdTextInsert.ToString(), parmsInsert);

            if (result > 0)
            {
                return true;
            }

            return false;
        }

    }
}
