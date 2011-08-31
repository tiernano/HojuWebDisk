using System;

namespace HojuWebDisk.WebDavServer
{
    #region Lock / Unlock

    /// <summary>
    /// Lock owner type
    /// </summary>
    public enum LockOwnerType : int
    {
        /// <summary>
        /// User
        /// </summary>
        User,

        /// <summary>
        /// URI Lock owner
        /// </summary>
        Href
    }

    public enum LockResType : int
    {
        /// <summary>
        /// File
        /// </summary>
        File,

        /// <summary>
        /// Folder
        /// </summary>
        Folder
    }

    /// <summary>
    /// Lock Type
    /// </summary>
    public enum LockType : int
    {
        /// <summary>
        /// Read lock
        /// </summary>
        Read,

        /// <summary>
        /// Write lock
        /// </summary>
        Write
    }


    /// <summary>
    /// Lock Scope
    /// </summary>
    public enum LockScope : int
    {
        /// <summary>
        /// Shared lock
        /// </summary>
        Shared,

        /// <summary>
        /// Exclusive lock
        /// </summary>
        Exclusive
    }

    /// <summary>
    /// WebDav Depth
    /// </summary>
    public enum DepthType : int
    {
        /// <summary>
        /// The method is applied only to the resource
        /// </summary>
        ResourceOnly = 0,

        /// <summary>
        /// The method is applied to the resource and to its immediate children
        /// </summary>
        ResourceChildren = 1,

        /// <summary>
        /// The method is applied to the resource and to all of its children
        /// </summary>
        Infinity = 2
    }

    /// <summary>
    /// DAV Lock Responses
    /// </summary>
    public enum DavLockResponseCode : int
    {
        /// <summary>
        /// 0: None
        /// </summary>
        /// <remarks>
        ///		Default enumerator value
        /// </remarks>
        None = 0,

        /// <summary>
        /// 200: Ok
        /// </summary>
        /// <remarks>
        ///		The lock request succeeded and the value of the lockdiscovery 
        ///		property is included in the body.
        /// </remarks>
        Ok = 200,

        /// <summary>
        /// 400: Bad Request
        /// </summary>
        /// <example>If a Depth of 1 is requested</example>
        BadRequest = 400,

        /// <summary>
        /// 412: Precondition Failed
        /// </summary>
        /// <remarks>
        ///		The included lock token was not enforceable on this resource or 
        ///		the server could not satisfy the request in the lockinfo XML element.
        /// </remarks>
        PreconditionFailed = 412,

        /// <summary>
        /// 423: Resource Locked
        /// </summary>
        /// <remarks>
        ///		If the resource is already locked with an exclusive lock or if the resource
        ///		is already locked with a shared lock and the client requests and exclusive lock
        /// </remarks>
        Locked = 423,

        /// <summary>
        /// 424: FailedDependency
        /// </summary>
        /// <remarks>
        ///		Implies the action would have succeeded by itself
        /// </remarks>
        FailedDependency = 424
    }

    public enum DavUnlockResponseCode : int
    {
        /// <summary>
        /// 0: None
        /// </summary>
        /// <remarks>
        ///		Default enumerator value
        /// </remarks>
        None = 0,

        /// <summary>
        /// 204: NoContent
        /// </summary>
        /// <remarks>The unlock command completed successfully</remarks>
        NoContent = 204,

        /// <summary>
        /// 400: Bad Request
        /// </summary>
        /// <example>If the client does not provide a lock token</example>
        BadRequest = 400,


        /// <summary>
        /// 401: Unauthorized
        /// </summary>
        /// <example>If the client is not authorized to unlock the resource</example>
        Unauthorized = 401,

        /// <summary>
        /// 412: Precondition Failed
        /// </summary>
        /// <example>
        ///		If the client provides a lock token to unlock a resource that isn't locked or
        ///		provides an incorrect lock token
        ///	</example>
        PreconditionFailed = 412
    }

    #endregion

   
    /// <summary>
    /// WebDav Property Handling Behavior
    /// </summary>
    /// <remarks>
    ///		Used by MOVE / COPY
    ///	</remarks>
    public enum PropertyBehavior : int
    {
        /// <summary>
        /// Omit
        /// </summary>
        /// <remarks>
        ///		The omit XML element instructs the server that it should use best effort to 
        ///		copy properties but a failure to copy a property MUST NOT cause the method to fail.
        /// </remarks>
        Omit,

        /// <summary>
        /// Keep Alive
        /// </summary>
        /// <remarks>
        ///		Specifies requirements for the copying/moving of live properties.
        /// </remarks>
        KeepAlive
    }



    /// <summary>
    /// WebDav Resource Type
    /// </summary>
    public enum ResourceType : int
    {
        /// <summary>
        /// Collection Resource
        /// </summary>
        Collection,

