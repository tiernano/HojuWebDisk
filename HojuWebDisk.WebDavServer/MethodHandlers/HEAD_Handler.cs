using System;
using System.Text;
using System.Web;
using System.IO;

using HojuWebDisk.DataEntities;
using HojuWebDisk.BLC;

namespace HojuWebDisk.WebDavServer.MethodHandlers
{
    public class HEAD_Handler : IMethod_Handler
    {
        private HttpApplication _httpApplication;
        private string _requestPath;

       
        public HEAD_Handler(HttpApplication HttpApplication)
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

            this._requestPath = WebDavHelper.getRelativePath(this._httpApplication, this._httpApplication.Request.FilePath);

            if (!(WebDavHelper.ValidResourceByPath(this._requestPath)))
                return (int)ServerResponseCode.NotFound;

            return (int)ServerResponseCode.Ok;

        }

        #endregion
    }
}

