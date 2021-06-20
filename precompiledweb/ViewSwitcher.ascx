<%@ control language="C#" autoeventwireup="true" inherits="ViewSwitcher, App_Web_v1e322iu" %>
<div id="viewSwitcher">
    <%: CurrentView %> view | <a href="<%: SwitchUrl %>" data-ajax="false">Switch to <%: AlternateView %></a>
</div>