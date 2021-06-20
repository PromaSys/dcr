<%@ page title="" language="C#" masterpagefile="~/Site.master" autoeventwireup="true" inherits="ForgotPassword, App_Web_1k3rtyqv" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
        <asp:Panel ID="divLogin" class="bgImage" runat="server"  ClientIDMode="Static">

    </asp:Panel>

    <div  class="LoginFormBox">
    <div style="background-color: #ffffff; border-radius: 0px;padding: 0px;">
        <img class="LoginLogo" src="../Images/<%=Application["ApplicationLogo"].ToString() %>" style="width: 100%; "/>
    </div>
        

    <div class="row">
        <div class="col-md-12">
            <asp:PlaceHolder id="loginForm" runat="server">
                <div class="form-horizontal">
                    <br />
                    Forgot your password?
                    <hr />
                    <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                        <p class="text-danger">
                            <asp:Literal runat="server" ID="FailureText" />
                        </p>
                    </asp:PlaceHolder>
                    <div class="form-group row">
                        <asp:Label runat="server" AssociatedControlID="Email" CssClass="col-md-2 control-label">Email</asp:Label>
                        <div class="col-md-10">
                            <asp:TextBox runat="server" ID="Email" CssClass="form-control" TextMode="Email" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Email"
                                CssClass="text-danger" ErrorMessage="The email field is required." />
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="offset-md-2 col-md-10">
                            <asp:Button runat="server" OnClick="Forgot" Text="Email Link" CssClass="btn btn-primary form-control" />
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="DisplayEmail" Visible="false">
                <p class="text-info">
                    Please check your email to reset your password.
                </p>
            </asp:PlaceHolder>
        </div>
    </div>
        </div>
</asp:Content>

