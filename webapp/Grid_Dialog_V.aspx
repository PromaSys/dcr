<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Grid_Dialog_V.aspx.cs" Inherits="Grid_Dialog_V" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="margin: 50px;">
    <form id="form1" runat="server">
        
        <vg:VGrid
        ID ="Grid"
        runat="server" 
        cssClass ="table table-condensed table-borderless vg"
    />
    </form>

    <script type="text/javascript">
        $(document).ready(function () {
            $('.vgLabel:contains("Allocate")').next().find('.hgResult').keyup(function () {

                if (this.value == '') return;

                var qs = unescape($('#diaAllocate').attr('qs'));
                var ca = getUrlParameter('ca', qs);
                if (this.value * 1 > ca * 1) {
                    MessageBox('Available Channel Licenses Exceeded', 'You attempted to allocate more licenses (' + this.value + ') than is availabele (' + ca + ').<br /><br />Edit to allocate from available licenses.', 'danger');
                    this.value = ca;
                }
            });

            $('.vgLabel:contains("available")').next().find('.hgResult').keyup(function () {

                if (this.value == '') return;
                //debugger;
                var ca = $(this).closest('tr').find('.channelquantity').text();
                if (this.value * 1 > ca * 1) {
                    MessageBox('Available Channel Licenses Exceeded', 'You attempted to use more licenses (' + this.value + ') than is available (' + ca + ').<br /><br />Edit to use from available licenses.', 'danger');
                    this.value = ca;
                }
            });

        });
        
    </script>
</body>
</html>
