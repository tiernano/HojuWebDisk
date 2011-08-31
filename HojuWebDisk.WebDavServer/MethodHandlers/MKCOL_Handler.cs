using System;
using System.Text;
using System.Web;
using System.IO;

using HojuWebDisk.DataEntities;
using HojuWebDisk.BLC;

namespace HojuWebDisk.WebDavServer.MethodHandlers
{
    public class MKCOL_Handler: IMethod_Handler
    {

        private HttpApplication _httpApplication;
        private string _requestPath;
        
        public MKCOL_Handler(HttpApplication HttpApplication) {

            this._httpApplication = HttpApplication;

        }

        #region IMethod_Handler Interface
       
        public string ResponseXml
        {
            get
            {
                return "";
            }
        }
        public string RequestXml
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
            if (_httpApplication==null)
                return (int)ServerResponseCode.BadRequest;

            this._requestPath = WebDavHelper.getRelativePath(this._httpApplication, this._httpApplication.Request.FilePath);
            
            //Check to see if the RequestPath is already a resource
			if (WebDavHelper.ValidResourceByPath(this._requestPath))
                return (int)DavMKColResponseCode.MethodNotAllowed;
			else {
				//Check to see if the we can create a new folder
				FoldersDS.FoldersRow _dirInfo = WebDavHelper.getParentFolder(this._requestPath);

				//The parent folder does not exist
				if (_dirInfo == null)
                    return (int)DavMKColResponseCode.Conflict;
				else {
					string _requestedFolder = WebDavHelper.getResourceName(this._requestPath);

					try {
						
                        FoldersDS fds = new FoldersDS();                        
                        FoldersDS.FoldersRow fdr = fds.Folders.NewFoldersRow();

                        fdr.ParentID = _dirInfo.ID;
                        fdr.FolderName = _requestedFolder;
                        fdr.update_user_stamp = HttpContext.Current.User.Identity.Name;
                       if ((FolderBLC.SaveFolder(fdr) == -1)){
                           return (int)DavMKColResponseCode.InsufficientStorage;
                        }
						
					}
					catch (Exception) {
                        return (int)DavMKColResponseCode.InsufficientStorage;
					}

				}

			}
            return (int)DavMKColResponseCode.Created;
		}
            
        		
        #endregion
    }
}
