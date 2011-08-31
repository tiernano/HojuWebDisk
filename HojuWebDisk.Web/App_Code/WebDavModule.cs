using System;
using System.Web;
using System.Text;

using HojuWebDisk.WebDavServer;

/// <summary>
/// Summary description for WebDavModule
/// </summary>

    public class WebDavModule:IHttpModule
    {

		public void Dispose() { }

		public void Init(HttpApplication httpApp) {
			//TODO: add digest / basic authentication capabilities
			httpApp.AuthenticateRequest += new EventHandler(httpApp_AuthenticateRequest);
            
		}

		private void httpApp_AuthenticateRequest(object sender, EventArgs e) {
			
            HttpApplication _httpApplication = (HttpApplication)sender;
            WebDavHandler.HandleRequest(_httpApplication);
		}
	}

    

