using System;
using System.Text;
using System.Web;
using System.IO;
using Microsoft.Win32;
using HojuWebDisk.DataEntities;
using HojuWebDisk.BLC;

namespace HojuWebDisk.WebDavServer.MethodHandlers
{
    public class PUT_Handler : IMethod_Handler
    {
        private HttpApplication _httpApplication;

        private string _requestPath;

        public PUT_Handler(HttpApplication HttpApplication)
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
                return "";
            }
        }
        public int Handle()
        {
            if (_httpApplication == null)
                return (int)ServerResponseCode.BadRequest;

            if (WebDavHelper.getRequestLength(_httpApplication) == 0)
                return (int)DavPutResponseCode.Created;

            this._requestPath = WebDavHelper.getRelativePath(this._httpApplication, this._httpApplication.Request.FilePath);

           FoldersDS.FoldersRow _dirInfo = WebDavHelper.getParentFolder(this._requestPath);
			//The parent folder does not exist
			if (_dirInfo == null)
				return (int)ServerResponseCode.NotFound;
			else {
				if (!OverwriteExistingResource()) {
					//Check to see if the resource already exists
                    if (WebDavHelper.getFileAttribsOnly(this._requestPath) != null)
                        return (int)DavPutResponseCode.Conflict;
                    else
                        if (SaveFile(_dirInfo.ID)==-1)
                            return (int)DavPutResponseCode.InsufficientStorage;
				}
				else
                    if (SaveFile(_dirInfo.ID)==-1)
                        return (int)DavPutResponseCode.InsufficientStorage;
			}
            return (int)DavPutResponseCode.Created;

        }

       #endregion

        #region private handler methods
        private bool OverwriteExistingResource()
        {
              if (this._httpApplication.Request.Headers["If-None-Match"] != null)
                    return false;

                return true;
        }
        private byte[] GetRequestInput()
        {
            StreamReader _inputStream = new StreamReader(_httpApplication.Request.InputStream);

            long _inputSize = _inputStream.BaseStream.Length;

            byte[] _inputBytes = new byte[_inputSize];
            _inputStream.BaseStream.Read(_inputBytes, 0, (int)_inputSize);
            return _inputBytes;
        }
        private int SaveFile(int PID)
        {
            byte[] _requestInput = GetRequestInput();

            FilesDS.FilesRow _newFile = WebDavHelper.getFile(this._requestPath);

            if (_newFile == null)
            {
                FilesDS fds = new FilesDS();
                _newFile = fds.Files.NewFilesRow();
                _newFile.FileData = _requestInput;
                _newFile.FileDataSize = _requestInput.LongLength;
                _newFile.ParentID = PID;
                _newFile.FileName = WebDavHelper.getResourceName(this._requestPath);
                _newFile.ContentType = GetMIMEType(_newFile.FileName);
                _newFile.update_user_stamp = HttpContext.Current.User.Identity.Name;

            }
            else
            {
                _newFile.FileData = _requestInput;
                _newFile.FileDataSize = _requestInput.LongLength;
            }

            return WebDavHelper.SaveFile(_newFile);
        }
        private string GetMIMEType(string fileName)
        {
            if (fileName.Contains("."))
            {
                string[] dot = fileName.Split(@".".ToCharArray());

                if (dot[dot.Length - 1] == "exe") return "application/octet-stream";

                RegistryKey rkContentTypes = Registry.ClassesRoot.OpenSubKey("." + dot[dot.Length - 1]);
                if (rkContentTypes != null)
                {
                    object key = rkContentTypes.GetValue("Content Type", "binary/octet-stream");
                    return key.ToString();
                }
                else
                {
                    return "binary/octet-stream";

                }
            }
            else
            {
                return "binary/octet-stream";
            }
        }
        #endregion

    }
    }