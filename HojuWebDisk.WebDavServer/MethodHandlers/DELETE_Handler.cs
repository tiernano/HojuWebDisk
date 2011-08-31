using System;
using System.Text;
using System.Web;
using System.IO;

using HojuWebDisk.WebDavServer.XMLDBObjects;

using HojuWebDisk.DataEntities;
using HojuWebDisk.BLC;

namespace HojuWebDisk.WebDavServer.MethodHandlers
{
    public class DELETE_Handler : IMethod_Handler
    {
        private HttpApplication _httpApplication;
        private string _requestPath;
        private ProcessingErrorCollection _Errors = new ProcessingErrorCollection();

        public DELETE_Handler(HttpApplication HttpApplication)
        {

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
                if (_Errors.Count == 0)
                {
                    return "";
                }else{
                    return XMLWebDavError.ProcessErrorCollection(this._Errors);
                }
            }
        }
        public int Handle()
        {
            //Check to see if the resource is a folder
            this._requestPath = WebDavHelper.getRelativePath(this._httpApplication, this._httpApplication.Request.FilePath);
            FoldersDS.FoldersRow _dirInfo = WebDavHelper.getFolder(this._requestPath);

            if (_dirInfo != null)
            {

                try
                {
                    //Check for locks recursively.
                    DelFolder(_dirInfo, true);
                    //if there are no lock errors then remove the files and folders.
                    if (this._Errors.Count == 0)
                    {
                        DelFolder(_dirInfo, false);
                        FolderBLC.DelFolder(_dirInfo);
                    }

                }
                catch (Exception) {
                    return (int)DavDeleteResponseCode.Locked;
                }
            }
            else
            {
                Search_FilesDS.FilesRow _fileInfo = WebDavHelper.getFileAttribsOnly(this._requestPath);
                if (_fileInfo != null)
                {
                    if (WebDavHelper.getLock(_fileInfo.ID) == null)
                    {
                        FileBLC.DelFile(_fileInfo.ID);
                    }
                    else
                    {
                        //this is for a single file so just respond in header.
                        return (int)DavDeleteResponseCode.Locked;

                        }
                }
                else
                {
                    //ToDo: return an error tree
                }
            }
            return (int)ServerResponseCode.Ok;

        }

        #endregion

        #region private Handler Methods

        private void DelFolder(FoldersDS.FoldersRow _sourceDirectory, bool JustCheckforLocks)
        {
            if (_sourceDirectory != null)
            {
                try
                {
                    //Move over the directory files
                    Search_FilesDS _filelist = WebDavHelper.getFolderFiles(_sourceDirectory.ID);

                    foreach (Search_FilesDS.FilesRow sfr in _filelist.Files)
                    {
                        if (JustCheckforLocks)
                        {
                            if (WebDavHelper.getLock(sfr.ID) != null)
                            {
                                this._Errors.Add(new ProcessingError(WebDavHelper.getFolderFullPath(sfr.ParentID) + sfr.FileName, WebDavHelper.getEnumHttpResponse(DavDeleteResponseCode.Locked)));

                            }
                        }
                        else
                        {
                            WebDavHelper.RemoveFile(sfr.ID);
                        }
                    }

                    FoldersDS _dirlist = WebDavHelper.getSubFolders(_sourceDirectory.ID);

                    foreach (FoldersDS.FoldersRow _dir in _dirlist.Folders)
                    {

                        DelFolder(_dir, JustCheckforLocks);
                        if (!(JustCheckforLocks))
                            WebDavHelper.RemoveFolder(_dir.ID);

                    }
                }

                catch (Exception)
                {
                    //To do exception on delete handling
                }
            }
        }

        #endregion
    }
}

