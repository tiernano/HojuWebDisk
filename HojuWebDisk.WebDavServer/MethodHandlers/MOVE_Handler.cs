using System;
using System.Text;
using System.Web;
using System.IO;

using HojuWebDisk.WebDavServer.XMLDBObjects;
using HojuWebDisk.DataEntities;
using HojuWebDisk.BLC;

namespace HojuWebDisk.WebDavServer.MethodHandlers
{
    public class MOVE_Handler : IMethod_Handler
    {
        private HttpApplication _httpApplication;
        private string _requestPath;
 
        private string _destinationPath;
        private bool _isrename = false;
      
        private bool _overwriteResource;

        private ProcessingErrorCollection _Errors = new ProcessingErrorCollection();

        public MOVE_Handler(HttpApplication HttpApplication)
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
                }
                else
                {
                    return XMLWebDavError.ProcessErrorCollection(this._Errors);
                }
            }
        }
        public int Handle()
        {

            this._requestPath = WebDavHelper.getRelativePath(this._httpApplication, this._httpApplication.Request.FilePath);
            this._destinationPath = getRequestDestination();
            this._overwriteResource = true;

            if (_httpApplication.Request.Headers["Overwrite"] != null)
                this._overwriteResource = !(_httpApplication.Request.Headers["Overwrite"] == "f");

                        //Check to make sure the resource is valid
             if (!WebDavHelper.ValidResourceByPath(this._requestPath))
                 return (int)ServerResponseCode.NotFound;
             else
             {
                 if (WebDavHelper.getParentResourcePath(this._destinationPath,1) == WebDavHelper.getParentResourcePath(this._requestPath, 1))
                     this._isrename = true;


                 if (_isrename)
                 {
                     if (WebDavHelper.ValidResourceByPath(this._destinationPath))
                     {
                         return (int)DavMoveResponseCode.Conflict;
                     }
                     else
                     {
                         FilesDS.FilesRow _sourceFile = WebDavHelper.getFile(this._requestPath);
                         FoldersDS.FoldersRow _sourceDir = WebDavHelper.getFolder(this._requestPath);
                         if (_sourceDir != null)
                         {
                             _sourceDir.FolderName = WebDavHelper.getResourceName(this._destinationPath);
                             WebDavHelper.SaveFolder(_sourceDir);

                         }
                         else if (_sourceFile != null)
                         {
                             _sourceFile.FileName = WebDavHelper.getResourceName(this._destinationPath);
                             WebDavHelper.SaveFile(_sourceFile);
                         }
                         else
                         {
                             return (int)DavMoveResponseCode.BadGateway;
                         }
                     }

                 }
                 else
                 {

                     //Make sure the destination directory exists
                     FoldersDS.FoldersRow _destFolder = WebDavHelper.getParentFolder(this._destinationPath);
                     if (_destFolder == null)
                         return (int)DavMoveResponseCode.Conflict;
                     else
                     {
                         FilesDS.FilesRow _sourceFile = WebDavHelper.getFile(this._requestPath);
                         FoldersDS.FoldersRow _sourceDir = WebDavHelper.getFolder(this._requestPath);
                         if (_sourceDir != null)
                         {
                             CheckforDestinationLocks(_sourceDir, this._destinationPath);
                             if (this._Errors.Count == 0)
                             {
                                 FoldersDS.FoldersRow _DestDir = WebDavHelper.getFolder(this._destinationPath);
                                 if (_DestDir != null)
                                 {
                                     DelFolder(_DestDir, _sourceDir.ID);
                                     WebDavHelper.RemoveFolder(_DestDir.ID);
                                 }
                                 _sourceDir.ParentID = _destFolder.ID;
                                 _sourceDir.FolderName = WebDavHelper.getResourceName(this._destinationPath);
                                 WebDavHelper.SaveFolder(_sourceDir);
                             }

                         }
                         else if (_sourceFile != null)
                         {
                             Search_FilesDS.FilesRow _destFile =WebDavHelper.getFileAttribsOnly(this._destinationPath);


                             if ((!this._overwriteResource) && (_destFile != null))
                             {

                                 return (int)DavMoveResponseCode.BadGateway;

                             }
                             else
                             {
                                 CheckforDestinationLocks(_sourceFile.FileName, this._destinationPath);
                                 if (this._Errors.Count == 0)
                                 {
                                     if (_destFile != null)
                                     {
                                         WebDavHelper.RemoveFile(_destFile.ID);
                                     }

                                     _sourceFile.ParentID = _destFolder.ID;
                                     _sourceFile.FileName = WebDavHelper.getResourceName(this._destinationPath);
                                     WebDavHelper.SaveFile(_sourceFile);
                                 }

                             }


                         }
                     }
                 }
             }
             return (int)DavMoveResponseCode.Created;
        }
        
        #endregion

        #region private Method functions
        private string getRequestDestination()
        {
            string _destination = this._httpApplication.Request.Headers["Destination"];
            if (_destination == null)
            {
                _destination = "";
            }
            else
            {
                _destination = HttpUtility.UrlDecode(_destination.Trim('/'));
            }
            return WebDavHelper.getRelativePath(this._httpApplication, _destination);
        }
        private void CheckforDestinationLocks(FoldersDS.FoldersRow _sourceDirectory, string destination)
        {
            //loop through the destination and determine if there are any filelocks
            // if they exist then add the error to the errorlist

            if (_sourceDirectory != null)
            {
                try
                {
                    FoldersDS.FoldersRow _destFolder = WebDavHelper.getFolder(destination);

                    if (_destFolder == null) return;

                    Search_FilesDS _filelist = WebDavHelper.getFolderFiles(_sourceDirectory.ID);

                    foreach (Search_FilesDS.FilesRow sfr in _filelist.Files)
                    {
                        Search_FilesDS.FilesRow destfile = WebDavHelper.getFileAttribsOnly(destination + "/" + sfr.FileName);
                        if (destfile != null)
                        {
                            if (WebDavHelper.getLock(destfile.ID) != null)
                            {
                                this._Errors.Add(new ProcessingError(sfr.FileName, WebDavHelper.getEnumHttpResponse(DavMoveResponseCode.Locked)));
                                //We already have an error so leave now
                                return;
                            }
                        }
                    }

                    FoldersDS _dirlist = WebDavHelper.getSubFolders(_sourceDirectory.ID);

                    foreach (FoldersDS.FoldersRow _dir in _dirlist.Folders)
                        CheckforDestinationLocks(_dir, destination + "/" + _dir.FolderName);

                }
                catch (Exception)
                {
                    this._Errors.Add(new ProcessingError(_sourceDirectory.FolderName, WebDavHelper.getEnumHttpResponse(DavMoveResponseCode.Forbidden)));

                }

            }


        }
        private void CheckforDestinationLocks(string SourceName, string destination)
        {
            //Check for a lock at a destination string(used for single filecopy only)
            //if they exist then add the error to the errorlist
            try
            {
                Search_FilesDS.FilesRow destFile = WebDavHelper.getFileAttribsOnly(destination);

                if (destFile == null) return;

                if (WebDavHelper.getLock(destFile.ID) != null)
                    this._Errors.Add(new ProcessingError(SourceName, WebDavHelper.getEnumHttpResponse(DavMoveResponseCode.Locked)));

            }
            catch (Exception)
            {
                this._Errors.Add(new ProcessingError(SourceName, WebDavHelper.getEnumHttpResponse(DavMoveResponseCode.Forbidden)));

            }

        }
        private void DelFolder(FoldersDS.FoldersRow _sourceDirectory,int oSourceID)
        {
            if (_sourceDirectory != null)
            {
                //Check that the directory we are about to delete and transverse is not in fact the
                //orginal source of the move. This can occur if the source folder already lies under the destination
                //and a folder of the same name exists in the destination folder.

                if (_sourceDirectory.ID != oSourceID)
                {
                    try
                    {
                        //Move over the directory files
                        Search_FilesDS _filelist = WebDavHelper.getFolderFiles(_sourceDirectory.ID);

                        foreach (Search_FilesDS.FilesRow sfr in _filelist.Files)
                            WebDavHelper.RemoveFile(sfr.ID);

                        FoldersDS _dirlist = WebDavHelper.getSubFolders(_sourceDirectory.ID);

                        foreach (FoldersDS.FoldersRow _dir in _dirlist.Folders)
                        {
                            DelFolder(_dir,oSourceID);
                            if (oSourceID!=_dir.ID)
                                WebDavHelper.RemoveFolder(_dir.ID);
                        }

                        
                    }

                    catch (Exception)
                    {
                        //To do exception on delete handling
                    }
                }
            }
        }
        #endregion
    }
}
