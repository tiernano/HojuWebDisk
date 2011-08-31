//  ============================================================================
//  AUTHOR		 : Simon
//  CREATE DATE	 : 3 May 2007
//  PURPOSE		 : 
//  Handles a COPY request for a resource or collection.
//  SPECIAL NOTES: 
//  I have not implemented the keepAliveURis for a copy, all properties of a resource are copies by deafult, even if the client 
//  specifies otherwise.This is because this is meant as a server for windows explorer and Office, neither of these clients support keepAliveURis 
//  In fact neither of these client send any form of request XML.

//  (
//  ===========================================================================

using System;
using System.Collections;
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
    public class COPY_Handler : IMethod_Handler
    {
        private HttpApplication _httpApplication;
        private string _requestPath;
        private string _destinationPath;
       
        private XPathNavigator _requestXmlNavigator = null;
        private bool _overwriteResource;

        private ProcessingErrorCollection _Errors = new ProcessingErrorCollection();

        public COPY_Handler(HttpApplication HttpApplication)
        {

            this._httpApplication = HttpApplication;
            if (_httpApplication.Request.InputStream.Length != 0)
            {
                _requestXmlNavigator = new XPathDocument(_httpApplication.Request.InputStream).CreateNavigator();
            }
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
                if (_requestXmlNavigator == null) return "";
                return _requestXmlNavigator.InnerXml;
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
            if (_httpApplication == null)
                return (int)ServerResponseCode.BadRequest;

            int _httpResponseCode = GetRequestType();
            if (_httpResponseCode == (int)ServerResponseCode.Ok)
            {
                this._requestPath = WebDavHelper.getRelativePath(this._httpApplication, this._httpApplication.Request.FilePath);
                this._destinationPath = getRequestDestination();

                //Check to make sure the resource is valid
                if (this._requestPath == this._destinationPath)
                    return (int)DavCopyResponseCode.Conflict;

                if (!WebDavHelper.ValidResourceByPath(this._requestPath))
                    return (int)ServerResponseCode.NotFound;

                //Make sure the destination directory exists
                FoldersDS.FoldersRow _destFolder = WebDavHelper.getParentFolder(this._destinationPath);
                if (_destFolder == null)
                    return (int)DavCopyResponseCode.Conflict;
                else
                {
                    FilesDS.FilesRow _sourceFile = WebDavHelper.getFile(this._requestPath);
                    FoldersDS.FoldersRow _sourceDir = WebDavHelper.getFolder(this._requestPath);
                    if (_sourceDir != null)
                    {
                        //How much do we want to copy?
                        switch (WebDavHelper.getRequestDepth(_httpApplication))
                        {
                            case DepthType.ResourceOnly:
                                CreateDirectory(this._destinationPath);
                                break;

                            case DepthType.Infinity:
                                CheckforDestinationLocks(_sourceDir, this._destinationPath);
                                if (this._Errors.Count == 0)
                                       CloneDirectory(_sourceDir, this._destinationPath);
                                
                                break;
                        }
                        return (int)DavCopyResponseCode.Created;
                    }
                    else if (_sourceFile != null)
                    {
                        CheckforDestinationLocks(_sourceFile.FileName, this._destinationPath);
                        if (this._Errors.Count == 0)
                                CopyFile(_sourceFile, this._destinationPath);
                        return (int)DavCopyResponseCode.Created;
                    }
                    else
                    {
                        return (int)DavCopyResponseCode.BadGateway;
                    }
                }
            }
            else
            {
                return _httpResponseCode;
            }


        

        }

        #endregion

        #region private hanlder methods
        private int GetRequestType()
        {

            if ((WebDavHelper.getRequestDepth(_httpApplication) != DepthType.ResourceOnly) && (WebDavHelper.getRequestDepth(_httpApplication) != DepthType.Infinity))
                return (int)ServerResponseCode.BadRequest;
            if (this._httpApplication.Request.Headers["Destination"] == null)
                return (int)ServerResponseCode.BadRequest;


            /* None of this is used below as we always copy all properties
             if (this._requestXmlNavigator == null)
                return (int)ServerResponseCode.BadRequest;
             
            XPathNodeIterator _propertyBehaviorNodeIterator = this._requestXmlNavigator.SelectDescendants("propertybehavior", "DAV:", false);

            
            if (_propertyBehaviorNodeIterator.MoveNext())
            {

                XPathNodeIterator _keepAliveNodeIterator = _propertyBehaviorNodeIterator.Current.SelectDescendants("keepalive", "DAV:", false);

                if (_keepAliveNodeIterator.MoveNext())
                {
                    this._propertyBehavior = PropertyBehavior.KeepAlive;

                    XPathNodeIterator _keepAliveChildren = _keepAliveNodeIterator.Current.SelectChildren(XPathNodeType.All);
                    while (_keepAliveChildren.MoveNext())
                    {
                        string _nodeValue = _keepAliveChildren.Current.Value;

                        switch (_nodeValue)
                        {
                            case "*":
                                _keepAliveAllProperties = true;
                                break;

                            default:
                                _keepAliveURIs.Add(_nodeValue);
                                break;
                        }
                    }
                }
            }
            */
            this._overwriteResource = true;

            if (_httpApplication.Request.Headers["Overwrite"] != null)
                this._overwriteResource = !(_httpApplication.Request.Headers["Overwrite"] == "f");

            return (int)ServerResponseCode.Ok;
        }
        private string getRequestDestination()
        {
		    string _destination =this._httpApplication.Request.Headers["Destination"];
			if (_destination == null){
				_destination = "";
            }else{
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
                                this._Errors.Add(new ProcessingError(sfr.FileName, WebDavHelper.getEnumHttpResponse(DavCopyResponseCode.Locked)));
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
                    this._Errors.Add(new ProcessingError(_sourceDirectory.FolderName, WebDavHelper.getEnumHttpResponse(DavCopyResponseCode.Forbidden)));

                }

            }


        }
        private void CheckforDestinationLocks(string SourceName,string destination)
        {
            //Check for a lock at a destination string(used for single filecopy only)
            //if they exist then add the error to the errorlist
             try
                {
                    Search_FilesDS.FilesRow destFile = WebDavHelper.getFileAttribsOnly(destination);

                    if (destFile == null) return;

                    if (WebDavHelper.getLock(destFile.ID) != null)
                        this._Errors.Add(new ProcessingError(SourceName, WebDavHelper.getEnumHttpResponse(DavCopyResponseCode.Locked)));
                         
                }
                catch (Exception)
                {
                    this._Errors.Add(new ProcessingError(SourceName, WebDavHelper.getEnumHttpResponse(DavCopyResponseCode.Forbidden)));

                }
                        
        }
        private void CloneDirectory(FoldersDS.FoldersRow _sourceDirectory, string destination)
        {
            if (_sourceDirectory != null)
            {
                try
                {
                    FoldersDS.FoldersRow _destFolder = WebDavHelper.getFolder(destination);

                    if (!this._overwriteResource && _destFolder != null)
                    {
                        this._Errors.Add(new ProcessingError(_sourceDirectory.FolderName, WebDavHelper.getEnumHttpResponse(DavCopyResponseCode.PreconditionFailed)));
                    }
                    else
                    {
                        //Create the destination directory
                        if (_destFolder == null)
                        {
                            CreateDirectory(destination);
                        }
                        //Move over the directory files
                        Search_FilesDS _filelist = WebDavHelper.getFolderFiles(_sourceDirectory.ID);

                        foreach (Search_FilesDS.FilesRow sfr in _filelist.Files)
                        {
                            FilesDS.FilesRow _file = WebDavHelper.getFile(sfr.ID);
                            CopyFile(_file, destination + "/" + _file.FileName);
                        }

                        FoldersDS _dirlist = WebDavHelper.getSubFolders(_sourceDirectory.ID);

                        foreach (FoldersDS.FoldersRow _dir in _dirlist.Folders)
                            CloneDirectory(_dir, destination + "/" + _dir.FolderName);
                    }
                }
                catch (Exception)
                {
                    this._Errors.Add(new ProcessingError(_sourceDirectory.FolderName, WebDavHelper.getEnumHttpResponse(DavCopyResponseCode.Forbidden)));
                   
                }
            }
        }
        private void CopyFile(FilesDS.FilesRow _sourceFile, string destination)
        {
            FilesDS.FilesRow _destFile;

            if (_sourceFile != null)
            {
                try
                {
                   _destFile = WebDavHelper.getFile(destination);

                    if (!this._overwriteResource && _destFile != null)
                    {
                        this._Errors.Add(new ProcessingError(_sourceFile.FileName, WebDavHelper.getEnumHttpResponse(DavCopyResponseCode.PreconditionFailed)));
                    }
                    else
                    {
                        FoldersDS.FoldersRow _destdir = WebDavHelper.getParentFolder(destination);
                        if (_destdir == null)
                        {
                            this._Errors.Add(new ProcessingError(_sourceFile.FileName, WebDavHelper.getEnumHttpResponse(DavCopyResponseCode.BadGateway)));
                        }
                        else
                        {
                            if (_destFile == null)
                            {
                                FilesDS fds = new FilesDS();
                                _destFile = fds.Files.NewFilesRow();     
                            }
                            _destFile.ContentType = _sourceFile.ContentType;
                            _destFile.FileData = _sourceFile.FileData;
                            _destFile.FileDataSize = _sourceFile.FileDataSize;
                            _destFile.FileName = _sourceFile.FileName;
                            _destFile.ParentID = _destdir.ID;
                            _destFile.update_user_stamp = HttpContext.Current.User.Identity.Name;
                            WebDavHelper.SaveFile(_destFile);

                        }
                    }
                }
                catch (Exception)
                {
                    this._Errors.Add(new ProcessingError(_sourceFile.FileName, WebDavHelper.getEnumHttpResponse(DavCopyResponseCode.Forbidden)));
                  
                }
            }
        }
        private void CreateDirectory(string path)
        {

            FoldersDS fds = new FoldersDS();
            FoldersDS.FoldersRow fdr;
            FoldersDS.FoldersRow pdr;

            try
            {

                pdr = WebDavHelper.getParentFolder(path);

                fdr = fds.Folders.NewFoldersRow();
                fdr.ParentID = pdr.ID;
                fdr.FolderName = WebDavHelper.getResourceName(path);
                fdr.update_user_stamp = HttpContext.Current.User.Identity.Name;
                WebDavHelper.SaveFolder(fdr);

            }
            catch (Exception)
            {
              
                this._Errors.Add(new ProcessingError(WebDavHelper.getResourceName(path), WebDavHelper.getEnumHttpResponse(DavCopyResponseCode.InsufficientStorage)));
               
            }
        }

            
        #endregion
    }
}