<%@ page title="" language="C#" masterpagefile="~/Site_No_Menu.master" autoeventwireup="true" inherits="Documents, App_Web_hcfhn0u3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

     <input id="File1" runat="Server" name="File1" size="44" type="file" ClientIdMode="Static" style="display: none;" onchange="submit();" />

        <hg:HGrid
            ID ="Docs"
            runat="server" 
            Edit = "false"
            Labels ="FileName|File Name|Insert_Date|Upload Date"
            ColumnStyle ="Delete|text-align: center;"
            cssClass ="table table-hover scrollable fixedheader js-dynamitable hg"
            DeleteColumn = "true"
            Table ="app_Documents"
            KeyField ="Document_ID"
        />

       <script type="text/javascript">
        $(document).ready(function () {
            
            if ('<%=Session["DocsReadOnly"].ToString() %>' != '1') {

                $('#divDocs').on('dragenter', function () {
                    DocsDragEnter(window.event);
                });

                $('#divDocs').on('dragover', function () {
                    DocsDragOver(window.event);
                });

                $('#divDocs').on('drop', function () {
                    DocsDrop(window.event);
                });

                $('#divDocs').on('dragleave', function () {
                    DocsDragLeave(window.event);
                });
            }

           
            

            // check for error cookies
            var ErrorMessage = GetCookie("DocumentError");

            if (ErrorMessage != "" && ErrorMessage != null) {
                WaitBoxClose();
                SetCookie("DocumentError", "", -1);
                MessageBox('Import Error', ErrorMessage, 'danger', 400, function () {
                    var $r = $('#tabDocs>tbody>tr').first();
                    var kfv = $r.attr('kfv');
                    var $tb = $('#tabDocs');

                    var c = "<data tb='" + $tb.attr('tb') + "' kf='" + $tb.attr('kf') + "' kfv='" + kfv + "' as='" + $tb.attr('kf') + "' de='1' ></data>";
                    //alert(c);
                    //var ModalID = $(e).closest('.ui-dialog').attr('aria-describedby');

                    //MessageBox('Test', 'Yada yada yada yada yada yada yada yada yada Yada yada yada yada yada yada yada yada yada<br/>Yada yada yada yada yada<br />Yada<br/>Yada yada yada yada yada<br />Yada', 'info');

                    hgSaveChange(c, $r, null);
                    $r.remove();
                });
            }
            
            // check for message cookies
            var SuccessMessage = GetCookie("DocumentMessage");


            if (SuccessMessage != "" && SuccessMessage != null) {
                
                gridBox({ closeBox: true});
                SuccessMessage = SuccessMessage.replace(/\\n/g, "<br/>");
                SetCookie('DocumentMessage', '', -1);
                //MessageBox('Import Successfull', SuccessMessage, 'success');
                
                //MessageBox('Import Successfull', SuccessMessage, 'success', 400, function () {
                gridBox({
                    boxType: 'message',
                    title: 'Import Successfull',
                    body: SuccessMessage,
                    type: 'success',
                    width: 400,
                    yesFunction: function () {
                        var c = '<%=Session["Context"].ToString() %>';
                        if (c == 'Eligibility' || c == 'Enrollment') {
                            parent.location.reload();
                        }
                    }
                });
            }
        });
       </script>

</asp:Content>

