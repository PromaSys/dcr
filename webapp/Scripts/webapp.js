function displayTemplateFields(elem) {

    let templateId = getRowKeyFieldValue(elem);
    let templateTitle = getRowValueByColumnName(elem, 'Name');

    gridPop({
        context: `TemplateFields`,
        type: 'horizontal',
        title: `${templateTitle} Fields`,
        w: 1200,
        h: 600,
        data: {
            Template_ID: templateId
        }
    })
}

function getRowValueByColumnName(elem, columnName) {
    let columnIndex = $(`.hgHeaderRow th:contains("${columnName}")`).index();
    return $(elem).closest('tr').children().eq(columnIndex)[0].innerText;
}

function getRowElementByColumnName(elem, columnName) {
    let columnIndex = $(`.hgHeaderRow th:contains("${columnName}")`).index();
    return $(elem).closest('tr').children().eq(columnIndex)[0];
}


function getRowKeyFieldValue(elem) {
    return $(elem).closest('tr').attr('kfv')
}

function grabDialogByContext(context) {
    return $('#dia' + context);
}

function reloadDialog(dialog, endpoint, data) {
    dialog.load(endpoint, data);
}

function attachOnChangeToTypeSelect() {
    let choicesRowText = getRowElementByColumnName(event.target, 'Choices')
    $("#selFieldType").on('change', (event) => {
        jqxhr = $.getJSON("Process_Request.aspx", {
            action: "IsAChoiceField",
            selectedValueId: event.target.value
        }).done(function (res) {
            if (res.isAChoiceField === "True") {
                choicesRowText.textContent = 'Edit Choices';
            } else {
                choicesRowText.textContent = 'Field type does not have choices';
            }
        });
    })
}