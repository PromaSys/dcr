<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="VerifyPhoneNumber.aspx.cs" Inherits="VerifyPhoneNumber" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
 <h2><%: Title %>.</h2>
    <p class="text-danger">
        <asp:Literal runat="server" ID="ErrorMessage" />
    </p>
    <div class="form-horizontal" style="width: 40%; margin: auto">
        <h4>Enter verification code</h4>
        <hr />
        <asp:HiddenField runat="server" ID="PhoneNumber" />
        <asp:ValidationSummary runat="server" CssClass="text-danger" />
        <div class="form-group row">
            <asp:Label runat="server" AssociatedControlID="Code" CssClass="col-md-2 control-label">Code</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" ID="Code" CssClass="form-control" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="Code"
                    CssClass="text-danger" ErrorMessage="The Code field is required." />
            </div>
        </div>
        <div class="form-group row">
            <div class="offset-md-2 col-md-10">
                <asp:Button runat="server" OnClick="Code_Click"
                    Text="Submit" CssClass="btn btn-primary form-control" />
            </div>
        </div>
    </div>
</asp:Content>

