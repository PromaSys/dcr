<%@ Page Title="Forms" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Forms.aspx.cs" Inherits="Forms" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

        <table class="LeftRightTable">
        <tr>
            <td class="LeftTD" style="width: 240px; _display: none;">
                <table id="tabLeftMenu" class="LeftRightTable">
                    <tr>
                        <td class="LeftMenuTitle">Filters<a class="btn">&nbsp;</a></td>
                    </tr>
                    <tr>
                        <td class="LeftMenuTD Filters">
                            <asp:Literal ID="litLeftMenu" runat="server" />
                        </td>
                    </tr>
                </table>
            </td>
            <td class="RightTD">
                    <hg:HGrid
                        ID="hgForms"
                        Table ="dcr_Forms"
                        KeyField ="Form_ID"
                        runat="server"
                        Title ="Forms|<a href='#' onclick='gridEditorFormNew({element: this, w:400, h:180});' class='btn btn-primary'>New Form</a>"
                        Edit = "false"
                        Links = "Form|form(this);"
                        _Required ="Text"
                        Hide="Form_ID"
                        Labels ="Category_ID|Category"
                        _DoNotEdit ="Field_Count"
                        _Formats ="Date|MM/dd/yyyy"
                        _ColumnStyle ="Field_Count|text-align: center;"
                        Widths="default"
                        _Calendars="Date"
                        _TextAreas = "Long_Text|3"
                        _NumberFormat = "Money|$#,###.00"
                        DeleteColumn="false"
                        _Chat ="Chat|Test|Subject_ID|800,600"
                        _Docs="Docs|Test|Subject_ID|1000,400" 
                        _NewRecords="1" 
                        Search ="menu"
                        GroupBy ="Subject,e,1,1"
                        cssClass ="table table-hover scrollable fixedheader js-dynamitable hg"
                        />

            </td>
        </tr>
    </table>

</asp:Content>

