<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Dialog.aspx.cs" Inherits="Dialog" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title> 
    
    <script>

        //var PickerLinkDefaults = '{}';
        var SpreadInitialized = false;
        var PickerLinkDefaults = [];
        var SpreadColumns;
    </script>
    <script src="Scripts/signature_pad.umd.js"></script>

</head>
<body >
    <form id="form1" runat="server">
        
        <hg:HGrid
            ID ="HGrid"
            runat="server" 
            cssClass ="table table-hover scrollable fixedheader js-dynamitable hg"
            Visible="False" 
         />

        <vg:VGrid
            ID ="VGrid"
            runat="server" 
            cssClass ="table table-condensed table-borderless vg"
            Visible="False" 
        />

        <hs:HSpread
            ID ="SGrid"
            runat="server" 
            cssClass ="table tabledetail table-hover scrollable fixedheader js-dynamitable sg griddialog table-striped"
            Visible="False" 
        />

        <hg:HGrid
            ID ="CGrid"
            runat="server" 
            Edit = "true"
            Labels ="Comment|New Comment"
            cssClass ="table hg cg"
            Table ="app_Chat"
            KeyField ="Chat_ID"
            NewRecords ="1"
            TextAreas ="Comment|4"
            Visible="False" 
        />

        <asp:Panel runat="server" ID="divChat" style="position: relative; margin: 8px; overflow: auto;" Visible="false">
            <asp:Literal ID="litChat" runat="server" />
        </asp:Panel>

    </form>

    <script type="text/javascript">

        // for spread
        if ($('.spread:visible').length > 0) {
            var SpreadID = $('.spread').attr('id');

            table = document.getElementById(SpreadID).jexcel;

            var columns = table.getConfig().columns;

            var required = columns.filter(function (e) { return e.required == true });

            $(required).each(function () {
                var ch = this.title;

                $('#' + SpreadID + ' thead tr td[title="' + ch + '"]').html(ch + '<font color="red">*</font>');
            });

            table.updateSelectionFromCoords(1, 0, 1, 0);
        } else if ($('.cg:visible').length > 0) {

            $('tfoot').css('display', 'none');
            $('#divChatComment').css('overflow', 'hidden');

            $('.hgNewRecord').click();

            $('.hgResult').focus();
        }
        //else if ($('#div' + gridID + '>.hg.fixedheader:visible').length > 0) {
        //    FixedHeaders('#div' + gridID);
        //}
        InitializeSignatures();

    </script>
</body>
</html>
