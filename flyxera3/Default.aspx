<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="flyxera3.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>flyxera</title>
</head>
<body>
    <p>Hello, world!</p>
    <form runat="server">
        <asp:ScriptManager runat="server" />
        <asp:UpdatePanel runat="server">
            <ContentTemplate>
                <asp:Repeater ID="ListOfUsers" runat="server">
                    <ItemTemplate>
                        (
                        <asp:Label runat="server" Text='<%#Eval("Email")%>' />
                        ,
                        <asp:Label runat="server" Text='<%#Eval("Name") %>' />
                        )
                        <br />
                    </ItemTemplate>
                </asp:Repeater>
            </ContentTemplate>

        </asp:UpdatePanel>

        Email: 
        <asp:TextBox ID="email" runat="server" /><br />
        Name:
        <asp:TextBox ID="name" runat="server" /><br />

        <asp:Button runat="server" OnClick="Button_Click" />
    </form>

    <script src="scripts.js"></script>
</body>
</html>
