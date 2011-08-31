using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using HojuWebDisk.BLC;
using HojuWebDisk.DataEntities;
using HojuWebDisk.WebDavServer;

public partial class Folder : System.Web.UI.Page
{
    private string _FPath;
    private string _sa = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        
        if (Request.QueryString.GetValues("FPath") == null)
        {

            _FPath = "";

        }
        else
        {

            _FPath = Request.QueryString.GetValues("FPath").GetValue(0).ToString();
        }

       
        if (_FPath != "") _sa = "/";

        FoldersDS.FoldersRow _dirInfo = WebDavHelper.getFolder(_FPath);

        if (_dirInfo == null)
        {
            FolderUp.Text = "Folder not found";
        }
        else
        {

            FolderGridView.DataSource = FolderBLC.GetFolders(_dirInfo.ID);
            FolderGridView.DataBind();
            ResourceGridView.DataSource = FileBLC.List(_dirInfo.ID);
            ResourceGridView.DataBind();
            
        }
        Page.Title = "HojuWebDisk:/" + _FPath + _sa;
        FolderUp.Text = "<table width=100% border=0 cellpadding=2 cellspacing=2><tr valign=top><td width=35px><img src=\"./Images/WebDrive.jpg?_MWDRes=1\"></td><td><h3>HojuWebDisk:/" + _FPath + _sa + "</h3></td></tr>";

        if (_dirInfo.ParentID != 0)
        {
            FolderUp.Text += "<tr valign=middle><td width=35px><a href=\"Folder.aspx?FPath=" + WebDavHelper.getParentResourcePath(_FPath, 1) + "&_MWDRes=1\"><img src=\"./Images/FolderUp.jpg?_MWDRes=1\" border=0></a></td>";
            FolderUp.Text += "<td><a href=\"Folder.aspx?FPath=" + WebDavHelper.getParentResourcePath(_FPath, 1) + "&_MWDRes=1\">[Parent Folder]</a></td></tr>";
        }
        FolderUp.Text += "</table>";

    }
    protected void ResourceGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string _FileName = Convert.ToString(e.Row.Cells[0].Text);
            Int64 _FileSize = (Int64)DataBinder.Eval(e.Row.DataItem, "FileDataSize");
            e.Row.Cells[0].Text = "<a href=\"" + _FPath + _sa + _FileName + "\"><img border=\"0\" src=\"./images/" + getAttimg(_FileName) + "?_MWDRes=1\"></a>";
            e.Row.Cells[1].Text = "<a href=\"" + _FPath + _sa + _FileName + "\">" + _FileName + " (" + FormatFileSize(_FileSize) + ")</a>";

        }
    }
    protected void FolderGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            
            e.Row.Cells[0].Text = "<a href=\"Folder.aspx?FPath=" + _FPath + _sa + Convert.ToString(DataBinder.Eval(e.Row.DataItem, "FolderName"))
                + "&_MWDRes=1\"><img src=\"./Images/Folder.jpg?_MWDRes=1\" border=0></a>";
            e.Row.Cells[1].Text = "<a href=\"Folder.aspx?FPath=" + _FPath + _sa + Convert.ToString(DataBinder.Eval(e.Row.DataItem, "FolderName"))
                            + "&_MWDRes=1\">" + e.Row.Cells[1].Text.ToString() + "</a>";


        }
    }

    protected string getAttimg(string filename)
    {
        string ext;
        string retval;

        if (filename.Contains("."))
        {
            string[] exta = filename.Split(@".".ToCharArray());
            ext = exta[exta.Length-1];

            switch (ext.ToLower())
            {

                case "doc":
                case "exe":
                case "htm":
                case "mpp":
                case "msi":
                case "pot":
                case "ppt":
                case "txt":
                case "xls":
                case "zip":
                    retval = ext + ".gif";
                    break;
                case "dot":
                    retval = "doc.gif";
                    break;
                case "csv":
                    retval = "xls.gif";
                    break;
                case "gif":
                case "jpg":
                case "jpeg":
                case "bmp":
                case "tif":
                    retval = "pic.gif";
                    break;
                case "html":
                    retval = "htm.gif";
                    break;
                default:
                    retval = "unk.gif";
                    break;
            }
        }
        else
        {

            retval = "unk.gif";
        }

        return retval;
    }
    protected string FormatFileSize(Int64 FS)
    {

        if (FS < 1000)
        {
            return FS.ToString() + " Bytes";
        }

        if (FS < 1048576)
        {

            return (FS / 1024).ToString() + " KB";

        }
        if (FS < 1073741824)
        {

            return (FS / 1048576).ToString() + " MB";
        }

        return (FS / 1073741824).ToString() + " GB";
    }
    
}
