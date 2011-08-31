//  ============================================================================
//  AUTHOR		 : Simon
//  CREATE DATE	 : 3 May 2007
//  PURPOSE		 : 
//  Handles first call from IHttpModule, checks the METHOD and hands of to the relevant method handler
//  SPECIAL NOTES: 
//  Version control and PROPPATCH are not implemented in this version as neither Windows Explorer or office support
//  these under WEBDAV. Please see summary messages for each method handler for information on how each method is handled.
//  (
//  ===========================================================================

using System;
using System.Web;
using System.Collections.Generic;
using System.Text;
using System.IO;

using HojuWebDisk.WebDavServer.MethodHandlers;

namespace HojuWebDisk.WebDavServer
{
    public class WebDavHandler
    {
        public WebDavHandler() { }

        public static void HandleRequest(HttpApplication httpApplication)
        {
            //Set the status code to NotImplemented by default
            int _statusCode = 501;
            string _responseXml="";
            string _requestXml="";
            string _ErrorXml = "";
            string _httpMethod="";

            IMethod_Handler _MethodHandler = null;

            if (httpApplication == null)
                throw new ArgumentNullException("httpApplication", "No Handle to HttpApplication Found");

            try
            {
                //Don't handle anything with the _MWDRes query string, this means that this is a request
                //for a local resource and should not be handled by this Handler. It should fall through
                //to the asp .net engine to handle.
                if (httpApplication.Request.QueryString["_MWDRes"] != null)
                    return;

                //Make sure we don't handle anything from the IDE.
                if (httpApplication.Request.Headers["User-Agent"] != null && !httpApplication.Request.Headers["User-Agent"].StartsWith("Microsoft-Visual-Studio.NET"))
                {

                    _httpMethod = httpApplication.Request.HttpMethod.ToUpper();

                    switch (_httpMethod)
                    {

                        case "OPTIONS":
                            _MethodHandler = new OPTIONS_Handler(httpApplication);
                            break;
                        case "MKCOL":
                            _MethodHandler = new MKCOL_Handler(httpApplication);
                            break;
                        case "PROPFIND":
                            _MethodHandler = new PROPFIND_Handler(httpApplication);
                            _requestXml = _MethodHandler.RequestXml;
                            break;
                        case "HEAD":
                            _MethodHandler = new HEAD_Handler(httpApplication);
                            break;
                        case "DELETE":
                            _MethodHandler = new DELETE_Handler(httpApplication);
                            _requestXml = _MethodHandler.RequestXml;
                            break;
                        case "MOVE":
                            _MethodHandler = new MOVE_Handler(httpApplication);
                             break;
                        case "COPY":
                            _MethodHandler = new COPY_Handler(httpApplication);
                             break;
                        case "PUT":
                            _MethodHandler = new PUT_Handler(httpApplication);
                            break;
                        case "GET":
                            _MethodHandler = new GET_Handler(httpApplication);
                            break;
                        case "LOCK":
                            _MethodHandler = new LOCK_Handler(httpApplication);
                            break;
                        case "UNLOCK":
                            _MethodHandler = new UNLOCK_Handler(httpApplication);
                            break;
                        case "PROPPATCH":
                            
                            break;
                        default:
                            _statusCode = (int)ServerResponseCode.MethodNotImplemented;

                            break;

                    }
                    if (_MethodHandler != null)
                    {
                         _statusCode = _MethodHandler.Handle();
                        _responseXml = _MethodHandler.ResponseXml;
                        _ErrorXml = _MethodHandler.ErrorXml;
                    }
                }
            }
            catch (Exception ex)
            {
                httpApplication.Response.StatusCode = (int)ServerResponseCode.BadRequest;
                httpApplication.Response.Write(WebDavHelper.getHtmlErrorMsg(ex.Message));
                httpApplication.Response.ContentType = "text/html";
                httpApplication.Response.End();
                return;

            }

            if (_httpMethod == "GET" && _ErrorXml != "")
            {
                //this is a request for a folder that has been redirected through pass-through so handle the redirection
                httpApplication.Response.Redirect(_ErrorXml, true);
                return;
            }

            httpApplication.Response.StatusCode = _statusCode;

            if (_ErrorXml.Length != 0)
            {
                httpApplication.Response.StatusCode = (int)ServerResponseCode.MultiStatus;
                httpApplication.Response.ContentEncoding = System.Text.Encoding.UTF8;
                httpApplication.Response.ContentType = "text/xml";
                httpApplication.Response.Write(_ErrorXml);
            }
            else
            {
                if (_responseXml.Length != 0)
                {
                    httpApplication.Response.ContentEncoding = System.Text.Encoding.UTF8;
                    httpApplication.Response.ContentType = "text/xml";
                    httpApplication.Response.Write(_responseXml);
                }
            }
           //For debugging
           /* using (System.IO.StreamWriter _file = File.AppendText(@"C:\davOutput.txt"))
            {

                _file.Write(_httpMethod + "\r\n--------------------\r\n");
                _file.WriteLine();
                _file.Write(_requestXml);
                _file.WriteLine();
                if (_ErrorXml.Length!=0) 
                    _file.Write(_responseXml);
                if (_responseXml.Length!=0) 
                    _file.Write(_responseXml);
                _file.WriteLine();
                _file.Close();
            }
            */
            httpApplication.Response.End();

        }
    }
}
