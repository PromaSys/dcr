<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="TwoFactorAuthenticationSignIn.aspx.cs" Inherits="TwoFactorAuthenticationSignIn" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Panel ID="divLogin" class="bgImage" runat="server"  ClientIDMode="Static">

    </asp:Panel>

    <div  class="LoginFormBox">
    <div style="background-color: #ffffff; border-radius: 0px;padding: 20px;">
        <img class="LoginLogo" src="../Images/<%=Application["ApplicationLogo"].ToString() %>" style="width: 100%;"/>
    </div>

    <asp:PlaceHolder runat="server" ID="PlaceHolder1" Visible="false">
        <p class="text-danger">
            <asp:Literal runat="server" ID="Literal1" />
        </p>
    </asp:PlaceHolder>  

    <asp:PlaceHolder runat="server" ID="sendcode">
        <section>
            <br />
            <h4>Send verification code</h4>
            <hr />
            
            
                <div class="col-md-12">
                    Select Two-Factor Authentication Provider:
                    <br /><br />
                </div>
            
            
            <div class="form-group">
                <div class="col-md-12">
                    <asp:DropDownList runat="server" ID="Providers" CssClass="form-control">
                    </asp:DropDownList>
                </div>
            </div>
                    
            <div class="form-group">
                <div class="col-md-12">                    
                    <br />
                    <asp:Button Text="Submit" ID="ProviderSubmit" OnClick="ProviderSubmit_Click" CssClass="btn btn-primary" runat="server" />
                </div>
            </div>
        
        </section>
    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="verifycode" Visible="false">
        <section>
            <br />
            <h4>Enter verification code</h4>
            <asp:Label runat="server" ID="lblCode" Text="test"/>
            <hr />
            <asp:HiddenField ID="SelectedProvider" runat="server" />
            <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                <p class="text-danger">
                    <asp:Literal runat="server" ID="FailureText" />
                </p>
            </asp:PlaceHolder>

            <div class="form-group">
                <asp:Label Text="Code" runat="server" AssociatedControlID="Code" CssClass="col-md-2 control-label" />
                <div class="col-md-10">
                    <asp:TextBox runat="server" ID="Code" CssClass="form-control Code" />
                    <br />
                </div>
            </div>

            <!--
            <div class="form-group">
                <div class="col-md-offset-2 col-md-10">
                    <div class="checkbox">
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:CheckBox Text="" ID="RememberBrowser" runat="server" />
                        <asp:Label Text="Remember Browser" AssociatedControlID="RememberBrowser" runat="server" />
                        <br /><br />
                    </div>
                </div>
            </div>
            -->

            <div class="form-group">
                <div class="col-md-offset-2 col-md-10">
                    <asp:Button Text="Submit" ID="CodeSubmit" OnClick="CodeSubmit_Click" CssClass="btn btn-primary" runat="server" />
                </div>
            </div>
        </section>
    </asp:PlaceHolder>
        </div>

    <script type="text/javascript">
        $('.Code').focus();
    </script>

</asp:Content>

