<%@ Page Title="Subjects" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Subjects.aspx.cs" Inherits="Subjects" %>

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
                        ID="hgSubjects"
                        Table ="dcr_Form_Subjects"
                        KeyField ="Subject_ID"
                        runat="server"
                        Title ="Subjects|<a href='#' onclick='gridEditorFormNew({element: this, w:400, h:600});' class='btn btn-primary'>New Subject</a>"
                        Edit = "false"
                        Links = "Subject|Editor, RelativePixels('w', 1, 600), RelativePixels('h', .7, 260)"
                        Required ="Subject_ID"
                        Hide="Subject_ID"
                        Labels ="Category_ID|Category"
                        DoNotEdit ="Chat|Docs"
                        ColumnStyle ="Category_ID|text-align: left;|Active|text-align: center;|Del|text-align: center;"
                        Widths=""
                        Calendars=""
                        TextAreas = ""
                        NumberFormat = ""
                        DeleteColumn="true"
                        Chat ="Chat|Subject|Subject_ID|RelativePixels('w', 1, 500), RelativePixels('h', .75, 600)"
                        Docs="Docs|Subject|Subject_ID|RelativePixels('w', 1, 1000), RelativePixels('h', .75, 600)|Images|Subject Image|Subject_ID|RelativePixels('w', 1, 1000), RelativePixels('h', .75, 600)" 
                        Search ="menu"
                        CssClass ="table table-hover scrollable fixedheader js-dynamitable hg"

                        />

            </td>
        </tr>
    </table>


    <input type="hidden" name="txtTestID" id="txtTestID" />
</asp:Content>

