<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Form.aspx.cs" Inherits="Form" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <vg:VGrid
        ID ="vgForm"
        runat="server" 
        cssClass ="table table-condensed table-borderless vg"        
        Edit =" true"    
        KeyField = "Form_ID"
        Table ="dcr_Form_Fields"
        Stacked="true"
        />
    </form>

    <script type="text/javascript">

        var title = '<%=Session["FormTitle"].ToString() %>';
        $('.ui-dialog-title').html(title);

        // initialize datepicker
        $('.datepicker').datepicker({
            dateFormat: 'm/d/yy',
            autoclose: true,
            width: 300
        });
    </script>

</body>
</html>
