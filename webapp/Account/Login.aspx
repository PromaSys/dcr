<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Account_Login" %>

<%@ Register Src="~/Account/OpenAuthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    
    <asp:Panel ID="divLogin" class="bgImage" runat="server"  ClientIDMode="Static">

    </asp:Panel>

   
    
    <div  class="LoginFormBox">
    <div style="background-color: #ffffff; border-radius: 0px;padding: 0px;">
        <img class="LoginLogo" src="../Images/<%=Application["ApplicationLogo"].ToString() %>" style="width: 100%;"/>
    </div>
    <br /><br />
    <div class="row">
        <div class="col-md-12">
            <section id="loginForm" >
                <div >
                    <!--
                    <h4>Use a local account to log in.</h4>
                    <hr  />
                        -->
                    <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                        <p class="text-danger">
                            <asp:Literal runat="server" ID="FailureText" />
                        </p>
                    </asp:PlaceHolder>
                    <div class="form-group row">
                        <asp:Label runat="server" AssociatedControlID="Email" CssClass="col-md-3 control-label" >Login</asp:Label>
                        <div class="col-md-9">
                            <asp:TextBox runat="server" ID="Email" CssClass="form-control Email" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Email"
                                CssClass="text-danger" ErrorMessage="The email field is required." />
                        </div>
                    </div>
                    <div class="form-group row">
                        <asp:Label runat="server" AssociatedControlID="Password" CssClass="col-md-3 control-label">Password</asp:Label>
                        <div class="col-md-9">
                            <asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="form-control" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Password" CssClass="text-danger" ErrorMessage="The password field is required." />
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="offset-md-3 col-md-9">
                            <div>
                                &nbsp;<asp:CheckBox runat="server" ID="RememberMe" />&nbsp;
                                <asp:Label runat="server" AssociatedControlID="RememberMe" Font-Bold="false">Keep me logged in</asp:Label>
                                <br /><br />
                            </div>
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="offset-md-3 col-md-9">
                            <asp:Button runat="server" OnClick="LogIn" Text="Log in" CssClass="btn btn-primary form-control" />
                        </div>
                    </div>
                </div>
                <%--
                <p>
                    <asp:HyperLink runat="server" ID="RegisterHyperLink" ViewStateMode="Disabled">Register as a new user</asp:HyperLink>
                </p>
                 --%>
                    <!--
                        Enable this once you have account confirmation enabled for password reset functionality
                    -->
                    <div class="offset-md-3 col-md-9">
                    <br />
                    <asp:HyperLink runat="server" ID="ForgotPasswordHyperLink" ViewStateMode="Disabled">Forgot your password?</asp:HyperLink>
                     </div>
                
                 <div >
            
                <!--
                <uc:OpenAuthProviders runat="server" ID="OpenAuthLogin" />
            -->
        </div>
            </section>
        </div>
        
       
        
    </div>
    </div>
    
    <script type="text/javascript">
        $('.Email').focus();
    </script>
</asp:Content>

