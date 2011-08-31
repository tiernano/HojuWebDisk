//  ============================================================================
//  AUTHOR		 : Simon
//  CREATE DATE	 : 9 May 2007
//  PURPOSE		 : Data Layer for the Access Table . All access through standard SPs with typed Datasets.
//  SPECIAL NOTES: 
//  
//  ===========================================================================

using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Corp.Framework.InterfaceCommon;
using Corp.Framework.DALC;
using Corp.Framework.Security;
using HojuWebDisk.DataEntities;

namespace HojuWebDisk.DLC
{
    public static class AccessDLC
    {
        public const string SP_Get = "up_Access_get";
        public const string SP_Del = "up_Access_del";
        public const string SP_Del_B = "up_Access_del_bulk";
        public const string SP_List = "up_Access_list";
        public const string SP_Add = "up_Files_add";
       
        public const string DataKeyName = "ID";

        #region Public Methods

        public static  AccessDS.AccessRow Get(int KeyValue)
        {
            AccessDS ds = new AccessDS();

            DataTable dt = new DataTable();
            dt.Columns.Add(DataKeyName);
            DataRow dr = dt.NewRow();
            dr[DataKeyName] = KeyValue;

            Database db = DALCHelper.GetDatabase();

            using (DbConnection dbConn = db.CreateConnection())
            {
                DALCHelper.List(db, SP_Get, ds, ds.Access.TableName, dr);
                dbConn.Close();

            }

            if (ds.Access.Rows.Count == 1)
            {

                return (AccessDS.AccessRow)ds.Access.Rows[0];
            }
            else
            {
                return null;
            }

        }
        public static bool Add(AccessDS.AccessRow dr, string userid)
        {
            bool transuc = false;
            Database db = DALCHelper.GetDatabase();
            using (DbConnection dbConn = db.CreateConnection())
            {
                dbConn.Open();
                DbTransaction dbTran = dbConn.BeginTransaction();
                try
                {
                    DALCHelper.Add(dbTran, db, SP_Add, dr, userid);
                    dbTran.Commit();
                    transuc = true;
                }
                catch
                {
                    transuc = false;
                    dbTran.Rollback();
                    throw;
                }
                finally
                {

                    dbTran.Dispose();
                }
                dbConn.Close();
            }
            return transuc;
        }
        public static bool Del(int KeyValue, string userid)
        {
            bool transuc = false;

            DataTable dt = new DataTable();
            dt.Columns.Add(DataKeyName);
            DataRow dr = dt.NewRow();
            dr[DataKeyName] = KeyValue;

            Database db = DALCHelper.GetDatabase();
            using (DbConnection dbConn = db.CreateConnection())
            {
                dbConn.Open();
                DbTransaction dbTran = dbConn.BeginTransaction();
                try
                {
                    DALCHelper.Delete(dbTran, db, SP_Del, dr, userid);
                    dbTran.Commit();
                    transuc = true;
                }
                catch
                {
                    transuc = false;
                    dbTran.Rollback();
                    throw;
                }
                finally
                {

                    dbTran.Dispose();
                }
                dbConn.Close();
            }
            return transuc;
        }
        public static bool BulkDel(int KeyValue, string userid)
        {
            bool transuc = false;

            DataTable dt = new DataTable();
            dt.Columns.Add("FolderID");
            DataRow dr = dt.NewRow();
            dr["FolderID"] = KeyValue;

            Database db = DALCHelper.GetDatabase();
            using (DbConnection dbConn = db.CreateConnection())
            {
                dbConn.Open();
                DbTransaction dbTran = dbConn.BeginTransaction();
                try
                {
                    DALCHelper.Delete(dbTran, db, SP_Del_B, dr, userid);
                    dbTran.Commit();
                    transuc = true;
                }
                catch
                {
                    transuc = false;
                    dbTran.Rollback();
                    throw;
                }
                finally
                {

                    dbTran.Dispose();
                }
                dbConn.Close();
            }
            return transuc;
        }
        public static AccessDS List(int FolderID,string UID)
        {
            AccessDS ds = new AccessDS();

            DataTable dt = new DataTable();
            dt.Columns.Add("FolderID");
            dt.Columns.Add("sort_col");
            if (UID != "")
                dt.Columns.Add("UID");
            
            DataRow dr = dt.NewRow();
            dr["FolderID"] = FolderID;
            dr["sort_col"] = "UID";
            if (UID != "")
                dr["UID"] = UID;
                
            Database db = DALCHelper.GetDatabase();
            using (DbConnection dbConn = db.CreateConnection())
            {
                DALCHelper.List(db, SP_List, ds, ds.Access.TableName, dr);
                dbConn.Close();
            }

            return ds;
        }

        #endregion


    }
}