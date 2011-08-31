//  ============================================================================
//  AUTHOR		 : Simon
//  CREATE DATE	 : 23 Jan 2006
//  PURPOSE		 : Class to support properties of BaseObjects , supports Sortable collections
//  SPECIAL NOTES: 
//  (
//  ===========================================================================
using System;
using System.Xml.Serialization;
using System.Collections;
using System.Xml;

namespace HojuWebDisk.WebDavServer.XMLDBObjects
{
    #region "RequestedPropertyCollection Definition"

    /// <summary>
    /// Provides a collection of ObjectProperty items. 
    /// The class inherits Collection to allow for sorting
    /// the objects in the list and allowing faster searching.
    /// <seealso cref="SortableCollection"/>
    /// <seealso cref="ObjectProperty"/>
    /// </summary>
    public class RequestedPropertyCollection : CollectionBase
    {

        /// <summary>
        /// Read only Indexer
        /// </summary>
        public RequestedProperty this[int index]
        {
            get
            {
                return ((RequestedProperty)InnerList[index]);
            }
        }

        /// <summary>
        /// Add a new ObjectProperty to the collection.
        /// </summary>
        /// <param name="value">The RequestedProperty to add to the collection</param>
        /// <returns>an integer value of the index of the item added</returns>
        public int Add(RequestedProperty value)
        {
            return (InnerList.Add(value));
        }


        public void Remove(RequestedProperty value)
        {
            InnerList.Remove(value);
        }

        /// <summary>
        /// Overloaded method.
        /// Determines in the collection contains the given RequestedProperty.
        /// </summary>
        /// <param name="value">The RequestedProperty to search for.</param>
        /// <returns>Whether the collection contains the RequestedProperty.</returns>
        public bool Contains(RequestedProperty value)
        {
            return (InnerList.Contains(value));
        }

    }

    #endregion

    #region "RequestedProperty Definition"

    /// <summary>
    /// Represents a property object with Active Directory.
    /// </summary>
    public class RequestedProperty :IComparable
    {
        private string localName;
        private string ns;

        /// <summary>
        /// The localName for the property
        /// </summary>
        [XmlAttribute()]
        public string LocalName
        {
            get { return localName; }
            set { this.localName = value; }
        }
        /// <summary>
        /// The NameSpace for the property
        /// </summary>
        [XmlAttribute()]
        public string NS
        {
            get { return ns; }
            set { this.ns = value; }
        }

        

        /// <summary>
        /// Default constructor. This is required for serialization.
        /// </summary>
        public RequestedProperty()
        {
            localName = string.Empty;
            ns = string.Empty;
          
        }

        /// <summary>
        /// Constructor.
        /// </summary>
       ///<param name="LocalName">the localname of the property</param>
       ///<param name="ns">The NameSpace of the property</param>
        public RequestedProperty(string LocalName, string ns)
        {
            this.localName = LocalName;
            this.ns = ns;
            
        }

        #region "Object Overrides"

        /// <summary>
        /// Compares this object to another object.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>Whether the two objects are equal.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is RequestedProperty))
                throw new ArgumentException("Incorrect object type.");

            return (this.CompareTo(obj) == 0);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region IComparable Members

        /// <summary>
        /// Compares this object to another object.
        /// </summary>
        /// <param name="obj">the object to compare to.</param>
        /// <returns>Zero if the objects are equal, Less than zero if this object is less than the other object
        /// and greater than zero if this object is greater than the other object.</returns>
        public int CompareTo(object obj)
        {
            if (!(obj is RequestedProperty))
                throw new ArgumentException("Incorrect object type.");

            return string.Compare(this.LocalName, ((RequestedProperty)obj).LocalName, true);
        }

        #endregion

        
    }

    #endregion



}
