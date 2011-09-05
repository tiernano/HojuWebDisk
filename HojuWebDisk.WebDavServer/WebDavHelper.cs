using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;
using System.Web;
using System.IO;
using System.Globalization;

using HojuWebDisk.WebDavServer;
using HojuWebDisk.DataEntities;
using HojuWebDisk.BLC;

namespace HojuWebDisk.WebDavServer
{
    public class WebDavHelper
    {
        /// <summary>
        /// Private constructor
        /// </summary>
        private WebDavHelper() { }

        /// <summary>
        /// Retrieves the parent path from a resouces URL
        /// </summary>
        /// <param name="urlPath">Absolute or relative URL path</param>
        /// <returns></returns>
        public static string getParentResourcePath(string path, int removeEndTokenCount)
        {
            if (removeEndTokenCount == 0) return path;
            if (!path.Contains("/")) return "";

            StringBuilder _relativePhysicalPath = new StringBuilder();
            string[] _path = path.Split('/');

            for (int i = 0; i < _path.Length - removeEndTokenCount; i++)
            {
                _relativePhysicalPath.Append(_path[i] + @"/");
            }
            string _rpath = _relativePhysicalPath.ToString();


            if (_rpath.EndsWith("/"))
            {
                return _rpath.Substring(0, _rpath.Length - 1);
            }
            else
            {
                return _rpath;
            }
        }

        /// <summary>
        /// Retrieves the resource name from an enitre URL
        /// </summary>
        /// <param name="urlPath">Absolute or relative URL path</param>
        /// <returns></returns>
        public static string getResourceName(string urlPath)
        {
            string _tpath = urlPath.Trim();
            if (_tpath.EndsWith("/"))
            {
                _tpath = _tpath.Substring(0, _tpath.Length - 1);
            }
            if (!(_tpath.Contains("/")))
            {
                return _tpath;
            }

            string[] _path = _tpath.Split('/');

            return _path[_path.Length - 1];

        }


