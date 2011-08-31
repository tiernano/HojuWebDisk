//  ============================================================================
//  AUTHOR		 : Simon
//  CREATE DATE	 : 3 May 2007
//  PURPOSE		 : 
//  Handles a WebDav UNLOCK request for a resource.
//  SPECIAL NOTES: 
//  Please refer to the summary of the LOCK_Handler for special notes on supported locks

//  (
//  ===========================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Web;
using System.IO;
using System.Globalization;

using HojuWebDisk.WebDavServer;
using HojuWebDisk.WebDavServer.XMLDBObjects;

using HojuWebDisk.DataEntities;
using HojuWebDisk.BLC;

namespace HojuWebDisk.WebDavServer.MethodHandlers
{
    public class UNLOCK_Handler : IMethod_Handler
    {

        private HttpApplication _httpApplication;

        private string _LockToken = "";
       
            

        public UNLOCK_Handler(HttpApplication httpApplication)
        {

            _httpApplication = httpApplication;
           
        }

        #region IMethod_Handler Interface

        public string RequestXml
        {
            get
            {
                return "";
            }
        }
        public string ResponseXml
        {
            get
            {
                return "";
            }
        }
        public string ErrorXml
        {
            get
            {
                return "";
            }
        }
        public int Handle()
        {

            if (WebDavHelper.getRequestLength(this._httpApplication) != 0)
                return (int)ServerResponseCode.BadRequest;

            if (this._httpApplication.Request.Headers["Lock-Token"] == null)
               return (int)DavUnlockResponseCode.BadRequest;

            this._LockToken = WebDavHelper.ParseOpaqueLockToken(this._httpApplication.Request.Headers["Lock-Token"]);

            LocksDS.LocksRow _lock = WebDavHelper.getFileLockbyToken(this._LockToken);

            if (_lock == null)
            {
                return (int)DavUnlockResponseCode.PreconditionFailed;
            }
            else
            {
                //Delete the locked files
                WebDavHelper.RemoveLock(_lock.ID);
                
            }


            return (int)DavUnlockResponseCode.NoContent;
        }


        #endregion

        



    }
}
