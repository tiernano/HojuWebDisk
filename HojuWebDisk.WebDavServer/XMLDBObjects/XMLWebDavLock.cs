//  ============================================================================
//  AUTHOR		 : Simon
//  CREATE DATE	 : 2 May  2007
//  PURPOSE		 : Class to build a XML representation of a FileLock in the WebDav Namespace.
//  SPECIAL NOTES: 
//  (
//  ===========================================================================
using System;
using System.Collections;
using System.Xml;

using HojuWebDisk.WebDavServer;
using HojuWebDisk.DataEntities;

namespace HojuWebDisk.WebDavServer.XMLDBObjects
{
    /// <summary>
    /// Class to build a XML representation of a FileLock in the WebDav Namespace 
    /// </summary>

    public static class XMLWebDavLock
    {

        public static void GetXML(LocksDS.LocksRow ldr, XmlWriter xmlWriter)
        {
            if (ldr == null) return;

            xmlWriter.WriteStartElement("activelock", "DAV:");

            xmlWriter.WriteStartElement("locktype", "DAV:");
            switch ((LockType)ldr.LockType)
            {
                case LockType.Read:
                    xmlWriter.WriteElementString("read", "DAV:");
                    break;

                case LockType.Write:
                    xmlWriter.WriteElementString("write", "DAV:");
                    break;
            }
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("lockscope", "DAV:");
            switch ((LockScope)ldr.LockScope)
            {
                case LockScope.Exclusive:
                    xmlWriter.WriteElementString("exclusive", "DAV:");
                    break;

                case LockScope.Shared:
                    xmlWriter.WriteElementString("shared", "DAV:");
                    break;
            }
            xmlWriter.WriteEndElement();

            DepthType LockDepth = (DepthType)ldr.LockDepth;

            if (LockDepth == DepthType.Infinity)
                xmlWriter.WriteElementString("depth", "DAV:", LockDepth.ToString());
            else
                xmlWriter.WriteElementString("depth", "DAV:", (string)System.Enum.Parse(LockDepth.GetType(), LockDepth.ToString(), true));

            //Append the owner
            xmlWriter.WriteElementString("owner", "DAV:", ldr.LockOwner);
            xmlWriter.WriteElementString("timeout", "DAV:", "Seconds-" + ldr.Timeout.ToString());

            //Append all the tokens
            xmlWriter.WriteStartElement("locktoken", "DAV:");

            //Get LockTokens from the DB

            Locks_TokensDS _ltds = WebDavHelper.getLockTokens(ldr.ID);
            foreach (Locks_TokensDS.Locks_TokensRow _ltr in _ltds.Locks_Tokens)
            {
                xmlWriter.WriteElementString("href", "DAV:", "opaquelocktoken:" + _ltr.Token);
            }

            xmlWriter.WriteEndElement();

            //End ActiveLock
            xmlWriter.WriteEndElement();


        }
    }

}

