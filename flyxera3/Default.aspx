<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="flyxera3.WebForm1" %>

<!DOCTYPE html>

<html lang="en">
<head runat="server">
    <meta charset="utf-8">

    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="description" content="Location-centric microtransactions.">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="google-signin-client_id" content="765190230709-0fsqio3ugacf6i702dso6b04fbon2c2l.apps.googleusercontent.com">
    <title>flyxera</title>

    <!-- Add to homescreen for Chrome on Android -->
    <meta name="mobile-web-app-capable" content="yes">
    <link rel="icon" sizes="196x196" href="images/favicon-196x196.png">

    <link rel="shortcut icon" href="images/favicon16x16.png" sizes="16x16">
    <link rel="shortcut icon" href="images/favicon32x32.png" sizes="32x32">
    <link rel="shortcut icon" href="images/favicon64x64.png" sizes="64x64">
    <link rel="shortcut icon" href="images/favicon196x196.png" sizes="196x196">

    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Roboto:regular,bold,italic,thin,light,bolditalic,black,medium&amp;lang=en">
    <link rel="stylesheet" href="https://fonts.googleapis.com/icon?family=Material+Icons">
    <link rel="stylesheet" href="https://code.getmdl.io/1.1.1/material.cyan-light_blue.min.css">
    <style type="text/css" media="screen">
        @import url( stylesheet.css );
    </style>
    <link type="text/css" rel="stylesheet" href="http://code.jquery.com/ui/1.10.3/themes/smoothness/jquery-ui.css" />
    <script type="text/javascript" src="http://code.jquery.com/jquery-1.8.2.js"></script>
    <script type="text/javascript" src="http://code.jquery.com/ui/1.10.3/jquery-ui.js"></script>
    <script src="https://apis.google.com/js/platform.js" async defer></script>
