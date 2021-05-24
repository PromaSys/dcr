<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Chat.aspx.cs" Inherits="Chat" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>

            <hg:HGrid
                ID ="ChatComment"
                runat="server" 
                Edit = "true"
                Labels ="Comment|New Comment"
                cssClass ="table _table-hover hg"
                Table ="app_Chat"
                KeyField ="Chat_ID"
                NewRecords ="1"
                TextAreas ="Comment|4"
                Height ="150"
            />
    
            <div style="position: relative; margin: 8px; height: <%= Session["ChatHeight"].ToString() %>px; overflow: auto;">
            <asp:Literal ID="litChat" runat="server" />
            </div>


        </div>
    </form>

    <script type="text/javascript">
        $(document).ready(function () {

            $('tfoot').css('display', 'none');
            $('#divChatComment').css('overflow', 'hidden');

            $('.hgNewRecord').click();

            $('.hgResult').focus();
            //setTimeout(function () { $('.hgNewRecord').click(); }, 2000);
        });
    </script>
</body>
</html>
