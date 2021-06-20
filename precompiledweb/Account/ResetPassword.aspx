<%@ page title="Reset Password" language="C#" masterpagefile="~/Site.master" autoeventwireup="true" inherits="ResetPassword, App_Web_1k3rtyqv" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">

    <asp:Panel ID="divLogin" class="bgImage" runat="server" ClientIDMode="Static">
    </asp:Panel>




    <div class="LoginFormBox">
        <div style="background-color: #ffffff; border-radius: 0px; padding: 0px;">
            <img class="LoginLogo" src="../Images/<%=Application["ApplicationLogo"].ToString() %>" style="width: 100%;" />
        </div>

        <div class="row">
            <div class="col-md-12">

                <p class="text-danger">
                    <asp:Literal runat="server" ID="ErrorMessage" />
                </p>
                <br />
                Enter your email and new password
        <hr />
                <asp:ValidationSummary runat="server" CssClass="text-danger" />
                <div>
                    <div class="form-group row">
                        <asp:Label runat="server" AssociatedControlID="Email" CssClass="col-md-3 control-label">Email</asp:Label>
                        <div class="col-md-9">
                            <asp:TextBox runat="server" ID="Email" CssClass="form-control" TextMode="Email" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Email"
                                CssClass="text-danger" ErrorMessage="The email field is required." />
                        </div>
                    </div>
                    <div class="form-group row">
                        <asp:Label runat="server" AssociatedControlID="Password" CssClass="col-md-3 control-label">Password</asp:Label>
                        <div class="col-md-9">
                            <asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="form-control" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Password"
                                CssClass="text-danger" ErrorMessage="The password field is required." />
                        </div>
                    </div>
                    <div class="form-group row">
                        <asp:Label runat="server" AssociatedControlID="ConfirmPassword" CssClass="col-md-3 control-label">Confirm password</asp:Label>
                        <div class="col-md-9">
                            <asp:TextBox runat="server" ID="ConfirmPassword" TextMode="Password" CssClass="form-control" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="ConfirmPassword"
                                CssClass="text-danger" Display="Dynamic" ErrorMessage="The confirm password field is required." />
                            <asp:CompareValidator runat="server" ControlToCompare="Password" ControlToValidate="ConfirmPassword"
                                CssClass="text-danger" Display="Dynamic" ErrorMessage="The password and confirmation password do not match." />
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="offset-md-3 col-md-9">
                            <asp:Button runat="server" OnClick="Reset_Click" Text="Reset" CssClass="btn btn-primary form-control" />
                        </div>
                    </div>
                </div>


            </div>
        </div>
    </div>
</asp:Content>

