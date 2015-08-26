#HojuWebDisk

##what is it?
WebDav server with SQL Backend.

Original code from [TheHojuSaram][1]

##How Do i use it?
Some notes about setting this up.

* I have moved from VS2005 to VS2010
* you need to set the IIS App to Managed Pipeline Mode: Classic when running on Windows 7 (Only tested on 7, guess you should do it for Win2k8+ and Vista also...
* Change the SQL connection in the Web.config to point at the correct SQL server. 
* to get the SQL server working, you manually need to run the Create.SQL file in the Database project. 

NOTE: if you run this in Casini (VS IIS) and set it to use a virtual directory of just /, and view in browser, it seems to show all files. if you open [CyberDuck][2] and point to the local server, it will allow uploads and downloads... IIS full on my desktop is having problems... think there is a module causing problems.

##Future Plans?
* Get it upgraded to run on a Windows 2012R2/8.1/10 box. 
* newer SQL server?
* other backend storage options? 

[1]: http://thehojusaram.blogspot.com/2007/06/c-webdav-server-with-sql-backend-source.html
[2]: http://cyberduck.ch/
