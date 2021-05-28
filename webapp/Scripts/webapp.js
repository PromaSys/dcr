
function createTemplate() {

    gridPop({
        context: `SelectCategory`,
        type: 'vertical',
        title: 'New Template - Please select a Category',
        w: 400,
        h: 240,
        saveFunction: function () {
            let selectTag = vGridGetFieldValueByFieldName('Category')
            let categoryId = selectTag.value;
            closeDialogFromContext('NewTemplate');
            gridPop({
                context: `NewTemplate`,
                type: 'vertical',
                title: 'Enter Template information',
                w: 400,
                h: 500,
                data: {
                    categoryId: categoryId
                }
            })
        }
    })
}

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

function displayTemplateCategories(elem) {
    let templateId = getRowKeyFieldValue(elem);
    let templateName = getRowValueByColumnName(elem, 'Name');

    gridPop({
        context: `TemplateCategories`,
        type: 'horizontal',
        title: `${templateName} Categories`,
        w: 400,
        h: 800,
        data: {
            Template_ID: templateId
        },
        afterSave: function () {
            ReloadPage();
        }
    })
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
            Template_Field_ID: fieldId
        }
    })

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

function vGridGetFieldValueByFieldName(fieldName) {
    let fieldElement = $(`td.vgLabel:contains('${fieldName}')`)
    return fieldElement.next().children(":first")[0];
}

function getRowValueByColumnName(elem, columnName) {
    return getRowElementByColumnName(elem, columnName).innerText;
}

function getRowElementByColumnName(elem, columnName) {
    let columnIndex = $(`.hgHeaderRow:last th:contains('${columnName}')`).index()
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

function closeDialogFromContext(context) {
    $('#dia' + context).dialog('destroy').remove()
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
