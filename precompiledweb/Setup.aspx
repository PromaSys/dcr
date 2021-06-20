<%@ page title="Setup" language="C#" masterpagefile="~/Site.master" autoeventwireup="true" inherits="Setup, App_Web_hcfhn0u3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <table class="LeftRightTable">
        <tr>
            <td class="LeftTD">
                
                <table class="LeftTable">
                    <tr>
                        <td class="LeftMenuTitle">
                            Setup
                        </td>
                    </tr>
                    <tr>
                        <td class="LeftMenuCategory">
                            Security
                        </td>
                    </tr>
                    <tr>
                        <td class="LeftMenuItem" onclick="LeftMenu('Users');">
                            Users
                        </td>
                    </tr>
                    <tr>
                        <td class="LeftMenuItem" onclick="LeftMenu('Groups');">
                            Groups
                        </td>
                    </tr>
                    <tr>
                        <td class="LeftMenuItem" onclick="LeftMenu('Templates');">
                            Templates
                        </td>
                    </tr>
                    <tr>
                        <td class="LeftMenuCategory">
                            Application
                        </td>
                    </tr>
                     <% if (!(bool)Application["ThemeIsSet"])
                        {%>
                            <tr>
                                <td class="LeftMenuItem" onclick="LeftMenu('Theme');">
                                    Theme
                                </td>
                            </tr>
                        <%}
                     %>
                     <tr>
                        <td class="LeftMenuItem" onclick="LeftMenu('Subject Categories');">
                            Subject Categories
                        </td>
                     </tr>
                     <tr>
                        <td class="LeftMenuItem" onclick="LeftMenu('Settings');">
                            Settings
                        </td>
                     </tr>
                    <!--
                    <tr>
                        <td class="LeftMenuItem" onclick="LeftMenu('Card Channel Compatibility');">
                            Card Channel Compatibility
                        </td>
                    </tr>
                    -->
                </table>

            </td>
            <td class="RightTD">
                <hg:HGrid
                    ID ="Grid"
                    runat="server" 
                    cssClass ="table table-hover scrollable fixedheader js-dynamitable hg setup"
                    Edit ="False"
                />

            </td>
        </tr>
    </table>
    <asp:HiddenField runat="server" ID="txtTopic" ClientIDMode="Static" />
    <asp:HiddenField runat="server" ID="txtTopicID" ClientIDMode="Static" />
    <asp:HiddenField runat="server" ID="txtTopicTitle" ClientIDMode="Static" />
    <asp:HiddenField runat="server" ID="txtSearch" ClientIDMode="Static" />
    <script type="text/javascript">

        function _Search(s, ev) {

            var Search;

            if (s == null) {
                Search = '';
            }
            else if (ev == null) {
                Search = s.value;
            }
            else if (ev.keyCode != 13) {
                return;
            }
            else {
                Search = s.value;
            }


            SetCookie("SearchUsers", Search, 30);
            /*
            var e = document.getElementById("selFilter");

            var selectedFilter = 'All';

            if (e.selectedIndex != null) {
                selectedFilter = e.options[e.selectedIndex].text;
            }

            document.forms[0].txtFilter.value = selectedFilter; // + ' - Search like: ' + Search;
            */
            document.forms[0].txtSearch.value = Search;
            document.forms[0].submit();
        }

        $(document).ready(function () {
            var Topic = $('#txtTopic').val();
            if (Topic == "") {
                Topic = "Users";
            }
            $('.LeftTable td[onclick*="\'' + Topic + '\'"]').css('font-weight', 'bold');
            
            $(window).on('beforeunload', function () {
                event.preventDefault();
                gridSaveGrids({
                    oninvalid: function () { $(window).off('beforeunload'); }
                });
                //event.returnValue = '';
            });
            
            // search users
            var SearchUsers = GetCookie('SearchUsers');

            if (SearchUsers != "") {
                if (document.getElementById("txtSearchText") != null) {
                    document.getElementById("txtSearchText").value = SearchUsers;
                }

                // highlight
                $('#tabGrid tbody:last tr td:not(:has("a"))').each(function () {
                    $(this).html($(this).html().replace(new RegExp(SearchUsers, 'ig'), '<span class="highlightsearchterm">$&</span>'));
                });
            }

            $('#txtSearchText').closest('li').css('display', 'none');

            if (Topic == 'Users') {
                $('#txtSearchText').closest('li').css('display', '');
                $('#txtSearchText').focus();
            }

            PopEditor('Grid');

        });

        

    </script>
</asp:Content>


