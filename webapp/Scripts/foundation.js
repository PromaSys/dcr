var LastFunction;
var LastPopPickere;
var LastPPopPickerf;
var LastPopPickerw;
var NoExec = false;

var LastPopSpreade;
var LastPopSpreadf;

var SpreadOD = [];

/*
var SpreadInterval;

var SpreadInterval = window.setInterval(function () {
    var today = new Date();
    var time = today.getHours() + ":" + today.getMinutes() + ":" + today.getSeconds();

    // get cookie
    var SpreadStatus = GetCookie('AppStatus');
    $('#spnSpreadStatus').html(time + ' ' + SpreadStatus);
}, 1000);
*/

function setHeight() {

    // only for site master pages
    if ($('.navbar').length == 0) return;

    var windowHeight = $(window).innerHeight();
    $('.pageheight').css('height', windowHeight - 105, 'overflow', 'auto');
    var menuHeight = $('.navbar').innerHeight();
    var footerHeight = $('.footer').innerHeight();
    if (footerHeight == null) {
        footerHeight = 0;
    }
    var AvailableClientHeight = (windowHeight * 1.0) - (menuHeight * 1.0) - (footerHeight * 1.0);

    //$('.tablediv').css('height', windowHeight - 105);
    SetCookie("AvailableClientHeight", AvailableClientHeight, 7);

    SetCookie("WindowInnerHeight", windowHeight, 1);
    SetCookie("WindowInnerWidth", $(window).innerWidth(), 1);

    SetCookie("BrowserZoom", window.devicePixelRatio || 1);

    ScaleMenu();
};

function FixedHeaders(s) {

    var selector = "table.fixedheader";

    if (s != null) {
        selector = s + ">.table.fixedheader";
    }

    // ie 11
    if (navigator.userAgent.indexOf('Trident') == -1) {
        //if (1 == 1) {

        $(selector).each(function () {

            var top = this.offsetTop + 1;

            var FixedRows = $(this).find('thead>tr');

            // get second row height
            FixedRows.each(function (index) {

                if ($(this).css('display') != 'none') {
                    var h = $(this).height();

                    $(this).find('>th').css('top', top + 'px');

                    $(this).find('>th').has('.SearchField').css('vertical-align', 'top');

                    top += h - 1;
                }
            });

        });

    }
    else {

        $(selector).each(function (index, value) {

            if ($(value).find('thead').length > 1) {
                $(value).find('thead:first').remove();
                // $('thead:first').remove();
            }

            var TableOffset = $(value).position();
            var x = TableOffset.left;
            var y = TableOffset.top;
            var sw = 0;
            var sh = 0;
            var sx = 0;
            var hx = 2;

            if (navigator.appName == "Microsoft Internet Explorer") {
                c = $(value).find('thead:last tr th').length * 1.0;
                //sw = 34.0 / c;
                sw = 10.0;
                sh = 1.0;
                sx = 0.0;
            }

            var hcfirst = parseFloat($(value).find('thead:last tr:first').css("height"));
            var hclast = parseFloat($(value).find('thead:last tr:last').css("height"));

            var thTopFirst = $(value).find('thead tr:first th:first').position().top + sh;
            var thTopLast = $(value).find('thead tr:last th:first').position().top + sh;

            var thtfirst = $(value).find('thead tr:first th');
            var thtlast = $(value).find('thead tr:last th');

            $(value).find('thead').clone().insertBefore('#' + value.id + ' thead').css({ 'background-color': 'white' });

            /*
            // replace th with tr in last header
            $('thead:last tr').each(function () {
                $(this).html($(this).html().replace(/th/gi, "td"));
            });
            */

            $(value).find('thead:first tr:first th').each(function (index, value) { $(value).css({ 'position': 'absolute', 'background-color': 'white', '_z-index': '1000' }) });
            $(value).find('thead:first tr:last th').each(function (index, value) { $(value).css({ 'position': 'absolute', 'background-color': 'white', '_z-index': '1000' }) });


            $(value).find('thead:first tr:last th').each(function (index, value) {
                if (index == thtlast.length - 1 && navigator.appName == "Microsoft Internet Explorer") {
                    sw += 16;
                }
                $(value).css({ 'top': thTopLast + 'px', 'left': (thtlast[index].offsetLeft + x) + 'px', 'width': (thtlast[index].offsetWidth - sw - sx) + 'px', 'height': (thtlast[index].offsetHeight + hx) + 'px' })
            });

            if (navigator.appName == "Microsoft Internet Explorer") {
                sw = 10;
            }

            $(value).find('thead:first tr:first th').each(function (index, value) {
                if (index == thtfirst.length - 1 && navigator.appName == "Microsoft Internet Explorer") {
                    sw += 16;
                }
                $(value).css({ 'top': thTopFirst + 'px', 'left': (thtfirst[index].offsetLeft + x) + 'px', 'width': (thtfirst[index].offsetWidth - sw - sx) + 'px', 'height': (thtfirst[index].offsetHeight + hx) + 'px' })
            });

        });
    }
}

$(document).ready(function () {

    setHeight();
    FixedHeaders();

    //initialize column sorting    
    $(document).find('.js-dynamitable').each(function () {

        $(this).find('thead:first tr:last th').addClass('js-sorter sortnew');

        $(this).dynamitable()
            //.addFilter('.js-filter')
            .addSorter('.js-sorter', 'asc')
            //.addSorter('.js-sorter-desc', 'desc')
            ;

    });

    // highlight active menu
    
    if ($('.nav-item').length > 0) { //bootstrap 4
        // remove active class
        $('.nav-item').removeClass('active');
        var page = window.location.pathname.split("/").pop();
        // set current page active
        $('.nav-item a[href="' + page + '"]').addClass('active');
    }
    else {
        
        $('.active').hide();
        var hash = window.location.pathname.split("/").pop();

        if (hash) {
            //var selectedTab = $('.nav li a[href="../' + hash + '"]');
            var selectedTab = $('.nav li a[href="' + hash + '"]');
            selectedTab.addClass("activetab");
        }
    }
    

    // support filters
    $('.hg').each(function () {
        //debugger;
        $('.MainMenuSearch').css('display', 'none');

        if ($(this).attr('ser') == 'menu') {
            //display main menu filters
            $('.MainMenuSearch').css('display', '');
        }

        if ($(this).attr('ser') == 'title') {
            //display main menu filters
            $('.TitleSearch').css('display', '');
        }
    });

    //main menu search text update
    var page = location.pathname.split('/').slice(-1)[0];
    var pageSearch = GetCookie('Search_' + page);

    if (pageSearch == '') pageSearch = null;

    // update menu search box
    $('#txtSearchText').val(pageSearch);

    // highlight finds
    if (pageSearch != null) {
    $('.hg tbody:last tr td:not(:has("a"))').each(function () {
        $(this).html($(this).html().replace(new RegExp(pageSearch, 'ig'), '<span class="searchterm">$&</span>'));
    });
    }


    $.support.transition = false;

    $('#tabvgParameters.vgTitle').css('text-align', 'center');

    // initialize datepicker
    $('.datepicker').datepicker({
        dateFormat: 'm/d/yy',
        autoclose: true,
        width: 300
    });

    // Expand and collapse row groups
    SetRowGroups();

    // highlight current row for hg
    $('.hg').on('mousedown', 'tbody tr', function (event) {
        if ($(this).parent().parent().hasClass('hgTitleSplitter')) return;
        $(this).addClass('hghighlight').siblings().removeClass('hghighlight');
    });

    // position left menu vertical scrollbar
    if (document.getElementById("divLeftMenu") != null) {
        document.getElementById("divLeftMenu").scrollTop = GetCookie("divLeftMenu");
    }

    // hg vertical scroll top
    var st = GetCookie('hgScrollTop');
    if (st != null && st != 'undefined') {
        document.getElementById("divBody").scrollTop = st;
        SetCookie('hgScrollTop', 0, -1);

        var kfv = GetCookie('hgHighlightedID');
        if (kfv != null) {
            $('tr[kfv="' + kfv + '"]').addClass('hghighlight');
        }
    }


    // After resize events
    var id;
    var startWidth = window.innerWidth; //get the original screen width

    $(window).resize(function () {
        clearTimeout(id);
        id = setTimeout(doneResizing, 500);
    });
    function doneResizing() {

        ResizeContent();
    }

});

// resize
function ResizeContent() {

    var oh = GetCookie("WindowInnerHeight");
    var ow = GetCookie("WindowInnerWidth");

    setHeight();
    FixedHeaders();
    //location.reload();

    var nh = $(window).innerHeight();
    var nw = $(window).innerWidth();

    var hx = nh / oh;
    var wx = nw / ow;

    // rescale background images
    /*
    $('.bgImage').each(function () {
        var d = this.id;
        $("#" + d).load(location.href + " #" + d);
    });
    */

    // rescale and relocate popups
    $('div.ui-dialog').each(function () {
        //this.style.width = (parseInt(this.style.width) * wx) + "px";
        //this.style.height = (parseInt(this.style.height) * hx) + "px";
        this.style.top = (parseInt(this.style.top) * hx) + "px";
        this.style.left = (parseInt(this.style.left) * wx) + "px";

    });
}

function ScaleMenu() {

    //if (window.navigator.userAgent.indexOf('Trident') > -1) { return; }

    if ($('.navbar-toggle:visible').length > 0) {
        $('.navbar-fixed-top').children().css('zoom', 1.0);
        $('.navbar-fixed-top').children().css('margin-top', '0px');
        return;
    }

    var nbWidth = $('.navbar-nav').width() + $('.navbar-header').width() + $('.navbar-right').width() + 240;
    var winWidth = $(window).innerWidth() - 80;
    if (nbWidth > winWidth) {
        var scale = winWidth / nbWidth;

        if (window.navigator.userAgent.indexOf('Trident') > -1) {
            // IE
            $('.navbar-fixed-top').css('height', '50px');
            $('.navbar-fixed-top').children().css('zoom', scale).css('width', (100 / scale) + '%'); //.css('height', '50px').css('border', '1px solid #000');;
            $('.navbar-fixed-top').children().css('margin-top', (((50 / scale) - 50) / 4) + 'px');
        }
        else {
            $('.navbar-fixed-top').children().css('zoom', scale);
            $('.navbar-fixed-top').children().css('margin-top', (((50 / scale) - 50) / 2) + 'px');
        }
    }
    else {
        $('.navbar-fixed-top').children().css('zoom', 1.0);
        $('.navbar-fixed-top').children().css('margin-top', '0px');
    }
}

//#region update hg changes
function GridRowClicked(e, gid, r)
{
    if (e.innerHTML.toString().toLowerCase().indexOf("input") > -1) return false;

    //get row being edited
    var re = $("table[id*=" + gid + "] tbody tr input").first();
    var reindex;

    if (re.length == 1)
    {
        reindex = re[0].parentNode.parentNode.rowIndex;

        __doPostBack('ctl00$MainContent$' + gid, 'Update$' + r);
    }
    else
    {
        __doPostBack('ctl00$MainContent$' + gid, 'Edit$' + r);
    }



}

function GetResultTextBox(e) {
    var result = e.value;
    //var resultkey = result;
    e.setAttribute("result", result);
    //e.setAttribute("resultkey", result);
}

function GetResultTextArea(e) {

    var result = e.value;

    if ($(e).attr('tm') == '1') { // tinymce
        e.setAttribute("result", result);
    }
    else {
        //var resultkey = result;
        result = result.replace(/\r\n/g, '<br>');
        result = result.replace(/\r?\n/g, '<br>');
        e.setAttribute("result", result);
    }
}

function GetResultSelect(e) {
    var resultkey = e.options[e.selectedIndex].value;
    var result = e.options[e.selectedIndex].innerHTML;

    /*
    if (result == "Additional Insured Endorsement") {
        var $t = $(e).closest('.table');
        var dc = $t("[class^='Certificates_']").eq[0].html();
    }
    */

    if (result == "Not in this list") {
        AddNewDropDownItem(e);
        return;
    }

    var Changed = false;

    if (resultkey != e.getAttribute("resultkey")) {
        Changed = true;
    }

    e.setAttribute("result", result);
    e.setAttribute("resultkey", resultkey);

    if (Changed) {
        FilterCascading(e);
    }
}

function GetResultMultiSelect(e) {

    //var $tb = $(e).closest('table');
    var cbks = $(e).find('input');
    var resultkey = "";
    var result = "";

    for (var i = 0; i < cbks.length; i++) {
        var cbk = cbks[i];
        if (cbk.checked) {
            resultkey += cbk.value + ", ";
            result += cbk.nextSibling.nodeValue + ", ";
        }
    }

    if (resultkey != "") {
        resultkey = resultkey.substr(0, resultkey.length - 2);
        result = result.substr(0, result.length - 2);
    }

    $(e).attr("result", result);
    $(e).attr("resultkey", resultkey);
}

function GetResultCheckBox(e) {

    var result = "";
    var resultkey = "";

    if (e.checked) {
        //result = "☑"; //"&#9745;";
        result = "✓";
        resultkey = "1";
    }
    else
    {
        //result = "☐"; //"&#9744;";
        result = "";
        resultkey = "0";
    }

    e.setAttribute("result", result);
    e.setAttribute("resultkey", resultkey);
}

function GetResultCalendar(e) {
    var result = e.value;
    //var resultkey = result;
    e.parentNode.setAttribute("result", result);
    //alert(e.value);
}

function UpdateTemplate(e) {

    var template = e.getAttribute('template');

    switch (template) {
        case 'text':
            GetResultTextBox(e);
            break;
        case 'checkbox':
            GetResultCheckBox(e);
            break;
        case 'select':
            GetResultSelect(e)
            break;
        case 'multipleselect':
            GetResultMultiSelect(e);
            break;
        case 'calendar':
            GetResultCalendar(e.childNodes[0]);
            break;
        case 'textarea':
            GetResultTextArea(e);
            break;
        case 'signature':
            GetResultSignature(e);
            break;
    }

}
//#endregion update hg changes

//#region datepicker
function InitializeDatePicker() {
    setTimeout("SetDatePicker()", 500);
}

function SetDatePicker() {
    return;
    $('.datepicker').datepicker({
        dateFormat: 'm/d/yy',
        autoclose: true /*,
        onSelect: function (t, e) {
            $(this).datepicker('destroy');
            setTimeout(function () { SetDatePicker(); }, 1000);
        }*/
    });
}
//#endregion datepicker