        /// <summary>
        /// Verifies the requested resource is valid
        /// </summary>
        /// <param name="urlPath">Absolute or relative URL path</param>
        /// <returns></returns>
        public static bool ValidResourceByPath(string urlPath)
        {
            FoldersDS.FoldersRow _folderrow = getFolder(urlPath);
            if (_folderrow != null)
            {
                return true;
            }
            Search_FilesDS.FilesRow _filerow = getFileAttribsOnly(urlPath);
            if (_filerow != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Retrieves a directory
        /// </summary>
        /// <param name="urlPath">Absolute or relative URL path</param>
        /// <returns>Null if the directory does not exist</returns>
        public static FoldersDS.FoldersRow getFolder(string urlPath)
        {
            int ParentID = 1;
            FoldersDS.FoldersRow _Folder;

            if (urlPath.Contains("/"))
            {
                //Get the root folder
                _Folder = FolderBLC.GetFolder(1);

                string[] _Path = urlPath.Split("/".ToCharArray());

                for (int _pe = 0; _pe < _Path.Length; _pe++)
                {
                    _Folder = FolderBLC.GetFolder(ParentID, _Path[_pe]);
                    if (_Folder == null)
                    {
                        _pe = _Path.Length + 1;
                    }
                    else
                    {
                        ParentID = _Folder.ID;
                    }
                }

            }
            else
            {
                if (urlPath == "")
                {
                    _Folder = FolderBLC.GetFolder(1);
                }
                else
                {
                    _Folder = FolderBLC.GetFolder(ParentID, urlPath);
                }
            }
            return _Folder;
        }

        public static FoldersDS.FoldersRow getFolder(int FolderID)
        {
            return FolderBLC.GetFolder(FolderID);
        }

        /// <summary>
        /// Retrieves a directory / file's parent directory
        /// </summary>
        /// <param name="urlPath">Absolute or relative URL path</param>
        /// <returns>Null if the parent directory does not exist</returns>
        public static FoldersDS.FoldersRow getParentFolder(string urlPath)
        {
            string _physicalPath = getParentResourcePath(urlPath, 1);
            return getFolder(_physicalPath);

        }

        /// <summary>
        /// Retrieves a file
        /// </summary>
        /// <param name="urlPath">Absolute or relative URL path</param>
        /// <returns>Null if the file does not exist</returns>
        public static FilesDS.FilesRow getFile(string urlPath)
        {
            FoldersDS.FoldersRow _ParentFolder = getFolder(getParentResourcePath(urlPath, 1));
            if (_ParentFolder != null)
            {
                return FileBLC.GetFile(_ParentFolder.ID, getResourceName(urlPath));
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// Retrieves a file
        /// </summary>
        /// <param name="ID">The ID of the file resource in the Database</param>
        /// <returns>Null if the file does not exist</returns>
        public static FilesDS.FilesRow getFile(int ID)
        {
            return FileBLC.GetFile(ID);

        }

        /// <summary>
        /// Deletes a file
        /// </summary>
        /// <param name="ID">The ID of the file resource in the Database</param>
        /// <returns>Null if the file does not exist</returns>
        public static bool RemoveFile(int ID)
        {
            return FileBLC.DelFile(ID);

        }

        /// <summary>
        /// Deletes a Folder
        /// </summary>
        /// <param name="ID">The ID of the file resource in the Database</param>
        /// <returns>Null if the file does not exist</returns>
        public static bool RemoveFolder(int ID)
        {
            return FolderBLC.DelFolder(ID);

        }

        

        /// <summary>
        /// Retrieves a Lock
        /// </summary>
        /// <param name="ID">The ID of the file resource in the Database</param>
        /// <returns>Null if the lock does not exist</returns>
        public static LocksDS.LocksRow getLock(int FileID)
        {
            return FileBLC.getLock(FileID);
        }

        /// <summary>
        /// Retrieves the attibutes of a file
        /// </summary>
        /// <param name="urlPath">Absolute or relative URL path</param>
        /// <returns>Null if the file does not exist</returns>
        public static Search_FilesDS.FilesRow getFileAttribsOnly(string urlPath)
        {
            FoldersDS.FoldersRow _ParentFolder = getFolder(getParentResourcePath(urlPath, 1));
            if (_ParentFolder != null)
            {
                return FileBLC.GetFileAttribs(_ParentFolder.ID, getResourceName(urlPath));
            }
            else
            {
                return null;
            }


        }

        /// <summary>
        /// Retrieves the attributes of a file
        /// </summary>
        /// <param name="ID">The ID of the file resource in the Database</param>
        /// <returns>Null if the file does not exist</returns>
        public static Search_FilesDS.FilesRow getFileAttribsOnly(int ID)
        {
            return FileBLC.GetFileAttribs(ID);

        }

         /// <summary>
        /// Retrieves all of the Folder Rows with a parent ID
        /// </summary>
        /// <param name="FolderID">The ID of the Parent Folder in the Database</param>
        /// <returns></returns>          
        public static FoldersDS getSubFolders(int FolderID)
        {

            return FolderBLC.GetFolders(FolderID);
        }

        /// <summary>
        /// Retrieves the full path of a Folder given an ID
        /// </summary>
        /// <param name="FolderID">The ID of the file in the Database</param>
        /// <returns></returns>   
        public static string getFolderFullPath(int FolderID)
        {
            string _retval = "";
            FoldersDS.FoldersRow _fr = getFolder(FolderID);
            if (_fr != null)
            {
                while (_fr.ParentID != 0)
                {

                    _retval += _fr.FolderName + "/";
                    _fr = getFolder(_fr.ParentID);
                }
            }
            return _retval;
        }



        /// <summary>
        /// Retrieves all of the File Rows with a parent ID
        /// </summary>
        /// <param name="FolderID">The ID of the Parent Folder in the Database</param>
        /// <returns></returns>   
        public static Search_FilesDS getFolderFiles(int FolderID)
        {
            return FileBLC.List(FolderID);
        }

        /// <summary>
        /// Saves a File
        /// </summary>
        /// <param name="rdr">The file row to save</param>
        /// <returns></returns>   
        public static int SaveFile(FilesDS.FilesRow rdr)
        {
            return FileBLC.SaveFile(rdr);
        }

        // <summary>
        /// Saves a Folder
        /// </summary>
        /// <param name="rdr">The folder row to save</param>
        /// <returns></returns>   
        public static int SaveFolder(FoldersDS.FoldersRow rdr)
        {
            return FolderBLC.SaveFolder(rdr);
        }
        /// <summary>
        /// Deletes a Lock
        /// </summary>
        /// <param name="FolderID">The ID of the Lock in the Database</param>
        /// <returns></returns>   
        public static bool RemoveLock(int LockID)
        {
            //To do get id from HojuWebDisk context
            //String userid = HttpContext.Current.User.Identity.Name.Split(@"\".ToCharArray(), 2)[1];
            return FileBLC.RemoveLock(LockID);

        }

        /// <summary>
        /// Saves a Lock
        /// </summary>
        /// <param name="FolderID">The ID of the Lock in the Database</param>
        /// <returns></returns>   
        public static int SaveLock(LocksDS.LocksRow nlr)
        {
            return FileBLC.SaveLock(nlr);

        }

        /// <summary>
        /// Saves a Lock Token
        /// </summary>
        /// <param name="FolderID">The ID of the Lock in the Database</param>
        /// <returns></returns>   
        public static int SaveLockToken(Locks_TokensDS.Locks_TokensRow nlr)
        {
          return FileBLC.SaveLockToken(nlr);

        }
        /// <summary>
        /// Retrieves all of the LockTokens for a given Lock ID
        /// </summary>
        /// <param name="FolderID">The ID of the Parent Folder in the Database</param>
        /// <returns></returns>   
        public static Locks_TokensDS getLockTokens(int LockID)
        {
            return FileBLC.getLockTokens(LockID);
        }
        /// <summary>
        /// Retrieves the LockDS.LocksRow for a given Token
        /// </summary>
        /// <param name="FolderID">The ID of the Parent Folder in the Database</param>
        /// <returns></returns>   
        public static LocksDS.LocksRow getFileLockbyToken(string Token)
        {
            return FileBLC.getFileLockbyToken(Token);
        }

        /// <summary>
        /// HttpRequest Length
        /// </summary>
        /// <param name="httpApplication"></param>
        public static long getRequestLength(HttpApplication httpApplication)
        {

            if (httpApplication == null)
                throw new ArgumentNullException("HttpApplication", "Cannot find handle to HTTP Application");

            return httpApplication.Request.InputStream.Length;

        }

        /// <summary>
        /// WebDav Requested Depth
        /// </summary>
        /// <param name="httpApplication"></param>
        public static DepthType getRequestDepth(HttpApplication httpApplication)
        {
            DepthType _depth = DepthType.Infinity;

            if (httpApplication == null)
                throw new ArgumentNullException("HttpApplication", "Cannot find handle to HTTP Application");

            try
            {
                _depth = (DepthType)Enum.Parse(typeof(DepthType), httpApplication.Request.Headers["Depth"], true);
            }
            catch (ArgumentException) { }
            return _depth;

        }

        /// <summary>
        /// WebDav Parse Lock Token If Header
        /// </summary>
        /// <param name="inputString">This should be the HttpApplication.Request.Headers["If"]</param>
        /// <returns>The Token string if it exists</returns>

        public static string ParseOpaqueLockToken(string inputString)
        {
            string _opaqueLockToken = "";

            if (inputString != null)
            {
                string _prefixTag = "<opaquelocktoken:";
                int _prefixIndex = inputString.IndexOf(_prefixTag);
                if (_prefixIndex != -1)
                {
                    int _endIndex = inputString.IndexOf('>', _prefixIndex);
                    if (_endIndex > _prefixIndex)
                        _opaqueLockToken = inputString.Substring(_prefixIndex + _prefixTag.Length, _endIndex - (_prefixIndex + _prefixTag.Length));
                }
            }

            return _opaqueLockToken;
        }

        /// <summary>
        /// Gets the NonPathPart of a URI/URL
        /// </summary>
        /// <param name="httpApplication"></param>
        public static string getNonPathPart(HttpApplication httpApplication)
        {
            if (httpApplication == null)
                throw new ArgumentNullException("HttpApplication", "Cannot find handle to HTTP Application");

            string _completePath = httpApplication.Request.Url.AbsoluteUri;
            string _relativePath = httpApplication.Request.Url.AbsolutePath;

            return _completePath.Substring(0, _completePath.Length - _relativePath.Length);
        }


        /// <summary>
        /// Function for obtaining a URIPath's relative path (Removes the NonPathPart)
        /// </summary>
        /// <param name="httpApplication"></param>
        /// <param name="URIPath"></param>
        /// <returns></returns>
        public static string getRelativePath(HttpApplication httpApplication, string URIPath)
        {
            if (httpApplication == null)
                throw new ArgumentNullException("HttpApplication", "Cannot find handle to HTTP Application");

            string _nonPathPart = getNonPathPart(httpApplication);

            string _retValue;
            if (URIPath.ToLower().StartsWith(_nonPathPart.ToLower()))
                _retValue = URIPath.Remove(0, _nonPathPart.Length);
            else
                _retValue = URIPath;

            //Remove the application path
            string _appPath = httpApplication.Request.ApplicationPath;
            if (_retValue.ToLower().StartsWith(_appPath.ToLower()))
                _retValue = _retValue.Remove(0, _appPath.Length);

            return HttpUtility.UrlDecode(_retValue.Trim('/'));
        }



        /// <summary>
        /// Parse the Timeout header of a lock request to determine if a timeout was specified.
        /// </summary>
        /// <param name="httpApplication"></param>
        /// <returns></returns>
        public static int ParseTimeoutHeader(HttpApplication httpApplication, int defaulttimeout)
        {
            int _lockTimeout = defaulttimeout;

            if (httpApplication.Request.Headers["Timeout"] != null)
            {
                //Parse the Timeout lock request
                string _timeoutHeader = httpApplication.Request.Headers["Timeout"];
                string[] _timeoutInfo = _timeoutHeader.Split('-');

                //There should only be 2 segments
                if (_timeoutInfo.Length == 2)
                {
                    try
                    {
                        _lockTimeout = Convert.ToInt32(_timeoutInfo[1], CultureInfo.InvariantCulture);
                    }
                    catch (InvalidCastException)
                    {
                        return defaulttimeout;
                    }
                }
            }
            //Do not allow the client to override the maximum timeout
            if (_lockTimeout > defaulttimeout)
                    _lockTimeout = defaulttimeout;

                return _lockTimeout;
        }

        
        /// <summary>
        /// Returns the string version of a http status response code
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public static string getEnumHttpResponse(Enum statusCode)
        {
            string _httpResponse = "";

            switch (getEnumValue(statusCode))
            {
                case 200:
                    _httpResponse = "HTTP/1.1 200 OK";
                    break;

                case 404:
                    _httpResponse = "HTTP/1.1 404 Not Found";
                    break;

                case 423:
                    _httpResponse = "HTTP/1.1 423 Locked";
                    break;

                case 424:
                    _httpResponse = "HTTP/1.1 424 Failed Dependency";
                    break;

                case 507:
                    _httpResponse = "HTTP/1.1 507 Insufficient Storage";
                    break;

                default:
                    break;
            }

            return _httpResponse;
        }

        public static string getHtmlErrorMsg(Enum statusCode)
        {
            return getHtmlErrorMsg(getEnumHttpResponse(statusCode));
        }

        public static string getHtmlErrorMsg(string msg)
        {

            string _retval = "<HTML><HEAD>";
            _retval += "<TITLE>Resource Error</TITLE>\r\n";
            _retval += "\r\n<STYLE>.Error{font-size:9pt;font-family:'trebuchet ms',helvetica,sans-serif;}</STYLE>\r\n";
            _retval += String.Format("</HEAD>\r\n<BODY>\r\n<H3 class=\"Error\">{0}</h3>\r\n", msg);
            _retval += "</BODY></HTML>";
            return _retval;
        }

        public static string getHtmlFolderMsg()
        {
            string msg = "This is a WEBDAV server, please connect using a WEBDAV client, such as Windows Explorer, that understand how to issue a http PROPFIND method<BR/>Directory browsing via http GET is not allowed";
            string _retval = "<HTML><HEAD>";
            _retval += "<TITLE>WebDav Server</TITLE>\r\n";
            _retval += "\r\n<STYLE>.Error{font-size:9pt;font-family:'trebuchet ms',helvetica,sans-serif;}</STYLE>\r\n";
            _retval += String.Format("</HEAD>\r\n<BODY>\r\n<H3 class=\"Error\">{0}</h3>\r\n", msg);
            _retval += "</BODY></HTML>";
            return _retval;

        }
        public static bool ValidateEnumType(Enum enumToValidate)
        {
            if (enumToValidate.GetTypeCode() != TypeCode.Int32)
                throw new Exception("Invalid Enum Type");

            return true;
        }
        public static int getEnumValue(Enum statusCode)
        {
            int _enumValue = 0;
            if (ValidateEnumType(statusCode))
                _enumValue = (int)System.Enum.Parse(statusCode.GetType(), statusCode.ToString(), true);

            return _enumValue;
        }

        public static string StreamtoString(Stream stream)
        {
            string _retval = "";
            using (StreamReader _streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                //Go to the begining of the stream
                _streamReader.BaseStream.Position = 0;
                _retval = _streamReader.ReadToEnd();
            }
            return _retval;
        }


    }
}
