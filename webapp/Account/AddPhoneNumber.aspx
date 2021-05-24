<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="AddPhoneNumber.aspx.cs" Inherits="AddPhoneNumber" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>

    <div class="form-horizontal" style="width: 40%; margin: auto">
        <h4>Add a phone number</h4>
        <hr />
        <asp:ValidationSummary runat="server" CssClass="text-danger" />
        <p class="text-danger">
            <asp:Literal runat="server" ID="ErrorMessage" />
        </p>
        <div class="form-group row">
            <asp:Label runat="server" AssociatedControlID="PhoneNumber" CssClass="col-md-2 control-label">Phone Number</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" ID="PhoneNumber" CssClass="form-control" TextMode="Phone" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="PhoneNumber"
                    CssClass="text-danger" ErrorMessage="The PhoneNumber field is required." />
            </div>
        </div>
        <div class="form-group row">
            <div class="offset-md-2 col-md-10">
                <asp:Button runat="server" OnClick="PhoneNumber_Click"
                    Text="Submit" CssClass="btn btn-primary form-control" />
            </div>
        </div>
    </div>
</asp:Content>

