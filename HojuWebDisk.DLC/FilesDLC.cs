//  ============================================================================
//  AUTHOR		 : Simon
//  CREATE DATE	 : 11 Apr 2007
//  PURPOSE		 : Data Layer for the Files Table . All access through standard SPs with typed Datasets.
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
    public static class FilesDLC
    {
        public const string SP_Get = "up_Files_get";
        public const string SP_GetP = "up_Files_get_ByPath";
        public const string SP_GetPA = "up_Files_get_ByPath_Att";
        public const string SP_Del = "up_Files_del";
        public const string SP_List = "up_Files_list";
        public const string SP_Add = "up_Files_add";
        public const string SP_Update = "up_Files_upd";
        public const string SP_Move = "up_Files_Move";

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
        private static SqlCommand GenerateSqlCommandfromDataRow(FilesDS.FilesRow Data, string StoredProcName)
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

        public static FilesDS.FilesRow Get(int KeyValue, string SQLConnectionName)
        {

           FilesDS ds = new FilesDS();

           try
            {
                SqlConnection SQLConn = new SqlConnection(getConnstring(SQLConnectionName));

                SqlDataAdapter iDataAdapter = new SqlDataAdapter(SP_Get, SQLConn);
                iDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                
                iDataAdapter.SelectCommand.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                iDataAdapter.SelectCommand.Parameters["@ID"].Value = KeyValue;

                SQLConn.Open();
                //Fill the DataSet with the rows that are returned.
                iDataAdapter.Fill(ds, ds.Files.TableName);
                iDataAdapter.Dispose();

                SQLConn.Close();

            }
            catch
            {
                return null;
            }

            if (ds.Files.Rows.Count == 1)
            {

                return (FilesDS.FilesRow)ds.Files.Rows[0];
            }
            else
            {
                return null;
            }
            

        }
        public static FilesDS.FilesRow Get(int ParentID, string FileName, string SQLConnectionName)
        {
            FilesDS ds = new FilesDS();
            
            try
            {
                SqlConnection SQLConn = new SqlConnection(getConnstring(SQLConnectionName));

                SqlDataAdapter iDataAdapter = new SqlDataAdapter(SP_GetP, SQLConn);
                iDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                
                iDataAdapter.SelectCommand.Parameters.Add(new SqlParameter("@ParentID", SqlDbType.Int));
                iDataAdapter.SelectCommand.Parameters["@ParentID"].Value = ParentID;
                iDataAdapter.SelectCommand.Parameters.Add(new SqlParameter("@FileName", SqlDbType.VarChar, 255));
                iDataAdapter.SelectCommand.Parameters["@FileName"].Value = FileName;


                SQLConn.Open();
                //Fill the DataSet with the rows that are returned.
                iDataAdapter.Fill(ds, ds.Files.TableName);
                iDataAdapter.Dispose();

                SQLConn.Close();

            }
            catch
            {
                return null;
            }

            if (ds.Files.Rows.Count == 1)
            {

                return (FilesDS.FilesRow)ds.Files.Rows[0];
            }
            else
            {
                return null;
            }

        }
        public static Search_FilesDS.FilesRow View(int ParentID, string FileName, string SQLConnectionName)
        {
            Search_FilesDS ds = new Search_FilesDS();

            try
            {
                SqlConnection SQLConn = new SqlConnection(getConnstring(SQLConnectionName));

                SqlDataAdapter iDataAdapter = new SqlDataAdapter(SP_GetPA, SQLConn);
                iDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                
                iDataAdapter.SelectCommand.Parameters.Add(new SqlParameter("@ParentID", SqlDbType.Int));
                iDataAdapter.SelectCommand.Parameters["@ParentID"].Value = ParentID;
                iDataAdapter.SelectCommand.Parameters.Add(new SqlParameter("@FileName", SqlDbType.VarChar, 255));
                iDataAdapter.SelectCommand.Parameters["@FileName"].Value = FileName;


                SQLConn.Open();
                //Fill the DataSet with the rows that are returned.
                iDataAdapter.Fill(ds, ds.Files.TableName);
                iDataAdapter.Dispose();

                SQLConn.Close();

            }
            catch
            {
                return null;
            }


            if (ds.Files.Rows.Count == 1)
            {

                return (Search_FilesDS.FilesRow)ds.Files.Rows[0];
            }
            else
            {
                return null;
            }

        }
        public static Search_FilesDS.FilesRow View(int KeyValue, string SQLConnectionName)
        {
            Search_FilesDS ds = new Search_FilesDS();

            try
            {
                SqlConnection SQLConn = new SqlConnection(getConnstring(SQLConnectionName));

                SqlDataAdapter iDataAdapter = new SqlDataAdapter(SP_GetPA, SQLConn);
                iDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                
                iDataAdapter.SelectCommand.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                iDataAdapter.SelectCommand.Parameters["@ID"].Value = KeyValue;

                SQLConn.Open();
                //Fill the DataSet with the rows that are returned.
                iDataAdapter.Fill(ds, ds.Files.TableName);
                iDataAdapter.Dispose();

                SQLConn.Close();

            }
            catch
            {
                return null;
            }

            if (ds.Files.Rows.Count == 1)
            {

                return (Search_FilesDS.FilesRow)ds.Files.Rows[0];
            }
            else
            {
                return null;
            }

        }
        public static int Add(FilesDS.FilesRow dr, string SQLConnectionName)
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
        public static int Update(FilesDS.FilesRow dr, string SQLConnectionName)
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
        public static Search_FilesDS List(int ParentID, string SQLConnectionName)
        {

            Search_FilesDS ds = new Search_FilesDS();

            try
            {
                SqlConnection SQLConn = new SqlConnection(getConnstring(SQLConnectionName));

                SqlDataAdapter iDataAdapter = new SqlDataAdapter(SP_List, SQLConn);
                iDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                
                iDataAdapter.SelectCommand.Parameters.Add(new SqlParameter("@ParentID", SqlDbType.Int));
                iDataAdapter.SelectCommand.Parameters["@ParentID"].Value = ParentID;
                iDataAdapter.SelectCommand.Parameters.Add(new SqlParameter("@sort_col", SqlDbType.VarChar, 120));
                iDataAdapter.SelectCommand.Parameters["@sort_col"].Value = "FileName";
                
                SQLConn.Open();
                //Fill the DataSet with the rows that are returned.
                iDataAdapter.Fill(ds, ds.Files.TableName);
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