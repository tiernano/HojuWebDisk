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
    public class PROPFIND_Handler : IMethod_Handler
    {

        private HttpApplication _httpApplication;
        private PropertyRequestType _requestPropertyType;
        private RequestedPropertyCollection _requestedProperties;

        private HttpCacheability _responseCache = HttpCacheability.NoCache;

        private string _requestPath;
        private int _httpResponseCode = (int)ServerResponseCode.Ok;
        private string _responseXml = "";
        private XPathNavigator _requestXmlNavigator = null;
           
        public PROPFIND_Handler(HttpApplication httpApplication) {

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
            _httpResponseCode = GetRequestType();
            if (_httpResponseCode == (int)ServerResponseCode.Ok)
            {
                if (this._requestPropertyType == PropertyRequestType.NamedProperties)
                           this._requestedProperties = getRequestedProps();
                                
                this._requestPath = WebDavHelper.getRelativePath(this._httpApplication, this._httpApplication.Request.FilePath);
                _httpApplication.Response.Cache.SetCacheability(_responseCache);               
                _httpResponseCode = BuildResponse();

                       					
            }
            return _httpResponseCode;

        }
       
        #endregion
        
        #region Private Handler Methods
        private int GetRequestType()
        {
            int _returnCode = (int)ServerResponseCode.Ok;

            //NOTE: An empty PROPFIND request body MUST be treated as a request for the names 
            //	and values of all properties.



            if (_requestXmlNavigator == null)
                this._requestPropertyType = PropertyRequestType.AllProperties;

            else
            {
                XPathNodeIterator _propFindNodeIterator = _requestXmlNavigator.SelectDescendants("propfind", "DAV:", false);
                if (_propFindNodeIterator.MoveNext())
                {
                    if (_propFindNodeIterator.Current.MoveToFirstChild())
                    {
                        switch (_propFindNodeIterator.Current.LocalName.ToLower(CultureInfo.InvariantCulture))
                        {
                            case "propnames":
                                this._requestPropertyType = PropertyRequestType.PropertyNames;
                                break;
                            case "allprop":
                                this._requestPropertyType = PropertyRequestType.AllProperties;
                                break;
                            default:
                                this._requestPropertyType = PropertyRequestType.NamedProperties;
                                break;
                        }
                    }
                    else
                        _returnCode = (int)ServerResponseCode.BadRequest;
                }
                else
                    _returnCode = (int)ServerResponseCode.BadRequest;
            }

            return _returnCode;
        }
        private RequestedPropertyCollection getRequestedProps()
        {
            RequestedPropertyCollection _davProperties = new RequestedPropertyCollection();

            if (_requestXmlNavigator != null)
            {
                XPathNodeIterator _propNodeIterator = _requestXmlNavigator.SelectDescendants("prop", "DAV:", false);
                if (_propNodeIterator.MoveNext())
                {
                    XPathNodeIterator _nodeChildren = _propNodeIterator.Current.SelectChildren(XPathNodeType.All);
                    while (_nodeChildren.MoveNext())
                    {
                        XPathNavigator _currentNode = _nodeChildren.Current;

                        if (_currentNode.NodeType == XPathNodeType.Element)

                            _davProperties.Add(new RequestedProperty(_currentNode.LocalName, _currentNode.NamespaceURI));
                    }
                }
            }
            if (_davProperties.Count==0)
                    return null;

            return _davProperties;
        }
        private int BuildResponse()
        {
                using (Stream _responseStream = new MemoryStream()){
            
                XmlTextWriter _xmlWriter = new XmlTextWriter(_responseStream, Encoding.UTF8);

                _xmlWriter.Formatting = Formatting.Indented;
                _xmlWriter.IndentChar = '\t';
                _xmlWriter.Indentation = 1;
                _xmlWriter.WriteStartDocument();

                //Set the Multistatus
                _xmlWriter.WriteStartElement("D", "multistatus", "DAV:");
                _xmlWriter.WriteAttributeString("xmlns:b", "urn:uuid:c2f41010-65b3-11d1-a29f-00aa00c14882");
                _xmlWriter.WriteAttributeString("xmlns:c", "urn:schemas-microsoft-com:office:office");

              FoldersDS.FoldersRow _dirInfo = WebDavHelper.getFolder(this._requestPath);

              if (_dirInfo == null)
              {
                  Search_FilesDS.FilesRow _fileInfo = WebDavHelper.getFileAttribsOnly(this._requestPath);

                  if (_fileInfo != null)
                  {
                      XMLWebDavFile.GetXML(_fileInfo, _xmlWriter, this._requestedProperties, this._requestPropertyType);
                  }
              }
              else
              {

                  FoldersDS _subDirs = FolderBLC.GetFolders(_dirInfo.ID);

                  foreach (FoldersDS.FoldersRow _subDir in _subDirs.Folders)
                  {
                    XMLWebDavFolder.GetXML(_subDir, _xmlWriter, this._requestedProperties, this._requestPropertyType);
                  }

                  Search_FilesDS _subFiles = WebDavHelper.getFolderFiles(_dirInfo.ID);

                  foreach (Search_FilesDS.FilesRow _fileInfo in _subFiles.Files)
                  {

                   XMLWebDavFile.GetXML(_fileInfo, _xmlWriter, this._requestedProperties, this._requestPropertyType);
                  }
              }

              _xmlWriter.WriteEndElement();
              _xmlWriter.WriteEndDocument();
              _xmlWriter.Flush();

              this._responseXml = WebDavHelper.StreamtoString(_responseStream);
              _xmlWriter.Close();

             }
                          
            return (int)ServerResponseCode.MultiStatus;
        }
        #endregion

    }
}
