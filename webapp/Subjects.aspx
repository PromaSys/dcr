<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Subjects.aspx.cs" Inherits="Subjects" %>

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
                        Links = "_Long_Text|gridBox({boxType: 'wait', title: 'Wut', body: 'I am body', type: 'success', confirmFunction: () => {alert(1)}}); setTimeout(function () { gridBox({closeBox: true}) }, 4000);|Test_ID|gridEditorForm({element:this, w:400, h:600 })|Money|gridPop({context: 'VTest', type: 'vertical', title: 'Test Vertical', w: 400, h: 600, data: {Test_ID: 1}});"
                        _Required ="Text"
                        Hide="Subject_ID|Docs|Chat"
                        Labels ="Category_ID|Category"
                        _DoNotEdit =""
                        _Formats ="Date|MM/dd/yyyy"
                        ColumnStyle ="Category_ID|text-align: left;"
                        Widths="default"
                        Calendars="Date"
                        TextAreas = "Long_Text|3"
                        _NumberFormat = "Money|$#,###.00"
                        DeleteColumn="true"
                        Chat ="Chat|Test|Subject_ID|800,600"
                        Docs="Docs|Test|Subject_ID|1000,400" 
                        NewRecords="5" 
                        Search ="menu"
                        cssClass ="table table-hover scrollable fixedheader js-dynamitable hg"

                        />

            </td>
        </tr>
    </table>


    <input type="hidden" name="txtTestID" id="txtTestID" />
</asp:Content>

