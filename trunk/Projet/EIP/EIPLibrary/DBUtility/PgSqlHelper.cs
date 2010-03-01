using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Npgsql;


namespace EIPLibrary.DBUtility
{
    public class PgSqlHelper
    {

        public static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;

        private static void PrepareCommand(NpgsqlConnection conn, NpgsqlCommand cmd, CommandType cmdType, string cmdText, List<NpgsqlParameter> commandParameters)
        {
            //Open the connection if required
            if (conn.State != ConnectionState.Open)
                conn.Open();

            //Set up the command
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;

            // Bind the parameters passed in
            if (commandParameters != null)
            {
                foreach (NpgsqlParameter param in commandParameters)
                    cmd.Parameters.Add(param);
            }
            //Debug.Write(cmd);
        }

        public static NpgsqlDataReader ExecuteReader(CommandType cmdType, string cmdText, List<NpgsqlParameter> commandParameters)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(Context.Instance.ConnectStr))
            {
                //Create the command and connection
                
                NpgsqlCommand cmd = new NpgsqlCommand();
                //Prepare the command to execute
                PrepareCommand(conn, cmd, cmdType, cmdText, commandParameters);

                //Execute the query, stating that the connection should close when the resulting datareader has been read
                NpgsqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
        }

        public static DataTable ExecuteDataTable(CommandType cmdType, string cmdText, List<NpgsqlParameter> commandParameters)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(Context.Instance.ConnectStr))
            {
                NpgsqlCommand cmd = new NpgsqlCommand();
                PrepareCommand(conn, cmd, cmdType, cmdText, commandParameters);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd); 
                DataTable dt = new DataTable();
                da.Fill(dt);
                cmd.Dispose();
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
                return dt;
            }
        }

        public static int ExecuteNonQuery(CommandType cmdType, string cmdText, List<NpgsqlParameter> commandParameters)
        {

            using (NpgsqlConnection conn = new NpgsqlConnection(Context.Instance.ConnectStr))
            {
                NpgsqlCommand cmd = new NpgsqlCommand();
                PrepareCommand(conn, cmd, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
                return val;
            }

        }

        public static int ExecuteScalar(CommandType cmdType, string cmdText, List<NpgsqlParameter> commandParameters, string table)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(Context.Instance.ConnectStr))
            {
                NpgsqlCommand cmd = new NpgsqlCommand();
                PrepareCommand(conn, cmd, cmdType, cmdText + ";SELECT IDENT_CURRENT('" + table + "')", commandParameters);
                object ob = cmd.ExecuteScalar();
                int lastId = Convert.ToInt32(ob);
                cmd.Parameters.Clear();
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
                return lastId;
            }
        }


    }
}
