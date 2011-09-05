using System;
using System.Web.UI;
using HojuWebDisk.BLC;
using HojuWebDisk.DataEntities;
using HojuWebDisk.WebDavServer;
using System.Web.UI.WebControls;

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
        Page.Title = String.Format("HojuWebDisk:/{0}{1}", _FPath, _sa);
        FolderUp.Text = String.Format("<table width=100% border=0 cellpadding=2 cellspacing=2><tr valign=top><td width=35px><img src=\"./Images/WebDrive.jpg?_MWDRes=1\"></td><td><h3>HojuWebDisk:/{0}{1}</h3></td></tr>", _FPath, _sa);

        if (_dirInfo.ParentID != 0)
        {
            FolderUp.Text += String.Format("<tr valign=middle><td width=35px><a href=\"Folder.aspx?FPath={0}&_MWDRes=1\"><img src=\"./Images/FolderUp.jpg?_MWDRes=1\" border=0></a></td>", WebDavHelper.getParentResourcePath(_FPath, 1));
            FolderUp.Text += String.Format("<td><a href=\"Folder.aspx?FPath={0}&_MWDRes=1\">[Parent Folder]</a></td></tr>", WebDavHelper.getParentResourcePath(_FPath, 1));
        }
        FolderUp.Text += "</table>";

    }
    protected void ResourceGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string _FileName = Convert.ToString(e.Row.Cells[0].Text);
            Int64 _FileSize = (Int64)DataBinder.Eval(e.Row.DataItem, "FileDataSize");
            e.Row.Cells[0].Text = String.Format("<a href=\"{0}{1}{2}\"><img border=\"0\" src=\"./images/{3}?_MWDRes=1\"></a>", _FPath, _sa, _FileName, getAttimg(_FileName));
            e.Row.Cells[1].Text = String.Format("<a href=\"{0}{1}{2}\">{2} ({3})</a>", _FPath, _sa, _FileName, FormatFileSize(_FileSize));

        }
    }
    protected void FolderGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            e.Row.Cells[0].Text = String.Format("<a href=\"Folder.aspx?FPath={0}{1}{2}&_MWDRes=1\"><img src=\"./Images/Folder.jpg?_MWDRes=1\" border=0></a>", _FPath, _sa, Convert.ToString(DataBinder.Eval(e.Row.DataItem, "FolderName")));
            e.Row.Cells[1].Text = String.Format("<a href=\"Folder.aspx?FPath={0}{1}{2}&_MWDRes=1\">{3}</a>", _FPath, _sa, Convert.ToString(DataBinder.Eval(e.Row.DataItem, "FolderName")), e.Row.Cells[1].Text);


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
            return FS + " Bytes";
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
