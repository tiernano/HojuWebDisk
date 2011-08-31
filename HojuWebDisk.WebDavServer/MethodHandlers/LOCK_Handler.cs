//  ============================================================================
//  AUTHOR		 : Simon
//  CREATE DATE	 : 3 May 2007
//  PURPOSE		 : 
//  Handles a WebDav LOCK request for a resource.
//  SPECIAL NOTES: 
//  I have only implemented LOCK for resources not for collections. This is because this is meant as a 
//  server for windows explorer and Office, and neither of these clients support collection locking.
//  This also makes LOCK Management far more easy, as there is no LOCK heirarchy to manage.
//  
//  Currently there is no support for Shared Locks a Office doesn't request them. In the future I will
//  test the read-only functionality to see what can be incorporated.

//  (
//  ===========================================================================

using System;
using System.Collections.Generic;
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
    public class LOCK_Handler : IMethod_Handler
    {

        private HttpApplication _httpApplication;
       
        private string _requestPath;
        
        private string _responseXml = "";
        private string _LockToken="";
        //Default Lock of 3 minutes.
        private int _LockTimeOut = 180;
        //Default Lock grace period.. Allows for bottlenecking of processing of token refresh
        private int _GraceLockTimeOut = 10; //Seconds

        private LockType _LockType;
        private LockScope _LockScope;
        private LockOwnerType _LockOwnerType;
        private DepthType _LockDepth;
        private string _LockOwner="";

        private XPathNavigator _requestXmlNavigator = null;

        public LOCK_Handler(HttpApplication httpApplication)
        {

            _httpApplication = httpApplication;
            if (_httpApplication.Request.InputStream.Length != 0)
            {
                _requestXmlNavigator = new XPathDocument(_httpApplication.Request.InputStream).CreateNavigator();
            }
        }

        #region IMethod_Handler Interface

        public string RequestXml
        {
            get
            {
                if (_requestXmlNavigator == null) return "";
                return _requestXmlNavigator.InnerXml;
            }
        }
        public string ResponseXml
        {
            get
            {
                return _responseXml;
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
                this._LockDepth = WebDavHelper.getRequestDepth(this._httpApplication);
                //No support for ResourceChildren LOCKing
                if ( this._LockDepth == DepthType.ResourceChildren)
                    return  (int)DavLockResponseCode.BadRequest;
                    
                this._requestPath = WebDavHelper.getRelativePath(this._httpApplication, this._httpApplication.Request.FilePath);

                //Check to see if this is a lock refresh request.
                if (this._httpApplication.Request.Headers["If"] != null)
                {
                    _LockToken = WebDavHelper.ParseOpaqueLockToken(_httpApplication.Request.Headers["If"]);
                    _LockTimeOut = WebDavHelper.ParseTimeoutHeader(_httpApplication,this._LockTimeOut);
                    
                    //Check to see that the lock exists on the requested resource.

                    LocksDS.LocksRow ltr = WebDavHelper.getFileLockbyToken(_LockToken);
                    if (ltr==null)
                           return (int)DavLockResponseCode.PreconditionFailed;

                    //Check if the lock is expired , include token grace timeout in calculation
                    TimeSpan span = DateTime.Now.Subtract(ltr.update_date_stamp);
                    if (span.TotalSeconds > ltr.Timeout + this._GraceLockTimeOut)
                    {
                        //the Lock has expired so delete it an return as if it did not exist
                        WebDavHelper.RemoveLock(ltr.ID);
                        return (int)DavLockResponseCode.PreconditionFailed;
                    }

                    //the lock exists and is not expired so update the timeout and update_date_stamp
                    ltr.Timeout = _LockTimeOut;
                    WebDavHelper.SaveLock(ltr);



                    //Send timeout header back to client
                    _httpApplication.Response.AppendHeader("Timeout", "Second-" + this._LockTimeOut.ToString());
                    //Deserialise the lock token in the DB to get the rest of the data
                    DeserialiseLock(ltr);
                    BuildResponse();
                    return (int)DavLockResponseCode.Ok;
                
                }else{
                    //This is not a refresh it is a new LOCK request. So check that it is valid
                    
                    //Check to see if the resource exists
                    
                     Search_FilesDS.FilesRow _fileInfo = WebDavHelper.getFileAttribsOnly(this._requestPath);
                     if (_fileInfo == null)
                         return (int)DavLockResponseCode.BadRequest;
                      
                    //Need to workout how to resolve this problem where office attempts to lock a resource
                    //it knows does not exist.
                           

                     //Check that it is not already locked
                     LocksDS.LocksRow ltr = WebDavHelper.getLock(_fileInfo.ID);
                     if (ltr != null)
                     {
                         //Check if the lock is expired , include token grace timeout in calculation
                         TimeSpan span = DateTime.Now.Subtract(ltr.update_date_stamp);
                         if (span.TotalSeconds > ltr.Timeout + this._GraceLockTimeOut)
                         {
                             //the Lock has expired so delete it an return as if it did not exist
                             WebDavHelper.RemoveLock(ltr.ID);
                         }
                         else
                         {
                             return (int)DavLockResponseCode.Locked;
                         }
                     }
                    //Check that the request XML is valid for the LOCK request
                     if (_requestXmlNavigator == null)
                        return (int)ServerResponseCode.BadRequest;

                    //Load the valid properties
                    XPathNodeIterator _lockInfoNodeIterator =_requestXmlNavigator.SelectDescendants("lockinfo", "DAV:", false);
                    if (!_lockInfoNodeIterator.MoveNext())
                        return (int)ServerResponseCode.BadRequest;

                    //Create a new Lock
                    this._LockToken = System.Guid.NewGuid().ToString("D");
                    this._LockTimeOut = WebDavHelper.ParseTimeoutHeader(_httpApplication,this._LockTimeOut);     
     
                    //Get the lock type
                    _LockType = LockType.Write;
                    XPathNodeIterator _lockTypeNodeIterator = _lockInfoNodeIterator.Current.SelectDescendants("locktype", "DAV:", false);
                    if (_lockTypeNodeIterator.MoveNext())
                        {
                            XPathNavigator _currentNode = _lockTypeNodeIterator.Current;
                            if (_currentNode.MoveToFirstChild())
                            {
                                switch (_currentNode.LocalName.ToLower(CultureInfo.InvariantCulture))
                                {
                                    case "read":
                                        _LockType = LockType.Read;
                                        break;

                                    case "write":
                                        _LockType = LockType.Write;
                                        break;
                                }
                            }
                        }
                    //Get the lock scope
                    _LockScope = LockScope.Exclusive;
                    XPathNodeIterator _lockScopeNodeIterator = _lockInfoNodeIterator.Current.SelectDescendants("lockscope", "DAV:", false);
                    if (_lockScopeNodeIterator.MoveNext())
                    {
                        XPathNavigator _currentNode = _lockScopeNodeIterator.Current;
                        if (_currentNode.MoveToFirstChild())
                        {
                            switch (_currentNode.LocalName.ToLower(CultureInfo.InvariantCulture))
                            {
                                case "shared":
                                    _LockScope = LockScope.Shared;
                                    break;

                                case "exclusive":
                                    _LockScope = LockScope.Exclusive;
                                    break;
                            }
                        }
                    }

                    //Get the lock owner
                    _LockOwnerType = LockOwnerType.User;
                    XPathNodeIterator _lockOwnerNodeIterator = _lockInfoNodeIterator.Current.SelectDescendants("owner", "DAV:", false);
                    if (_lockOwnerNodeIterator.MoveNext())
                    {
                        XPathNavigator _currentNode = _lockOwnerNodeIterator.Current;

                        if (_currentNode.NodeType == XPathNodeType.Text)
                        {
                            _LockOwnerType = LockOwnerType.User;
                        }
                        else
                        {
                            if (_currentNode.MoveToFirstChild())
                            {
                                //TODO: Expand this to other LockOwnerTypes

                                switch (_currentNode.LocalName.ToLower(CultureInfo.InvariantCulture))
                                {
                                    case "href":
                                        _LockOwnerType = LockOwnerType.Href;
                                    break;
                                }
                            }
                        }

                        _LockOwner = _currentNode.Value;
                    }
                    //Now save the Lock to the DB;
                    saveLock(_fileInfo.ID);
                    BuildResponse();
                    
                }
            
            return (int)DavLockResponseCode.Ok;
        }
         

        #endregion

        #region Private Handler Methods
        private int saveLock(int FileID){

            int retval = 1;
            Locks_TokensDS ltds = new Locks_TokensDS();
            LocksDS lds = new LocksDS();

            LocksDS.LocksRow ltr = lds.Locks.NewLocksRow();

            //ResType=0 as we aren't supporting Locked Collections

            ltr.ResType = 0;

            ltr.LockDepth = (int)this._LockDepth;
            ltr.LockOwner = _LockOwner;
            ltr.LockOwnerType = (int)_LockOwnerType;
            ltr.LockScope = (int)_LockScope;
            ltr.LockType = (int)_LockType;
            ltr.ResID = FileID;
            ltr.Timeout = _LockTimeOut;
            ltr.update_user_stamp = HttpContext.Current.User.Identity.Name;

            retval = WebDavHelper.SaveLock(ltr);

            if (retval!=-1)
            {
                Locks_TokensDS.Locks_TokensRow nltr = ltds.Locks_Tokens.NewLocks_TokensRow();
                nltr.Token = this._LockToken;
                nltr.LockID = retval;
                nltr.update_user_stamp = HttpContext.Current.User.Identity.Name;
                retval = WebDavHelper.SaveLockToken(nltr);
            }
            return retval;
        }
        private void DeserialiseLock(LocksDS.LocksRow _lockrow){

            this._LockOwner = _lockrow.LockOwner;
            this._LockDepth = (DepthType)_lockrow.LockDepth;
            this._LockOwnerType = (LockOwnerType)_lockrow.LockOwnerType;
            this._LockType = (LockType)_lockrow.LockType;
            this._LockScope = (LockScope)_lockrow.LockScope;
            
        }
        private void BuildResponse()
        {
            using (Stream _responseStream = new MemoryStream())
            {

                XmlTextWriter _xmlWriter = new XmlTextWriter(_responseStream, Encoding.UTF8);

                _xmlWriter.Formatting = Formatting.Indented;
                _xmlWriter.IndentChar = '\t';
                _xmlWriter.Indentation = 1;
                _xmlWriter.WriteStartDocument();

                //Open the prop element section
                _xmlWriter.WriteStartElement("prop", "DAV:");
                _xmlWriter.WriteStartElement("lockdiscovery", "DAV:");

                _xmlWriter.WriteStartElement("activelock", "DAV:");
              
                _xmlWriter.WriteStartElement("locktype", "DAV:");
               
				switch (this._LockType) {
					case LockType.Read:
                        _xmlWriter.WriteElementString("read", "DAV:", "");
						break;

					case LockType.Write:
						_xmlWriter.WriteElementString("write", "DAV:", "");
						break;
				}

                _xmlWriter.WriteEndElement();

                _xmlWriter.WriteStartElement("lockscope", "DAV:");
				
				switch (this._LockScope) {
					case LockScope.Exclusive:
                        _xmlWriter.WriteElementString("exclusive", "DAV:", "");
						
						break;

					case LockScope.Shared:
						_xmlWriter.WriteElementString("shared", "DAV:", "");
						break;
				}
                 _xmlWriter.WriteEndElement();


				//Append the depth
				if (this._LockDepth == DepthType.Infinity) {
                    _xmlWriter.WriteElementString("depth", "DAV:", this._LockDepth.ToString());
                }else{
                    if (this._LockDepth == DepthType.ResourceOnly)
                     _xmlWriter.WriteElementString("depth", "DAV:", "0");
                    else
                    _xmlWriter.WriteElementString("depth", "DAV:", "1");
                }
               	

				//Append the owner
				
				switch (this._LockOwnerType) {
					case LockOwnerType.User:
						_xmlWriter.WriteElementString("owner", "DAV:", this._LockOwner);
						break;
					case LockOwnerType.Href:
                        _xmlWriter.WriteStartElement("owner", "DAV:");
                        _xmlWriter.WriteElementString("href", "DAV:", this._LockOwner);
                        _xmlWriter.WriteEndElement();
						break;
				}

				//Append the timeout
                _xmlWriter.WriteElementString("timeout", "DAV:", "Second-" + this._LockTimeOut.ToString());

				//Append the lockToken
                _xmlWriter.WriteStartElement("locktoken", "DAV:");
                _xmlWriter.WriteElementString("href", "DAV:", "opaquelocktoken:" + this._LockToken);
                _xmlWriter.WriteEndElement();
                
                //close activelock
                _xmlWriter.WriteEndElement();
                //close lockdiscovery
                _xmlWriter.WriteEndElement();

                _xmlWriter.WriteEndDocument();
                _xmlWriter.Flush();
                
                this._responseXml = WebDavHelper.StreamtoString(_responseStream);
                _xmlWriter.Close();

            }

          
        }
        #endregion



    }
}
