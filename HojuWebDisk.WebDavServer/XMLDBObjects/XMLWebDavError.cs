using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace HojuWebDisk.WebDavServer.XMLDBObjects
{
    public class XMLWebDavError
    {
        public static string ProcessErrorCollection(ProcessingErrorCollection errorResources)
        {
            string _errorRequest = "";

            //Build the response 
            using (Stream _responseStream = new MemoryStream())
            {
                XmlTextWriter _xmlWriter = new XmlTextWriter(_responseStream, Encoding.UTF8);

                _xmlWriter.Formatting = Formatting.Indented;
                _xmlWriter.IndentChar = '\t';
                _xmlWriter.Indentation = 1;
                _xmlWriter.WriteStartDocument();

                //Set the Multistatus
                _xmlWriter.WriteStartElement("D", "multistatus", "DAV:");
                _xmlWriter.WriteAttributeString("xmlns:b", "urn:uuid:c2f41010-65b3-11d1-a29f-00aa00c14882");

                foreach (ProcessingError _err in errorResources)
                {

                        //Open the response element
                        _xmlWriter.WriteStartElement("response", "DAV:");
                        _xmlWriter.WriteElementString("href", "DAV:", _err.ResourcePath);
                        _xmlWriter.WriteElementString("status", "DAV:", _err.ErrorCode);
                        //Close the response element section
                        _xmlWriter.WriteEndElement();
                   
                }

                _xmlWriter.WriteEndElement();
                _xmlWriter.WriteEndDocument();
                _xmlWriter.Flush();

                using (StreamReader _streamReader = new StreamReader(_responseStream, Encoding.UTF8))
                {
                    //Go to the begining of the stream
                    _streamReader.BaseStream.Position = 0;
                    _errorRequest = _streamReader.ReadToEnd();
                }
                _xmlWriter.Close();
            }
            return _errorRequest;
        }
         
    }

}
