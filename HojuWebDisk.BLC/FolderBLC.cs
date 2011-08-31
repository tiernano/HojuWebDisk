//  ============================================================================
//  AUTHOR		 : Simon
//  CREATE DATE	 : 14 Apr 2007
//  PURPOSE		 : 
//  Business Abstraction Layer for Folders.
//  SPECIAL NOTES: 
//  This class provides the business abstraction layer the HojuWebDisk server. It purpose is to peform all tasks that
//  relate to extracting data from the backend database and passing this upstream to the HojuWebDisk Business components
//  so the HojuWebDisk components are unaware that the underlying infrastructure is SQL not the file system.
//  ===========================================================================

using System;
using System.Web;
using System.Collections.Generic;
using System.Text;
using HojuWebDisk.DataEntities;
using HojuWebDisk.DLC;

namespace HojuWebDisk.BLC
{

    public class FolderBLC
    {
        #region Constructors
        public FolderBLC()
        {

        }
        #endregion

        public const string DBConnName = "Webdav";

        #region Folder Management
        public static FoldersDS.FoldersRow GetFolder(int ID)
        {
            return FoldersDLC.Get(ID,DBConnName);
        }
        public static FoldersDS.FoldersRow GetFolder(int ParentID, string FolderName)
        {
            return FoldersDLC.Get(ParentID,FolderName,DBConnName);
        }
        public static FoldersDS GetFolders(int ParentID)
        {
            return FoldersDLC.List(ParentID, DBConnName);
        }
        public static int SaveFolder(FoldersDS.FoldersRow rdr)
        {
            //To do get id from HojuWebDisk context
            //String userid = HttpContext.Current.User.Identity.Name.Split(@"\".ToCharArray(), 2)[1];

          

            if (rdr.ID == 0)
            {
                //this is a new entry.
                return FoldersDLC.Add(rdr, DBConnName);
            }
            else
            {
                //this is an existing entry
                return FoldersDLC.Update(rdr, DBConnName);
            }

        }
        public static bool DelFolder(FoldersDS.FoldersRow rdr)
        {
            //To do get id from HojuWebDisk context
            //String userid = HttpContext.Current.User.Identity.Name.Split(@"\".ToCharArray(), 2)[1];

            
            return FoldersDLC.Del(rdr,DBConnName);
        }
        public static bool DelFolder(int ID)
        {
            //To do get id from HojuWebDisk context
            //String userid = HttpContext.Current.User.Identity.Name.Split(@"\".ToCharArray(), 2)[1];

          
            return FoldersDLC.Del(ID, DBConnName);

        }
        #endregion

        #region HojuWebDisk Properties



        #endregion


    }
}