        /// <summary>
        /// File Resource
        /// </summary>
        Resource
    }

    /// <summary>
    /// WebDav Server Responses
    /// </summary>
    public enum ServerResponseCode : int
    {
        /// <summary>
        /// 0: None
        /// </summary>
        /// <remarks>
        ///		Default enumerator value
        /// </remarks>
        None = 0,

        /// <summary>
        ///	200: Ok 
        /// </summary>
        Ok = 200,

        /// <summary>
        /// 207: Multi Status
        /// </summary>
        /// <remarks>Used by PropFind</remarks>
        MultiStatus = 207,

        /// <summary>
        /// 400: Bad Request
        /// </summary>
        BadRequest = 400,

        /// <summary>
        /// 404: Not Found
        /// </summary>
        NotFound = 404,

        /// <summary>
        /// 412: Precondition Failed
        /// </summary>
        /// <remarks></remarks>
        PreconditionFailed = 412,

        /// <summary>
        /// 501: Method Not Implemented
        /// </summary>
        MethodNotImplemented = 501
    }

    public enum PropertyRequestType
    {
        /// <summary>
        /// No properties requested
        /// </summary>
        None,

        /// <summary>
        /// Specific properties requested
        /// <seealso cref="DavResourceFindBase.RequestProperties"/>
        /// </summary>
        NamedProperties,

        /// <summary>
        /// A summary of all the available properties
        /// </summary>
        PropertyNames,


        /// <summary>
        /// All properties requested
        /// </summary>
        AllProperties
    }

    /// <summary>
    /// WebDav MKCOL Response Codes
    /// </summary>
    public enum DavMKColResponseCode : int
    {
        /// <summary>
        /// 0: None
        /// </summary>
        /// <remarks>
        ///		Default enumerator value
        /// </remarks>
        None = 0,

        /// <summary>
        /// 201: Created
        /// </summary>
        /// <remarks>
        ///		Collection completed successfully
        /// </remarks>
        Created = 201,

        /// <summary>
        /// 403: Forbidden
        /// </summary>
        /// <remarks>
        ///		This indicates one of two conditions: 
        ///			- The server does not allow the creation of collections at the given 
        ///				location in its namespace
        ///			- The parent collection of the Request-URI exists but cannot accept members.
        /// </remarks>
        Forbidden = 403,

        /// <summary>
        /// 405: Method Not Allowed
        /// </summary>
        /// <remarks>
        ///		Can only be executed on a deleted/non-existent resource
        /// </remarks>
        MethodNotAllowed = 405,

        /// <summary>
        /// 409: Conflict
        /// </summary>
        /// <remarks>
        ///		If the parent collection does not exist, or a resource exists with the parent's
        ///		name but is not a collection
        /// </remarks>
        Conflict = 409,

        /// <summary>
        /// 415: Unsupported Media Type
        /// </summary>
        /// <remarks>
        ///		The server does not support the request type of the body
        /// </remarks>
        UnsupportedMediaType = 415,

        /// <summary>
        /// 507: Insufficient Storage
        /// </summary>
        /// <remarks>
        ///		The resource does not have sufficient space to record the state of the 
        ///		resource after the execution of this method.
        /// </remarks>
        InsufficientStorage = 507
    }

    /// <summary>
    /// WebDav DELETE Response Codes
    /// </summary>
    public enum DavDeleteResponseCode : int
    {
        /// <summary>
        /// 0: None
        /// </summary>
        /// <remarks>
        ///		Default enumerator value
        /// </remarks>
        None = 0,

        /// <summary>
        /// 102: Processing
        /// </summary>
        Processing = 102,

        /// <summary>
        /// 204: NoContent
        /// </summary>
        /// <remarks>
        ///		The delete command completed successfully
        ///	</remarks>
        NoContent = 204,

        /// <summary>
        /// 423: Locked
        /// </summary>
        /// <remarks>
        ///		The resource is locked
        ///	</remarks>
        Locked = 423
    }

    /// <summary>
    /// WebDav PUT Response Codes
    /// </summary>
    public enum DavPutResponseCode : int
    {
        /// <summary>
        /// 0: None
        /// </summary>
        /// <remarks>
        ///		Default enumerator value
        /// </remarks>
        None = 0,

        /// <summary>
        /// 201: Created
        /// </summary>
        /// <remarks>
        ///		Resource completed successfully
        /// </remarks>
        Created = 201,


        /// <summary>
        /// 403: Forbidden
        /// </summary>
        /// <remarks>
        ///		This indicates one of two conditions: 
        ///			- The server does not allow the creation of the resource at the given 
        ///				location in its namespace
        ///			- The parent collection of the Request-URI exists but cannot accept members.
        /// </remarks>
        Forbidden = 403,

