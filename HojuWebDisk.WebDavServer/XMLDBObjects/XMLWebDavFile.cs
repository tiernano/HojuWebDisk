//  ============================================================================
//  AUTHOR		 : Simon
//  CREATE DATE	 : 30 Apr 2007
//  PURPOSE		 : Class to build a XML representation of a File record in the WebDav Namespace.
//  SPECIAL NOTES: 
//  (
//  ===========================================================================
using System;
using System.Xml.Serialization;
using System.Collections;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Globalization;

using HojuWebDisk.WebDavServer;

using HojuWebDisk.DataEntities;

namespace HojuWebDisk.WebDavServer.XMLDBObjects
{
   
    /// <summary>
    /// Class to build a XML representation of a File record in the WebDav Namespace.
    /// </summary>
    public static class XMLWebDavFile
    {
        #region WebDav Response Example
        /* Example of a WEBDAV PropFind Response for a File Resource
     
      <D:response>
       <D:href>File.doc</D:href>
       <D:propstat>
           <D:status>HTTP/1.1 200 OK</D:status>
           <D:prop>
               <D:ContentLanguage>shendry</D:ContentLanguage>
               <D:CreationDate b:dt="dateTime.tz">2007-04-20T15:01:37</D:CreationDate>
               <D:lockdiscovery />
               <D:ContentLength>34816</D:ContentLength>
               <D:FilePath>Adoption.doc</D:FilePath>
               <D:supportedlock>0<D:lockentry><D:lockscope><D:exclusive /></D:lockscope><D:locktype><D:write /></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared /></D:lockscope><D:locktype><D:write /></D:locktype></D:lockentry></D:supportedlock>
               <D:isHidden>0</D:isHidden>
               <D:LastModified b:dt="dateTime.rfc1123">Tue, 24 Apr 2007 15:54:32 GMT</D:LastModified>
               <D:ContentType>application/msword</D:ContentType>
               <D:DisplayName>File.doc</D:DisplayName>
				
           </D:prop>
       </D:propstat>
       <D:propstat>
           <D:status>HTTP/1.1 404 Not Found</D:status>
           <D:prop>
               <D:LastAccessed />
           </D:prop>
       </D:propstat>
   </D:response>
        * 
        */
        #endregion

        #region XML Generation

        /// <summary>
        /// Generates the D:Response for a given backend File Row
        /// <param name="_fdr">A FileDS.FileRow of a file record in the Files Table</param>
        /// <param name="xmlWriter">The xmlWriter to add the xml to</param>
        /// <param name="ReqProps">The list of request properties in the PROPFIND request if any</param>
        /// <param name="isPropOnly">If this is a PROPFIND request for properties only. If true then ReqProps must be null</param>
        /// </summary>

