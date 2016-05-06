<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="flyxera3.WebForm1" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>flyxera</title>
    <link rel="icon" sizes="196x196" href="images/favicon-196x196.png">
    <link rel="shortcut icon" href="images/favicon16x16.png" sizes="16x16">
    <link rel="shortcut icon" href="images/favicon32x32.png" sizes="32x32">
    <link rel="shortcut icon" href="images/favicon64x64.png" sizes="64x64">
    <link rel="shortcut icon" href="images/favicon196x196.png" sizes="196x196">

    <script src="https://ajax.googleapis.com/ajax/libs/jquery/2.1.4/jquery.min.js"></script>
    <link rel="stylesheet" type="text/css" href="stylesheet.css" />
</head>
<body>
    <p>Hello, world!</p>
    <form runat="server">
        <asp:ScriptManager runat="server" />
        <asp:UpdatePanel runat="server">
            <ContentTemplate>
                <asp:Repeater ID="ListOfOffers" runat="server">
                    <ItemTemplate>
                        (
                        <asp:Label runat="server" Text='<%#Eval("Id")%>' />
                        ,
                        <asp:Label runat="server" Text='<%#Eval("ShortDescription") %>' />
                        )
                        <br />
                    </ItemTemplate>
                </asp:Repeater>
            </ContentTemplate>

        </asp:UpdatePanel>

        <asp:HiddenField ID="email" runat="server" />
        <asp:HiddenField ID="name" runat="server" />
        <asp:HiddenField ID="photoURL" runat="server" />
        <asp:HiddenField ID="latitude" runat="server" />
        <asp:HiddenField ID="longitude" runat="server" />
        <asp:HiddenField ID="amount" runat="server" />
        <asp:HiddenField ID="shortDesc" runat="server" />
        <asp:HiddenField ID="longDesc" runat="server" />

        <asp:Button ID="ShowAllOffers" OnClick="ShowAllOffers_Click" runat="server" Style="display: none" />
        <asp:Button ID="ShowMyOffers" OnClick="ShowMyOffers_Click" runat="server" Style="display: none" />
        <asp:Button runat="server" ID="sendLoginAndLocation" OnClick="DataAndLocation_Click" Style="display: none" />
        <asp:Button ID="testCreateOffer" OnClick="TestCreateOffer_Click" runat="server" Style="display: none" />

    </form>









    <h4>Create Offer</h4>
    Amount:
    <input type="number" id="offerAmountClient" min="0"><br />
    Short description:
    <input type="text" id="offerShortDescriptionClient"><br />
    Long description:
    <input type="text" id="offerLongDescriptionClient"><br />

    <button id="createOfferButtonClient" type="button">Create</button>





    <script type="text/javascript">
        window.ispostback = '<%= Page.IsPostBack %>';

        $("document").ready(function () {
            if (window.ispostback == "False") {
                alert('going to click');
                $('#<%=email.ClientID%>').val("dm635@cornell.edu");
                $('#<%=name.ClientID%>').val("Daniel Miller");
                $('#<%=photoURL.ClientID%>').val("URL");
                $('#<%=latitude.ClientID%>').val("42.3");
                $('#<%=longitude.ClientID%>').val("41.5");
                $('#<%=sendLoginAndLocation.ClientID%>').click();
            }
        });


        $('#createOfferButtonClient').click(function () {
            $('#<%=amount.ClientID%>').val($('#offerAmountClient').val());
            $('#<%=shortDesc.ClientID%>').val($('#offerShortDescriptionClient').val());
            $('#<%=longDesc.ClientID%>').val($('#offerLongDescriptionClient').val());
            $('#<%=testCreateOffer.ClientID%>').click();
        });
    </script>

</body>
</html>
