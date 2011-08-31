WebDav server with SQL Backend.

Original code from [TheHojuSaram][1]

Some notes about setting this up.

* I have moved from VS2005 to VS2010
* you need to set the IIS App to Managed Pipeline Mode: Classic when running on Windows 7 (Only tested on 7, guess you should do it for Win2k8+ and Vista also...
* Change the SQL connection in the Web.config to point at the correct SQL server. 
* the website is running on IIS Express at the moment...
* to get the SQL server working, you manually need to run the Create.SQL file in the Database project. 

[1]: http://thehojusaram.blogspot.com/2007/06/c-webdav-server-with-sql-backend-source.html