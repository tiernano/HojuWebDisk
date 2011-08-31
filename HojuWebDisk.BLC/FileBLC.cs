//  ============================================================================
//  AUTHOR		 : Simon
//  CREATE DATE	 : 14 Apr 2007
//  PURPOSE		 : 
//  Business Abstraction Layer for Files.
//  SPECIAL NOTES: 
//  This class provides the business abstraction layer the HojuWebDisk server. It purpose is to peform all tasks that
//  relate to extracting data from the backend database and passing this upstream to the HojuWebDisk Business components
//  so the HojuWebDisk components are unaware that the underlying infrastructure is SQL not the file system.
//  
//  ===========================================================================

using System;
using System.Web;
using System.Collections.Generic;
using System.Text;
using HojuWebDisk.DataEntities;
using HojuWebDisk.DLC;

namespace HojuWebDisk.BLC
{
    public class FileBLC
    {

        public const string DBConnName = "Webdav";

        #region Constructors
        public FileBLC()
        {

        }
        #endregion
        
        #region File Storage Management
        public static FilesDS.FilesRow GetFile(int ID)
        {
            return FilesDLC.Get(ID, DBConnName);
        }
        public static FilesDS.FilesRow GetFile(int ParentID, string FileName)
        {
            return FilesDLC.Get(ParentID,FileName,DBConnName);
        }
        public static Search_FilesDS.FilesRow GetFileAttribs(int ParentID, string FileName)
        {
            return FilesDLC.View(ParentID,FileName,DBConnName);
        }
        public static Search_FilesDS.FilesRow GetFileAttribs(int ID)
        {
            return FilesDLC.View(ID,DBConnName);
        }
        public static Search_FilesDS List(int ParentID)
        {
            return FilesDLC.List(ParentID,DBConnName);
        }
        public static int SaveFile(FilesDS.FilesRow rdr)
        {
            //To do get id from HojuWebDisk context
            //String userid = HttpContext.Current.User.Identity.Name.Split(@"\".ToCharArray(), 2)[1];

           

            if (rdr.ID == 0)
            {
                //this is a new entry.
                return FilesDLC.Add(rdr, DBConnName);
            }
            else
            {
                //this is an existing entry
                return FilesDLC.Update(rdr, DBConnName);
            }

        }
        public static bool DelFile(FilesDS.FilesRow rdr)
        {
            //To do get id from HojuWebDisk context
            //String userid = HttpContext.Current.User.Identity.Name.Split(@"\".ToCharArray(), 2)[1];

            
            return FilesDLC.Del(rdr.ID, DBConnName);

        }
        public static bool DelFile(int ID)
        {
            //To do get id from HojuWebDisk context
            //String userid = HttpContext.Current.User.Identity.Name.Split(@"\".ToCharArray(), 2)[1];

           
            return FilesDLC.Del(ID, DBConnName);

        }
        #endregion

        #region HojuWebDisk Lock Management
                
        public static LocksDS.LocksRow getFileLockbyToken(string Token)
        {
            Locks_TokensDS lds = Locks_TokensDLC.List(Token,DBConnName);
            Locks_TokensDS.Locks_TokensRow ltr;

            if (lds.Locks_Tokens.Rows.Count == 0)
            {
                return null;
            }
            else
            {
               ltr = (Locks_TokensDS.Locks_TokensRow)lds.Locks_Tokens.Rows[0];
            }
            return LocksDLC.Get(ltr.LockID,DBConnName);
        }
        public static bool RemoveLock(int LockID)
        {
            //To do get id from HojuWebDisk context
            //String userid = HttpContext.Current.User.Identity.Name.Split(@"\".ToCharArray(), 2)[1];

            
            if (LocksDLC.Del(LockID, DBConnName))
            {
                if (Locks_TokensDLC.Del(LockID, DBConnName))
                {
                    return Locks_TokensDLC.Del(LockID, DBConnName);
                }
                else
                {
                    return false;
                }
            }
            else
            {

                return false;
            }

            
        }
        public static LocksDS.LocksRow getLock(int FileID)
        {
            LocksDS _locks = LocksDLC.List(FileID,DBConnName);

            if (_locks.Locks.Rows.Count == 0)
            {
                return null;
            }
            return (LocksDS.LocksRow)_locks.Locks.Rows[0];

        }
        public static Locks_TokensDS getLockTokens(int LockID)
        {
            return Locks_TokensDLC.List(LockID,DBConnName);
        }
        public static int SaveLock(LocksDS.LocksRow nlr)
        {
           
            if (nlr.ID == 0)
            {
                return LocksDLC.Add(nlr, DBConnName);
            }
            else
            {
                return LocksDLC.Update(nlr, DBConnName);
            }

        }
        public static int SaveLockToken(Locks_TokensDS.Locks_TokensRow nltr)
        {
           
            return Locks_TokensDLC.Add(nltr, DBConnName);

        }
        #endregion
    }
}
