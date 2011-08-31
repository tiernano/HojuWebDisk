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
    #region "ProcessingErrorCollection Definition"

    /// <summary>
    /// Provides a collection of ProcessingError items. 
    /// The class inherits Collection to allow for sorting
    /// the objects in the list and allowing faster searching.
    /// <seealso cref="SortableCollection"/>
    /// <seealso cref="ObjectProperty"/>
    /// </summary>
    public class ProcessingErrorCollection : CollectionBase
    {

        /// <summary>
        /// Read only Indexer
        /// </summary>
        public ProcessingError this[int index]
        {
            get
            {
                return ((ProcessingError)InnerList[index]);
            }
        }

        /// <summary>
        /// Add a new ProcessingError to the collection.
        /// </summary>
        /// <param name="value">The ProcessingError to add to the collection</param>
        /// <returns>an integer value of the index of the item added</returns>
        public int Add(ProcessingError value)
        {
            return (InnerList.Add(value));
        }


        public void Remove(ProcessingError value)
        {
            InnerList.Remove(value);
        }

        /// <summary>
        /// Overloaded method.
        /// Determines in the collection contains the given ProcessingError.
        /// </summary>
        /// <param name="value">The ProcessingError to search for.</param>
        /// <returns>Whether the collection contains the ProcessingError.</returns>
        public bool Contains(ProcessingError value)
        {
            return (InnerList.Contains(value));
        }

    }

    #endregion

    #region "ProcessingError Definition"

    /// <summary>
    /// Represents a Processing Error.
    /// </summary>
    public class ProcessingError : IComparable
    {
        private string resourcepPath;
        private string errorcode;

        /// <summary>
        /// The Resource URI that the error occurred at
        /// </summary>
        [XmlAttribute()]
        public string ResourcePath
        {
            get { return resourcepPath; }
            set { this.resourcepPath = value; }
        }

        /// <summary>
        /// The string of the ErrorCode
        /// </summary>
        [XmlAttribute()]
        public string ErrorCode
        {
            get { return errorcode; }
            set { this.errorcode = value; }
        }



        /// <summary>
        /// Default constructor. This is required for serialization.
        /// </summary>
        public ProcessingError()
        {
            resourcepPath = string.Empty;
            errorcode = string.Empty;

        }

        /// <summary>
        /// Constructor.
        /// </summary>
       ///<param name="ResourcePath">The URI of the resource where the error occurred</param>
       ///<param name="ErrorCode">The String representation of the errorcode</param>
       
        public ProcessingError(string ResourcePath, string ErrorCode)
        {
            this.resourcepPath = ResourcePath;
            this.errorcode = ErrorCode;

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
            if (!(obj is ProcessingError))
                throw new ArgumentException("Incorrect object type.");

            return string.Compare(this.ResourcePath, ((ProcessingError)obj).ResourcePath, true);
        }

        #endregion


    }

    #endregion



}