</head>
<body>
     <div class="demo-layout mdl-layout mdl-js-layout mdl-layout--fixed-drawer mdl-layout--fixed-header">
        <header class="demo-header mdl-layout__header mdl-color--grey-100 mdl-color-text--grey-600">
            <div class="mdl-layout__header-row">
                <span class="mdl-layout-title">Home</span>
                <div class="mdl-layout-spacer"></div>
                <div class="mdl-textfield mdl-js-textfield mdl-textfield--expandable">
                    <label class="mdl-button mdl-js-button mdl-button--icon" for="search">
                        <i class="material-icons">search</i>
                    </label>
                    <div class="mdl-textfield__expandable-holder">
                        <input class="mdl-textfield__input" type="text" id="search">
                        <label class="mdl-textfield__label" for="search">Enter your query...</label>
                    </div>
                </div>
            </div>
        </header>
        <div class="demo-drawer mdl-layout__drawer mdl-color--blue-grey-900 mdl-color-text--blue-grey-50">
            <header class="demo-drawer-header">
                <img src="images/user.jpg" class="demo-avatar">
                <div class="demo-avatar-dropdown">
                    <span>hello@example.com</span>
                </div>
            </header>
            <nav class="demo-navigation mdl-navigation mdl-color--blue-grey-800">
                <button id="showOffersAll" class="mdl-button mdl-js-button mdl-color-text--blue-grey-400">Offers</button>
                <button id="showOffersClient" class="mdl-button mdl-js-button mdl-color-text--blue-grey-400">My Offers</button>
            </nav>
        </div>
        <main class="mdl-layout__content mdl-color--grey-100">
            <div class="mdl-grid demo-content">

                <div class="demo-cards mdl-cell mdl-cell--4-col mdl-cell--8-col-tablet mdl-grid mdl-grid--no-spacing">

                    <form id="flyxeraForm" runat="server">
                        <asp:ScriptManager ID="ScriptManager1" runat="server" />
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <!-- Template for an "Offer" Card -->
                                <asp:Repeater ID="ListOfOffers" runat="server">
                                    <ItemTemplate>
                                        <div class="demo-updates mdl-card mdl-shadow--2dp mdl-cell mdl-cell--4-col mdl-cell--4-col-tablet mdl-cell--12-col-desktop">
                                            <div class="mdl-card__title mdl-card--expand mdl-color--teal-300">
                                                <h2 class="mdl-card__title-text">$
                                              <asp:Label runat="server" ID="offerAmmount" Text='<%# Eval("Amount") %>' />
                                                    @
                                              <asp:Label runat="server" ID="offerTime" Text='<%#Eval("Time") %>' />
                                                </h2>
                                            </div>
                                            <div class="mdl-card__supporting-text mdl-color-text--grey-600">
                                                <p>
                                                    <asp:Label runat="server" ID="offererName" Text='<%#Eval("Offerer.Name") %>' />
                                                </p>
                                                <p>
                                                    <asp:Label runat="server" ID="offerShortDescription" Text='<%#Eval("ShortDescription") %>' />
                                                </p>
                                                <asp:Label class="style: hidden" runat="server" ID="offerLongDescription" Text='<%#Eval("LongDescription")%>' />
                                            </div>
                                            <div class="mdl-card__actions mdl-card--border">
                                                <button type="button" class="mdl-button mdl-js-button mdl-js-ripple-effect">Go to offer</button>
                                            </div>
                                        </div>
                                        <div class="demo-separator mdl-cell--1-col"></div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ContentTemplate>
                        </asp:UpdatePanel>


                        <!-- This dialog shows the long description of an offer -->
                        <dialog id="viewOffer" class="mdl-dialog">
                            <h4 class="mdl-dialog__title">
                                <label id="offererName" />
                            </h4>
                            <div class="mdl-dialog__content">
                                <p id="offerLongText" />
                            </div>
                            <div class="mdl-dialog__actions">
                                <asp:Button ID="acceptOfferButton" Text="Accept" runat="server" OnClick="AcceptButton_Click" class="mdl-button mdl-js-button mdl-js-ripple-effect" />
                                <button id="cancelOfferButton" type="button" class="mdl-button close">Cancel</button>
                            </div>
                        </dialog>


                        <asp:HiddenField ID="email" runat="server" />
                        <asp:HiddenField ID="name" runat="server" />
                        <asp:HiddenField ID="photoURL" runat="server" />
                        <asp:HiddenField ID="latitude" runat="server" />
                        <asp:HiddenField ID="longitude" runat="server" />
                        <asp:HiddenField ID="firstLoad" runat="server" />
                        <asp:Button ID="sendLoginAndLocation" OnClick="DataAndLocation_Click" runat="server" Style="display: none" />

                        <asp:HiddenField ID="amount" runat="server" />
                        <asp:HiddenField ID="shortDesc" runat="server" />
                        <asp:HiddenField ID="longDesc" runat="server" />
                        <asp:Button ID="testCreateOffer" OnClick="TestCreateOffer_Click" runat="server" Style="display: none" />

                        <asp:Button ID="ShowAllOffers" OnClick="ShowAllOffers_Click" runat="server" Style="display: none"/>
                        <asp:Button ID="ShowMyOffers" OnClick="ShowMyOffers_Click" runat="server" Style="display: none"/>


                    </form>


                    
                        <dialog id="createOfferDialog" class="mdl-dialog">
                            <h4 class="mdl-dialog__title">Create Offer
                            </h4>
                            <div class="mdl-dialog__content">
                                <div class="mdl-textfield mdl-js-textfield">
                                    <input class="mdl-textfield__input" type="number" id="offerAmountClient" min="0">
                                    <label class="mdl-textfield__label" for="offerAmountClient">Amount</label>
                                </div>
                                <div class="mdl-textfield mdl-js-textfield">
                                    <input class="mdl-textfield__input" type="text" id="offerShortDescriptionClient">
                                    <label class="mdl-textfield__label" for="offerShortDescriptionClient">Short description</label>
                                </div>
                                <div class="mdl-textfield mdl-js-textfield">
                                    <input class="mdl-textfield__input" type="text" id="offerLongDescriptionClient">
                                    <label class="mdl-textfield__label" for="offerLongDescriptionClient">Long description</label>
                                </div>
                            </div>
                            <div class="mdl-dialog__actions">
                                <button id="createOfferButtonClient" type="button" class="mdl-button mdl-js-button mdl-js-ripple-effect">Create</button>
                                <button id="cancelCreateOfferButton" type="button" class="mdl-button close">Cancel</button>
                            </div>
                        </dialog>

                    
                        <!-- FAB button with ripple -->
                        <button id="createOfferFAB" class="mdl-button mdl-js-button mdl-button--fab mdl-js-ripple-effect">
                            <i class="material-icons">add</i>
                        </button>

                    <div id="my-signin2"></div>

                <script type="text/javascript">
 
                      // Gets the location and, once it is known, evaluates callback on it.
                        function getLocation(callback) {
                            if (navigator.geolocation) {
                                navigator.geolocation.getCurrentPosition(
                                    function (position) {
                                        callback(position);
                                    }
                                );
                            } else {
                                // TODO: error here. 
                            }
                        }
                        // Called after the user has signed in.
                        function onSuccess(googleUser) {
                            var profile = googleUser.getBasicProfile();
                            console.log('Logged in as: ' + profile.getName());
                            $('#<%=email.ClientID%>').val(profile.getEmail());
                            $('#<%=name.ClientID%>').val(profile.getName());
                            $('#<%=photoURL.ClientID%>').val(profile.getImageUrl());


                            getLocation(function (position) {
                                $('#<%=latitude.ClientID%>').val(position.coords.latitude);
                                $('#<%=longitude.ClientID%>').val(position.coords.longitude);
                                $('<%=sendLoginAndLocation.ClientID%>').click();
                            })
                        }

                        function onFailure(error) {
                            console.log(error);
                        }

                        function renderButton() {
                            // Only sign in on first page load.
                            if('<%=Page.IsPostBack%>' == "False") {
                                gapi.signin2.render('my-signin2', {
                                    'scope': 'profile email',
                                    'width': 240,
                                    'height': 50,
                                    'longtitle': true,
                                    'theme': 'dark',
                                    'onsuccess': onSuccess,
                                    'onfailure': onFailure
                                });
                            }
                        }
                    $('#createOfferButtonClient').click(function () {
                        $('#<%=amount.ClientID%>').val($('#offerAmountClient').val());
                        $('#<%=shortDesc.ClientID%>').val($('#offerShortDescriptionClient').val());
                        $('#<%=longDesc.ClientID%>').val($('#offerLongDescriptionClient').val());
                        $('#<%=testCreateOffer.ClientID%>').click();
                    });
                    $('#createOfferFAB').click(function () {
                        document.querySelector('#createOfferDialog').showModal();
                    });
                    $('#showOffersAll').click(function () {
                        $('#ShowAllOffers').click();
                    });
                    $('#showOffersClient').click(function () {
                        $('#ShowMyOffers').click();
                    });

                    $('#UpdatePanel1 .mdl-button').click(function () {
                        var c = $(this).parents("div .mdl-card");
                        var longDescription = c.find("span[id*=offerLongDescription]").text();
                        var offererName = c.find("span[id*=offererName]").text();
                        $("#offerLongText").text(longDescription);
                        $("#offererName").text(offererName);
                        document.querySelector('#viewOffer').showModal();
                        return false;
                    });
                    $('#cancelOfferButton').click(function () {
                        $('#viewOffer').close();
                    });
                </script>
                    <script src="https://apis.google.com/js/platform.js?onload=renderButton" async defer></script>
                </div>
            </div>
        </main>
    </div>
    <script src="https://code.getmdl.io/1.1.1/material.min.js"></script>

</body>
</html>