        /// <summary>
        /// 409: Conflict
        /// </summary>
        /// <remarks>
        ///		If all ancestor collections do not exist
        /// </remarks>
        Conflict = 409,

        /// <summary>
        /// 423: Resource Locked
        /// </summary>
        /// <remarks>
        ///		If the resource is already locked with an exclusive lock or if the resource
        ///		is already locked with a shared lock and the client requests and exclusive lock
        /// </remarks>
        Locked = 423,

        /// <remarks>
        ///		The resource does not have sufficient space to record the state of the 
        ///		resource after the execution of this method.
        /// </remarks>
        InsufficientStorage = 507
    }

    /// <summary>
    /// WebDav COPY Response Codes
    /// </summary>
    public enum DavCopyResponseCode : int
    {
        /// <summary>
        /// 0: None
        /// </summary>
        /// <remarks>
        ///		Default enumerator value
        /// </remarks>
        None = 0,

        /// <summary>
        /// 201: Created
        /// </summary>
        /// <remarks>
        ///		The source resource was successfully copied. 
        ///		The copy operation resulted in the creation of a new resource.
        /// </remarks>
        Created = 201,

        /// <summary>
        /// 204: No Content
        /// </summary>
        /// <remarks>
        ///		The source resource was successfully copied to a pre-existing destination resource.
        /// </remarks>
        NoContent = 204,

        /// <summary>
        /// 403: Forbidden
        /// </summary>
        /// <remarks>
        ///		The source and destination URIs are the same
        /// </remarks>
        /// 
        Forbidden = 403,

        /// <summary>
        /// 409: Conflict
        /// </summary>
        /// <remarks>
        ///		A resource cannot be created at the destination until one or more intermediate 
        ///		collections have been created.
        /// </remarks>
        Conflict = 409,

        /// <summary>
        /// 412: Precondition Failed
        /// </summary>
        /// <remarks>
        ///		The server was unable to maintain the liveness of the properties listed in the 
        ///		propertybehavior XML element or the Overwrite header is "F" and the state of 
        ///		the destination resource is non-null.
        /// </remarks>
        PreconditionFailed = 412,

        /// <summary>
        /// 423: Locked
        /// </summary>
        /// <remarks>
        ///		The destination resource was locked.
        /// </remarks>
        Locked = 423,

        /// <summary>
        /// 502: Bad Gateway
        /// </summary>
        /// <remarks>
        ///		This may occur when the destination is on another server 
        ///		and the destination server refuses to accept the resource.
        /// </remarks>
        BadGateway = 502,

        /// <summary>
        /// 507: Insufficient Storage
        /// </summary>
        /// <remarks>
        ///		The destination resource does not have sufficient space to record 
        ///		the state of the resource after the execution of this method.
        /// </remarks>
        InsufficientStorage = 507
    }

    /// <summary>
    /// WebDav MOVE Response Codes
    /// </summary>
    public enum DavMoveResponseCode : int
    {
        /// <summary>
        /// 0: None
        /// </summary>
        /// <remarks>
        ///		Default enumerator value
        /// </remarks>
        None = 0,

        /// <summary>
        /// 201: Created
        /// </summary>
        /// <remarks>
        ///		The source resource was successfully moved, and a new resource was created 
        ///		at the destination.
        /// </remarks>
        Created = 201,

        /// <summary>
        /// 204: No Content
        /// </summary>
        /// <remarks>
        ///		The source resource was successfully moved to a pre-existing destination resource.
        /// </remarks>
        NoContent = 204,

        /// <summary>
        /// 403: Forbidden
        /// </summary>
        /// <remarks>
        ///		The source and destination URIs are the same
        /// </remarks>
        Forbidden = 403,

        /// <summary>
        /// 409: Conflict
        /// </summary>
        /// <remarks>
        ///		A resource cannot be created at the destination until one or more intermediate 
        ///		collections have been created
        /// </remarks>
        Conflict = 409,

        /// <summary>
        /// 412: Precondition Failed
        /// </summary>
        /// <remarks>
        ///		The server was unable to maintain the liveness of the properties listed in 
        ///		the propertybehavior XML element or the Overwrite header is "F" and the state 
        ///		of the destination resource is non-null.
        /// </remarks>
        PreconditionFailed = 412,

        /// <summary>
        /// 423: Locked
        /// </summary>
        /// <remarks>
        ///		The source or the destination resource was locked.
        /// </remarks>
        Locked = 423,

        /// <summary>
        /// 502: Bad Gateway
        /// </summary>
        /// <remarks>
        ///		This may occur when the destination is on another server 
        ///		and the destination server refuses to accept the resource.
        /// </remarks>
        BadGateway = 502
    }

}


