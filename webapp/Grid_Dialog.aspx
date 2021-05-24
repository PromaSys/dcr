<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Grid_Dialog.aspx.cs" Inherits="Grid_Dialog" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body >
    <form id="form1" runat="server">
        
        <hg:HGrid
        ID ="Grid"
        runat="server" 
        cssClass ="table table-hover scrollable fixedheader js-dynamitable hg"
    />
    </form>

    <script type="text/javascript">
        
        //var GridID = "<%=Session["GridID"].ToString() %>";

        /*
        $(document).ready(function () {
            //FixedHeaders
            FixedHeaders('#tab' + GridID);

            // column sorting
            $(document).find('#tab' +  + GridID + '.js-dynamitable').each(function () {

                $(this).find('thead:first tr:last th').addClass('js-sorter sortnew');

                $(this).dynamitable()
                    //.addFilter('.js-filter')
                    .addSorter('.js-sorter', 'asc')
                //.addSorter('.js-sorter-desc', 'desc')
                ;
            });
            
            // pop first item 
            $('#tab' + GridID + ' tbody:last tr:first').click();
        });
        */
    </script>
</body>
</html>
