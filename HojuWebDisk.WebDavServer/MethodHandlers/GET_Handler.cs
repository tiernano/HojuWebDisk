using System;
using System.Text;
using System.Web;
using System.IO;

using HojuWebDisk.DataEntities;
using HojuWebDisk.BLC;

namespace HojuWebDisk.WebDavServer.MethodHandlers
{
    public class GET_Handler:IMethod_Handler
    {
        private HttpApplication _httpApplication;
        private string _requestPath;
        private string _errorxml = "";

        private HttpCacheability _responseCache = HttpCacheability.NoCache;

        public GET_Handler(HttpApplication HttpApplication)
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
                return _errorxml;
            }
        }
        public int Handle()
        {
            if (_httpApplication==null)
                return (int)ServerResponseCode.BadRequest;

            this._requestPath = WebDavHelper.getRelativePath(this._httpApplication, this._httpApplication.Request.FilePath);

            FoldersDS.FoldersRow _FolderInfo = WebDavHelper.getFolder(_requestPath);
            FilesDS.FilesRow _fileInfo = WebDavHelper.getFile(_requestPath);
            string _UserAgent = this._httpApplication.Request.Headers["User-Agent"];

			if (_fileInfo == null){


                if (_FolderInfo == null)
                {
                    
                    {
                          _httpApplication.Response.ContentType = "text/html";
                           _httpApplication.Response.Write(WebDavHelper.getHtmlErrorMsg(ServerResponseCode.NotFound));
                    }
                    
                }
                else
                {
                    //redirect the Get request to the pass-through or this folder. The _MWDRes will make sure the 
                    //redirected page will be handled by the ASP engine not this handler.
                    _errorxml = _httpApplication.Request.ApplicationPath + "/Folder.aspx?FPath=" + HttpUtility.UrlEncode(this._requestPath) + "&_MWDRes=1";
                    if (_errorxml.Contains("//"))
                    {
                        _errorxml = _errorxml.Replace("//", "/"); //HACK!!!
                    }
                    
                }
                return (int)ServerResponseCode.NotFound;
			}else {
				
                _httpApplication.Response.Cache.SetCacheability(_responseCache);
                _httpApplication.Response.ContentType = _fileInfo.ContentType;
                _httpApplication.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + HttpUtility.HtmlEncode(_fileInfo.FileName) + "\"");
                _httpApplication.Response.AddHeader("Content-Length", _fileInfo.FileDataSize.ToString());
             
            
                using (BinaryWriter _outputStream = new BinaryWriter(_httpApplication.Response.OutputStream))
                {
                    _outputStream.Write(_fileInfo.FileData);
                    _outputStream.Close();
                }

			}
            return (int)ServerResponseCode.Ok;
            
        }
		
        #endregion
    }
}