// hg pop edit fields
function hgEdit(e) {
    //debugger;
    //check if item is edited
    if ($(e).hasClass("RowEdit")) return;

    var valid = true;
    var Change = "";

    var $tb = $(e).closest('table');

    var $r = $tb.find('tbody tr.RowEdit');

    if ($r.length > 0) {
        var $Results = $r.find('.hgResult');
        var TableID = $tb[0].id;
        var kfv = $r.attr('kfv');

        // validate and save last edited
        valid = ProcessChange(TableID, kfv, $Results, "");

        if (valid) {
            /*
            $r.children('td').each(function (index) {
                $(this).attr('key', $Results.eq(index).attr('resultkey'));
                $(this).html($Results.eq(index).attr('result'));
            });
            */
            $Results.each(function () {
                $(this).parent('td').attr('key', $(this).attr('resultkey'));
                $(this).parent('td').html($(this).attr('result'));
            });

            $r.removeClass("RowEdit");
        }

    }

    if (valid) {

        // edit new row
        $(e).addClass("RowEdit");
        // get spec
        var $ths = $tb.find('.hgHeaderRow:first th');

        $(e).children('td').each(function (index) {

            var ftp = $ths.eq(index).attr('ftp');
            var fd = $ths.eq(index).attr('fd');
            var fde = $ths.eq(index).attr('fde');
            var ftpAttr = "";
            var id = $ths.eq(index).attr('id')

            // change the ID to the field name
            ftp = ftp.replace(/_id=/g, "id=");

            var Key = $(this).attr('key');
            //var Text = $(this).text();
            var Text = $(this).html();
            var OldKey = Key;
            var OldText = Text;

            if (ftp.indexOf("template='textarea'") > -1) {
                Text = $(this).html().replace(/<br>/g, "&#13;&#10;");
                OldText = Text;
            }

            if (Text == "" && fde != "") {
                Text = fde;
                OldText = "";
            }

            if (ftp != "") {
                ftpAttr += "class='hgResult' ";
                ftpAttr += "oresult='" + OldText + "' ";
                ftpAttr += "oresultkey='" + OldKey + "' ";
                ftpAttr += "result='" + Text + "' ";
                ftpAttr += "resultkey='" + Key + "' ";
                ftpAttr += "tdo='" + index + "' ";
                ftpAttr += "style='_width: 95%;' ";

                if (ftp.indexOf("template='select'") > -1) {
                    //ftpAttr += "_onchange='GetResultSelect(this);' ";
                    //ftpAttr = ftpAttr.replace("style='_width: 95%;'", "style='width: auto;'");
                    ftp = ftp.replace("value='" + Key + "'", "value='" + Key + "' selected='selected'")
                    ftp = ftp.replace(/<select/i, "<select " + ftpAttr);
                    ftp = ftp.replace('hgResult', 'hgResult form-control');
                }
                else if (ftp.indexOf("template='multipleselect'") > -1) {
                    //ftpAttr += "onchange='GetResultMultiSelect(this);' ";
                    var values = $(this).attr('key').split(", ");
                    for (var i = 0; i < values.length; i++)
                    {
                        ftp = ftp.replace("value='" + values[i] + "'", "value='" + values[i] + "' checked");
                    }
                    ftp = ftp.replace(/value=/gi, " _onclick='GetResultMultiSelect(this);' value=");

                    ftp = ftp.replace(/<table/i, "<table " + ftpAttr);
                }
                else if (ftp.indexOf("template='checkbox'") > -1) {
                    var cbvalue = $(this).attr('key');
                    if (cbvalue == "1" || cbvalue == "True") {
                        ftp = ftp.replace("value=", " checked value=");
                    }
                    ftp = ftp.replace(/value=/i, " _onclick='GetResultCheckBox(this);' value=");
                    ftp = ftp.replace(/<input/i, "<input " + ftpAttr);
                    //ftp = "<div class='checkbox'>" + ftp + "</div>";
                    //ftp = ftp.replace('hgResult', 'hgResult form-checkbox');

                }
                else if (ftp.indexOf("template='calendar'") > -1) {
                    //var datevalue = $(this).text();
                    //ftpAttr = ftpAttr.replace("class='", "class='col-sm-1 ");
                    //ftpAttr = ftpAttr.replace("style='_width: 95%;'", "style='width: 120px; padding: 0px;'");
                    ftp = ftp.replace(/<input/i, "<input value='" + Text + "'");
                    ftp = ftp.replace("class='", "class='datepicker ");
                    ftp = ftp.replace(/<div/i, "<div " + ftpAttr);

                    InitializeDatePicker();
                }
                else if (ftp.indexOf("template='textarea'") > -1) {
                    //var textarea = $(this).text();
                    //textarea = textarea.replace(/<br>/g, "\r\n");
                    ftp = ftp.replace(/\>\</i, ftpAttr + ">" + Text + "<");
                    ftp = ftp.replace('hgResult', 'hgResult form-control');
                }
                else if (ftp.indexOf("template='multiplepicker'") > -1) {

                    //ftp = ftp.replace(/key=/i, "_key=");
                    if (Text == "") {
                        Text = "None";
                    }
                    ftp = ftp.replace(/\>\</i, ">" + Text + "<");
                    ftp = ftp.replace(/<a /i, "<a " + ftpAttr);
                }
                else if (ftp.indexOf("template='picker'") > -1) {

                    ftp = ftp.replace(/\>\</i, ">" + Text + "<");
                    ftp = ftp.replace(/<a /i, "<a " + ftpAttr);
                }
            }
            else if (fd == "1") {
                ftp = Text;
            }
            else {
                var EditText = Text;
                EditText = EditText.replace(/\'/g, "&apos;");
                if (Key != "") { EditText = Key };
                var EditKey = "";

                var EditOldText = OldText;
                EditOldText = EditOldText.replace(/\'/g, "&apos;");
                if (OldKey != "") { EditOldText = OldKey };

                var EditOldKey = "";

                ftp = "<input template='text' class='hgResult' oresult='" + EditOldText + "' oresultkey='" + EditOldKey + "' result='" + EditText + "' resultkey='" + EditKey + "' _onkeyup='GetResultTextBox(this);' type='textbox' value='" + EditText + "' style='_width: 95%;' tdo='" + index + "' />";
                //ftp = "<input template='text' type='textbox' " ftpAttr
                ftp = ftp.replace('hgResult', 'hgResult form-control');
                //$(this).html("<input class='hgResult' oresult='" + EditText + "' oresultkey='" + EditKey + "' result='" + EditText + "' resultkey='" + EditKey + "' onkeyup='GetResultTextBox(this);' type='textbox' value='" + EditText + "' style='width: 95%;' />");
            }

            $(this).html(ftp);

        });

        $(e).find('select[onchange=\"FilterSelect(this);\"]').each(function (index, value) {
            // get selected target item value
            tid = $(value).attr("tid");
            var sv = $('#' + tid).find('option:selected').val();
            FilterSelect(value);
            $('#' + tid).find('option[value=\"' + sv + '\"]').attr('selected', 'selected');
        });

        if (typeof hgRowEdit == 'function') {
            hgRowEdit($tb[0], $(e).closest('tr'));
        }
    }
}

function hgEditOff(e) {

    var valid = true;
    var Change = "";

    var $tb = $(e).closest('table .hg');
    //var $tr = $tb.find('.hgHeaderRow:first');
    //var $ths = $tr.find('th');

    var $r = $tb.find('tbody tr.RowEdit');

    if ($r.length > 0) {
        var $Results = $r.find('.hgResult');
        var TableID = $tb[0].id;
        var kfv = $r.attr('kfv');
        valid = ProcessChange(TableID, kfv, $Results, "");

        if (valid) {
            /*
            $r.children('td').each(function (index) {
                $(this).attr('key', $Results.eq(index).attr('resultkey'));
                $(this).html($Results.eq(index).attr('result'));
            });
            */
            $Results.each(function () {
                $(this).parent('td').attr('key', $(this).attr('resultkey'));
                $(this).parent('td').html($(this).attr('result'));
            });

            $r.removeClass("RowEdit");
        }
    }

    return valid;
}

function htmlEscape(str) {
    return String(str)
        .replace(/&/g, '&amp;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#39;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/\//g, '&#x2F;');
}

function htmlUnescape(value) {
    return String(value)
        .replace(/&quot;/g, '"')
        .replace(/&#39;/g, "'")
        .replace(/&lt;/g, '<')
        .replace(/&gt;/g, '>')
        .replace(/&amp;/g, '&')
        .replace(/&#x2F;/g, '/');
}

function ResultEncode(Text) {

    if ($.isNumeric(Text)) {
        return Text;
    }

    Text = Text.replace(/&nbsp;/g, " ");
    Text = Text.replace(/&#160;/g, " ");
    Text = Text.replace(/\xA0/g, " ");
    Text = Text.replace(/\s+/g, " ");
    Text = Text.replace(/&#39;/g, "[APO]");
    Text = Text.replace(/\'/g, "[APO]");
    Text = Text.replace(/&quot;/g, "[QOT]");
    Text = Text.replace(/\"/g, "[QOT]");
    Text = Text.replace(/&rsquo;/g, "[APO]");
    Text = Text.replace(/&lt;/g, "[LT]");
    Text = Text.replace(/\</g, "[LT]");
    Text = Text.replace(/&gt;/g, "[GT]");
    Text = Text.replace(/\>/g, "[GT]");
    Text = Text.replace(/&at;/g, "[AT]");
    Text = Text.replace(/\@/g, "[AT]");
    Text = Text.replace(/&copy;/g, "(c)");
    Text = Text.replace(/&reg;/g, "(R)");
    Text = Text.replace(/&trade;/g, "(TM)");
    Text = Text.replace(/\|/g, "l");

    Text = Text.replace(/&egrave;/g, "[EG]");
    Text = Text.replace(/&eacute;/g, "[EA]");
    Text = Text.replace(/&ecirc;/g, "[EC]");
    Text = Text.replace(/&agrave;/g, "[AG]");

    Text = Text.replace(/&amp;/g, "[AMP]");
    Text = Text.replace(/\&/g, "[AMP]");
    Text = Text.replace(/\#/g, "[LB]");
    Text = Text.replace(/\+/g, "[PLUS]");

    return Text;
}

// cookies
function SetCookie(Name, Value, Days) {
    var ckyDate = new Date;
    ckyDate.setDate(ckyDate.getDate() + Days);
    document.cookie = Name + "=" + escape(Value) + "; expires=" + ckyDate.toGMTString() + "; Path=/";
}

function GetCookie(CookieName) {
    var results = document.cookie.match(CookieName + '=(.*?)(;|$)');

    if (results)
        return (unescape(results[1]));
    else
        return null;
}

//#region new grid functions
function gridPop(o) {

    /* usage
    gridPop({

        type: 'horizontal', // default or 'vertical' or 'spread' or 'chat' or 'docs'

        element: ,
        context: '',
        contextID: ,

        my: null,
        at: null,
        of: null,
        w: RelativePixels('w', .7, 1000),
        h: RelativePixels('w', .6, 500),
        readOnly: // 1 for readOnly, blank or any other value for edit
        title: 'Test',
        load: 'dialog.aspx', // page, default: Dialog.aspx
        data: {Test_}, // JSON, post data
        buttons: , // default Cancel and Save (saves grid)
        buttonsInTitle: , // true/false, default false
        saveFunction: ,// default function save grid, update interface, remove dialog
        afterSave: // function
    });
    */

    if (o.type == 'docs') {
        //PopDocDialog(o.context, o.contextID, null, null, o.w, o.h, o.element);
        gridDocDialog(o);
        return;
    }

    if (o.my == null) {
        o.my = "center";
    }

    if (o.at == null) {
        o.at = "center";
    }

    if (o.of == null) {
        o.of = window;
    }

    if (o.load == null) {
        o.load = "dialog.aspx";
    }

    if (o.data == null) {
        o.data = {};
    }

    if (o.w > 0 && o.w < 1) {
        o.w = RelativePixels('w', o.w);
    }

    if (o.h > 0 && o.h < 1) {
        o.h = RelativePixels('h', o.h);
    }

    if (o.type == 'chat') {

        $(o.element).addClass('CurrentChat');

        o.buttons = {
            'Add': {
                text: 'Add Comment',
                priority: 'primary',
                style: 'background: #428BCA; color: #fff;',
                click: function () {

                    var LastComment = $('#tabChatComment .hgResult').val();

                    if (LastComment != '') {
                        if (LastComment.length > 20) {
                            LastComment = LastComment.substr(0, 17) + '...';
                        }

                        $('.CurrentChat').text(LastComment).removeClass('CurrentChat');
                    }

                    gridSave({
                        id: 'ChatComment',
                        afterSave: function () {

                            $('#dia' + o.context).dialog('destroy').remove();
                            return;
                        }
                    });
                }
            }
            ,
            Cancel: {
                text: 'Cancel',
                priority: 'secondary',
                click: function () {
                    $('#dia' + o.context).dialog('destroy').remove();
                    return;
                }
            }
        };
    }

    if (o.buttons == null) {
        o.buttons = {
            Cancel: {
                text: 'Cancel', priority: 'secondary', click: function () {
                    $('#dia' + o.context).dialog('destroy').remove();
                }
            },
            'Save': {
                text: 'Save', priority: 'primary', style: 'background: #428BCA; color: #fff;', click: function () {
                    if (o.saveFunction != null) {
                        o.saveFunction();
                        return;
                    }
                    else {
                        gridSave({
                            id: $('#dia' + o.context + ' table:first').attr('id'),

                            afterSave: function (response) {
                                if (o.afterSave != null) {
                                    o.afterSave();
                                    return;
                                }
                                else {
                                    if (response.split('|')[0] != 'Error') {
                                        $('#dia' + o.context).dialog('destroy').remove();
                                    }
                                }
                            }
                        });
                    }

                    return;
                }
            }
        }
    }

    var dialogP = $("<div id='dia" + o.context + "' style='position:relative; margin: 0px;'><img id='imgWait' src='images/wait.gif' class='waitgif' /></div>").dialog({
        autoOpen: false,
        title: o.title,
        position: { my: o.my, at: o.at, of: o.of },
        height: o.h,
        width: o.w,
        dialogClass: 'no-close',
        modal: true,
        closeOnEscape: false,
        buttons: o.buttons,
        dialogObj: o
    });

    o.data.c = o.context;
    o.data.cid = o.contextID;
    o.data.h = o.h;
    o.data.w = o.w;
    o.data.t = o.title;
    o.data.type = o.type;

    // post 
    dialogP.load(o.load.replace(/.aspx/, ''), o.data);

    dialogP.dialog("open");

    if (o.buttonsInTitle) {
        CloneButtonsInTitle('dia' + o.context);
    }

    return false;
}


function gridPopS(context, type, title, data, w, h, load, saveFunction) {
    // context is the case statement that will be executed in Dialog.cs

    gridPop({
        context: context,
        type: type,
        title: title,
        data: data,
        load: load,
        saveFunction: saveFunction,
        w: w,
        h: h,
    })
}

function gridValidate(o) {

    /* usage
    gridValidate({
        id: ,
        gasOverride: , // true or false/undefined
        onvalid: , // function
        oninvalid: // function
    });
    */

    var gridid = o.id;
    var gasOverride = o.gasOverride;
    var onvalid = o.onvalid;
    var oninvalid = o.oninvalid;

    var changes = '';
    var valid = true;

    // determine type of grid

    // hg
    $('.hg#' + o.id).each(function () {

        // verify if auto save is on, check all rows for changes
        var gas = $(this).attr('gas');

        // get edited rows
        var $r = $(this).find('tbody tr.RowEdit');

        if ($r.length > 0) {
            /*
            if (gas != '1') {
                hgEdit(this);
            }
            */
            var $Results = $r.find('.hgResult');
            var kfv = $r.attr('kfv');

            changes += gridValidateRow({
                id: o.id,
                kfv: kfv,
                results: $Results,
                oninvalid: function () {
                    valid = false;
                    o.invalid();
                    return false;
                }
            });
        }
    });

    // vg
    $('.vg#' + o.id).each(function () {
        var $Results = $(this).find('.hgResult');
        var kfv = $(this).attr('kfv');

        changes += gridValidateRow({
            id: o.id,
            kfv: kfv,
            results: $Results,
            oninvalid: function () {
                valid = false;
                o.oninvalid();
                return false;
            }
        });
    });


    // editorform
    $('.hgeditformtable').each(function () {
        var $Results = $(this).find('.hgResult');
        var kfv = $(this).attr('kfv');

        changes += gridValidateRow({
            id: o.id,
            kfv: kfv,
            results: $Results,
            oninvalid: function () {
                valid = false;
                o.oninvalid();
                return false;
            }
        });
    });

    if (valid) {
        o.onvalid(changes);
    }
}

function gridValidateRow(o) {

    /* usage
    gridValidateRow({
        id:,
        kfv: ,
        results:,
        oninvalid:
    });
    */

    var valid = true;
    var Change = "";

    var $tb = $('#' + o.id);
    var $tr = $tb.find('.hgHeaderRow:first');
    var $ths = $tr.find('th');

    var $r = $tb.find("tr[kfv='" + o.kfv + "']");

    var IsNewRecord = $r.hasClass('hgNewRecord');

    var as = $tb.attr('as');

    o.results.each(function (index) {

        var $resulte = $(this);

        var ftemplate = $resulte.attr('template');

        // update
        UpdateTemplate($resulte[0]);

        var Result = $resulte.attr('resultkey');
        if (Result == "" || ftemplate == 'text') {
            Result = $resulte.attr('result');
        }

        var fe = "";
        var fn = "";
        var ft = "";
        var fs = "";
        var fc = "";
        var fr = "";
        var fi = "";
        var FieldLabel = "";

        //if (hgfID != '') {
        if ($resulte.attr('tdo') != null) {
            var TDIndex = $resulte.attr('tdo');
            $th = $ths.eq(TDIndex);

            fe = $th.attr('fe');
            fn = $th.attr('fn');
            ft = $th.attr('ft');
            fs = $th.attr('fs');
            fc = $th.attr('fc');
            fr = $th.attr('fr');
            fi = $th.attr('fi');
            FieldLabel = $th.text().replace(/\*/, "");
        }
        else {

            var $tdl = $resulte.closest('tr').find('.vgLabel');

            if ($tdl.length == 0 && $tb.find('thead th').attr('colspan') == 1) {
                $tdl = $resulte.closest('tr').prev().find('td.vgLabel');
            }

            // account for inline checkboxes
            if ($tdl.length == 0 && ftemplate == 'checkbox') {
                $tdl = $resulte.closest('tr').find('td.vgLabel');
            }

            fe = $tdl.attr('fe');
            fn = $tdl.attr('fn');
            ft = $tdl.attr('ft');
            fs = $tdl.attr('fs');
            fc = $tdl.attr('fc');
            fr = $tdl.attr('fr');
            fi = $tdl.attr('fi');
            FieldLabel = $tdl.text().replace(/\*/, "");

            // unformat
            if ($tdl.attr('fnf').indexOf('%') > -1) {
                Result = Result.replace('#', '').replace(',', '').replace('%', '');
                if ($.isNumeric(Result)) {
                    Result = ((Result) / 100) + '';
                }
            } else if ($tdl.attr('fnf').indexOf('#') > -1) {
                Result = Result.replace('#', '').replace(',', '');
            }
        }

        if (fi == '1') return true;

        $resulte.css('background-color', 'white');

        // check for required for existing data
        if (fr == "1" && Result == "" && valid == true && !IsNewRecord) {
            $resulte.css('background-color', '#FCF8E3');
            //MessageBox("Validation: Required Field Empty", "You missed making an entry to a required field (" + FieldLabel + ").<br /><br />Please make an entry.", "warning");
            gridBox({ type: "message", title: "Validation: Required Field Empty", body: "You missed making an entry to a required field (" + FieldLabel + ").<br /><br />Please make an entry.", type: "warning" });
            // restore old values
            $resulte.attr('resultkey') == $resulte.attr('oresultkey');
            $resulte.attr('result') == $resulte.attr('oresult');
            valid = false;
            return valid;
        }

        //if (($resulte.attr('oresultkey') != $resulte.attr('resultkey') || $resulte.attr('oresult').replace(/\r\n/g, "<br>") != $resulte.attr('result')) && valid == true) {
        if (($resulte.attr('oresultkey') != $resulte.attr('resultkey') || $resulte.attr('oresult') != $resulte.attr('result')) && valid == true) {

            if (Result != "") {
                // validate string length
                if (ft == "s" && fs > 0 && Result.length > fs && ftemplate == 'text') {
                    $resulte.css('background-color', '#FCF8E3');
                    //MessageBox("Validation: Text Too Long", "The text you entered is longer than the maximum length allowed (" + fs + " characters).<br /><br />Please edit.", "warning");
                    gridBox({ type: "message", title: "Validation: Text Too Long", body: "The text you entered is longer than the maximum length allowed (" + fs + " characters).<br /><br />Please edit.", type: "warning" });
                    // restore old values
                    $resulte.attr('resultkey') == $resulte.attr('oresultkey');
                    $resulte.attr('result') == $resulte.attr('oresult');
                    valid = false;
                    return valid;
                }

                // Validate numbers
                if (ft == "n" && !$.isNumeric(Result) && ftemplate == 'text') {
                    $resulte.css('background-color', '#FCF8E3');
                    //MessageBox("Validation: Not a Number", "The text you entered is not a number. A number is expected.<br /><br />Please edit.", "warning");
                    gridBox({ type: "message", title: "Validation: Not a Number", body: "The text you entered is not a number. A number is expected.<br /><br />Please edit.", type: "warning" });
                    // restore old values
                    $resulte.attr('resultkey') == $resulte.attr('oresultkey');
                    $resulte.attr('result') == $resulte.attr('oresult');
                    valid = false;
                    return valid;
                }

                // Validate dates
                if (fc == "1" && isNaN(Date.parse(Result))) {

                    // autocomplete dates
                    if (!isNaN(Date.parse(Result + '/' + (new Date).getFullYear()))) {
                        Result = Result + '/' + (new Date).getFullYear();
                        $resulte.attr('result', Result);
                    }
                    else {
                        $resulte.css('background-color', '#FCF8E3');
                        //MessageBox("Validation: Not a Date", "The text you entered is not a date. A date is expected.<br /><br />Please edit.", "warning");
                        gridBox({ type: "message", title: "Validation: Not a Date", body: "The text you entered is not a date. A date is expected.<br /><br />Please edit.", type: "warning" });
                        // restore old values
                        $resulte.attr('resultkey') == $resulte.attr('oresultkey');
                        $resulte.attr('result') == $resulte.attr('oresult');
                        valid = false;
                        return valid;
                    }
                }
            }

            Change += "<fv f='" + fn + "' v='" + ResultEncode(Result) + "' t='" + ft + "' e='" + fe + "' />";
        }

    });


    if (valid) {

        if (Change != "") {

            // check for required for new records
            //if (IsNewRecord && 1==2) {
            if (IsNewRecord) {

                o.results.each(function (index) {

                    var $resulte = $(this);

                    var Result = $resulte.attr('resultkey');
                    if (Result == "") {
                        Result = $resulte.attr('result');
                    }

                    var fr = $ths.eq(index).attr('fr');
                    var FieldLabel = $ths.eq(index).text().replace(/\*/, "");

                    if (fr == "1" && Result == "") {
                        $resulte.css('background-color', '#FCF8E3');
                        //MessageBox("Validation: Required Field Empty", "You missed making an entry to a required field (" + FieldLabel + ").<br /><br />Please make an entry.", "warning");
                        gridBox({ type: "message", title: "Validation: Required Field Empty", body: "You missed making an entry to a required field (" + FieldLabel + ").<br /><br />Please make an entry.", type: "warning" });
                        // restore old values
                        $resulte.attr('resultkey') == $resulte.attr('oresultkey');
                        $resulte.attr('result') == $resulte.attr('oresult');
                        valid = false;
                        return valid;
                    }


                });
            }

            if (valid) {
                Change = "<data tb='" + $tb.attr('tb') + "' kf='" + $tb.attr('kf') + "' kfv='" + o.kfv + "' as='" + as + "' >" + Change + "</data>";
                //alert(Change);
                //hgSaveChange(Change, $r, $Results, ModalID, cbParameters);
            }
        }
        else {
            // set post save flag in Grid
            $r.closest('.hg, .vg').attr('ps', '1');
            o.results.closest('.hg, .vg').attr('ps', '1');

            if (typeof (cbParameters) == 'function') {
                cbParameters();
                return;
            }
        }
    }


    if (!valid) {
        o.oninvalid();
        return;
    }


    return Change;

}

function gridSave(o) {
    /* usage
    gridSave({
        id: ,
        gasOverride: , // true or false/undefined
        afterSave: // function 
    });
    */

    if (o.id.substr(0, 3) != 'tab') {
        o.id = 'tab' + o.id;
    }

    var gasOverride = o.gasOverride;

    var grid = $('#' + o.id);
    var gas = grid.attr('gas');

    if (grid.hasClass('vg')) {
        gas = '1';
    }

    if (!gasOverride) {
    gridValidate({
        id: o.id,
        onvalid: function (changes) {
                // save if gas = 1 or gasOverride
                if (gas == '1' && !gasOverride) {
            gridPostChanges({
                changes: changes,
                success: function (response) {
                    var r = response.split('|');
                    if (r[0] == 'Error') {
                        //WaitBoxClose();                        //WaitBoxClose();
                        gridBox({ closeBox: true });
                        //MessageBox("Error Saving Data", response.replace('Error|', '') + "<br /><br />Please edit or cancel.", "danger");
                        gridBox({ type: "message", title: "Error Saving Data", body: response.replace('Error|', '') + "<br /><br />Please edit or cancel.", type: "danger" });
                        return false;
                    }
                    else {
                        if (typeof (o.afterSave) == 'function') {
                            o.afterSave(response);
                        }
                        else {
                            return true;
                        }
                    }
                },
                error: function (response) {
                    //WaitBoxClose();
                    gridBox({ closeBox: true });
                    //MessageBox("Error Saving Data", response.replace('Error|', '') + "<br /><br />Please edit or cancel.", "danger");
                    gridBox({ type: "message", title: "Error Saving Data", body: response.replace('Error|', '') + "<br /><br />Please edit or cancel.", type: "danger" });
                    return false;
                }
            });
                } else {
                    if (typeof (o.afterSave) == 'function') {
                        o.afterSave();
                    }
                    else {
                        return true;
                    }
                }
        },
        oninvalid: function () {
            return false;
        }
    });
    } else {

        //hide as we iterate through rows
        //$(grid).find('tbody:last').hide();

        $r = $(grid).find('tbody:last tr');

        var changes = '';

        $r.each(function () {

            gridRowPopFields({ element: this });

            if ($(this).find('.hgResult:not([result=""]).hgResult:not([resultkey="0"])').length > 0) {

                var $Results = $(this).find('.hgResult');

                // reset old results
                $Results.attr('oresult', '');
                $Results.attr('oresultkey', '');

                var kfv = $r.attr('kfv');

                // get changes
                changes += gridValidateRow({
                    id: o.id,
                    kfv: kfv,
                    results: $Results,
                    oninvalid: function () {
                        valid = false;
                        o.invalid();
                        return false;
                    }
                });
            }
        });

        if (changes != '') {
            //$(hgrid).find('tbody:last').show();

            gridPostChanges({
                changes: changes,
                success: function (response) {
                    var r = response.split('|');
                    if (r[0] == 'Error') {
                        //WaitBoxClose();                        //WaitBoxClose();
                        gridBox({ closeBox: true });
                        //MessageBox("Error Saving Data", response.replace('Error|', '') + "<br /><br />Please edit or cancel.", "danger");
                        gridBox({ type: "message", title: "Error Saving Data", body: response.replace('Error|', '') + "<br /><br />Please edit or cancel.", type: "danger" });
                        return false;
                    }
                    else {
                        if (typeof (o.afterSave) == 'function') {
                            o.afterSave(response);
                        }
                        else {
                            return true;
                        }
                    }
                },
                error: function (response) {
                    //WaitBoxClose();
                    gridBox({ closeBox: true });
                    //MessageBox("Error Saving Data", response.replace('Error|', '') + "<br /><br />Please edit or cancel.", "danger");
                    gridBox({ type: "message", title: "Error Saving Data", body: response.replace('Error|', '') + "<br /><br />Please edit or cancel.", type: "danger" });
                    return false;
                }
            });

        }
    }
}

function gridSaveGrids() {

    //saves all grids on a page

    /* usage
    gridSaveGrids({
       oninvalid: // function 
    });
    */

    // save all hg and vg
    var allChanges = '';
    $('.hg, .vg').each(function (index, hgrid) {
        gridValidate({
            id: this.id,
            onvalid: function (changes) {
                //allChanges += changes;
                gridPostChanges({
                    changes: changes,
                    success: function (response) {
                        var r = response.split('|');
                        if (r[0] == 'Error') {
                            //WaitBoxClose();                        //WaitBoxClose();
                            gridBox({ closeBox: true });
                            //MessageBox("Error Saving Data", response.replace('Error|', '') + "<br /><br />Please edit or cancel.", "danger");
                            gridBox({ type: "message", title: "Error Saving Data", body: response.replace('Error|', '') + "<br /><br />Please edit or cancel.", type: "danger" });
                            return false;
                        }
                        else {
                            if (typeof (o.afterSave) == 'function') {
                                o.afterSave(response);
                            }
                            else {
                                return true;
                            }
                        }
                    },
                    error: function (response) {
                        //WaitBoxClose();
                        gridBox({ closeBox: true });
                        //MessageBox("Error Saving Data", response.replace('Error|', '') + "<br /><br />Please edit or cancel.", "danger");
                        gridBox({ type: "message", title: "Error Saving Data", body: response.replace('Error|', '') + "<br /><br />Please edit or cancel.", type: "danger" });
                        return false;
                    }
                });
            },
            oninvalid: function () {
                o.invalid();
            }
        });
    });
}

function gridPostChanges(o) {
    // usage
    /*
     gridPostChanges({
        changes: ,
        success: ,// function
        newid: ,
        error:
     });
     */

    if (o.changes == '') {
        o.success("Status|Success");
        return;
    }

    //Encode
    changes = o.changes.replace(/\</g, "-[_").replace(/\>/g, "_]-");

    // save
    $.ajax({
        type: 'POST',
        url: 'Process_Change',
        async: true,
        processData: false,
        data: o.changes,
        success: function (response) {
            o.success(response);
        },
        error: function (response) {
            o.error(response);
        }
        /*

            function (r) {

            // parse response
            var Result = r.split("|");

            if (Result[0] == 'Error') {
                WaitBoxClose();
                MessageBox("Error Saving Data", r.replace('Error|', '') + "<br /><br />Please edit or cancel.", "danger");
                return;
            }

            if (PickerElement != null) {

                if (Result[0] == 'NewID') {
                    NewIDs = Result[3];
                }

                var FieldForm = $(PickerElement).closest('.vg, .hg');

                var kf = FieldForm.attr('kf');
                var kfv = FieldForm.attr('kfv');
                var tb = FieldForm.attr('tb');

                PickerCurrentIDs = spread.getColumnData(keyfieldindex).filter(Boolean).join(',');

                // spread update
                var jqxhr = $.getJSON("Process_Request.aspx", {
                    action: "PickerLink",
                    cids: PickerCurrentIDs,
                    nid: NewIDs,
                    form: PickerForm,
                    field: PickerField,
                    ls: 'PC',
                    kf: kf,
                    kfv: kfv,
                    table: tb
                })
                    .done(function (data) {

                        $(PickerElement).attr('resultkey', data.LinkIDs);
                        $(PickerElement).text(data.LinkText);

                        $('#diaSpread' + PickerForm).dialog('destroy').remove();

                        // focus on next fields
                        if (f.indexOf('Product_Designer_') == -1) {

                            //var nr = $(e).closest('tr').next('tr').find('.hgResult');
                            var nr = $(PickerElement).closest('tr').nextAll('tr:visible:first').find('.hgResult');

                            if (nr.find('input').length == 1) {
                                nr.find('input').caretToEnd();
                            }
                            else if (nr.is('a')) {
                                //nr.focus();
                                //setTimeout(function () { nr.click(); }, 500);
                                setTimeout(function () {
                                    nr.focus();

                                    if (nr.text() == 'None') {
                                        nr.click();
                                    }
                                },
                                    500);
                            }
                            else {
                                nr.focus();
                            }
                        }

                        WaitBoxClose();

                        //alert("done");
                    }).fail(function (data) {
                        WaitBoxClose();
                        alert("PickerLink failed.");
                    });
            }

            if (typeof (afterSubmitChanges) == 'function') {
                var CurrentIDs = spread.getColumnData(keyfieldindex).filter(Boolean).join(',');
                if (Result[0] == 'NewID') {
                    NewIDs = Result[3];
                }
                afterSubmitChanges(CurrentIDs, NewIDs);
            }

        },
        error: function (response) {
            WaitBoxClose();
            MessageBox("Error Saving Data", "Please edit or cancel.", "danger");
            return;
        }
        */
    });
}

// creates single row edit form in an hg
function gridEditorForm(o) {

    /* usage
    gridEditorForm({
        element: ,
        w: RelativePixels('w', 1, 400),
        h: RelativePixels('h', 1, 600),
        afterLoad: // function optional
    });
    */


    //return;
    var $r = $(o.element).closest('tr');
    var key = $r.attr('kfv');
    $t = $(o.element).closest('table');
    var TableID = $t.attr('id');
    var $ths = $t.find('.hgHeaderRow:first th');
    var $title = $t.find('.hgTitle:first');
    var Title = $title.html();
    Title = Title.substring(0, Title.lastIndexOf("New"));
    var hgfID = $t.attr('id').replace(/tab/, "hgf");
    var Reference = $(o.element).closest('td').text();
    var HTML = "";

    if (Title == "") {
        // closest ui title
        Title = $('.ui-dialog-title').html();
    }

    if ($t.hasClass('grouped')) {
        Title = $title.find('td:first').text().trim();
    }
    else if ($title.find('span:first').length > 0) {
        Title = $title.find('span:first').html();
    }
    else if ($title.find('.hgTitleSplitter').length > 0) {
        Title = $title.find('td:first').html();
    }
    else if (Title == null) {
        Title = $('.hg thead tr:first').text();
    }

    if (Reference == "" || Reference == null) {
        Reference = "New";
    }

    $r.children('td').each(function (index) {

        var ftp = $ths.eq(index).attr('ftp');
        var fd = $ths.eq(index).attr('fd');
        var fde = $ths.eq(index).attr('fde');
        var fhe = $ths.eq(index).attr('fhe');

        var ftpAttr = "";
        var id = $ths.eq(index).attr('id')
        var fai = $ths.eq(index).attr('fai');

        // change the ID to the field name
        ftp = ftp.replace(/_id=/g, "id=");

        var Key = $(this).attr('key');
        var Text = $(this).text();
        var OldKey = Key;
        var OldText = Text;


        if (ftp.indexOf("template='textarea'") > -1) {
            if (ftp.indexOf("tm='1'") > -1) {
                Text = $(this).html().replace(/'/g, '&apos;');
            }
            else {
                Text = $(this).html().replace(/<br>/g, "&#13;&#10;");
            }
            OldText = Text;
        }

        if (Text == "" && fde != "") {
            Text = fde;
            OldText = "";
        }

        if (fd == "1" && $(this).html().indexOf("PopDocDialog") > -1) {
            ftp = $(this).html();
            HTML += "<tr><td>" + $ths.eq(index).html() + "</td><td>" + ftp + "</td></tr>";
        }
        else if (fai == "1") {
            if (key == '-1') {
                HTML += "<tr><td>" + $ths.eq(index).html() + "</td><td>" + unescape(fde).replace(/\+/g, ' ') + "</td></tr>";
            }
            else {
                HTML += "<tr><td>" + $ths.eq(index).html() + "</td><td>" + $(this).html() + "</td></tr>";
            }
        }
        else if (fd != "1") {
            if (ftp != "") {
                ftpAttr += "class='hgResult' ";
                ftpAttr += "oresult='" + OldText + "' ";
                ftpAttr += "oresultkey='" + OldKey + "' ";
                ftpAttr += "result='" + Text + "' ";
                ftpAttr += "resultkey='" + Key + "' ";
                ftpAttr += "tdo='" + index + "' ";
                ftpAttr += "style='_width: 95%;' ";

                if (ftp.indexOf("template='select'") > -1) {
                    ftpAttr += "onchange='GetResultSelect(this);' ";
                    ftp = ftp.replace("value='" + Key + "'", "value='" + Key + "' selected='selected'")
                    ftp = ftp.replace(/<select/i, "<select " + ftpAttr);
                    ftp = ftp.replace('hgResult', 'hgResult form-control');
                }
                else if (ftp.indexOf("template='multipleselect'") > -1) {
                    //ftpAttr += "onchange='GetResultMultiSelect(this);' ";
                    var values = Key.split(", ");
                    for (var i = 0; i < values.length; i++) {
                        ftp = ftp.replace("value='" + values[i] + "'", "value='" + values[i] + "' checked");
                    }
                    ftp = ftp.replace(/value=/gi, " onclick='GetResultMultiSelect(this);' value=");

                    ftp = ftp.replace(/<table/i, "<table " + ftpAttr);
                }
                else if (ftp.indexOf("template='checkbox'") > -1) {
                    var cbvalue = Key;
                    if (cbvalue == "1" || cbvalue == "True") {
                        ftp = ftp.replace("value=", " checked value=");
                    }
                    ftp = ftp.replace(/value=/i, " onclick='GetResultCheckBox(this);' value=");
                    ftp = ftp.replace(/<input/i, "<input " + ftpAttr);
                    //ftp = "<div class='checkbox'>" + ftp + "</div>";
                    //ftp = ftp.replace('hgResult', 'hgResult form-control');

                }
                else if (ftp.indexOf("template='calendar'") > -1) {
                    //ftpAttr = ftpAttr.replace("class='", "class='col-sm-1 ");
                    //ftpAttr = ftpAttr.replace("style='_width: 95%;'", "style='width: 120px;padding: 0px;'");
                    ftp = ftp.replace(/<input/i, "<input value='" + Text + "'");
                    ftp = ftp.replace("class='", "class='datepicker ");
                    ftp = ftp.replace(/<div/i, "<div " + ftpAttr);

                    InitializeDatePicker();
                    //ftp = ftp.replace('hgResult', 'hgResult form-control');
                }
                else if (ftp.indexOf("template='textarea'") > -1) {
                    //var textarea = $(this).html();
                    //textarea = textarea.replace(/\r\n/g, "&#13;&#10;");
                    //textarea = textarea.replace(/\n/g, "&#10;");
                    //textarea = textarea.replace(/<br>/g, "&#13;&#10;");

                    //ftpAttr = ftpAttr.replace("class='", "class='col-sm-1 ");
                    //ftpAttr = ftpAttr.replace("style='_width: 95%;'", "style='width: 120px; padding: 0px;'");
                    //ftp = ftp.replace(/\>\</i, ftpAttr + ">" + textarea + "<");
                    ftp = ftp.replace(/\>\</i, ftpAttr + ">" + Text + "<");
                    /*
                    if (fhe == '1') {
                        ftp = "<textarea template='textarea' tm='1' rows='20' " + ftpAttr + ">" + Text + "</textarea>";
                    }
                    else {
                        ftp = "<textarea template='textarea' rows='6' " + ftpAttr + ">" + Text + "</textarea>";
                    }
                    */
                    ftp = ftp.replace('hgResult', 'hgResult form-control');
                }
            }
            else if (fd == "1") {
                ftp = Text;
            }
            else {
                var EditText = Text;
                EditText = EditText.replace(/\'/g, "&apos;");
                if (Key != "") { EditText = Key };

                var EditKey = "";

                var EditOldText = OldText;
                EditOldText = EditOldText.replace(/\'/g, "&apos;");
                if (OldKey != "") { EditOldText = OldKey };

                var EditOldKey = "";

                ftp = "<input template='text' class='hgResult' oresult='" + EditOldText + "' oresultkey='" + EditOldKey + "' result='" + EditText + "' resultkey='" + EditKey + "' onkeyup='GetResultTextBox(this);' type='textbox' value='" + EditText + "' style='_width: 95%;' tdo='" + index + "' />";
                ftp = ftp.replace('hgResult', 'hgResult form-control');
                //$(this).html("<input class='hgResult' oresult='" + EditText + "' oresultkey='" + EditKey + "' result='" + EditText + "' resultkey='" + EditKey + "' onkeyup='GetResultTextBox(this);' type='textbox' value='" + EditText + "' style='width: 95%;' />");
            }

            //$(this).html(ftp);
            //HTML += "<div class='form-group'><label class='control-label col-sm-2' for='" + $ths.eq(index).text() + "<'>" + $ths.eq(index).text() + "</label><div class='col-sm-10'>" + ftp + "</div></div>";
            HTML += "<tr><td>" + $ths.eq(index).html() + "</td><td>" + ftp + "</td></tr>";
        }
    });

    if (HTML != "") {
        HTML = "<div id=\"diae" + TableID + "\" style=\"padding: 10px; width: 100%; height: " + o.h + "px; overflow: auto;\" ><table class='table table-condensed table-borderless hgeditformtable' kfv=\"" + key + "\" srctableid=\"" + TableID + "\">" + HTML + "</table>";

        dialog = $(HTML).dialog({
            open: function (event, ui) { ApplyCascades(this); },
            autoOpen: false,
            title: Title + " - " + Reference,
            height: o.h + 100,
            width: o.w,
            dialogClass: 'no-close',
            modal: true,
            buttons: {
                Cancel: {
                    text: 'Cancel', priority: 'secondary', click: function () {
                        $(this).dialog("destroy").remove();
                    }
                },
                'Save': {
                    text: 'Save', priority: 'primary', style: 'background: #428BCA; color: #fff;', kfv: key, click: function () {
                        gridSave({
                            id: TableID,
                            afterSave: function () {

                                // update interface
                                var results = $('.hgeditformtable .hgResult');
                                var kfv = $('.hgeditformtable').attr('kfv');

                                $hgTDs = $('#' + TableID + ' tr[kfv="' + kfv + '"]').children('td');

                                if (results != null) {
                                    results.each(function () {
                                        var TDIndex = $(this).attr('tdo');
                                        $hgTD = $hgTDs.eq(TDIndex);
                                        $hgTD.attr('key', $(this).attr('resultkey'));
                                        var link = $hgTD.find('a');
                                        if (link.length == 1) {
                                            link.html($(this).attr('result'));
                                        }
                                        else {
                                            $hgTD.html($(this).attr('result'));
                                        }
                                    });
                                    $('#diae' + TableID).dialog('destroy').remove();
                                }
                                else {
                                    $('#diae' + TableID).dialog('destroy').remove();
                                    ReloadPage();
                                }
                            }
                        });
                    }
                }
            }
        });
        dialog.dialog("open");

        if (key == "-1") {
            // remove
            $t.find("tr[kfv='-1']").remove();
        }

        if (typeof afterLoad != 'undefined') {
            afterLoad(this);
        }

        var fe = $(dialog).first('.hgResult');
        $('#diae' + TableID + ' .hgResult:first').caretToEnd();

        $('.datepicker').datepicker({
            dateFormat: 'm/d/yy',
            autoclose: true,
            width: 300
        });
    }
}

// creates new form for an hg
function gridEditorFormNew(o) {
    
    /* usage
        gridEditorFormNew({
            element: ,
            w:,
            h:
        });
    */

    //add new row
    $t = $(o.element).closest('.hg');
    var n = $t.find('tr:last td').length;
    // if no data exists
    if (n == 0) {
        n = $t.find('tr:last th').length;
    }
    var tds = '<tr kfv="-1" style="display: none;">';
    for (var i = 0; i < n; i++) {
        tds += '<td key></td>';
    }
    tds += '</tr>';
    if ($t.find('tbody').length > 0) {
        $t.find('tbody').append(tds);
    } else {
        $t.append(tds);
    }

    $t.attr('kfv', '-1');

    e = $t.find('tr:last td:first');

    //gridEditorForm(e, w, h);
    gridEditorForm({
        element: e,
        w: o.w,
        h: o.h
    });
}

// deletes a row from a hgrid
function gridDeleteRow(o) {

    //usage:
    //gridDeleteRow({
    //   element: element,
    //})


    var $r = $(o.element).closest('tr[kfv]');
    var kfv = $r.attr('kfv');
    var $tb = $r.closest('table');
    var c = "<data tb='" + $tb.attr('tb') + "' kf='" + $tb.attr('kf') + "' kfv='" + kfv + "' as='" + $tb.attr('kf') + "' de='1' ></data>";

    gridBox({
        boxType: "confirm", body: "Are you sure you want to delete this item?<br /><br />This action cannot be reversed.", width: 400, type: "warning", yesFunction: function () {


            var status = true;


            if ($($r).closest('.hg').attr('gas') == "0" && cbParameters == null) {
                return status;
            }
            //Encode
            c = c.replace(/\</g, "-[_").replace(/\>/g, "_]-");

            // get jquery
            $.ajax({
                type: 'POST',
                url: 'Process_Change',
                async: true,
                processData: false,
                data: c,
                success: function (response) {
                    console.log(response);
                    if (response.split('|')[1] == "Success") {
                        //gridBox({ boxType: 'message', body: 'Row deleted successfully', type: 'success' });
                        $r.remove()
                    }
                },
                error: function (response) {
                    //hgSaveChangeCallback(response, tr, results, ModalID, cbParameters);
                    if (response.split('|')[1] != "Success") {
                        gridBox({ boxType: 'message', body: 'Error deleting row', type: 'danger' });
                    }
                }
            });

            return status;
        }
    });

    //if (confirm("Are you sure you want to delete this record?\n\nThis action cannot be reversed.")) {
    //hgSaveChange(c, $r, null, ModalID);
    //}

}

function gridBox(o) {

    /* usage
    gridBox({
        boxType:        'message' is default    | options: 'message', 'wait', 'confirm'
        type:           'success' is default    | options: 'success', 'danger', 'info', 'warning'
        my:             'center'  is default
        at:             'center'  is default
        of:             window    is default
        width:          500       is default
        title:          'Message Box' is default
        body:           'Enter a body key' is default
        buttons:    
        buttonsInTitle: false is default false
        yesButtonText: ' Yes '
        noButtonText: ' No '
        yesFunction:    void function is default
        noFunction:     void function is default
        closeBox: if true, will close a wait box, to use with setTimeout or async code.
    });
    */

    //$('#MessageBox .modal-body').html(body);
    //$('#MessageBox .modal-title').html(title);

    if (o.closeBox == true) {
        $('#divWaitBox').dialog('destroy').remove();
        return;
    }

    var boxType = o.boxType || 'message';
    var title = o.title || 'Message Box';
    var attachingDiv = "";
    var type = o.type || "success";
    var body = o.body || "Enter a body key";

    var cancel = o.cancel;
    var cancelDisplay = 'none';


    //var confirmFunction = o.confirmFunction;
    var yesFunction = o.yesFunction || function () { }
    var noFunction = o.noFunction || function () { }
    //var closefunction = o

    var yesButtonText = o.yesButtonText || " Yes ";
    var noButtonText = o.noButtonText || " No ";

    if (o.boxType == 'message') {
        yesButtonText = ' OK ';
    }


    var width = o.width || 500;
    var my = o.my || "center";
    var at = o.at || "center";
    var of = o.of || window;

    // type
    var backgroundColor = '#337AB7';
    if (type == "danger") { backgroundColor = '#D9534F'; }
    if (type == "success") { backgroundColor = '#5CB85C'; }
    if (type == "info") { backgroundColor = '#5BC0DE'; }
    if (type == "warning") { backgroundColor = '#F0AD4E'; }

    if ($(window).innerWidth() < width) {
        width = $(window).innerWidth() * .9;
    }


    if (cancel == true) {
        cancelDisplay = '';
    }

    var noButtonStyle;

    //$('#MessageBox .modal-header').css('background-color', backgroundColor);

    //size
    //$('.hgeditformmodaldialog').css('width', w + "px");
    //$('MessageBox .modal-dialog').css('width', w + "px");

    // popup
    //$('#MessageBox').modal(); $('#MessageBox .modal-dialog').draggable({ handle: '.modal-header' });

    //buttons


    if (boxType == "message") {
        attachingDiv = "<div class='message-dialog' >" + body + "</div>";
        noButtonStyle = 'display: ' + cancelDisplay + '; '
    }
    else if (boxType == "wait") {
        attachingDiv = "<div id='divWaitBox' class='message-dialog' style='text-align: center;'><img id='imgWait' src='images/wait.gif' /><br /><br />" + body + "</div>";
    }
    else if (boxType == "confirm") {
        attachingDiv = "<div id='diaConfirm' class='confirm-dialog' >" + body + "</div>"
    }

    var buttons = {
        Cancel: {
            text: noButtonText, priority: 'secondary', style: noButtonStyle, click: function () {
                $(this).dialog('destroy');
                noFunction();
                return true;
            }
        },
        'OK': {
            text: yesButtonText, priority: 'primary', style: 'background: #428BCA; color: #fff;', click: function () {

                $(this).dialog('destroy');
                yesFunction();
                return true;
            }
        }
    }

    dialogM = $(attachingDiv).dialog({
        autoOpen: true,
        title: title,
        width: width,
        position: { my: my, at: at, of: of },
        dialogClass: 'no-close',
        modal: true,
        buttons: boxType != 'wait' ? buttons : null
    }).prev('.ui-dialog-titlebar').css('background', backgroundColor);

    //dialogM.find('.ui-dialog-buttonpane').show();



}

function gridBoxS(title, body, type, width) {

    // type | options: 'success', 'danger', 'info', 'warning'

    type === null ? 'message' : type; 

    width === null ? 400 : width;


    var object = {
        title: title,
        type: type,
        body: body,
        width: width,
        yesButtonText: 'OK'
    }

    gridBox(object)
}

function capitalizeFirstLetter(string) {
    return string.charAt(0).toUpperCase() + string.slice(1);
}

function gridDocDialog(o) {

    var Context = o.context;
    var ContextID = o.contextID;

    var w = o.w || 500;
    var h = o.h || 275;
    var x = o.x;
    var y = o.y;
    var e = o.element;

    var QS = "c=" + escape(Context) + "&cid=" + ContextID + "&w=" + w + "&h=" + h + "&t="; // + escape(Context);

    if (o.readOnly == '1') {
        QS += '&ro=1';
    }
    else {
        QS += '&ro=0';
    }

    //dialog = $("<div style='position:relative;'>").dialog({
    dialog = $("<div id='diaDocs' ><iframe id='ifrDocs' name='ifrDocs' style='width: " + (w - 2) + "px; height: " + (h - 135) + "px; margin: 0; padding: 0; border: 0px solid #000;' src='documents.aspx?" + QS + "' ></iframe></div>").dialog({
        autoOpen: false,
        title: Context + ' Documents',
        height: h,
        width: w,
        dialogClass: 'no-close',
        modal: true,
        closeOnEscape: false,
        buttons: {
            'Upload': {
                text: 'Select Document', priority: 'primary', style: 'background: #428BCA; color: #fff;', click: function () {
                    //document.frames["ifrDocs"].document.    BrowseFiles('File1');

                    $('#ifrDocs').contents().find('#File1').click();
                    //WaitBox('Uploading Document', 'Please wait', 'info');
                }
            }
            ,
            Cancel: {
                text: 'Close', priority: 'secondary', click: function () {
                    //dialog.dialog("close");
                    //return false;
                    //PostDocDialog(Context, ContextID);
                    if (typeof (o.uiUpdateFunction) == 'function') {
                        o.uiUpdateFunction(e);
                    }
                    else {
                        var docCount = $('#ifrDocs').contents().find('tbody tr').length;
                        $(e).text(docCount + ' Docs');
                        if (docCount == 0) {
                            $(e).text('None');
                        }
                    }

                    $(this).dialog('destroy').remove();
                }
            }
        }
    });


    //dialog.load("documents.aspx?" + QS);
    dialog.dialog("open");

    if ('ondrop' in document.createElement('div')) {

        var buttons = $('#diaDocs').parent().find('.ui-dialog-buttonset');

        if (o.readOnly == '1') {
            buttons.find('button:contains("Select Document")').css('display', 'none');
        }
        else {
            //buttons.before($('<div ondragenter="DocsDragEnter(window.event);" ondragover="DocsDragOver(window.event);" ondrop="DocsDrop(window.event);" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only" style="padding: 15px;" >Drag and Drop</div>'));
            buttons.before($('<span style="color: gray;">Click Select Document or Drag & Drop documents to panel above.</span>'));
        }
    }

    if (typeof (PopDocDialogTitle) == 'function') {
        PopDocDialogTitle(Context, ContextID, e);
    }

    return false;
}

function DocsDragEnter(e) {
    e.stopPropagation();
    e.preventDefault();
}

function DocsDragOver(e) {
    e.stopPropagation();
    e.preventDefault();
    $('#divDocs').css('background-color', 'lightblue');
}

function DocsDragLeave(e) {
    e.stopPropagation();
    e.preventDefault();
    $('#divDocs').css('background-color', 'white');
}

function DocsDrop(e) {
    e.stopPropagation();
    e.preventDefault();

    $('#divDocs').css('background-color', 'white');

    var selectedFiles = e.dataTransfer.files;
    //alert(selectedFiles.length);

    //WaitBox('Uploading', 'Uploading ' + selectedFiles.length + ' file(s)', 'info', 400);
    gridBox({ boxType: 'wait', type: 'info', body: 'Uploading ' + selectedFiles.length + ' file(s)', title: 'Uploading', width: 400 });

    var $ifrDocs = $("#ifrDocs");
    var loc;

    if ($ifrDocs.length == 0) {
        $ifrDocs = parent.$("#ifrDocs");
        loc = parent.$("#ifrDocs").contents()[0].location;
    }
    else {
        loc = $("#ifrDocs").contents()[0].location;
    }


    var data = new FormData($ifrDocs.contents().find('form')[0]);

    for (var i = 0; i < selectedFiles.length; i++) {
        data.append(selectedFiles[i].name, selectedFiles[i]);
    }

    $.ajax({
        type: "POST",
        url: loc.href,
        contentType: false,
        processData: false,
        data: data,
        success: function (result) {
            gridBox({ closeBox: true });
            loc.reload(true);
        },
        error: function () {
            alert("There was error uploading files!");
        }
    });

    //$("#ifrDocs").contents().find('form').submit();
    //$('#ifrDocs').contents().find('#butDocs').click();

}

function gridSaveChange(o) {
    var c = o.change;
    var tr = o.table_row;
    var results = o.results;
    var ModalID = o.modalId;

    var status = true;

    if ($(tr).closest('.hg').attr('gas') == "0" && cbParameters == null) {
        return status;
    }

    c = c.replace(/\</g, "-[_").replace(/\>/g, "_]-");

    $.ajax({
        type: 'POST',
        url: 'Process_Change',
        async: false,
        processData: false,
        data: c,
        success: function (response) {
            hgSaveChangeCallback(response, tr, results, ModalID, cbParameters);
        },
        error: function (response) {
            hgSaveChangeCallback(response, tr, results, ModalID, cbParameters);
        }
    });

    return status

}

function gridSetdPage(o) {
    /* -- usage
     gridSetPage({
        page: ,
        cookieName:
     });
    */

    SetCookie(o.cookieName, o.page, 1);
    document.forms[0].submit();
}

function gridEdit(o) {

    /* usage
    gridEdit({
        element:
    });
    */

    var e = o.element;

    //check if item is being edited
    if ($(e).hasClass("RowEdit")) return;

    var valid = true;
    var Change = "";

    var $tb = $(e).closest('table');

    var $r = $tb.find('tbody tr.RowEdit');

    if ($r.length > 0) {
        var $Results = $r.find('.hgResult');
        var TableID = $tb[0].id;
        var kfv = $r.attr('kfv');

        // validate and save last edited
        gridSave({
            id: TableID,
            afterSave: function () {
                $Results.each(function () {
                    $(this).parent('td').attr('key', $(this).attr('resultkey'));
                    $(this).parent('td').html($(this).attr('result'));
                });

                $r.removeClass("RowEdit");
            }
        });
    }

    // edit new row
    $(e).addClass("RowEdit");

    // get spec
    var $ths = $tb.find('.hgHeaderRow:first th');

    $(e).children('td').each(function (index) {

        var ftp = $ths.eq(index).attr('ftp');
        var fd = $ths.eq(index).attr('fd');
        var fde = $ths.eq(index).attr('fde');
        var ftpAttr = "";
        var id = $ths.eq(index).attr('id')

        // change the ID to the field name
        ftp = ftp.replace(/_id=/g, "id=");

        var Key = $(this).attr('key');
        //var Text = $(this).text();
        var Text = $(this).html();
        var OldKey = Key;
        var OldText = Text;

        if (ftp.indexOf("template='textarea'") > -1) {
            Text = $(this).html().replace(/<br>/g, "&#13;&#10;");
            OldText = Text;
        }

        if (Text == "" && fde != "") {
            Text = fde;
            OldText = "";
        }

        if (ftp != "") {
            ftpAttr += "class='hgResult' ";
            ftpAttr += "oresult='" + OldText + "' ";
            ftpAttr += "oresultkey='" + OldKey + "' ";
            ftpAttr += "result='" + Text + "' ";
            ftpAttr += "resultkey='" + Key + "' ";
            ftpAttr += "tdo='" + index + "' ";
            ftpAttr += "style='_width: 95%;' ";

            if (ftp.indexOf("template='select'") > -1) {
                //ftpAttr += "_onchange='GetResultSelect(this);' ";
                //ftpAttr = ftpAttr.replace("style='_width: 95%;'", "style='width: auto;'");
                ftp = ftp.replace("value='" + Key + "'", "value='" + Key + "' selected='selected'")
                ftp = ftp.replace(/<select/i, "<select " + ftpAttr);
                ftp = ftp.replace('hgResult', 'hgResult form-control');
            }
            else if (ftp.indexOf("template='multipleselect'") > -1) {
                //ftpAttr += "onchange='GetResultMultiSelect(this);' ";
                var values = $(this).attr('key').split(", ");
                for (var i = 0; i < values.length; i++) {
                    ftp = ftp.replace("value='" + values[i] + "'", "value='" + values[i] + "' checked");
                }
                ftp = ftp.replace(/value=/gi, " _onclick='GetResultMultiSelect(this);' value=");

                //ftp = ftp.replace(/<table/i, "<table " + ftpAttr);
                ftp = ftp.replace(/<table/i, "<table class" + ftpAttr);

                //limit height

                ftp = ftp.replace(/_width: 95%;/i, 'height: 150px; overflow: auto;');
                //ftp = '<div class="MultipleselectInline">' + ftp + '</div>';
            }
            else if (ftp.indexOf("template='checkbox'") > -1) {
                var cbvalue = $(this).attr('key');
                if (cbvalue == "1" || cbvalue == "True") {
                    ftp = ftp.replace("value=", " checked value=");
                }
                ftp = ftp.replace(/value=/i, " _onclick='GetResultCheckBox(this);' value=");
                ftp = ftp.replace(/<input/i, "<input " + ftpAttr);
                //ftp = "<div class='checkbox'>" + ftp + "</div>";
                //ftp = ftp.replace('hgResult', 'hgResult form-checkbox');

            }
            else if (ftp.indexOf("template='calendar'") > -1) {
                //var datevalue = $(this).text();
                //ftpAttr = ftpAttr.replace("class='", "class='col-sm-1 ");
                //ftpAttr = ftpAttr.replace("style='_width: 95%;'", "style='width: 120px; padding: 0px;'");
                ftp = ftp.replace(/<input/i, "<input value='" + Text + "'");
                ftp = ftp.replace("class='", "class='datepicker ");
                ftp = ftp.replace(/<div/i, "<div " + ftpAttr);

                InitializeDatePicker();
            }
            else if (ftp.indexOf("template='textarea'") > -1) {
                //var textarea = $(this).text();
                //textarea = textarea.replace(/<br>/g, "\r\n");
                ftp = ftp.replace(/\>\</i, ftpAttr + ">" + Text + "<");
                ftp = ftp.replace('hgResult', 'hgResult form-control');
            }
            else if (ftp.indexOf("template='multiplepicker'") > -1) {

                //ftp = ftp.replace(/key=/i, "_key=");
                if (Text == "") {
                    Text = "None";
                }
                ftp = ftp.replace(/\>\</i, ">" + Text + "<");
                ftp = ftp.replace(/<a /i, "<a " + ftpAttr);
            }
            else if (ftp.indexOf("template='picker'") > -1) {

                ftp = ftp.replace(/\>\</i, ">" + Text + "<");
                ftp = ftp.replace(/<a /i, "<a " + ftpAttr);
            }
        }
        else if (fd == "1") {
            ftp = Text;
        }
        else {
            var EditText = Text;
            EditText = EditText.replace(/\'/g, "&apos;");
            if (Key != "") { EditText = Key };
            var EditKey = "";

            var EditOldText = OldText;
            EditOldText = EditOldText.replace(/\'/g, "&apos;");
            if (OldKey != "") { EditOldText = OldKey };

            var EditOldKey = "";

            ftp = "<input template='text' class='hgResult' oresult='" + EditOldText + "' oresultkey='" + EditOldKey + "' result='" + EditText + "' resultkey='" + EditKey + "' _onkeyup='GetResultTextBox(this);' type='textbox' value='" + EditText + "' style='_width: 95%;' tdo='" + index + "' />";
            //ftp = "<input template='text' type='textbox' " ftpAttr
            ftp = ftp.replace('hgResult', 'hgResult form-control');
            //$(this).html("<input class='hgResult' oresult='" + EditText + "' oresultkey='" + EditKey + "' result='" + EditText + "' resultkey='" + EditKey + "' onkeyup='GetResultTextBox(this);' type='textbox' value='" + EditText + "' style='width: 95%;' />");
        }

        $(this).html(ftp);

    });

    $('.datepicker').datepicker({
        dateFormat: 'm/d/yy',
        autoclose: true,
        width: 300
    });

    $(e).find('select[onchange=\"FilterSelect(this);\"]').each(function (index, value) {
        // get selected target item value
        tid = $(value).attr("tid");
        var sv = $('#' + tid).find('option:selected').val();
        FilterSelect(value);
        $('#' + tid).find('option[value=\"' + sv + '\"]').attr('selected', 'selected');
    });

    // focus
    var $input = $(e).find('.datepicker,.hgResult,input').first().caretToEnd();

    if (typeof gridRowEdit == 'function') {
        //gridRowEdit($tb[0], $(e).closest('tr'));
        gridRowEdit({ element: e });
    }

}

function gridRowPopFields(o) {

    /* usage
    gridEdit({
        element:
    });
    */

    var e = o.element;

    $tb = $(e).closest('.hg');

    // collapse edit fields
    $tb.find('tbody tr.RowEdit').each(function () {
        $(this).children('td').each(function (index) {
            $(this).attr('key', $(this).find('.hgResult').attr('resultkey'));
            $(this).html($(this).find('.hgResult').attr('result'));
        });
    });    

    // edit new row
    $(e).addClass('RowEdit');

    // get spec
    var $ths = $tb.find('.hgHeaderRow:first th');

    $(e).children('td').each(function (index) {

        var ftp = $ths.eq(index).attr('ftp');
        var fd = $ths.eq(index).attr('fd');
        var fde = $ths.eq(index).attr('fde');
        var ftpAttr = "";
        var id = $ths.eq(index).attr('id')

        // change the ID to the field name
        ftp = ftp.replace(/_id=/g, "id=");

        var Key = $(this).attr('key');
        //var Text = $(this).text();
        var Text = $(this).html();
        var OldKey = Key;
        var OldText = Text;

        if (ftp.indexOf("template='textarea'") > -1) {
            Text = $(this).html().replace(/<br>/g, "&#13;&#10;");
            OldText = Text;
        }

        if (Text == "" && fde != "") {
            Text = fde;
            OldText = "";
        }

        if (ftp != "") {
            ftpAttr += "class='hgResult' ";
            ftpAttr += "oresult='" + OldText + "' ";
            ftpAttr += "oresultkey='" + OldKey + "' ";
            ftpAttr += "result='" + Text + "' ";
            ftpAttr += "resultkey='" + Key + "' ";
            ftpAttr += "tdo='" + index + "' ";
            ftpAttr += "style='_width: 95%;' ";

            if (ftp.indexOf("template='select'") > -1) {
                //ftpAttr += "_onchange='GetResultSelect(this);' ";
                //ftpAttr = ftpAttr.replace("style='_width: 95%;'", "style='width: auto;'");
                ftp = ftp.replace("value='" + Key + "'", "value='" + Key + "' selected='selected'")
                ftp = ftp.replace(/<select/i, "<select " + ftpAttr);
                ftp = ftp.replace('hgResult', 'hgResult form-control');
            }
            else if (ftp.indexOf("template='multipleselect'") > -1) {
                //ftpAttr += "onchange='GetResultMultiSelect(this);' ";
                var values = $(this).attr('key').split(", ");
                for (var i = 0; i < values.length; i++) {
                    ftp = ftp.replace("value='" + values[i] + "'", "value='" + values[i] + "' checked");
                }
                ftp = ftp.replace(/value=/gi, " _onclick='GetResultMultiSelect(this);' value=");

                ftp = ftp.replace(/<table/i, "<table " + ftpAttr);
            }
            else if (ftp.indexOf("template='checkbox'") > -1) {
                var cbvalue = $(this).attr('key');
                if (cbvalue == "1" || cbvalue == "True") {
                    ftp = ftp.replace("value=", " checked value=");
                }
                ftp = ftp.replace(/value=/i, " _onclick='GetResultCheckBox(this);' value=");
                ftp = ftp.replace(/<input/i, "<input " + ftpAttr);
                //ftp = "<div class='checkbox'>" + ftp + "</div>";
                //ftp = ftp.replace('hgResult', 'hgResult form-checkbox');

            }
            else if (ftp.indexOf("template='calendar'") > -1) {
                //var datevalue = $(this).text();
                //ftpAttr = ftpAttr.replace("class='", "class='col-sm-1 ");
                //ftpAttr = ftpAttr.replace("style='_width: 95%;'", "style='width: 120px; padding: 0px;'");
                ftp = ftp.replace(/<input/i, "<input value='" + Text + "'");
                ftp = ftp.replace("class='", "class='datepicker ");
                ftp = ftp.replace(/<div/i, "<div " + ftpAttr);

                InitializeDatePicker();
            }
            else if (ftp.indexOf("template='textarea'") > -1) {
                //var textarea = $(this).text();
                //textarea = textarea.replace(/<br>/g, "\r\n");
                ftp = ftp.replace(/\>\</i, ftpAttr + ">" + Text + "<");
                ftp = ftp.replace('hgResult', 'hgResult form-control');
            }
            else if (ftp.indexOf("template='multiplepicker'") > -1) {

                //ftp = ftp.replace(/key=/i, "_key=");
                if (Text == "") {
                    Text = "None";
                }
                ftp = ftp.replace(/\>\</i, ">" + Text + "<");
                ftp = ftp.replace(/<a /i, "<a " + ftpAttr);
            }
            else if (ftp.indexOf("template='picker'") > -1) {

                ftp = ftp.replace(/\>\</i, ">" + Text + "<");
                ftp = ftp.replace(/<a /i, "<a " + ftpAttr);
            }
        }
        else if (fd == "1") {
            ftp = Text;
        }
        else {
            var EditText = Text;
            EditText = EditText.replace(/\'/g, "&apos;");
            if (Key != "") { EditText = Key };
            var EditKey = "";

            var EditOldText = OldText;
            EditOldText = EditOldText.replace(/\'/g, "&apos;");
            if (OldKey != "") { EditOldText = OldKey };

            var EditOldKey = "";

            ftp = "<input template='text' class='hgResult' oresult='" + EditOldText + "' oresultkey='" + EditOldKey + "' result='" + EditText + "' resultkey='" + EditKey + "' _onkeyup='GetResultTextBox(this);' type='textbox' value='" + EditText + "' style='_width: 95%;' tdo='" + index + "' />";
            //ftp = "<input template='text' type='textbox' " ftpAttr
            ftp = ftp.replace('hgResult', 'hgResult form-control');
            //$(this).html("<input class='hgResult' oresult='" + EditText + "' oresultkey='" + EditKey + "' result='" + EditText + "' resultkey='" + EditKey + "' onkeyup='GetResultTextBox(this);' type='textbox' value='" + EditText + "' style='width: 95%;' />");
        }

        $(this).html(ftp);

    });

    $('.datepicker').datepicker({
        dateFormat: 'm/d/yy',
        autoclose: true,
        width: 300
    });

    // focus
    var $input = $(e).find('.datepicker,.hgResult,input').first().caretToEnd();

}

function _gridRowEdit(o) {
    /* usage
    gridRowEdit({
        element:
    });
    */

    //$tb[0], $(e).closest('tr')

    //function hgRowEdit(t, r) {
    var t = $(o.element).closest('table')

    if (t.id == 'tabGrid') {

        r.find('.hgResult').keydown(function (event) {
            var keyCode = (event.keyCode ? event.keyCode : event.which);
            //event.stopPropagation();

            if (keyCode == 40 && !r.is(':last-child')) {
                r.next('tr').click();
            }
            else if (keyCode == 38 && !r.is(':first-child')) {
                r.prev('tr').click();
            }
            else if (keyCode == 13 && !r.is(':last-child')) {
                r.next('tr').click();
            }
        });

        r.find('.datepicker,.hgResult').first().focus();

        var $input = r.find('.datepicker,.hgResult').first();
        var inputLength = $input.length;
        if (inputLength > 0) {
            try {
                $input[0].focus();
                $input[0].setSelectionRange(inputLength, inputLength);
            }
            catch (ex) {
                var a = 1;
            }
        }
    }
}



//#endregion new grid functions

function PopEditFields(t) {
    //$('#tab' + t + ' tr[kfv] td .hgResult').on('focus', function () { $(this).css('background-color', 'red'); });
    $('#tab' + t + ' tr[kfv]').first().click();
}

function PopEditor(TableID) {

    var fr = $('#tab' + TableID + ' tbody:last tr:first');
    fr.click();
}

function hgSaveScrollTops() {

    // save scroll position
    $('.hg').each(function () {
        var $div = $(this).closest('div');
        SetCookie('ScrollTop_' + $div.attr('id'), $div.scrollTop(), 1);
    });
}

function hgApplyScrollTops() {

    // save scroll position
    $('.hg').each(function () {
        var $div = $(this).closest('div');
        var st = GetCookie('ScrollTop_' + $div.attr('id'));
        if (st != null) {
            $div.scrollTop(st);
        }
    });
}

function vgUpdateByLabel(l, v) {
    var $r = $('.vgLabel:contains("' + l + '")').siblings().find('.hgResult');
    if ($r.is('input')) {
        $r.val(v);
        $r.attr("result", v);
    }
    else if ($r.is('select')) {
        var $o = $r.find('option:contains(' + v + ')');
        $o.attr('selected', true);
        $o.attr("resultkey", $o.val());
    }
    else if ($r.is('div')) {
        $r.find('input').val(v);
        $r.find('input').attr("result", v);
    }

}

function GetPairValue(text, find, w) {
    var delimiter = '|';
    if (text.indexOf(delimiter) == -1) {
        delimiter = ',';
    }

    // parse
    var pairs = text.split(delimiter);

    for (var i = 0; i < pairs.length; i = i + 2) {
        if (pairs[i].toLowerCase() == find.toLowerCase()) {
            w = pairs[i + 1];
            break;
        }
    }

    return w;
}

function gridApplyFilters_(e, setFilters) {

    // if setFilters are specified, then set
    if (setFilters != null) {
        if (setFilters != '') {
            // reset all filters
            $('.filter').each(function () {
                $(this).find('option').prop('selected', false);
            });

            // set filter
            var setFiltersValues = setFilters.split('|');

            for (var i = 0; i < setFiltersValues.length; i++) {
                $('#' + setFiltersValues[0]).val(setFiltersValues[1]);
            }

        }
    }

    var filters = '';
    //iterate through filters
    $('.filter').each(function () {
        filters += $(this).attr('id') + '|' + isNull($(this).val(),'') + '|';
    });

    if (filters != '') {
        filters = filters.substr(0, filters.length - 1);
    }

    // reset page search
    var page = location.pathname.split('/').slice(-1)[0];
    SetCookie('Search_' + page, '', -1);

    // submit
    poster({
        url: document.URL,
        target: '_self',
        data: { filters: filters }
    });
}

function gridApplyFilters(e, setFilters) {

    // store vscroll locations
    vs = '';
    $('.filter').each(function () {
        vs += $(this).attr('id') + ',' + $(this).scrollTop() + '|';
    });

    SetCookie('gridfiltersvscroll', vs, 1);


    if ($(e).closest('table').attr('template') == 'multipleselect') {
        // handle All
        if ($(e).parent().text() == 'All') {
            // uncheck all else
            $(e).closest('table').find('input').prop('checked', false);
            $(e).prop('checked', true);
        }
        else {
            // uncheck All
            $(e).closest('table').find('td:containsExactText("All") input').prop('checked', false);
        }
    }

    // if setFilters are specified, then set
    if (setFilters != null) {
        if (setFilters != '') {
            // reset all filters
            $('.filter').each(function () {
                $(this).find('option').prop('selected', false);
            });

            // set filter
            var setFiltersValues = setFilters.split('|');

            for (var i = 0; i < setFiltersValues.length; i++) {
                $('#' + setFiltersValues[0]).val(setFiltersValues[1]);
            }

        }
    }

    var filters = '';
    //iterate through filters
    $('.filter').each(function () {
        var template = $(this).attr('template');
        if (template == 'select') {
            filters += $(this).attr('id') + '|' + $(this).val() + '|';
        } else if (template = 'multipleselect') {
            var mf = '';
            var f = $(this).attr('id');
            $(this).find('input:checked').each(function () {
                if ($(this).parent().text() != 'All') {
                    mf += $(this).val() + ',';
                }
            });

            if (mf != '') {
                mf = mf.substr(0, mf.length - 1);
                filters += f + '|' + mf + '|';
            }
        }
    });

    if (filters != '') {
        filters = filters.substr(0, filters.length - 1);
    }

    // reset page search
    var page = location.pathname.split('/').slice(-1)[0];
    SetCookie('Search_' + page, '', -1);

    SetCookie('Search' + page, '', -1);
    //document.getElementById("txtSearch").value = '';

    // submit
    poster({
        url: document.URL,
        target: '_self',
        data: { filters: filters }
    });
}

function gridFiltersSetVScroll() {
    var vs = GetCookie('gridfiltersvscroll');
    if (vs == '' || vs == null) return;

    $(vs.split('|')).each(function () {
        var fvs = this.split(',');
        $('#' + fvs[0]).scrollTop(fvs[1]);
    });
}

//#region RowGroups

function gridGroup(o) {
    /* usage
    gridGroup({
        toggle: , // grid, row, set
        mode: , // c=collapsed, e=expanded
        element: 
    });
    */

    var mode = o.mode;
    var e = o.element;

    var t = $(o.element).closest('.hg');
    if (t == null) return;
    var trs = t.find('>tbody:last>tr');

    var cc = o.element.className;
    var hr = t.find('>thead>tr').has('th').length;
    var r = o.element.rowIndex - hr;
    var a = "";

    if (o.toggle == 'grid') {
        for (var r = 0; r < trs.length; r++) {

            var row = trs[r];
            var cn = row.className;

            if (cn.indexOf("RowGroup") == 0) {
                if (mode == "+") {
                    $(row).addClass('RowGroupCollapsed').removeClass('RowGroupExpanded');
                }
                else {
                    $(row).addClass('RowGroupExpanded').removeClass('RowGroupCollapsed');
                }
            }
            else {
                if (mode == "+") {
                    row.style.display = "none";
                }
                else {
                    row.style.display = "";
                }
            }
        }
    }

    if (o.toggle == 'row') {

        if (cc.indexOf("RowGroupCollapsed") > -1) {
            $(e).addClass('RowGroupExpanded').removeClass('RowGroupCollapsed');
        }
        else {
            $(e).addClass('RowGroupCollapsed').removeClass('RowGroupExpanded');
            a = "none";
        }

        var trs = t.find('>tbody:last>tr');

        for (var i = r + 1; i < trs.length; i++) {
            var row = trs[i];
            if ((row.className.indexOf("RowGroup") > -1 && row.className.substr(8, 2) <= cc.substr(8, 2)) || i == row.length - 1 || row.className == "hgNewRecord") {
                break;
            }
            else {
                row.style.display = a;
            }

        }
    }

    if (o.toggle == 'set') {
        $('[class^="RowGroup"]').each(function (index, e) {
            var t = e.parentNode;
            var cc = e.className;
            var r = e.rowIndex - 2;
            var a = "";
            var rowcount = 0

            if (cc.indexOf("RowGroupCollapsed") > -1) {
                a = "none";
            }

            for (var i = r + 1; i < t.rows.length; i++) {
                if ((t.rows(i).className != "" && t.rows(i).className <= cc) || i == t.rows(i).length - 1 || t.rows(i).className == "hgNewRecord") {
                    var GroupTD = e.childNodes[0];
                    //GroupTD.innerHTML = e.childNodes[0].textContent + "&nbsp;&nbsp;<span class='badge RowGroupBadge'>" + rowcount + "</span>";
                    //GroupTD.innerHTML = e.childNodes[0].textContent + "&nbsp;<sup>" + rowcount + "</sup>";
                    GroupTD.innerHTML = e.childNodes[0].textContent;
                    break;
                }
                else {
                    t.rows(i).style.display = a;
                    if (t.rows(i).className.indexOf('RowGroup') == -1) {
                        rowcount = rowcount + 1;
                    }
                }
            }
        });
    }
}

function SetRowGroups() {

    return;

    $('[class^="RowGroup"]').each(function (index, e) {
        var t = e.parentNode;
        var cc = e.className;
        var r = e.rowIndex - 4;
        var a = "";
        var rowcount = 0

        if (cc.indexOf("RowGroupCollapsed") > -1) {
            a = "none";
        }

        for (var i = r + 1; i < t.rows.length; i++) {
            if ((t.rows(i).className != "" && t.rows(i).className <= cc) || i == t.rows(i).length - 1 || t.rows(i).className == "hgNewRecord") {
                var GroupTD = e.childNodes[0];
                //GroupTD.innerHTML = e.childNodes[0].textContent + "&nbsp;&nbsp;<span class='badge RowGroupBadge'>" + rowcount + "</span>";
                //GroupTD.innerHTML = e.childNodes[0].textContent + "&nbsp;<sup>" + rowcount + "</sup>";
                GroupTD.innerHTML = e.childNodes[0].textContent;
                break;
            }
            else {
                t.rows(i).style.display = a;
                if (t.rows(i).className.indexOf('RowGroup') == -1) {
                    rowcount = rowcount + 1;
                }
            }
        }
    });
}

//#endregion





//#region spread
function PopSpread(e, cw) {

    if ($(e).hasClass('disabled')) return;

    //setTimeout(function () { WaitBox('Spreadsheet Loading', 'Please wait', 'Info'); });

    if (e != null) {
        LastPopSpreade = e;
        f = e.getAttribute('f');
        LastPopSpreadf = f;
    }

    if (e == null) {
        e = LastPopSpreade;
        f = LastPopSpreadf
    }


    var q = e.getAttribute('q');
    var info = e.getAttribute('info');
    var Title = $(e).closest('tr').find('td:first').text() + ' - ' + $(e).text();
    var Type = e.getAttribute('template');

    //validate foreign keys specified
    var fks = info.split(',');

    if (fks.length > 3) {
        for (var i = 3; i < fks.length; i++) {
            if ($('#pick' + fks[i]).attr('resultkey') == '' && $('#pick' + fks[i]).attr('result') == '') {
                var Label = $('#pick' + fks[i]).closest('td').prev().text();
                //MessageBox(Label + ' Selection Required', 'Please select a value for ' + Label + ' first.', 'warning');
                gridBox({
                    boxType: 'message',
                    title: Label + ' Selection Required',
                    body: 'Please select a value for ' + Label + ' first.',
                    type: 'warning'
                });
                return;
            }
        }
    }

    // add form key field
    var kt = $(e).closest('.vg');
    var kf = kt.attr('kf');
    var kfv = kt.attr('kfv');

    //var FormData = 'Designer_ID|1000|1000|';
    var FormData = kf + '|' + kfv + '|' + kfv + '|';

    $('.fieldsform').find('.hgResult').each(function () {
        var f = $(this).attr('fn');

        var v = $(this).attr('resultkey');
        if (v == '') {
            $(this).attr('oresultkey');
        }

        if (v == '') {
            $(this).attr('result');
        }

        if (v == '') {
            $(this).attr('oresult');
        }

        var l = $(this).text().replace(/\|/g, ',');

        if (l.length > 50) {
            l = l.substring(0, 50) + '...';
        }

        if (v != '') {
            FormData += f + '|' + escape(v) + '|' + escape(l) + '|';
        }

    });

    if (FormData != '') {
        FormData = FormData.substr(0, FormData.length - 1);
    }

    // initialize
    if ($('#diaSpread' + f).length == 0) {
        var Choices = $(e).attr('resultkey');
        if (Choices == "") {
            $(e).attr('oresultkey');
        }

        SetCookie('SpreadChoice', Choices, 1);
        SetCookie('SpreadSearch', '', -1);
        SetCookie('SpreadType', Type, 1);
    }

    var aFunction = function () {
        WaitBox('Validating', 'Please wait.', 'info');

        //SaveSpread('Spread' + f);
        //return;
        f = f.replace('Spread', '');

        ValidateSpread({
            spreadid: 'Spread' + f,
            onvalid: function (changes) {
                WaitBoxClose();
                /*
                if (changes == '') {
                    $('#diaSpread' + f).dialog('destroy').remove();
                    return;
                }
                */
                WaitBox('Saving', 'Please wait.', 'info');
                //SaveSpread('Spread' + f, e, changes);
                SaveSpread({
                    SpreadID: 'Spread' + f,
                    PickerElement: e,
                    changes: changes
                });
            },
            oninvalid: function () {
                WaitBoxClose();
                return false;
            }

        });
    }

    var w = RelativePixels('w', .9);
    var h = RelativePixels('h', .7); //RelativePixels('h', .7, 800)
    var my = null;
    var at = null;

    var pdw = GetCookie('PickerDetailWidth');

    var find = e.id.replace(/pick/i, '');

    var ww = GetPairValue(pdw, find, 0);

    if (ww != 0) {
        w = ww;
        my = 'right';
        at = 'right-50';
    }

    //if (cw == null) {
    cw = '';
    //}

    // highlight field row
    //$(e).closest('tr').removeClass('ffrhighlight');
    $(e).closest('tr').addClass('ffrhighlight');

    var DialogSpread;
    PopDialog(DialogSpread, 'Spread' + f, my, at, null, w, h, Title, '&eid=' + e.id + "&pq=" + q + "&info=" + info + "&ty=" + Type + '&fd=' + escape(FormData) + '&cw=' + cw, "spread.aspx", 'Save', aFunction);


    //var buttons = $('#dia' + 'Spread' + f).parent().find('.ui-dialog-buttonset');
    //buttons.before($('<div  style="display: inline; font-size: .9em;"><span id=\"spnSpreadStatus\" style=\"width: 500px; padding: 12px; color: gray;\" >Loading spreadsheet ...</span></div>'));
    //buttons.before($('<label class="switch"><input id="chkOnePage(this);" type="checkbox"><span class="slider round"></span></label>'));


}

function SpreadKeyPress(e, ev) {

    if ($(e).hasClass('disabled')) return;

    var code = ev.keyCode || ev.which;

    event.preventDefault();

    if (code == 13 || code == 32) {
        PopSpread(e);
    }
}

function SpreadFocus(e) {
    //alert(1);
}

function SpreadUpdate() {

    var e = LastPopSpreade;

    // get selected IDs
    var SelectedIDs = '';
    var SelectedTexts = 'None';
    /*
    $('#diaDetail' + LastPopDetailf + ' tbody:last tr td:first-child').not(':empty').each(function () {
 
        SelectedIDs += $(this).closest('tr').attr('kfv') + ', ';
        //SelectedTexts += $(this).closest('tr').attr('kfv') + ', ';
            
        //var st = $(this).find('select:not([result=""])').attr('result');
        var st = "";
        if ($(this).find('select').length > 0) {
 
            st = $(this).find('select').attr('result');
        }
        else if ($(this).find('input').length > 0) {
            
            st = $(this).find('input').attr('result');
        }
        else
        {
            st = $(this).text();
        }
 
        var st2 = "";
 
        if ($(this).next().find('select').length > 0) {
 
            st2 = $(this).next().find('select').attr('result');
        }
        else if ($(this).next().find('input').length > 0) {
 
            st2 = $(this).next().find('input').attr('result');
        }
        else {
            st2 = $(this).next().text();
        }
 
        if (st != 'None') {
            SelectedTexts += st + '-' + st2 + ', ';
        }
            
 
    });
    */

    // already saved
    $('#diaSpread' + LastPopSpreadf + ' tbody:last tr[class=""], tbody:last tr:not([class])').each(function () {
        SelectedIDs += $(this).closest('tr').attr('kfv') + ', ';
    });

    // under edit with results
    //$('#diaDetail' + LastPopDetailf + ' tbody:last .RowEdit').has('.hgResult:not([result=""]) , .hgResult:not([resultkey="0"])').each(function () {
    $('#diaSpread' + LastPopSpreadf + ' tbody:last .RowEdit').each(function () {
        //if (IsDirty($(this).find('.hgResult'))) {
        if ($(this).find('.hgResult:not([result=""]).hgResult:not([resultkey="0"])').length > 0) {
            SelectedIDs += $(this).closest('tr').attr('kfv') + ', ';
        }
    });

    // edit but no focus
    $('#diaSpread' + LastPopSpreadlf + ' tbody:last .hgNewRecord').not('.RowEdit').has('td:not(:empty)').each(function () {
        SelectedIDs += $(this).closest('tr').attr('kfv') + ', ';
    });

    if (SelectedIDs != "") {
        SelectedIDs = SelectedIDs.substr(0, SelectedIDs.length - 2);

        /*
        //SelectedTexts = SelectedTexts.substr(0, SelectedTexts.length - 2);
        var ItemCount = SelectedIDs.split(',').length;
        if (ItemCount == 1) {
            SelectedTexts = SelectedIDs.split(',').length + ' Item';
        }
        else {
            SelectedTexts = SelectedIDs.split(',').length + ' Items';
        }
        */

        // determine picker element
        var PickerElement = e;

        var PickerCurrentIDs = SelectedIDs;
        var PickerForm = $(PickerElement).attr('f');
        var PickerField = $(PickerElement).attr('fn');

        var jqxhr = $.getJSON("Process_Request.aspx", {
            action: "PickerLink",
            cids: PickerCurrentIDs,
            nid: '',
            form: PickerForm,
            field: PickerField,
            kfv: '',
            table: ''
        })
            .done(function (data) {

                $(PickerElement).attr('resultkey', data.LinkIDs);
                $(PickerElement).text(data.LinkText);

                $('#diaSpread' + LastPopSpreadf).dialog('destroy').remove();

                // focus on next fields
                if (f.indexOf('Product_Designer_') == -1) {

                    //var nr = $(e).closest('tr').next('tr').find('.hgResult');
                    var nr = $(e).closest('tr').nextAll('tr:visible:first').find('.hgResult');

                    if (nr.find('input').length == 1) {
                        nr.find('input').caretToEnd();
                    }
                    else if (nr.is('a')) {
                        //nr.focus();
                        //setTimeout(function () { nr.click(); }, 500);
                        setTimeout(function () {
                            nr.focus();

                            if (nr.text() == 'None') {
                                nr.click();
                            }
                        },
                            500);
                    }
                    else {
                        nr.focus();
                    }
                }

                //alert("done");
            }).fail(function (data) {
                alert("PickerLink failed.");
            });
    }

    /*
    // update e
    $(e).attr('resultkey', SelectedIDs);
    $(e).text(SelectedTexts);

    $('#diaDetail' + LastPopDetailf).dialog('destroy').remove();


    // focus on next fields
    if (f.indexOf('Product_Designer_') == -1) {

        //var nr = $(e).closest('tr').next('tr').find('.hgResult');
        var nr = $(e).closest('tr').nextAll('tr:visible:first').find('.hgResult');

        if (nr.find('input').length == 1) {
            nr.find('input').caretToEnd();
        }
        else if (nr.is('a')) {
            //nr.focus();
            //setTimeout(function () { nr.click(); }, 500);
            setTimeout(function () {
                nr.focus();

                if (nr.text() == 'None') {
                    nr.click();
                }
            },
                500);
        }
        else {
            nr.focus();
        }
    }
    */
}

function ValidateSpread(o) {

    var spreadid = o.spreadid;
    var spreadinstanceid = spreadid + '-' + document.getElementById(spreadid).getAttribute('iid');
    var pickerelement = o.pickerelement;
    var onvalid = o.onvalid;
    var oninvalid = o.oninvalid;

    var spread = document.getElementById(spreadid).jexcel;

    var columns = spread.getConfig().columns;

    var tb = document.getElementById(spreadid).getAttribute('tb');
    var kf = document.getElementById(spreadid).getAttribute('kf');
    var as = document.getElementById(spreadid).getAttribute('as');

    var kfindex = GetKeyColumnIndex(columns);

    // run pre validate function
    if (typeof (ValidateSpreadPre) == 'function') {
        ValidateSpreadPre(o);
    }
    // validate
    var NewData = spread.getData();
    var Changes = '';
    var valid = true;

    var OldData = SpreadOD.filter(function (e) { return e.id == spreadinstanceid; })[0].odata;

    // update and new
    $(NewData).each(function (index) {

        var rowindex = index;
        var rowdata = this;
        var Change = '';
        valid = true;

        var hasdata = spreadrowhasdata(rowdata);

        if (hasdata) {

            var ID = rowdata[kfindex];

            // get old data row
            oldrowdata = OldData.filter(function (e) { return e[kfindex] == ID })[0];

            $(columns).each(function (index) {

                if (!this.key) {

                    var cellname;

                    var value = spread.getValueFromCoords(index, rowindex);

                    // skip validting checkboxes
                    if (!this.type == 'checkbox') {

                        if (this.type != 'hidden') {
                            cellname = String.fromCharCode(index + 65) + (rowindex + 1);
                            spread.setStyle(cellname, 'background-color', 'white');
                        }

                        // required
                        if (this.required == true && SaveValue(value, this) == '') {

                            spread.setStyle(cellname, 'background-color', 'yellow');
                            // message
                            //MessageBox("Validation: Required Field Empty", "You missed making an entry to a required field (" + this.title + ").<br /><br />Please make an entry.", "warning");
                            gridBox({
                                boxType: 'message',
                                title: 'Validation: Required Field Empty',
                                body: "You missed making an entry to a required field (" + this.title + ").<br /><br />Please make an entry.",
                                type: 'warning'
                            });
                            valid = false;
                            return valid;
                        }

                        // length
                        if (this.length != null && value.length > this.length) {
                            spread.setStyle(cellname, 'background-color', 'yellow');
                            //MessageBox("Validation: Text Too Long", "The text you entered is longer than the maximum length allowed (" + this.length + " characters).<br /><br />Please edit.", "warning");
                            gridBox({
                                boxType: 'message',
                                title: 'Validation: Text Too Long',
                                body: "The text you entered is longer than the maximum length allowed (" + this.length + " characters).<br /><br />Please edit.",
                                type: 'warning'
                            });
                            valid = false;
                            return valid;
                        }

                        // is date
                        if (this.date == true && value != '' && isNaN(Date.parse(value))) {
                            spread.setStyle(cellname, 'background-color', 'yellow');
                            //MessageBox("Validation: Not a Date", "The text you entered is not a date. A date is expected.<br /><br />Please edit.", "warning");
                            gridBox({
                                boxType: 'message',
                                title: 'Validation: Text Too Long',
                                body: "The text you entered is longer than the maximum length allowed (" + this.length + " characters).<br /><br />Please edit.",
                                type: 'warning'
                            });
                            valid = false;
                            return valid;
                        }

                        // is number
                        if (this.ft == true && !$.isNumeric(value) && this.type == 'text') {
                            spread.setStyle(cellname, 'background-color', 'yellow');
                            //MessageBox("Validation: Not a Number", "The text you entered is not a number. A number is expected.<br /><br />Please edit.", "warning");
                            gridBox({
                                boxType: 'message',
                                title: 'Validation: Not a Number',
                                body: "The text you entered is not a number. A number is expected.<br /><br />Please edit.",
                                type: 'warning'
                            });
                            valid = false;
                            valid = false;
                            return valid;
                        }
                    }


                    var oldvalue;

                    if (oldrowdata != null) {
                        oldvalue = oldrowdata[index];
                    }

                    var savevalue = SaveValue(value, this);

                    //if (!(this.key && savevalue != '')) {  // insert
                    if (savevalue != SaveValue(oldvalue, this)) {
                        Change += "<fv f='" + this.fn + "'  v='" + savevalue + "' t='" + this.ft + "' e='1' />";
                    }
                    //}
                }
            });

            if (!valid) {
                return valid;
            }
        }

        if (!valid) {
            return valid;
        }

        if (Change != '') {
            Changes += "<data tb='" + tb + "' kf='" + kf + "' kfv='" + ID + "' as='" + escape(as) + "' >" + Change + "</data>";
        }

    });

    // deleted
    $(OldData).each(function () {

        var oldkfv = this[kfindex];

        if (oldkfv != null && oldkfv != '') {
            // missing in new data
            var newrowdata = NewData.filter(function (e) { return e[kfindex] == oldkfv })[0];
            if (newrowdata == null) {
                Changes += "<data tb='" + tb + "' kf='" + kf + "' kfv='" + oldkfv + "' as='" + escape(as) + "' de='1' ></data>";
            }
        }
    });

    if (valid) {
        onvalid(Changes);
    }
    else {
        oninvalid();
    }

    return;
}

function ValidateSpreadPre(o) {

    var spreadid = o.spreadid;

    var spread = document.getElementById(spreadid).jexcel;

    var columns = spread.getConfig().columns;

    var tb = document.getElementById(spreadid).getAttribute('tb');
    var kf = document.getElementById(spreadid).getAttribute('kf');
    var as = document.getElementById(spreadid).getAttribute('as');

    // validate
    var NewData = spread.getData();

    // set deleted rows
    $(NewData).each(function (index) {
        var rowdata = this;
        var visibledata = '';
        var rowindex = index;
        $(columns).each(function (index) {
            // visible columns
            if (this.type != 'hidden') {

                var celltext = rowdata[index].toString();

                // ignore unchecked checkboxes and none links
                if (celltext == 'false') {
                    celltext = '';
                } else if (celltext.indexOf('>None</') > -1) {
                    celltext = '';
                }

                visibledata += celltext;
            }
        });

        if (visibledata == '') {
            $(columns).each(function (index) {
                // invisible columns
                if (this.type == 'hidden') {
                    if (this.type == 'checkbox') {
                        spread.setValueFromCoords(index, rowindex, false, true);
                    }
                    else {
                        spread.setValueFromCoords(index, rowindex, '', true);
                    }

                }
            });
        }
    });

    // apply defaults
    $(NewData).each(function (index) {
        var rowdata = this;
        var visibledata = '';
        var rowindex = index;
        $(columns).each(function (index) {
            // visible columns
            if (this.type != 'hidden') {

                var celltext = rowdata[index].toString();

                // ignore unchecked checkboxes and none links
                if (celltext == 'false') {
                    celltext = '';
                } else if (celltext.indexOf('>None</') > -1) {
                    celltext = '';
                }

                visibledata += celltext;
            }
        });

        if (visibledata != '') {
            $(columns).each(function (index) {
                // with defaults
                if (this.default != null && rowdata[index] == '') {
                    if (this.ft == 's') {
                        spread.setValueFromCoords(index, rowindex, this.default, true);
                    }
                    else {
                        spread.setValueFromCoords(index, rowindex, this.default, true);
                    }

                }
            });
        }
    });

    var qs = unescape($('#dia' + spreadid).attr('qs'));
    var info = ParseQueryString('info', qs);

    //if (spreadid == 'SpreadSalesForce') {
    //if ($('form:last').attr('action').indexOf('info=PositionMin') > -1) {

    if (info.indexOf('PositionMin') == 0) {

        NewData = spread.getData();
        // update ignored fields

        //Product
        var ContractProductIDindex = GetColumnIndex(columns, 'Contract Product');
        var ProductIDindex = GetColumnIndex(columns, 'Product');

        // Dates
        var DeliveryDateindex = GetColumnIndex(columns, 'Delivery Date');

        var ContractDeliveryDeadlineindex = GetColumnIndex(columns, 'Contract Deadline');

        var EarliestPossibleDeliveryDateindex = GetColumnIndex(columns, 'Earliest Possible Delivery');
        var ContractEarliestPossibleDeliveryDateindex = GetColumnIndex(columns, 'Contract Earliest Date');

        $(NewData).each(function (index) {

            var rowindex = index;
            var rowdata = this;
            var Change = '';
            valid = true;

            var hasdata = spreadrowhasdata(rowdata);

            if (hasdata) {

                // update ProductID
                spread.setValueFromCoords([ProductIDindex], [index], rowdata[ContractProductIDindex], true);

                // update ContractDeliveryDeadline
                spread.setValueFromCoords([ContractDeliveryDeadlineindex], [index], rowdata[DeliveryDateindex], true);

                // update EarliestPossibleDeliveryDate
                spread.setValueFromCoords([EarliestPossibleDeliveryDateindex], [index], rowdata[ContractEarliestPossibleDeliveryDateindex], true);

            }
        });
    }

}

function spreadrowhasdata(spreadrow) {

    var hasdata = true;
    var changeddata = '';

    $(spreadrow).each(function (index) {

        var celltext = spreadrow[index].toString();

        // ignore unchecked checkboxes and none links
        if (celltext == 'false') {
            celltext = '';
        } else if (celltext.indexOf('>None</') > -1) {
            celltext = '';
        }

        changeddata += celltext;
    });

    if (changeddata == '') {
        hasdata = false;
    }

    return hasdata;
}

function SaveSpread(o) {

    var SpreadID = o.SpreadID;
    var PickerElement = o.PickerElement;
    var changes = o.changes;
    var postsave = o.postsave;

    //WaitBox('Saving Data', 'Please wait.', 'info');

    var spread = document.getElementById(SpreadID).jexcel;
    var columns = spread.getConfig().columns;
    var keyfieldindex = GetKeyColumnIndex(columns);
    var PickerCurrentIDs = '';
    var PickerForm = $(PickerElement).attr('f');
    var PickerField = $(PickerElement).attr('fn');
    var SavedRows = 0;
    var NewIDs = '';

    //Encode
    changes = changes.replace(/\</g, "-[_").replace(/\>/g, "_]-");

    // save
    $.ajax({
        type: 'POST',
        url: 'Process_Change',
        async: false,
        processData: false,
        data: 'changes=' + changes,
        success: function (r) {

            // parse response
            var Result = r.split("|");

            if (Result[0] == 'Error') {
                WaitBoxClose();
                //MessageBox("Error Saving Data", r.replace('Error|', '') + "<br /><br />Please edit or cancel.", "danger");
                gridBox({
                    boxType: 'message',
                    title: 'Error Saving Data',
                    body: r.replace('Error|', '') + "<br /><br />Please edit or cancel.",
                    type: 'danger'
                });
                return;
            }

            if (PickerElement != null) {

                if (Result[0] == 'NewID') {
                    NewIDs = Result[3];
                }

                var FieldForm = $(PickerElement).closest('.vg, .hg');

                var kf = FieldForm.attr('kf');
                var kfv = FieldForm.attr('kfv');
                var tb = FieldForm.attr('tb');

                PickerCurrentIDs = spread.getColumnData(keyfieldindex).filter(Boolean).join(',');

                // spread update
                var jqxhr = $.getJSON("Process_Request.aspx", {
                    action: "PickerLink",
                    cids: PickerCurrentIDs,
                    nid: NewIDs,
                    form: PickerForm,
                    field: PickerField,
                    ls: 'PC',
                    kf: kf,
                    kfv: kfv,
                    table: tb
                })
                    .done(function (data) {

                        $(PickerElement).attr('resultkey', data.LinkIDs);
                        $(PickerElement).text(data.LinkText);

                        $('#diaSpread' + PickerForm).dialog('destroy').remove();

                        // focus on next fields
                        if (f.indexOf('Product_Designer_') == -1) {

                            //var nr = $(e).closest('tr').next('tr').find('.hgResult');
                            var nr = $(PickerElement).closest('tr').nextAll('tr:visible:first').find('.hgResult');

                            if (nr.find('input').length == 1) {
                                nr.find('input').caretToEnd();
                            }
                            else if (nr.is('a')) {
                                //nr.focus();
                                //setTimeout(function () { nr.click(); }, 500);
                                setTimeout(function () {
                                    nr.focus();

                                    if (nr.text() == 'None') {
                                        nr.click();
                                    }
                                },
                                    500);
                            }
                            else {
                                nr.focus();
                            }
                        }

                        WaitBoxClose();

                        //alert("done");
                    }).fail(function (data) {
                        WaitBoxClose();
                        alert("PickerLink failed.");
                    });
            }

            if (typeof (postsave) == 'function') {
                var CurrentIDs = spread.getColumnData(keyfieldindex).filter(Boolean).join(',');
                if (Result[0] == 'NewID') {
                    NewIDs = Result[3];
                }
                postsave(CurrentIDs, NewIDs);
            }
        },
        error: function (response) {
            WaitBoxClose();
            //MessageBox("Error Saving Data", "Please edit or cancel.", "danger");
            gridBox({
                boxType: 'message',
                title: 'Error Saving Data',
                body: 'Please edit or cancel.',
                type: 'danger'
            });
            return;
        }
    });
}


var x1o = 0, y1o = 0, x2o = 0, y2o;
var spreadonselection = function (instance, x1, y1, x2, y2, origin) {

    var xdirection = 1;
    var ydirection = 1;

    if (x1 < x1o) {
        xdirection = 0;
    }

    if (y1 < y1o) {
        ydirection = 0;
    }

    var cols = instance.jexcel.getConfig().columns;

    var dia = $(instance).closest('.ui-dialog-content')[0];

    var clw = dia.clientWidth;
    var clh = dia.clientHeight;

    var sl = dia.scrollLeft;

    var x2max = 0;
    var x1min = 0;

    var x2width = 0;


    $(cols).each(function (index) {
        if (this.type != 'hidden') {
            x2max = x2max * 1.0 + this.width * 1.0;

            if (index < x1) {
                x1min = x1min * 1.0 + this.width * 1.0;
            }

        }
        if (index == x2) {
            x2width = this.width * 1.0;
            return false;
        }
    });

    var xinc = 200;

    if (xdirection == 1) {
        if (cols[x2 + 1] != null) {
            xinc = cols[x2 + 1].width * 1.0;
        }
        if (x2max + xinc > clw * 1.0 + dia.scrollLeft * 1.0) {
            dia.scrollLeft = dia.scrollLeft * 1.0 + xinc;
        }
    }

    if (xdirection == 0) {
        if (cols[x1 - 1] != null) {
            xinc = cols[x1 - 1].width * 1.0;
        }
        if (x1min - xinc < dia.scrollLeft) {
            dia.scrollLeft = dia.scrollLeft - xinc;
        }
    }

    var rows = instance.jexcel.rows;

    var y2max = 0;
    var y1min = 0;

    for (var index = 0; index < rows.length; index++) {
        y2max = y2max * 1.0 + rows[index].clientHeight * 1.0;

        if (index < y1) {
            y1min = y1min * 1.0 + rows[index].clientHeight * 1.0;
        }

        if (index == y2) {
            break;
        }
    }

    var yinc = 100;

    if (ydirection == 1) {
        if (rows[y2 + 1] != null) {
            yinc = rows[y2 + 1].clientHeight * 1.0;
        }
        if (y2max + yinc > clh * 1.0 + dia.scrollTop * 1.0 - 40.0) {
            dia.scrollTop = dia.scrollTop * 1.0 + yinc;
        }
    }

    if (ydirection == 0) {
        if (rows[y1 - 1] != null) {
            yinc = rows[y1 - 1].clientHeight * 1.0;
        }
        if (y1min - yinc < dia.scrollTop * 1.0) {
            dia.scrollTop = dia.scrollTop * 1.0 - yinc;
        }
    }

    /*
    if (x1 == x2) {
        x1o = 0; x2o = 0;
    }
    else {
        x1o = x1; x2o = x2;
    }

    if (y1 == y2) {
        y1o = 0; y2o;
    }
    else {
        y1o = y1; y2o = y2;
    }
    */
    x1o = x1; x2o = x2; y1o = y1; y2o = y2;

}

function spreadmousemove(e, ev) {
    /*
    var x = ev.clientX;
    var y = ev.clientY;

    var dia = $(e).closest('.ui-dialog-content')[0];

    var clw = dia.clientWidth;

    var sl = dia.scrollLeft;

    //alert(x);
    
    if (x + 50 > clw) {
        dia.scrollLeft = dia.scrollLeft + 50; //(x + 100) - (clw + sl);
    }

    if (x - 50 < 0) {
        dia.scrollLeft = dia.scrollLeft - 50;
    }

    */
}

function spreadpagewidth(e) {

    var dia = $('.ui-dialog-content:last');

    var spread = $('.spread:first')[0].jexcel;

    // get visible columns
    var columns = spread.getConfig().columns;

    var colcount = 0;

    $(columns).each(function (index) {
        if (this.type != 'hidden') {
            colcount++;
        }
    });

    var spreadwidth = dia[0].clientWidth;

    if (e.checked) {
        var colwidth = (spreadwidth * 1.0 - 55) / colcount * 1.0;

        $('.spread:first').css('font-size', '.7em');
        //dia.find('col:not(:first)').css('width', colwidth + 'px');

        $(columns).each(function (index) {
            if (this.type != 'hidden') {
                spread.setWidth(index, colwidth);
            }
        });

    }
    else {
        var colwidth = 100;

        $('.spread:first').css('font-size', '1.0em');
        //dia.find('col:not(:first)').css('width', colwidth + 'px');
        $(columns).each(function (index) {
            if (this.type != 'hidden') {
                spread.setWidth(index, colwidth);
            }
        });
    }

    dia.scrollLeft = 0;

    //setTimeout(functioon(){ dia.find('col:not(:first)').css(}, 100);
}

var spreadonload = function (instance) {

    // update original data
    spreadupdateoriginaldata(instance);
    spreadupdatepickers(instance);
    spreadrmarkeadonly(instance);
    //SpreadColumns = instance.jexcel.getConfig().columns;

}

var spreadonchange = function (instance, cell, x, y, value) {
    /*
    if (cell.innerText == '') {
        var pickerlink = instance.jexcel.getConfig().columns[col].pickerlink;
        if (pickerlink != null && pickerlink != '') {
            var Link = decodeURIComponent(unescape(pickerlink));
            Link = Link.replace('></a>', ' ><span style=\'color:#D3D3D3 !important;\'>None</span></a>');
            cell.innerHTML = Link;
        }
    }
    */
}

function spreadupdateoriginaldata(instance) {
    var id = $(instance).attr('id') + '-' + $(instance).attr('iid');

    var o = SpreadOD.filter(function (e) { return e.id == id; });

    if (o.length == 0) {
        SpreadOD.push({ id: id, odata: instance.jexcel.getData() });
    }
    else {
        o.odata = instance.jexcel.getData();
    }

    // remove SpreadOD for instances that no longer exist
    $.each(SpreadOD, function (key, value) {
        if (value != null) {
            var activespread = $('.spread').filter(function (eindex, e) {
                return $(e).attr('id') + '-' + $(e).attr('iid') == value.id;
            });
            if (activespread.length == 0) {
                SpreadOD.splice(key, 1);
            }
        }
    });
}

function spreadupdatepickers(instance) {

    // none for pickers
    var spread = instance.jexcel;
    var columns = spread.getConfig().columns;

    $(columns).each(function (index) {
        if (this.pickerlink != null) {
            var Link = decodeURIComponent(unescape(this.pickerlink));

            //Link = Link.replace('></a>', ' style=\'color:gray !important;\' >None</a>');
            Link = Link.replace('></a>', ' ><span style=\'color:#D3D3D3 !important;\'>None</span></a>');

            PickerLinkDefaults.push(index, Link);
            //.push(Link);

            // update all blanks in row
            var cdata = spread.getColumnData(index);

            $(cdata).each(function (row) {
                if (this == '') {
                    spread.setValueFromCoords(index, row, Link);
                }
            });
        }
    });

}

function spreadrmarkeadonly(instance) {
    var spread = instance.jexcel;
    var columns = spread.getConfig().columns;

    $(columns).each(function (index) {
        if (this.readOnly) {
            $('.spread colgroup col:nth-child(' + (index + 2) + ')').css('background-color', '#fafafa');
        }
    });
}

var spreadupdateTable = function (instance, cell, col, row, val, id) {


    if (!SpreadInitialized) {
        SpreadColumns = instance.jexcel.getConfig().columns;
        SpreadInitialized = true;
    }


    if ((cell.innerText).indexOf("<a ") > -1) {
        cell.innerHTML = val;
    } else if (cell.innerText == '') {
        var LinkIndex = PickerLinkDefaults.indexOf(col);
        if (LinkIndex != -1) {
            cell.innerHTML = PickerLinkDefaults[LinkIndex + 1];
            return;
        }
    }


    // readonly
    if (SpreadColumns[col].readOnly) {

        //var selectedrow = instance.jexcel.getSelectedRows()[0].rowIndex;

        // disable links
        $(cell).find('a').attr('onclick', '').attr('style', 'cursor: default !important; color: gray !important;');
    }

}

var spreadonpaste = function (instance, cell) {

    return;
    for (var r = 0; r < cell.length; r++) {
        for (var c = 0; c < cell[0].length; c++) {
            var pastedcell = cell[r, c];
            if ((pastedcell.innerText).indexOf("<a ") > -1) {
                //pastedcell.innerHTML = val;
            }
        }
    }
}

var spreadonchange = function (instance, cell, x, y, value) {

    if ((cell.innerText).indexOf("<a ") > -1) {
        cell.innerHTML = value;
    }
}

var spreadoneditionstart = function (instance, cell, x, y, value) {

    if ((cell.innerHTML).indexOf("<a ") > -1) {

        $(cell).addClass('readonly');

    }/*
    else if (table.getConfig().columns[x].date) {
        setTimeout(function () {
            
            $(cell).closest('input').datepicker();
            
        }, 5000);
    }*/
    else {
        return;
    }
}

var spreadoneditionend = function (instance, cell, x, y, value) {

    if (table.getConfig().columns[x].date) {
        // fix date
        var date = new Date(cell.innerHTML);

        if (date.getYear() < 100) {
            var newdate = new Date(date.getYear() + 2000, date.getMonth(), date.getDate());
            cell.innerHTML = formatDate(newdate, "MM/dd/yyyy");
            data[y][x] = cell.innerHTML;
        }
    }
}

function GetColumnIndex(columns, columntitle) {
    //return columns.filter(function (e, index) { return e.title == title; }).index;
    for (var i = 0; i < columns.length; i++) {
        if (columns[i].title == columntitle) {
            return i;
        }
    }
    return -1;
}

function GetKeyColumnIndex(columns) {
    //return columns.filter(function (e, index) { return e.title == title; }).index;
    for (var i = 0; i < columns.length; i++) {
        if (columns[i].key == true) {
            return i;
        }
    }
    return -1;
}

function SaveValue(value, column) {

    if (value == null) return value;

    if (value.length == 1 && value.charCodeAt(0) == 0) return '';

    if (value == true && column.type == 'checkbox') {
        value = 1; // checkbox checked
    }
    else if (value == false && column.type == 'checkbox') {
        value = 0; // checkbox unchecked
    }
    else if (value != '' && value.indexOf('<a ') > -1) {
        // get result key
        value = $(value).attr('resultkey');

        if (value == '0') value = '';
    }
    else if (value == '') {
        value = "";
    }

    return value;
}

var spreadonbeforechange = function (instance, cell, x, y, value) {
    // remove all readonly
    //$('.spread').removeClass('readonly');
    /*
    var selectedcells = instance.jexcel.selectedCell.filter(function (e, i) { return i % 2 == 0;});
    var firstx = selectedcells[0];
    var lastx = selectedcells[1];

    for (var c = firstx; c <= lastx; c++) {
        var col = instance.jexcel.getConfig().columns[c];
        var colreadonly = col.readOnly;

        if (colreadonly) {
            col.readOnly = false;
            col.wasreadOnly = true;
        }
    }
    */
}

var spreadonafterchanges = function (instance, cell, x, y, value) {
    // restore read only
    /*
    var selectedcells = instance.jexcel.selectedCell.filter(function (e, i) { return i % 2 == 0; });
    var firstx = selectedcells[0];
    var lastx = selectedcells[1];

    for (var c = firstx; c <= lastx; c++) {
        var col = instance.jexcel.getConfig().columns[c];
        var colreadonly = col.readOnly;

        if (colreadonly) {
            col.readOnly = true;
            col.wasreadOnly = false;
        }
    }
    */
}

var spreadcontextmenu = function (instance, x, y, e) {
    var items = [];

    // only row related items
    if (y != null) {

        var KeyfieldIndex = GetKeyColumnIndex(instance.getConfig().columns);

        // insert copy after
        items.push({
            title: 'Insert copy of this row after',
            onclick: function () {
                //alert('Insert copy of this row after');
                var rowdata = instance.getRowData(y);

                // copy all columns except key field value
                instance.insertRow(1, parseInt(y));
                var yy = parseInt(y) + 1;
                for (var i = 0; i < rowdata.length; i++) {
                    if (i != KeyfieldIndex) {
                        instance.setValueFromCoords(i, yy, rowdata[i], true);
                    }
                }
            }
        });

        // insert copy before
        items.push({
            title: 'Insert copy of this row before',
            onclick: function () {
                //alert('Insert copy of this row after');
                var rowdata = instance.getRowData(y);

                // copy all columns except key field value
                if (i != KeyfieldIndex) {
                    instance.insertRow(1, parseInt(y), true);
                }
                var yy = parseInt(y);

                setTimeout(function () {
                    for (var i = 0; i < rowdata.length; i++) {
                        instance.setValueFromCoords(i, yy, rowdata[i], true);
                    }
                });
            }
        });

        // Line
        items.push({ type: 'line' });

        // move row up
        if (parseInt(y) > 0) {
            items.push({
                title: 'Move this row up',
                onclick: function () {
                    instance.moveRow(y, parseInt(y) - 1);
                }
            });
        }

        // move row down
        items.push({
            title: 'Move this row down',
            onclick: function () {
                instance.moveRow(y, parseInt(y) + 1);
            }
        });

        // Line
        items.push({ type: 'line' });

        // insert row before
        if (instance.options.allowInsertRow == true) {
            items.push({
                title: instance.options.text.insertANewRowBefore,
                onclick: function () {
                    //alert('before');
                    instance.insertRow(1, parseInt(y), true);
                }
            });
        }

        // after
        if (instance.options.allowInsertRow == true) {
            items.push({
                title: instance.options.text.insertANewRowAfter,
                onclick: function () {
                    //alert('after');
                    instance.insertRow(1, parseInt(y));
                }
            });
        }

        // Line
        items.push({ type: 'line' });

        // delete
        if (instance.options.allowDeleteRow == true) {
            items.push({
                title: 'Delete current row', //instance.options.text.deleteSelectedRows,
                onclick: function () {
                    hgConfirm('Are you sure you want to delete row ' + (parseInt(y) + 1) + '?', function () { instance.deleteRow(parseInt(y)); }, 'Delete Row', 400);
                }
            });
        }

        // Line
        items.push({ type: 'line' });

        items.push({
            title: 'Undo',
            onclick: function () {
                //alert('delete');
                instance.undo();
            }
        });



        items.push({
            title: 'Redo',
            onclick: function () {
                //alert('delete');
                instance.redo();
            }
        });

        /*
        // Line
        items.push({ type: 'line' });

        items.push({
            title: 'Download CSV',
            onclick: function () {
                //alert('delete');
                instance.download();
            }
        });
        */

    }

    return items;
}

var calendaroptions = {
    // Date format
    format: 'MM/DD/YYYY',
    // Allow keyboard date entry
    readonly: 0,
    // Today is default
    today: 0,
    // Show timepicker
    time: 0,
    // Show the reset button
    resetButton: true,
    // Placeholder
    placeholder: 'xxx',
    // Translations can be done here
    months: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
    weekdays: ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'],
    weekdays_short: ['S', 'M', 'T', 'W', 'T', 'F', 'S'],
    // Value
    value: null,
    // Events
    onclose: null,
    onchange: null,
    // Fullscreen (this is automatic set for screensize < 800)
    fullscreen: false
};

var jquerydatepicker = {
    // Methods
    closeEditor: function (cell, save) {

        var value = cell.children[0].value;
        cell.innerHTML = value;
        return value;
    },
    openEditor: function (cell) {
        // Create input
        var element = document.createElement('input');
        element.value = cell.innerHTML;
        element.setAttribute('class', 'datepicker');

        // Update cell
        cell.classList.add('editor');
        cell.innerHTML = '';
        cell.appendChild(element);

        /*
        $(element).datepicker({
            dateFormat: 'm/d/yyyy',
            autoclose: true,
            width: 300,
            onSelect: function (t, e) {
                this.value = t;
                setTimeout(function () {

                    // To avoid double call
                    if (cell.children[0]) {
                        table.closeEditor(cell, true);
                    }
                });
            }
        });
        */
        // Focus on the element
        element.focus();
    },
    getValue: function (cell) {
        return cell.innerHTML;
    },
    setValue: function (cell, value) {
        cell.innerHTML = value;
    }
}

var noEditor = {
    closeEditor: function (cell, save) { return; },
    openEditor: function (cell) { return; },
    getValue: function (cell) {
        return $(cell).html();
    },
    setValue: function (cell, value) {
        $(cell).html(value);
        $(cell).css('color', value);
        return true;
    }
}
//#endregion

//#region compare text/html
$.extend($.expr[":"], {
    containsExactHTML: $.expr.createPseudo ?
        $.expr.createPseudo(function (text) {
            return function (elem) {
                return $.trim(elem.innerHTML.toLowerCase()) === text.toLowerCase();
            };
        }) :
        // support: jQuery <1.8
        function (elem, i, match) {
            return $.trim(elem.innerHTML.toLowerCase()) === match[3].toLowerCase();
        },

    containsExactCaseHTML: $.expr.createPseudo ?
        $.expr.createPseudo(function (text) {
            return function (elem) {
                return $.trim(elem.innerHTML) === text;
            };
        }) :
        // support: jQuery <1.8
        function (elem, i, match) {
            return $.trim(elem.innerHTML) === match[3];
        },
    containsExactText: $.expr.createPseudo ?
        $.expr.createPseudo(function (text) {
            return function (elem) {
                return $.trim(elem.textContent.toLowerCase()) === text.toLowerCase();
            };
        }) :
        // support: jQuery <1.8
        function (elem, i, match) {
            return $.trim(elem.textContent.toLowerCase()) === match[3].toLowerCase();
        },

    containsExactTextCase: $.expr.createPseudo ?
        $.expr.createPseudo(function (text) {
            return function (elem) {
                return $.trim(elem.textContent) === text;
            };
        }) :
        // support: jQuery <1.8
        function (elem, i, match) {
            return $.trim(elem.textContent) === match[3];
        },

    containsRegex: $.expr.createPseudo ?
        $.expr.createPseudo(function (text) {
            var reg = /^\/((?:\\\/|[^\/])+)\/([mig]{0,3})$/.exec(text);
            return function (elem) {
                return reg ? RegExp(reg[1], reg[2]).test($.trim(elem.innerHTML)) : false;
            };
        }) :
        // support: jQuery <1.8
        function (elem, i, match) {
            var reg = /^\/((?:\\\/|[^\/])+)\/([mig]{0,3})$/.exec(match[3]);
            return reg ? RegExp(reg[1], reg[2]).test($.trim(elem.innerHTML)) : false;
        },
    containsText: $.expr.createPseudo ?
        $.expr.createPseudo(function (text) {
            return function (elem) {
                return $.trim(elem.textContent.toLowerCase()).indexOf(text.toLowerCase()) >= 0;
            };
        }) :
        // support: jQuery <1.8
        function (elem, i, match) {
            return $.trim(elem.textContent.toLowerCase()).indexOf(match[3].toLowerCase()) >= 0;
        },
    containsHTML: $.expr.createPseudo ?
        $.expr.createPseudo(function (text) {
            return function (elem) {
                return $.trim(elem.innerHTML.toLowerCase()).indexOf(text.toLowerCase()) >= 0;
            };
        }) :
        // support: jQuery <1.8
        function (elem, i, match) {
            return $.trim(elem.innerHTML.toLowerCase()).indexOf(match[3].toLowerCase()) >= 0;
        }

});

//#endregion

//#region caret
(function ($) {
    // Behind the scenes method deals with browser
    // idiosyncrasies and such
    $.caretTo = function (el, index) {
        if (el.createTextRange) {
            var range = el.createTextRange();
            range.move("character", index);
            try { range.select(); } catch (err) { };
        } else if (el.selectionStart != null) {
            el.focus();
            el.setSelectionRange(index, index);
        }
    };

    // The following methods are queued under fx for more
    // flexibility when combining with $.fn.delay() and
    // jQuery effects.

    // Set caret to a particular index
    $.fn.caretTo = function (index, offset) {
        return this.queue(function (next) {
            if (isNaN(index)) {
                var i = $(this).val().indexOf(index);

                if (offset === true) {
                    i += index.length;
                } else if (offset) {
                    i += offset;
                }

                $.caretTo(this, i);
            } else {
                $.caretTo(this, index);
            }

            next();
        });
    };

    // Set caret to beginning of an element
    $.fn.caretToStart = function () {
        return this.caretTo(0);
    };

    // Set caret to the end of an element
    $.fn.caretToEnd = function () {
        return this.queue(function (next) {
            $.caretTo(this, $(this).val().length);
            next();
        });
    };
}(jQuery));
//#endregion

//#region reports

function ShowReport(ReportRDL, Format, FileName, Parameters, HideParameters) {
    window.open("report.aspx?rdl=" + escape(ReportRDL) + "&par=" + escape(Parameters) + "&hide=" + escape(HideParameters) + "&format=" + escape(Format) + "&file=" + escape(FileName), "report");
    //window.open("report.aspx?rdl=Daily Measurements.rdl&para=" + Parameters + "&hide=" + HideParameters + "&format=" + Format + "&file=" + FileName, "report");
}

function ShowReportx(ReportRDL, Format, FileName, Parameters, HideParameters) {
    window.open("reportx.aspx?rdl=" + escape(ReportRDL) + "&par=" + escape(Parameters) + "&hide=" + escape(HideParameters) + "&format=" + escape(Format) + "&file=" + escape(FileName), "report");
    //window.open("report.aspx?rdl=Daily Measurements.rdl&para=" + Parameters + "&hide=" + HideParameters + "&format=" + Format + "&file=" + FileName, "report");
}

function ApplyReportParameters() {

    $("body").css("cursor", "progress");

    var $Results = $('#tabvgParameters').find('.hgResult');

    $Results.each(function (index) {
        UpdateTemplate(this);
    });

    $lbls = $('#tabvgParameters').find('.vgLabel');
    $flds = $('#tabvgParameters').find('.vgField');

    var Parameters = "";

    $lbls.each(function (index) {
        Parameters += $(this).attr('fn') + '|';

        var ParameterValue = $flds.eq(index).find('.hgResult').attr('resultkey');

        if (ParameterValue == "" && $flds.eq(index).find('.hgResult')[0].tagName == 'SELECT') {
            ParameterValue = 'null';
        }


        if (ParameterValue == "") {
            ParameterValue = $flds.eq(index).find('.hgResult').attr('result');
        }
        Parameters += ParameterValue + '|';
    });

    if (Parameters != "") {
        Parameters = Parameters.substr(0, Parameters.length - 1);
    }

    $('#txtReportParameters').val(Parameters);
    $("form:first").submit();
    //__doPostBack('__Page', '');
}
//#endregion

//#region Setup users
function ResetPassword(e, p) {

    var HTML = "<div style='padding: 10px;'>Enter a new password:<br /><br /><input id=\"txtPassWord\" type=\"text\" class=\"form-control\" /></div>";

    if (p == null) {
        dialog = $(HTML).dialog({
            autoOpen: false,
            title: 'Reset Password',
            height: 240,
            width: 300,
            dialogClass: 'no-close',
            modal: true,
            buttons: {
                Cancel: {
                    text: 'Cancel', priority: 'secondary', click: function () {
                        dialog.dialog("close");
                    }
                },
                'Change Password': {
                    text: 'Change Password', priority: 'primary', style: 'background: #428BCA; color: #fff;', click: function () {
                        ResetPassword(e, escape($('#txtPassWord')[0].value));
                        return;
                    }
                }
            }
        });
        dialog.dialog("open");
    }

    if (p == null) return;

    var jqxhr = $.getJSON("Process_Request.aspx", {
        action: "ResetPassword",
        password: p,
        kfv: $(e).closest('tr').attr('kfv'),
        tableselector: $(e).closest('.table').selector,
        tdindex: $(e).closest('td').index()
    })
        .done(function (data) {
            gridBox({
                boxType: 'message',
                title: 'Password Reset',
                body: data.message,
                type: data.messagetype,
                yesFunction: function () { dialog.dialog("close"); }
            });

        })
        .fail(function (data) {
            gridBox({
                boxType: 'message',
                title: 'Password Reset',
                body: 'Password Reset failed',
                type: 'danger'
            });
        });

    /*
    jqxhr.complete(function () {
        alert("second complete");
    });
    */
}

function UnlockAccount(e) {
    var jqxhr = $.getJSON("Process_Request.aspx", {
        action: "Unlock",
        kfv: $(e).closest('tr').attr('kfv'),
        tableid: $(e).closest('.table')[0].id,
        tdindex: $(e).closest('td').index()
    })
        .done(function (data) {
            var r = data;
            $('#' + r.tableid).find('tr[kfv="' + r.kfv + '"] td').eq(r.tdindex).html('');
            gridBox({
                boxType: 'message',
                title: 'Unlock Account',
                body: r.message,
                type: r.messagetype
            });

            //alert("done");
        }).fail(function (data) {
            gridBox({
                boxType: 'message',
                title: 'Unlock Account',
                body: 'Unlock Account failed',
                type: 'danger'
            });
        });
}

function EmailResetLink(e) {
    var jqxhr = $.getJSON("Process_Request.aspx", {
        action: "EmailResetLink",
        kfv: $(e).closest('tr').attr('kfv'),
        tableid: $(e).closest('.table')[0].id,
        tdindex: $(e).closest('td').index()
    })
        .done(function (data) {
            var r = data;
            gridBox({
                boxType: 'message',
                title: 'Email Reset Link',
                body: r.message,
                type: r.messagetype
            });
            //alert("done");
        }).fail(function (data) {
            gridBox({
                boxType: 'message',
                title: 'Email Reset Link ',
                body: 'Email Reset Link  failed',
                type: 'danger'
            });
        });
}

function EmailWelcomeLink(e) {
    var jqxhr = $.getJSON("Process_Request.aspx", {
        action: "EmailWelcomeLink",
        kfv: $(e).closest('tr').attr('kfv'),
        tableid: $(e).closest('.table')[0].id,
        tdindex: $(e).closest('td').index()
    })
        .done(function (data) {
            var r = data;
            gridBox({
                boxType: 'message',
                title: 'Email Welcome Link',
                body: r.message,
                type: r.messagetype
            });
            //alert("done");
        }).fail(function (data) {
            gridBox({
                boxType: 'message',
                title: 'Email Welcome Link',
                body: 'Email Welcome Link failed',
                type: 'danger'
            });
        });
}

function LoginAs(e) {
    location.href = 'workorders.aspx?l=' + e.attr('login');
}
//#endregion

function OpenDoc(ID) {
    poster({ url: 'display_doc', target: '_blank', data: { did: ID } });
}

function poster(o) {

    /*  usage
    poster( { 
                url: 'display_doc', 
                target: '_blank', 
                data: { did: ID } 
            });
     */


    $.each(o.data, function (key, value) {
        $('form:first input[id="' + key + '"]').remove();
        $('form:first').append('<input type="hidden" id="' + key + '" name="' + key + '" value="' + value + '" />');
    });

    $('form:first').attr('target', o.target);
    $('form:first').attr('action', o.url);
    $('form:first').submit();
}

//#region Setup
function LeftMenu(Topic) {

    // mouse pointer
    $('body').css('cursor', 'wait');
    //WaitBox(Topic, 'Please wait.', info);

    // get div vertical scroll value
    if (document.getElementById("divLeftMenu") != null) {
        SetCookie("divLeftMenu", document.getElementById("divLeftMenu").scrollTop, 1);
    }

    document.forms[0].txtTopic.value = Topic;
    document.forms[0].submit();
}
//#endregion

//#region dropdowns
function FilterCascading(e) {

    var result = e.options[e.selectedIndex].innerHTML;

    // filter dependent selects
    var id = e.id.toLowerCase();
    $t = $(e).closest('.hgeditformtable');
    var hgID = $t.attr('srctableid');
    var sel = $('#' + hgID + ' th[ftp*="' + id + '"]').attr('ftp');
    $s = $t.find('select:has(option[' + id + '])');

    $s.each(function (index, el) {

        // copy reference
        $sel = $(el).parent().attr('sel');
        // if does not exist, create
        if (typeof ($sel) == 'undefined') {
            $(el).parent().attr('sel', $(el).html());
            $sel = $(el).parent().attr('sel');
        }
        else {
            // reset select
            $(el).html($sel);
        }



        // remove unconforming
        $(el).find('option[' + id + '!="' + result + '"][' + id + '!=""][' + id + ']').remove();
    });
}

function ApplyCascades(e) {
    $(e).find('select').each(function (index, el) {
        FilterCascading(el);
    });
}

function FilterSelect(e) {

    var result = e.options[e.selectedIndex].innerHTML;
    var tid = e.getAttribute("tid");
    var ta = e.getAttribute("ta");

    var hgID = $(e).closest('.hg').attr("id");

    var sel;
    $('#' + hgID + ' th[ftp]').each(function (index, value) {
        var s = $(value).attr('ftp');
        if (s.indexOf("_id='selItems'") > -1) {
            sel = s;
            return false;
        }

    });

    $("#" + tid).html(sel);

    // remove unconforming
    $("#" + tid).each(function (index, value) {
        $(value).find('option[' + ta + '!="' + result + '"][' + ta + '!=""][' + ta + ']').remove();
    });

}

//#endregion

function download(filename, text) {
    var element = document.createElement('a');
    element.setAttribute('href', 'data:text/csv;charset=utf-8,' + encodeURIComponent(text));
    element.setAttribute('download', filename);

    element.style.display = 'none';
    document.body.appendChild(element);

    element.click();

    document.body.removeChild(element);
}

function RelativePixels(wh, p, m) {
    // window dimensions
    var r;
    if (wh == 'w') {
        r = parseInt($(window).innerWidth() * p);
        if (r > m && m != null) {
            r = m;
        }
    }
    else {
        r = parseInt($(window).innerHeight() * p);
        if (r > m && m != null) {
            r = m;
        }
    }
    return r;
}

function ReloadPage() {
    var url = location.href;

    // strip bookmark
    if (url.indexOf("#") > -1) {
        url = url.substr(0, url.indexOf("#"), url.length - url.indexOf("#"));
    }
    location.href = url;
}

function SetGridPage(p, c) {
    //GrayOut(true);
    SetCookie(c, p, 1);
    //ReloadPage();
    //window.location = window.location.href;
    document.forms[0].submit();
}

function ParseQueryString(t, URL) {

    if (URL == null) {
        URL = document.location.href;
    }

    if (typeof (t) == "undefined") {
        return URL.split("?")[1];
    }

    URL = "&" + URL.split("?")[1] + "&";
    var Start = URL.indexOf("&" + t + "=");

    if (Start == -1) {
        return "";
    }

    var End = URL.indexOf("&", Start + t.length + 2);

    return URL.substr(Start + t.length + 2, End - Start - t.length - 2);
}

function RenewSession() {
    var jqxhr = $.getJSON("Process_Request.aspx", {
        action: "RenewSession"
    })
        .done(function (data) {
            var r = data;
            //MessageBox("Password Reset", r.message, r.messagetype);

            if (r.messagetype == "success") {
                //dialog.dialog("close");
                $('#diaConfirm').dialog('destroy').remove();
            }
        })
        .fail(function (data) {
            alert("Password session renewal failed");
        });
}

//#region Picker
function PopPicker(e) {

    if ($(e).hasClass('disabled')) return;

    if (e != null) {
        LastPopPickere = e;
        f = e.getAttribute('f');
        LastPopPickerf = f;
        LastPopPickerw = e.getAttribute('w');
    }

    if (e == null) {
        e = LastPopPickere;
        f = LastPopPickerf;
        w = LastPopPickerw;
    }


    var q = e.getAttribute('q');
    var info = e.getAttribute('info').trim();
    var Title = $(e).closest('tr').find('td:first').text() + ' - ' + $(e).text();
    var Type = e.getAttribute('template');

    //validate foreign keys specified
    var fks = info.split(',');

    if (fks.length > 3) {
        for (var i = 3; i < fks.length; i++) {
            if ($('#pick' + fks[i]).attr('resultkey') == '' && $('#pick' + fks[i]).attr('result') == '') {
                var Label = $('#pick' + fks[i]).closest('td').prev().text();
                //MessageBox(Label + ' Selection Required', 'Please select a value for ' + Label + ' first.', 'warning');
                gridBox({
                    boxType: 'message',
                    title: Label + ' Selection Required',
                    body: 'Please select a value for ' + Label + ' first.',
                    type: 'warning'
                });
                return;
            }
        }
    }

    var FormData = '';

    $('.fieldsform').find('.hgResult').each(function () {
        var f = $(this).attr('fn');

        var v = $(this).attr('resultkey');
        if (v == '') {
            v = $(this).attr('oresultkey');
        }

        if (v == '') {
            v = $(this).attr('result');
        }

        if (v == '') {
            v = $(this).attr('oresult');
        }

        if ($(this).attr('type') == 'text') {
            v = $(this).val();
        }

        var l = $(this).text().replace(/\|/g, ',');

        if (l.length > 50) {
            l = l.substring(0, 50) + '...';
        }

        if (v != '') {
            FormData += f + '|' + escape(v) + '|' + escape(l) + '|';
        }

    });

    // add detail row form data
    if ($(e).closest('.hg').hasClass('tabledetail')) {
        $(e).closest('tr').find('.hgResult').each(function () {
            var f = $(this).attr('id');

            if (f != null) {
                //f = f.substr(4);            
                if (f.substr(0, 3) == 'sel') f = f.substr(3);
                if (f.substr(0, 4) == 'pick') f = f.substr(4);

                var v = $(this).attr('resultkey');
                if (v == '') {
                    v = $(this).attr('oresultkey');
                }

                if (v == '') {
                    v = $(this).attr('result');
                }

                if (v == '') {
                    v = $(this).attr('oresult');
                }

                var l = $(this).text();

                if (v != '') {
                    FormData += f + '|' + v + '|' + l + '|';
                }
            }

        });
    }

    if (FormData != '') {
        FormData = FormData.substr(0, FormData.length - 1);
    }

    // initialize
    if ($('#diaPicker' + f).length == 0) {
        var Choices = $(e).attr('resultkey');
        if (Choices == "") {
            $(e).attr('oresultkey');
        }

        SetCookie('PickerChoice', Choices, 1);
        SetCookie('PickerSearch', '', -1);
        SetCookie('PickerType', Type, 1);

        //if (e.id != "pickProductIDs") {
        SetCookie('PickerIDs', '', 1);
        SetCookie('PFSearch', '', 1);
        //}
    }

    var aFunction = function () {

        // reset highlighted row
        $(e).closest('tr').css('background-color', '');

        // get selected IDs
        var SelectedIDs = '';
        var SelectedTexts = '';
        $('#diaPicker' + f + ' .pickerselected').each(function () {
            SelectedIDs += $(this).closest('tr').attr('kfv') + ', ';
            SelectedTexts += $(this).text() + ', ';
        });

        if (SelectedIDs != "") {
            SelectedIDs = SelectedIDs.substr(0, SelectedIDs.length - 2);
            SelectedTexts = SelectedTexts.substr(0, SelectedTexts.length - 2);
        }

        if (SelectedTexts.length > 200) {
            SelectedTexts = SelectedTexts.substr(0, 190) + '... (' + $('#diaPicker' + f + ' .pickerselected').length + ')';
        }

        // update e
        $(e).attr('resultkey', SelectedIDs);
        $(e).text(SelectedTexts);

        var Template = $(e).attr('template');

        if (Template == 'picker' || Template == 'multiplepicker') {
            $(e).attr('result', SelectedTexts);

            if (SelectedTexts == "") {
                $(e).text('None');
            }
        }

        if (f.indexOf('Spread') == 0) {
            // update the data
            $td = $(e).closest('td');
            var x = $td.attr('data-x');
            var y = $td.attr('data-y');

            document.getElementById(f).jexcel.setValueFromCoords(x, y, $td.html(), true);
        }

        //$('#diaPicker' + f).dialog('destroy').remove();

        // focus on next fields
        if (f.indexOf('Product_Designer_') == -1) {

            //var nr = $(e).closest('tr').next('tr').find('.hgResult');
            var nr = $(e).closest('tr').nextAll('tr:visible:first').find('.hgResult');

            if (f.indexOf('Receipts') > -1 && nr.attr('id') != 'pickCommodityID') {
                nr.focus();
                $('#diaPicker' + f).dialog('destroy').remove();
                return;
            }

            if (nr)

                if (nr.find('input').length == 1) {
                    nr.find('input').caretToEnd();
                }
                else if (nr.is('a')) {
                    //nr.focus();
                    //setTimeout(function () { nr.click(); }, 500);
                    setTimeout(function () {
                        nr.focus();

                        if (nr.text() == 'None') {
                            nr.click();
                        }
                    }, 500);
                }
                else {
                    nr.focus();
                }
        }


        setTimeout(function () {
            PickerSavePost(e, f);
        },
            1000
        );


        $('#diaPicker' + f).dialog('destroy').remove();
        return true;
    };

    var w = RelativePixels('w', .9);
    var h = RelativePixels('h', .9);
    var my = null;
    var at = null;

    /*
    // existing picker action

    if ($('#diaPicker' + LastPopPickerf).length == 1) {

        var action = $('#diaPicker' + LastPopPickerf + ' form').attr('action');
        w = ParseQueryString('w', action);
        h = ParseQueryString('h', action);
    }
    

    var pdw = GetCookie('PickerDetailWidth') + '|' + GetCookie('PickerDetailWidthDetail');

    var find = e.id.replace(/pick/i, '');

    var ww = GetPairValue(pdw, find, 0);

    if (ww != 0) {
        w = ww;
        my = 'right';
        at = 'right-50';
    }
    */

    if (LastPopPickerw != '') {
        w = LastPopPickerw;
        my = 'right';
        at = 'right-50';
    }

    // highlight field row
    $(e).closest('tr').addClass('ffrhighlight');

    var DialogPicker;
    PopDialog(DialogPicker, 'Picker' + f, my, at, null, w, h, Title, '&eid=' + e.id + "&pq=" + q + "&info=" + escape(info) + "&ty=" + Type + '&fd=' + escape(FormData), "picker.aspx", 'Select', aFunction);

}

function PickerAdd(e, inf) {

    var infs = inf.split(',');

    var Context = infs[0].trim();
    var Source = infs[1].trim();
    var KeyField = infs[2].trim();
    var ForeignKey = "";
    var fkv = "";

    if (infs.length > 3) {
        for (var i = 3; i < infs.length; i++) {
            ForeignKey = infs[i].trim();
            var ForeignFieldValue = $('#pick' + ForeignKey).attr('resultkey');
            if (ForeignFieldValue == "") {
                ForeignFieldValue = $('#pick' + ForeignKey).attr('oresultkey');
            }

            fkv += ForeignKey + '|' + ForeignFieldValue + '|';
        }


        if (fkv != "") {
            fkv = fkv.substr(0, fkv.length - 1);
        }
    }

    //alert(Info);

    var f = function () {

        // save data
        //var valid = SaveGrid(Context + 'Editor', 'PickerUpdate|' + Context);
        SaveGrid(Context + 'Editor', function (r) {

            WaitUntilGridSaves(Context + 'Editor', function () {

                // get the new ID
                var kfv = r.split('|')[1];

                // add new item to picker choices
                var Type = GetCookie('PickerType');
                if (Type == 'picker') {
                    SetCookie('PickerChoice', kfv);
                    SetCookie('PickerChoiceAdd', kfv);
                }
                else // multiple
                {
                    var PickerChoices = GetCookie('PickerChoice');

                    if (PickerChoices == '') {
                        PickerChoices = kfv;
                    }
                    else {
                        PickerChoices += ', ' + kfv;
                    }

                    SetCookie('PickerChoice', PickerChoices);
                }

                // close dialog
                $('#dia' + Context).dialog('destroy').remove();

                // refresh the picker
                //PopPicker(document.getElementById('Picker' + Context));
                NoExec = true;
                PopPicker();
                //Picker = $('#diaPicker' + LastPopPickerf);
                //Picker.load(Picker.attr('qs'));

                return;
            });
        });
        //return valid;
    };

    FieldsEditor(e, Context, Source, KeyField, -1, fkv, f);
}

function PickerKeyPress(e, ev) {

    if ($(e).hasClass('disabled')) return;

    var code = ev.keyCode || ev.which;

    ev.stopPropagation();
    ev.preventDefault();

    if (code == 13 || code == 32) {
        PopPicker(e);
    }
}

function PickerFocus(e) {
    //alert(1);
}

function PickerChoicePost(e, type) {

    var t = $(e).closest('table');

    if ((t.attr('id') == 'tabPickerBill' || t.attr('id') == 'tabPickerBrokerBill' || t.attr('id') == 'tabPickerBill_New' || t.attr('id') == 'tabPickerBrokerBill_New') && type == 'multiplepicker') {

        /*
        var s = t.find('tbody .pickerselected');

        if (s.length > 0) {
            var c = s.first().next('td').text();

            if (c != "") {
                t.find('tbody:last tr td:nth-child(2)').filter(function () { return $(this).text() != c; }).closest('tr').css('color', 'lightgray').css('pointer-events', 'none');
                //t.find('tbody:last tr td:nth-child(2)').filter(function () { return $(this).text() != c; }).closest('tr').prop('disabled',true);
            }
        }
        else {
            t.find('tbody:last tr').css('color', 'black').css('pointer-events', '');
        }
        */
    }
}

function PickerSavePost(e, f) {
    if (f == 'Order_Entry') {
        // check if LEED selected in Agreement or any other field
        var t = $(e).closest('.vg');
        if (t.find('.vgField:contains("TX LEED")').length > 0) {
            t.find('.vgLabel:contains("LEED")').closest('tr').show();
        }
        else {
            t.find('.vgLabel:contains("LEED")').closest('tr').hide();
        }
    }

    //if (f == 'Bill' || f == 'Bill_New' || f == 'BrokerBill' || f == 'BrokerBill_New' || f == 'VendorBills') {

    // handle document location for all forms
    var $c = $(e);


    //if ($bl.val() == '' && $c.attr('resultkey') != '') {
    if ($c.attr('resultkey') != '' && ($c.attr('id') == 'pickConfirmID' || $c.attr('id') == 'pickPositionIDs')) {

        // Bill lOcation
        var $bl = $('input[fn="kxGB59jPLKr1ax2MomXG9A=="]');

        // Document location
        if ($bl.length == 0) {
            $bl = $('input[fn="C94C+Lb+DUtQTgo9VETAzxfBts+jEsRQ"]');
        }

        var cid = '';
        var pid = '';

        if ($c.attr('id') == 'pickConfirmID') {
            cid = $c.attr('resultkey');
        } else if ($c.attr('id') == 'pickPositionIDs') {
            pid = $c.attr('resultkey');
        }


        // get location from confirm
        var jqxhr = $.getJSON("Process_Request.aspx", {
            action: "ConfirmDocumentLocation",
            cid: cid,
            pid: pid
        })
            .done(function (data) {
                if (data.messagetype == 'success') {
                    $bl.val(decodeURIComponent(unescape(data.message)));
                }
                else {
                    //MessageBox('Error', decodeURIComponent(unescape(data.message)), 'danger');
                    gridBox({
                        boxType: 'message',
                        title: Error,
                        body: decodeURIComponent(unescape(data.message)),
                        type: 'danger'
                    });
                }

                //alert("done");
            }).fail(function (data) {
                alert("Confirm Document Location failed");
            });
    }


    //}
}

function PickerSelectionEdit(e, ev) {

    ev.stopPropagation();
    ev.preventDefault();

    /*
    var action = $(e).closest('form').attr('action');
    var info = unescape(ParseQueryString('info', action));
    var eid = unescape(ParseQueryString('eid', action));
    */

    //var info = unescape($('.ffrhighlight a').last().attr('info'));
    //var eid = unescape($('.ffrhighlight a').last().attr('id'));

    /*
    var qs = unescape($('#dia' + spreadid).attr('qs'));
    */

    var qs = unescape($(e).closest('[qs]').attr('qs'));

    var info = ParseQueryString('info', qs);
    var eid = ParseQueryString('eid', qs);


    var infs = info.split(',');
    var key = $(e).closest('tr').attr('kfv');

    var Context = infs[0].trim();
    var Source = infs[1].trim();
    var KeyField = infs[2].trim();

    var wc = GetCookie('PickerDetailWidth');

    var f = function () {

        // save data
        var valid = SaveGrid(Context + 'Editor', function () {
            // reload picker         
            SetCookie('PickerDetailWidth', wc, 1);
            PopPicker(document.getElementById(eid));
            $('#dia' + Context).dialog('destroy').remove();
        });
        //return valid;
    };

    FieldsEditor(e, Context, Source, KeyField, key, null, f, 'left', 'left+60', window);
}

function ResearchPicker(ev, e) {
    ev.stopPropagation();
    ev.preventDefault();

    // search text
    var s = $(e).closest('td').text().trim();

    var searchbox = $('#txtPickerSearch');

    searchbox.val(s);

    SearchPicker(searchbox);
}

//#endregion

//#region detail
function PopDetail(e, cw) {

    if ($(e).hasClass('disabled')) return;

    if (e != null) {
        LastPopDetaile = e;
        f = e.getAttribute('f');
        LastPopDetailf = f;
    }

    if (e == null) {
        e = LastPopDetaile;
        f = LastPopDetailf
    }


    var q = e.getAttribute('q');
    var info = e.getAttribute('info');
    var Title = $(e).closest('tr').find('td:first').text() + ' - ' + $(e).text();
    var Type = e.getAttribute('template');

    //validate foreign keys specified
    var fks = info.split(',');

    if (fks.length > 3) {
        for (var i = 3; i < fks.length; i++) {
            if ($('#pick' + fks[i]).attr('resultkey') == '' && $('#pick' + fks[i]).attr('result') == '') {
                var Label = $('#pick' + fks[i]).closest('td').prev().text();
                //MessageBox(Label + ' Selection Required', 'Please select a value for ' + Label + ' first.', 'warning');
                gridBox({
                    boxType: 'message',
                    title: Label + ' Selection Required',
                    body: 'Please select a value for ' + Label + ' first.',
                    type: 'warning'
                });
                return;
            }
        }
    }

    // add form key field
    var kt = $(e).closest('.vg');
    var kf = kt.attr('kf');
    var kfv = kt.attr('kfv');

    //var FormData = 'Designer_ID|1000|1000|';
    var FormData = kf + '|' + kfv + '|' + kfv + '|';

    $('.fieldsform').find('.hgResult').each(function () {
        var f = $(this).attr('fn');

        var v = $(this).attr('resultkey');
        if (v == '') {
            $(this).attr('oresultkey');
        }

        if (v == '') {
            $(this).attr('result');
        }

        if (v == '') {
            $(this).attr('oresult');
        }

        var l = $(this).text().replace(/\|/g, ',');

        if (l.length > 50) {
            l = l.substring(0, 50) + '...';
        }

        if (v != '') {
            FormData += f + '|' + escape(v) + '|' + escape(l) + '|';
        }

    });

    if (FormData != '') {
        FormData = FormData.substr(0, FormData.length - 1);
    }

    // initialize
    if ($('#diaDetail' + f).length == 0) {
        var Choices = $(e).attr('resultkey');
        if (Choices == "") {
            $(e).attr('oresultkey');
        }

        SetCookie('DetailChoice', Choices, 1);
        SetCookie('DetailSearch', '', -1);
        SetCookie('DetailType', Type, 1);
    }

    var aFunction = function () {

        f = f.replace('Detail', '');

        if (f == 'SalesForce') {

            ValidateGrid({
                gridid: 'tabDetail' + f,
                onvalid: function (change) {

                    //alert('valid');
                    SaveGrid('Detail' + f, 'UpdateDetail');

                    /*
                    WaitUntilGridSaves('Detail' + f, function () {
                        setTimeout(function () { DetailUpdate(); }, 3000);
                    });
                    */
                    var g = $('#tabDetail' + f);

                    WaitUntil(function () {
                        return g.attr('psd') == 1;
                    }, function () {
                        g.attr('psd', '');
                        setTimeout(function () { DetailUpdate(); }, 500);
                    });
                },
                oninvalid: function () { return false; }
            });
        }
        else {
            SaveGrid('Detail' + f, 'UpdateDetail');

            WaitUntilGridSaves('Detail' + f, function () {
                DetailUpdate();
            });
        }

        return;



        /*
         $.when(SaveGrid('Detail' + LastPopDetailf, 'UpdateDetail')).then(function (valid) {
             //alert(valid);
             alert(valid);
             
             }).fail(function () {
                 alert(2);
             });
          */



        //var valid = SaveGrid('Detail' + LastPopDetailf, 'UpdateDetail');
        var valid = SaveGrid('Detail' + LastPopDetailf, 'UpdateDetail');

        if (!valid) {
            return valid;
        }

        //return true;
    };

    var w = RelativePixels('w', .9);
    var h = RelativePixels('h', .7); //RelativePixels('h', .7, 800)
    var my = null;
    var at = null;

    var pdw = GetCookie('PickerDetailWidth');

    var find = e.id.replace(/pick/i, '');

    var ww = GetPairValue(pdw, find, 0);

    if (ww != 0) {
        w = ww;
        my = 'right';
        at = 'right-50';
    }

    if (cw == null) {
        cw = '';
    }

    // highlight field row
    //$(e).closest('tr').removeClass('ffrhighlight');
    $(e).closest('tr').addClass('ffrhighlight');


    var DialogDetail;
    PopDialog(DialogDetail, 'Detail' + f, my, at, null, w, h, Title, '&eid=' + e.id + "&pq=" + q + "&info=" + info + "&ty=" + Type + '&fd=' + escape(FormData) + '&cw=' + cw, "detail.aspx", 'Save', aFunction);

    var buttons = $('#' + 'diaDetail' + f).parent().find('.ui-dialog-buttonset');

    buttons.before($('<button type="button" priority=secondary" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only" onclick="DetailClone(this);" ><span class="ui-button-text">Clone Current</span></button>'));

    WaitUntil(function () {
        return $('#' + 'tabDetail' + f).length == 1;
    }, function () {
        $('#' + 'tabDetail' + f + ' th[fr="1"]').attr('fr', '_1');
    });
}

function DetailKeyPress(e, ev) {

    if ($(e).hasClass('disabled')) return;

    var code = ev.keyCode || ev.which;

    event.preventDefault();

    if (code == 13 || code == 32) {
        PopDetail(e);
    }
}

function DetailFocus(e) {
    //alert(1);
}

function DetailUpdate() {

    var e = LastPopDetaile;

    // get selected IDs
    var SelectedIDs = '';
    var SelectedTexts = 'None';
    /*
    $('#diaDetail' + LastPopDetailf + ' tbody:last tr td:first-child').not(':empty').each(function () {
 
        SelectedIDs += $(this).closest('tr').attr('kfv') + ', ';
        //SelectedTexts += $(this).closest('tr').attr('kfv') + ', ';
            
        //var st = $(this).find('select:not([result=""])').attr('result');
        var st = "";
        if ($(this).find('select').length > 0) {
 
            st = $(this).find('select').attr('result');
        }
        else if ($(this).find('input').length > 0) {
            
            st = $(this).find('input').attr('result');
        }
        else
        {
            st = $(this).text();
        }
 
        var st2 = "";
 
        if ($(this).next().find('select').length > 0) {
 
            st2 = $(this).next().find('select').attr('result');
        }
        else if ($(this).next().find('input').length > 0) {
 
            st2 = $(this).next().find('input').attr('result');
        }
        else {
            st2 = $(this).next().text();
        }
 
        if (st != 'None') {
            SelectedTexts += st + '-' + st2 + ', ';
        }
            
 
    });
    */

    // already saved
    $('#diaDetail' + LastPopDetailf + ' tbody:last tr[class=""], tbody:last tr:not([class])').each(function () {
        SelectedIDs += $(this).closest('tr').attr('kfv') + ', ';
    });

    // under edit with results
    //$('#diaDetail' + LastPopDetailf + ' tbody:last .RowEdit').has('.hgResult:not([result=""]) , .hgResult:not([resultkey="0"])').each(function () {
    $('#diaDetail' + LastPopDetailf + ' tbody:last .RowEdit').each(function () {
        //if (IsDirty($(this).find('.hgResult'))) {
        if ($(this).find('.hgResult:not([result=""]).hgResult:not([resultkey="0"])').length > 0) {
            SelectedIDs += $(this).closest('tr').attr('kfv') + ', ';
        }
    });

    // edit but no focus
    $('#diaDetail' + LastPopDetailf + ' tbody:last .hgNewRecord').not('.RowEdit').has('td:not(:empty)').each(function () {
        SelectedIDs += $(this).closest('tr').attr('kfv') + ', ';
    });

    if (SelectedIDs != "") {
        SelectedIDs = SelectedIDs.substr(0, SelectedIDs.length - 2);

        /*
        //SelectedTexts = SelectedTexts.substr(0, SelectedTexts.length - 2);
        var ItemCount = SelectedIDs.split(',').length;
        if (ItemCount == 1) {
            SelectedTexts = SelectedIDs.split(',').length + ' Item';
        }
        else {
            SelectedTexts = SelectedIDs.split(',').length + ' Items';
        }
        */

        // determine picker element
        var PickerElement = e;

        var PickerCurrentIDs = SelectedIDs;
        var PickerForm = $(PickerElement).attr('f');
        var PickerField = $(PickerElement).attr('fn');

        var FieldForm = $(PickerElement).closest('.vg, .hg');

        var kf = FieldForm.attr('kf');
        var kfv = FieldForm.attr('kfv');
        var tb = FieldForm.attr('tb');

        var jqxhr = $.getJSON("Process_Request.aspx", {
            action: "PickerLink",
            cids: PickerCurrentIDs,
            nid: '',
            form: PickerForm,
            field: PickerField,
            kf: kf,
            kfv: kfv,
            table: tb
        })
            .done(function (data) {

                $(PickerElement).attr('resultkey', data.LinkIDs);
                $(PickerElement).text(data.LinkText);

                $('#diaDetail' + LastPopDetailf).dialog('destroy').remove();

                // focus on next fields
                if (f.indexOf('Product_Designer_') == -1) {

                    //var nr = $(e).closest('tr').next('tr').find('.hgResult');
                    var nr = $(e).closest('tr').nextAll('tr:visible:first').find('.hgResult');

                    if (nr.find('input').length == 1) {
                        nr.find('input').caretToEnd();
                    }
                    else if (nr.is('a')) {
                        //nr.focus();
                        //setTimeout(function () { nr.click(); }, 500);
                        setTimeout(function () {
                            nr.focus();

                            if (nr.text() == 'None') {
                                nr.click();
                            }
                        },
                            500);
                    }
                    else {
                        nr.focus();
                    }
                }

                //alert("done");
            }).fail(function (data) {
                alert("PickerLink failed.");
            });
    }

    /*
    // update e
    $(e).attr('resultkey', SelectedIDs);
    $(e).text(SelectedTexts);

    $('#diaDetail' + LastPopDetailf).dialog('destroy').remove();


    // focus on next fields
    if (f.indexOf('Product_Designer_') == -1) {

        //var nr = $(e).closest('tr').next('tr').find('.hgResult');
        var nr = $(e).closest('tr').nextAll('tr:visible:first').find('.hgResult');

        if (nr.find('input').length == 1) {
            nr.find('input').caretToEnd();
        }
        else if (nr.is('a')) {
            //nr.focus();
            //setTimeout(function () { nr.click(); }, 500);
            setTimeout(function () {
                nr.focus();

                if (nr.text() == 'None') {
                    nr.click();
                }
            },
                500);
        }
        else {
            nr.focus();
        }
    }
    */
}

//#endregion

//#region search
function Search(s, ev) {
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
    else
    {
        Search = s.value;
    }

    // reset filters


    var page = location.pathname.split('/').slice(-1)[0];

    // set page search cookie
    SetCookie('Search_' + page, Search, 1);

    poster({
        url: document.URL,
        data: { search: Search }
    });

}

function SearchField(e, ev) {
    if (ev.keyCode != 13) {
        return;
    }

    var SF = '';

    var sep = '¶';
    var sep2 = '§';

    $(e).closest('tr').find('.SearchField').each(function () {
        if ($(this).val().trim() != '') {
            SF += $(this).attr('sf') + sep + escape($(this).val()) + sep2;
        }
    });

    if (SF != "") {
        SF = SF.substr(0, SF.length - 1);
    }

    SetCookie("SearchData", SF, 1);

    document.forms[0].txtSearch.value = SF;
    document.forms[0].submit();
}

//#endregion

function formatDate(date, format) {

    var MONTH_NAMES = new Array('January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December', 'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec');
    var DAY_NAMES = new Array('Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat');
    format = format + "";
    var result = "";
    var i_format = 0;
    var c = "";
    var token = "";
    var y = date.getYear() + "";
    var M = date.getMonth() + 1;
    var d = date.getDate();
    var E = date.getDay();
    var H = date.getHours();
    var m = date.getMinutes();
    var s = date.getSeconds();
    var yyyy, yy, MMM, MM, dd, hh, h, mm, ss, ampm, HH, H, KK, K, kk, k;
    // Convert real date parts into formatted versions
    var value = new Object();
    if (y.length < 4) { y = "" + (y - 0 + 1900); }
    value["y"] = "" + y;
    value["yyyy"] = y;
    value["yy"] = y.substring(2, 4);
    value["M"] = M;
    value["MM"] = LZ(M);
    value["MMM"] = MONTH_NAMES[M - 1];
    value["NNN"] = MONTH_NAMES[M + 11];
    value["d"] = d;
    value["dd"] = LZ(d);
    value["E"] = DAY_NAMES[E + 7];
    value["EE"] = DAY_NAMES[E];
    value["H"] = H;
    value["HH"] = LZ(H);
    if (H == 0) { value["h"] = 12; }
    else if (H > 12) { value["h"] = H - 12; }
    else { value["h"] = H; }
    value["hh"] = LZ(value["h"]);
    if (H > 11) { value["K"] = H - 12; } else { value["K"] = H; }
    value["k"] = H + 1;
    value["KK"] = LZ(value["K"]);
    value["kk"] = LZ(value["k"]);
    if (H > 11) { value["a"] = "PM"; }
    else { value["a"] = "AM"; }
    value["m"] = m;
    value["mm"] = LZ(m);
    value["s"] = s;
    value["ss"] = LZ(s);
    while (i_format < format.length) {
        c = format.charAt(i_format);
        token = "";
        while ((format.charAt(i_format) == c) && (i_format < format.length)) {
            token += format.charAt(i_format++);
        }
        if (value[token] != null) { result = result + value[token]; }
        else { result = result + token; }
    }
    return result;
}

function CopyToClipboard(text, ev) {

    //ev.preventDefault();

    var $temp = $("<input>");
    $("body").append($temp);
    $temp.val(text).select();
    document.execCommand("copy");
    $temp.remove();
}

function WaitUntil(eFunction, xFunction, ms) {

    var timer;

    var interval = ms;

    if (interval == null) {
        interval = 200;
    }

    timer = setInterval(function () {
        if (eFunction()) {
            clearTimeout(timer);
            xFunction();
        }
    }, interval);

}

function WaitUntilGridSaves(Context, xFunction) {
    var g = $('#dia' + Context).find('.hg, .vg');

    if (g.length == 0 || g.attr('id').indexOf('tabPicker') > -1) {
        xFunction();
        return;
    }

    WaitUntil(function () {
        return g.attr('ps') == '1';
    },
        function () {
            g.attr('ps') == '';
            xFunction();
        }
    );
}

function CloneButtonsInTitle(m) {

    var buttons = $('#' + m).parent().find('.ui-dialog-buttonset button');
    var topbuttons = buttons.clone().css('visibility', 'visible').css('margin-right', '6px').css('padding-left', '6px').css('padding-right', '6px').css('float', 'right').each(function () {
        $(this).attr('label', $(this).text());
        $(this).attr('title', $(this).text());
        $(this).text($(this).text().substr(0, 1));
    });

    topbuttons.on('click', function () {
        var Caption = $(this).attr('label');
        $('#' + m).parent().find('.ui-dialog-buttonset button:contains("' + Caption + '")').click();
    });

    for (var i = topbuttons.length - 1; i > -1; i = i - 1) {
        $('#' + m).parent().find('.ui-dialog-titlebar').append(topbuttons[i]);
    }
}

function previewTheme(e, ev) {
    ev.stopPropagation();
    gridSave({
        id: 'Grid',
        afterSave: function () {
            var row = $(e).closest('tr')
            var rowId = row.attr('kfv');
            var jqxhr = $.getJSON("Process_Request.aspx", {
                action: "PreviewTheme",
                kfv: rowId,
            }).done(function (data) {
                LeftMenu("Theme");
            }).fail(function (data) {
                alert("Theme change Failed");
            });
        }
    })
}

function setTheme(e, ev) {
    ev.stopPropagation();
    gridSave({
        id: 'Grid',
        afterSave: function () {
            var row = $(e).closest('tr')
            var rowId = row.attr('kfv');
            var jqxhr = $.getJSON("Process_Request.aspx", {
                action: "SetTheme",
                kfv: rowId,
            }).done(function (data) {
                LeftMenu("Theme");
            }).fail(function (data) {
                alert("Theme change Failed");
            });
        }
    })
}


function isNull(o, r) {
    if (o == null) {
        return r;
    }
    else {
        return o;
    }
}

function InitializeSignatures() {
    // signatures
    $('.signature').each(function (index) {

        var canvas = $(this).find('canvas')[0];
        var signaturePad = new SignaturePad(canvas);

        canvas.setAttribute('blank', signaturePad.toDataURL());
        //canvas.prop('blank', signaturePad.toDataURL());

        signaturePad.fromDataURL(canvas.getAttribute('result'));

        $(this).find('a:first').on('click', function () {
            signaturePad.clear();
        });
    });
}

function GetResultSignature(e) {
    var result = e.toDataURL();
    //var resultkey = result;
    e.setAttribute("result", result);
    //e.setAttribute("resultkey", result);
}