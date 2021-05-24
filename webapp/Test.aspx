<%@ Page Title="Test" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Test.aspx.cs" Inherits="Test" %>

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
                        ID="hgTest"
                        runat="server"
                        Edit = "false"
                        cssClass ="table table-hover scrollable fixedheader js-dynamitable hg"
                        Title ="Test|<a href='#' onclick='eligibilityImports(this);' class='btn btn-info'>Eligibility Imports</a>&nbsp;&nbsp;<a href='#' onclick='gridEditorFormNew({element: this, w:400, h:600});' class='btn btn-primary'>New Item</a>"
                        KeyField ="Test_ID"
                        Table ="fnd_Test"
                        Required ="Text"
                        Hide="Signature"
                        Labels =""
                        Links = "_Long_Text|gridBox({boxType: 'wait', title: 'Wut', body: 'I am body', type: 'success', confirmFunction: () => {alert(1)}}); setTimeout(function () { gridBox({closeBox: true}) }, 4000);|Test_ID|gridEditorForm({element:this, w:400, h:600 })|Money|gridPop({context: 'VTest', type: 'vertical', title: 'Test Vertical', w: 400, h: 600, data: {Test_ID: 1}});"
                        __Links = "Test_ID|Editor, 400, 500"                        
                        ___Links ="Money|gridPop({type: 'chat', element: this, context: 'Test', contextID: 1, title: 'Test Chat', w: 1200, h: 600 })|Docs|gridPop({type: 'docs', element: this, context: 'Test', contextID: 1, title: 'Test Docs', w: 1200, h: 600 });"                        
                        _Links = "Text|messageModal('success', 'This is the body')|Test_ID|gridPop({context: 'VTest', type: 'vertical', title: 'Test Vertical', w: 400, h: 600, data: {Test_ID: 1}})|Money|gridPop({context: 'HTest', type: 'horizontal', title: 'Test Horizontal', w: 1200, h: 600, data: {Test_ID: 1}})|Integer|gridPop({context: 'STest', type: 'spread', title: 'Test Spread', w: 1200, h: 600, data: {Test_ID: 1}});"
                        _SLinks ="Test_ID|Editor, 400, 500|Money|gridPop({context: 'HTest', type: 'horizontal', title: 'Test Horizontal', w: 1200, h: 600, data: {Test_ID: 1}})|Integer|gridPop({context: 'STest', type: 'spread', title: 'Test Spread', w: 1200, h: 600, data: {Test_ID: 1}});"
                        ____Links ="Test_ID|Editor, 400, 500|Money|gridPopS('HTest', 'horizontal', 'Test Horizontal', {Test_ID: 1}, 1200, 600)|Integer|gridPopS('STest','spread', 'Test Spread', {Test_ID: 1}, 1200, 600)"
                        DoNotEdit ="Test_ID|Docs|Chat"
                        Formats ="Date|MM/dd/yyyy"
                        ColumnStyle ="Drop_Down_ID|text-align: left;"
                        Widths="default"
                        Calendars="Date"
                        TextAreas = "Long_Text|3"
                        NumberFormat = "Money|$#,###.00"
                        DeleteColumn="true"
                        Chat ="Chat|Test|Test_ID|800,600"
                        Docs="Docs|Test|Test_ID|1000,400" 
                        NewRecords="5" 
                        Search ="menu"

                        />

            </td>
        </tr>
    </table>


    <input type="hidden" name="txtTestID" id="txtTestID" />

    <script>
        $(document).ready(function () {

            var a = "";
            /*
            gridPop({
            context: 'Procurement',
            w: RelativePixels('w', .7, 1000),
            h: RelativePixels('w', .6, 500),
            title: 'Test',
            data: { tid: 1 }//, // JSON, post data
            //saveFunction: function () { alert(1); }
            });
            */

            gridFiltersSetVScroll();
        });
        
    </script>
</asp:Content>