        public static void GetXML(Search_FilesDS.FilesRow _fdr, XmlTextWriter xmlWriter, RequestedPropertyCollection ReqProps, PropertyRequestType RequestType)
        {
                        
            //Load the Valid Properties for the file resource
            ArrayList ValidProps = new ArrayList();
            ValidProps.Add("contentlanguage");
            ValidProps.Add("contentlength");
            ValidProps.Add("contenttype");
            ValidProps.Add("creationdate");
            ValidProps.Add("displayname");
            ValidProps.Add("filepath");
            ValidProps.Add("ishidden");
            ValidProps.Add("lastaccessed");
            ValidProps.Add("lastmodified");
            ValidProps.Add("lockdiscovery");
            ValidProps.Add("resourcetype");
            ValidProps.Add("supportedlock");
            ValidProps.Add("name");
            ValidProps.Add("parentname");
            ValidProps.Add("isreadonly");
            ValidProps.Add("contentclass");
            ValidProps.Add("iscollection");
            ValidProps.Add("isstructureddocument");
            ValidProps.Add("defaultdocument");
            ValidProps.Add("isroot");
            ValidProps.Add("href");

            RequestedPropertyCollection InValidProps = new RequestedPropertyCollection();

            if (RequestType == PropertyRequestType.PropertyNames && ReqProps != null) return;

            if (_fdr ==null) return;

            //Open the response element
            xmlWriter.WriteStartElement("response", "DAV:");

            //Load the valid items HTTP/1.1 200 OK
            xmlWriter.WriteElementString("href", "DAV:", _fdr.FileName);

            //Open the propstat element section
            xmlWriter.WriteStartElement("propstat", "DAV:");
            xmlWriter.WriteElementString("status", "DAV:", WebDavHelper.getEnumHttpResponse(ServerResponseCode.Ok));

            //Open the prop element section
            xmlWriter.WriteStartElement("prop", "DAV:");
            
            //If there are no requested Properties then return all props for File.
            if (ReqProps == null){
                ReqProps = new RequestedPropertyCollection();
                for (int _vPe=0;_vPe<ValidProps.Count;_vPe++){
                    ReqProps.Add(new RequestedProperty((string)ValidProps[_vPe],"DAV:"));
                }
            }

            foreach (RequestedProperty _ReqProp in ReqProps)
            {
                string _propertyName = _ReqProp.LocalName;
                if (_propertyName.ToLower(CultureInfo.InvariantCulture).StartsWith("get"))
                    _propertyName = _propertyName.Substring(3);

                if ((_ReqProp.NS != "DAV:") || (ValidProps.IndexOf(_propertyName.ToLower()) == -1))
                {
                    InValidProps.Add(_ReqProp);
                }else{

                    if (RequestType == PropertyRequestType.PropertyNames)
                    {
                        //if this is a request for property names only then just return the named elements:

                        xmlWriter.WriteElementString(_propertyName, _ReqProp.NS, "");
                       
                    }
                    else
                    {
                        //Map the property to the Row Data and return the PropStat XML.

                        switch (_propertyName.ToLower())
                        {

                            case "contentlanguage":
                                xmlWriter.WriteElementString(_ReqProp.LocalName, "DAV:", "en-us");
                                break;
                            case "contentlength":
                                xmlWriter.WriteElementString(_ReqProp.LocalName, "DAV:", _fdr.FileDataSize.ToString());
                                break;
                            case "contenttype":
                                xmlWriter.WriteElementString(_ReqProp.LocalName, "DAV:", _fdr.ContentType);
                                break;
                            case "displayname":
                                xmlWriter.WriteElementString(_ReqProp.LocalName, "DAV:", _fdr.FileName);
                                break;
                            case "filepath":
                                xmlWriter.WriteElementString(_ReqProp.LocalName, "DAV:", _fdr.FileName);
                                break;
                            case "ishidden":
                                //May adjust later to allow hidden files
                                xmlWriter.WriteElementString(_ReqProp.LocalName, "DAV:", "false");
                                break;
                            case "isreadonly":
                                //May adjust later to allow read-only access later
                                xmlWriter.WriteElementString(_ReqProp.LocalName, "DAV:", "false");
                                break;
                            case "resourcetype":
                                xmlWriter.WriteElementString(_ReqProp.LocalName, "DAV:", "");
                                break;
                            case "lastaccessed":
                                xmlWriter.WriteStartElement(_ReqProp.LocalName, "DAV:");
                                xmlWriter.WriteAttributeString("b:dt", "dateTime.rfc1123");

                                //This change is outside the spec for MS. If you set this date is set to the rfc1123 compliant dateformat
                                //the Windows Explorer errors out.
                                                                 
                                // xmlWriter.WriteString(_fdr.update_date_stamp.ToUniversalTime().ToString("r", CultureInfo.InvariantCulture));

                                xmlWriter.WriteString(_fdr.update_date_stamp.ToUniversalTime().ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture));
                                xmlWriter.WriteEndElement();
                                break;

                            case "lastmodified":
                                xmlWriter.WriteStartElement(_ReqProp.LocalName, "DAV:");
                                xmlWriter.WriteAttributeString("b:dt", "dateTime.rfc1123");
                                xmlWriter.WriteString(_fdr.update_date_stamp.ToUniversalTime().ToString("r", CultureInfo.InvariantCulture));
                                xmlWriter.WriteEndElement();
                                break;
                           
                            case "creationdate":
                                xmlWriter.WriteStartElement(_ReqProp.LocalName, "DAV:");
                                xmlWriter.WriteAttributeString("b:dt", "dateTime.tz");
                                xmlWriter.WriteString(_fdr.create_date_stamp.ToUniversalTime().ToString("s", CultureInfo.InvariantCulture));
                                xmlWriter.WriteEndElement();
                                break;
                            case "lockdiscovery":
                                xmlWriter.WriteStartElement("D", "lockdiscovery", "DAV:");
                                XMLWebDavLock.GetXML(WebDavHelper.getLock(_fdr.ID), xmlWriter);
                                xmlWriter.WriteEndElement();
                                break;
                            case "supportedlock":

                                xmlWriter.WriteStartElement("D", _ReqProp.LocalName, "DAV:");

                                xmlWriter.WriteStartElement("lockentry", "DAV:");
                                xmlWriter.WriteStartElement("lockscope", "DAV:");
                                xmlWriter.WriteElementString("exclusive", "DAV:", "");
                                xmlWriter.WriteEndElement();
                                xmlWriter.WriteStartElement("locktype", "DAV:");
                                xmlWriter.WriteElementString("write", "DAV:", "");
                                xmlWriter.WriteEndElement();
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteStartElement("lockentry", "DAV:");
                                xmlWriter.WriteStartElement("lockscope", "DAV:");
                                xmlWriter.WriteElementString("shared", "DAV:", "");
                                xmlWriter.WriteEndElement();
                                xmlWriter.WriteStartElement("locktype", "DAV:");
                                xmlWriter.WriteElementString("write", "DAV:", "");
                                xmlWriter.WriteEndElement();
                                xmlWriter.WriteEndElement();
                                xmlWriter.WriteEndElement();

                                break;
                            default:
                                xmlWriter.WriteElementString(_ReqProp.LocalName, "DAV:","");
                                break;
                        }
                    }
                }
                    
            }   
            
            //Close the prop element section
            xmlWriter.WriteEndElement();

            //Close the propstat element section
            xmlWriter.WriteEndElement();
            //END Load the valid items HTTP/1.1 200 OK
            
           	//Load the invalid items HTTP/1.1 404 Not Found
            if (InValidProps.Count > 0)
            {
                xmlWriter.WriteStartElement("propstat", "DAV:");
                xmlWriter.WriteElementString("status", "DAV:", WebDavHelper.getEnumHttpResponse(ServerResponseCode.NotFound));

                //Open the prop element section
                xmlWriter.WriteStartElement("prop", "DAV:");

                //Load all the invalid properties
                foreach (RequestedProperty _InValidProp in InValidProps)
                   xmlWriter.WriteElementString(_InValidProp.LocalName, _InValidProp.NS,"");
                  
                //Close the prop element section
                xmlWriter.WriteEndElement();
                //Close the propstat element section
                xmlWriter.WriteEndElement();
            }
			//END Load the invalid items HTTP/1.1 404 Not Found

			//Close the response element
			xmlWriter.WriteEndElement();

        }
       
      
       #endregion
            
    }
    
}

