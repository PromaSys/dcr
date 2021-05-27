function displayTemplateFields(elem) {

    let templateId = getRowKeyFieldValue(elem);
    let templateName = getRowValueByColumnName(elem, 'Name');

    gridPop({
        context: `TemplateFields`,
        type: 'horizontal',
        title: `${templateName} Fields`,
        w: 1200,
        h: 600,
        data: {
            Template_ID: templateId
        }
    })
}

function getRowValueByColumnName(elem, columnName) {
    return getRowElementByColumnName(elem, columnName).innerText;
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
    let rowChoicesElement = getRowElementByColumnName(event.target, 'Choices')
    $("#selFieldType").on('change', (event) => {
        jqxhr = $.getJSON("Process_Request.aspx", {
            action: "IsAChoiceField",
            selectedValueId: event.target.value
        }).done(function (res) {
            if (res.isAChoiceField === "True") {
                let choicesLink = createTag({
                    tagName: 'a', tagTextContent: 'Edit Choices',
                    attributes: {
                        'href': '#',
                        'onclick': 'displayChoicesEditor(this, event)'
                    },
                });
                $(rowChoicesElement).html(choicesLink);
            } else {
                $(rowChoicesElement).html('Field type does not have choices');
            }
        });
    })
}



function closeDialogFromContext(context) {
    $('#dia' + context).dialog('destroy').remove()
}

function displayChoicesEditor(elem, event) {

    let fieldId = getRowKeyFieldValue(elem);
    let fieldName = getRowValueByColumnName(elem, 'Field');
    closeDialogFromContext('TemplateFields');

    gridPop({
        context: 'FieldChoices',
        type: 'horizontal',
        title: `${fieldName} Field`,
        w: 1200,
        h: 600,
        data: {
            Field_ID: fieldId
        }
    })

}

function createTag(args) {
    let tag = document.createElement(args.tagName);
    let tagText = document.createTextNode(args.tagTextContent);

    args.attributes && Object.keys(args.attributes).forEach((attrKey) => {
        tag.setAttribute(attrKey, args.attributes[attrKey])
    })

    tag.appendChild(tagText);
    return tag
}
