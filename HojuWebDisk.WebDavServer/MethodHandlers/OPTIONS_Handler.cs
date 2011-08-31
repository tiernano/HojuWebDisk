using System;
using System.Web;
using System.Text;

namespace HojuWebDisk.WebDavServer.MethodHandlers
{
    public class OPTIONS_Handler : IMethod_Handler
    {
        private HttpApplication _httpApplication;

        public OPTIONS_Handler(HttpApplication HttpApplication)
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
            if (_httpApplication==null)
                return (int)ServerResponseCode.BadRequest;

            _httpApplication.Response.AppendHeader("DAV", "1,2");
            _httpApplication.Response.AppendHeader("MS-Author-Via", "DAV");
            _httpApplication.Response.AppendHeader("Versioning-Support", "DAV:basicversioning");
            _httpApplication.Response.AppendHeader("DASL", "<DAV:sql>");
            _httpApplication.Response.AppendHeader("Public", "COPY, DELETE, GET, HEAD, LOCK, MKCOL, MOVE, OPTIONS, PROPFIND, PROPPATCH, PUT, UNLOCK, REPORT, VERSION-CONTROL, CHECKOUT, CHECKIN, UNCHECKOUT");
            _httpApplication.Response.AppendHeader("Allow", "COPY, DELETE, GET, HEAD, LOCK, MKCOL, MOVE, OPTIONS, PROPFIND, PROPPATCH, PUT, UNLOCK, REPORT,  VERSION-CONTROL, CHECKOUT, CHECKIN, UNCHECKOUT");
            
            return (int)ServerResponseCode.Ok;
        }
        #endregion
    }


}
