<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Folder.aspx.cs" Inherits="Folder" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<link rel="Stylesheet" type="text/css" href="./Styles/HojuWebDisk.css?_MWDRes=1" />
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:Literal ID="FolderUp" runat="server"></asp:Literal>
 <asp:GridView ID="FolderGridView"  ShowHeader="false" ShowFooter="false" runat="server" CellPadding="4" AutoGenerateColumns="false" ForeColor="#333333" GridLines="None" Width="100%" DataKeyNames="ID" OnRowDataBound="FolderGridView_RowDataBound"   AllowPaging="false"  AllowSorting="false" PageSize="25">
            <RowStyle BackColor="#FFFFFF"  VerticalAlign="Middle" />
            <EmptyDataTemplate>     
            </EmptyDataTemplate>
            <Columns>
            <asp:BoundField DataField="FolderName" HeaderText="FolderName" SortExpression="FolderName"><ItemStyle Width="35px" /></asp:BoundField>
            <asp:BoundField DataField="FolderName" HeaderText="FolderName" SortExpression="FolderName"></asp:BoundField>                                               
        </Columns>
        </asp:GridView>
        <br />
 <asp:GridView ID="ResourceGridView" runat="server" ShowHeader="false" CellPadding="4" AutoGenerateColumns="false" ForeColor="#333333" GridLines="None" Width="100%" DataKeyNames="ID" OnRowDataBound="ResourceGridView_RowDataBound"   AllowPaging="false"  AllowSorting="false" PageSize="25">
             <RowStyle BackColor="#EFF3FB"  VerticalAlign="Middle" />
             <AlternatingRowStyle BackColor="White" />
            <EmptyDataTemplate>
               </EmptyDataTemplate>
            <Columns>
             <asp:BoundField DataField="FileName" HeaderText="FileName" SortExpression="FileName"><ItemStyle Width="35px" /></asp:BoundField>
             <asp:BoundField DataField="FileName" HeaderText="FileName" SortExpression="FileName"></asp:BoundField>
                       
                                    
        </Columns>
        </asp:GridView>
        <p></p>
    </div>
    </form>
</body>
</html>
