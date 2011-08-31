//  ============================================================================
//  AUTHOR		 : Simon
//  CREATE DATE	 : 11 Apr 2007
//  PURPOSE		 : Data Layer for the Locks Tokens Table . All access through standard SPs with typed Datasets.
//  SPECIAL NOTES: 

//  
//  ===========================================================================

using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;
using HojuWebDisk.DataEntities;

namespace HojuWebDisk.DLC
{
    public static class Locks_TokensDLC
    {
        public const string SP_Get = "up_Locks_Tokens_get";
        public const string SP_Del = "up_Locks_Tokens_del";
        public const string SP_List = "up_Locks_Tokens_list";
        public const string SP_Add = "up_Locks_Tokens_add";
        
        public const string DataKeyName = "ID";

        #region Private Methods
        private static string getConnstring(string ConnName)
        {

            try
            {
                System.Configuration.ConnectionStringSettingsCollection Constrings = System.Configuration.ConfigurationManager.ConnectionStrings;
                System.Configuration.ConnectionStringSettings Constring = Constrings[ConnName];
                return Constring.ConnectionString;

            }
            catch
            {
                return "";
            }

        }
        private static SqlCommand GenerateSqlCommandfromDataRow(Locks_TokensDS.Locks_TokensRow Data, string StoredProcName)
        {

            SqlCommand retCom = new SqlCommand(StoredProcName);
            retCom.CommandType = CommandType.StoredProcedure;

            for (int Eni = 0; Eni < Data.ItemArray.Length; Eni++)
            {
                string stype = Data[Eni].GetType().ToString();

                switch (stype)
                {
                    case "System.Int32":
                        retCom.Parameters.Add("@" + Data.Table.Columns[Eni].ColumnName, SqlDbType.Int);
                        retCom.Parameters["@" + Data.Table.Columns[Eni].ColumnName].Value = Data[Eni];
                        break;
                    case "System.Int64":
                        retCom.Parameters.Add("@" + Data.Table.Columns[Eni].ColumnName, SqlDbType.BigInt);
                        retCom.Parameters["@" + Data.Table.Columns[Eni].ColumnName].Value = Data[Eni];
                        break;
                    case "System.String":
                        retCom.Parameters.Add("@" + Data.Table.Columns[Eni].ColumnName, SqlDbType.VarChar);
                        retCom.Parameters["@" + Data.Table.Columns[Eni].ColumnName].Value = Data[Eni];
                        break;
                    case "System.Byte[]":
                        retCom.Parameters.Add("@" + Data.Table.Columns[Eni].ColumnName, SqlDbType.Image);
                        retCom.Parameters["@" + Data.Table.Columns[Eni].ColumnName].Value = Data[Eni];

                        break;
                    case "System.DateTime":
                        retCom.Parameters.Add("@" + Data.Table.Columns[Eni].ColumnName, SqlDbType.DateTime);
                        retCom.Parameters["@" + Data.Table.Columns[Eni].ColumnName].Value = Data[Eni];
                        break;

                }
            }
            return retCom;
        }
        #endregion

        #region Public Methods

        public static Locks_TokensDS.Locks_TokensRow Get(int KeyValue, string SQLConnectionName)
        {
            Locks_TokensDS ds = new Locks_TokensDS();

            try
            {
                SqlConnection SQLConn = new SqlConnection(getConnstring(SQLConnectionName));

                SqlDataAdapter iDataAdapter = new SqlDataAdapter(SP_Get, SQLConn);
                iDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                
                iDataAdapter.SelectCommand.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                iDataAdapter.SelectCommand.Parameters["@ID"].Value = KeyValue;

                SQLConn.Open();
                //Fill the DataSet with the rows that are returned.
                iDataAdapter.Fill(ds, ds.Locks_Tokens.TableName);
                iDataAdapter.Dispose();

                SQLConn.Close();

            }
            catch
            {
                return null;
            }

            if (ds.Locks_Tokens.Rows.Count == 1)
            {

                return (Locks_TokensDS.Locks_TokensRow)ds.Locks_Tokens.Rows[0];
            }
            else
            {
                return null;
            }

        }
        public static int Add(Locks_TokensDS.Locks_TokensRow dr, string SQLConnectionName)
        {
            try
            {
                SqlConnection SQLConn = new SqlConnection(getConnstring(SQLConnectionName));

                SqlCommand iSqlCommand = GenerateSqlCommandfromDataRow(dr, SP_Add);

                iSqlCommand.Connection = SQLConn;
                //Set the ID column as output as the GenerateSQLCommand would not have done this.
                iSqlCommand.Parameters["@ID"].Direction = ParameterDirection.InputOutput;
                iSqlCommand.UpdatedRowSource = UpdateRowSource.OutputParameters;

                SQLConn.Open();
                iSqlCommand.ExecuteNonQuery();
                int newID = (int)iSqlCommand.Parameters["@ID"].Value;
                iSqlCommand.Dispose();
                SQLConn.Close();
                return newID;

            }
            catch
            {
                return -1;
            }
        }
        public static bool Del(int KeyValue, string SQLConnectionName)
        {
            Locks_TokensDS ds = new Locks_TokensDS();
            try
            {
                SqlConnection SQLConn = new SqlConnection(getConnstring(SQLConnectionName));

                SqlCommand iSqlCommand = new SqlCommand(SP_Del, SQLConn);
                iSqlCommand.CommandType = CommandType.StoredProcedure;

                //Set the Column to the value of "ApplicationName" by retreiving from the web.config roleprovider settings
                iSqlCommand.Parameters.Add(new SqlParameter("@LockID", SqlDbType.Int));
                iSqlCommand.Parameters["@LockID"].Value = KeyValue;


                SQLConn.Open();
                iSqlCommand.ExecuteNonQuery();
                iSqlCommand.Dispose();
                SQLConn.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static Locks_TokensDS List(int LockID, string SQLConnectionName)
        {
            Locks_TokensDS ds = new Locks_TokensDS();

            try
            {
                SqlConnection SQLConn = new SqlConnection(getConnstring(SQLConnectionName));

                SqlDataAdapter iDataAdapter = new SqlDataAdapter(SP_List, SQLConn);
                iDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                
                iDataAdapter.SelectCommand.Parameters.Add(new SqlParameter("@LockID", SqlDbType.Int));
                iDataAdapter.SelectCommand.Parameters["@LockID"].Value = LockID;

                SQLConn.Open();
                //Fill the DataSet with the rows that are returned.
                iDataAdapter.Fill(ds, ds.Locks_Tokens.TableName);
                iDataAdapter.Dispose();

                SQLConn.Close();

            }
            catch
            {
                return null;
            }

           return ds;
        }
        public static Locks_TokensDS List(string Token, string SQLConnectionName)
        {
            Locks_TokensDS ds = new Locks_TokensDS();

            try
            {
                SqlConnection SQLConn = new SqlConnection(getConnstring(SQLConnectionName));

                SqlDataAdapter iDataAdapter = new SqlDataAdapter(SP_List, SQLConn);
                iDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                
                iDataAdapter.SelectCommand.Parameters.Add(new SqlParameter("@Token", SqlDbType.VarChar, 255));
                iDataAdapter.SelectCommand.Parameters["@Token"].Value = Token;

                SQLConn.Open();
                //Fill the DataSet with the rows that are returned.
                iDataAdapter.Fill(ds, ds.Locks_Tokens.TableName);
                iDataAdapter.Dispose();

                SQLConn.Close();

            }
            catch
            {
                return null;
            }
            return ds;

        }

        #endregion


    }
}
