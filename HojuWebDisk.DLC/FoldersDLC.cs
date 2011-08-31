//  ============================================================================
//  AUTHOR		 : Simon
//  CREATE DATE	 : 11 Apr 2007
//  PURPOSE		 : Data Layer for the Folders Table . All access through standard SPs with typed Datasets.
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
    public static class FoldersDLC
    {
        public const string SP_Get = "up_Folders_get";
        public const string SP_GetP = "up_Folders_get_ByPath";
        public const string SP_Del = "up_Folders_del";
        public const string SP_List = "up_Folders_list";
        public const string SP_Add = "up_Folders_add";
        public const string SP_Update = "up_Folders_upd";
        
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
        private static SqlCommand GenerateSqlCommandfromDataRow(FoldersDS.FoldersRow Data, string StoredProcName)
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

        public static FoldersDS.FoldersRow Get(int KeyValue, string SQLConnectionName)
        {
            FoldersDS ds = new FoldersDS();

            try
            {
                SqlConnection SQLConn = new SqlConnection(getConnstring(SQLConnectionName));

                SqlDataAdapter iDataAdapter = new SqlDataAdapter(SP_Get, SQLConn);
                iDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                                
                iDataAdapter.SelectCommand.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                iDataAdapter.SelectCommand.Parameters["@ID"].Value = KeyValue;

                SQLConn.Open();
                //Fill the DataSet with the rows that are returned.
                iDataAdapter.Fill(ds, ds.Folders.TableName);
                iDataAdapter.Dispose();

                SQLConn.Close();

            }
            catch
            {
                return null;
            }

            if (ds.Folders.Rows.Count == 1)
            {

                return (FoldersDS.FoldersRow)ds.Folders.Rows[0];
            }
            else
            {
                return null;
            }

        }
        public static FoldersDS.FoldersRow Get(int ParentID, string FolderName, string SQLConnectionName)
        {
            FoldersDS ds = new FoldersDS();

            try
            {
                SqlConnection SQLConn = new SqlConnection(getConnstring(SQLConnectionName));

                SqlDataAdapter iDataAdapter = new SqlDataAdapter(SP_GetP, SQLConn);
                iDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                
                iDataAdapter.SelectCommand.Parameters.Add(new SqlParameter("@ParentID", SqlDbType.Int));
                iDataAdapter.SelectCommand.Parameters["@ParentID"].Value = ParentID;
                iDataAdapter.SelectCommand.Parameters.Add(new SqlParameter("@FolderName", SqlDbType.VarChar, 255));
                iDataAdapter.SelectCommand.Parameters["@FolderName"].Value = FolderName;


                SQLConn.Open();
                //Fill the DataSet with the rows that are returned.
                iDataAdapter.Fill(ds, ds.Folders.TableName);
                iDataAdapter.Dispose();

                SQLConn.Close();

            }
            catch
            {
                return null;
            }

            if (ds.Folders.Rows.Count == 1)
            {

                return (FoldersDS.FoldersRow)ds.Folders.Rows[0];
            }
            else
            {
                return null;
            }

        }
        public static int Add(FoldersDS.FoldersRow dr, string SQLConnectionName)
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
        public static int Update(FoldersDS.FoldersRow dr, string SQLConnectionName)
        {
            try
            {
                SqlConnection SQLConn = new SqlConnection(getConnstring(SQLConnectionName));

                SqlCommand iSqlCommand = GenerateSqlCommandfromDataRow(dr, SP_Update);

                iSqlCommand.Connection = SQLConn;
                //Set the ID column as output as the GenerateSQLCommand would not have done this.

                SQLConn.Open();
                iSqlCommand.ExecuteNonQuery();
                iSqlCommand.Dispose();
                SQLConn.Close();
                return 1;

            }
            catch
            {
                return -1;
            }
        }
        public static bool Del(int KeyValue, string SQLConnectionName)
        {
            
            try
            {
                SqlConnection SQLConn = new SqlConnection(getConnstring(SQLConnectionName));

                SqlCommand iSqlCommand = new SqlCommand(SP_Del, SQLConn);
                iSqlCommand.CommandType = CommandType.StoredProcedure;

                //Set the Column to the value of "ApplicationName" by retreiving from the web.config roleprovider settings
                iSqlCommand.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                iSqlCommand.Parameters["@ID"].Value = KeyValue;
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

        public static bool Del(FoldersDS.FoldersRow dr, string SQLConnectionName)
        {
           return Del(dr.ID,SQLConnectionName);
        }

        public static FoldersDS List(int ParentID, string SQLConnectionName)
        {
            FoldersDS ds = new FoldersDS();

            try
            {
                SqlConnection SQLConn = new SqlConnection(getConnstring(SQLConnectionName));

                SqlDataAdapter iDataAdapter = new SqlDataAdapter(SP_List, SQLConn);
                iDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                
                iDataAdapter.SelectCommand.Parameters.Add(new SqlParameter("@ParentID", SqlDbType.Int));
                iDataAdapter.SelectCommand.Parameters["@ParentID"].Value = ParentID;
                iDataAdapter.SelectCommand.Parameters.Add(new SqlParameter("@sort_col", SqlDbType.VarChar, 120));
                iDataAdapter.SelectCommand.Parameters["@sort_col"].Value = "FolderName";

                SQLConn.Open();
                //Fill the DataSet with the rows that are returned.
                iDataAdapter.Fill(ds, ds.Folders.TableName);
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
