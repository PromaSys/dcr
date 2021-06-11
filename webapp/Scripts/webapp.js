// templates related functions
function temlateFormPreview(elem) {

    let templateId = $(elem).closest('tr').attr('kfv');
    let templateName = $(elem).closest('tr').find('td:first').text();
    let formId = -1;
    let context = 'previewForm';


    gridPop({
        context: context,
        load: 'Form.aspx',
        title: templateName + ' - Preview',
        w: RelativePixels('w', 1.0, 550),
        h: RelativePixels('h', 0.85),
        data: {
            ftid: templateId,
            fid: formId,
            sid: 0
        },
        afterLoad: function (elem) {
        },
        buttons: {
            'Validate': {
                text: 'Validate',
                priority: 'secondary',
                style: 'background: #428BCA; color: #fff;',
                click: function () {
                    gridValidate({ id: context });
                }
            },

            'Close': {
                text: 'Close',
                priority: 'primary',
                style: 'background: #428BCA; color: #fff;',
                click: function () {
                    SetCookie('hgScrollTop', $('.tablediv:first').scrollTop(), 1);
                    $('#dia' + context).dialog('destroy').remove();
                    gridApplyFilters();
                }
            }
        }
    });

}

function editTemplateField(elem) {
    gridEditorForm({
        element: elem,
        w: RelativePixels('w', 1, 800),
        h: RelativePixels('h', .7),
        afterSave: function () {
            // close form
            $('#diaetabGridTemplateFields').dialog('destroy').remove();

            gridReloadDialog('TemplateFields', true);
        },
        afterLoad: function () {

            // enable/disable form controls based on existing loaded data


            // add an onchange to field type select to alter form controls

        }
    });
}

function templateFields(elem) {

    let context = 'TemplateFields'
    let templateId = $(elem).closest('tr').attr('kfv');
    let templateName = $(elem).closest('tr').find('td:first').text();

    gridPop({
        context: context,
        type: 'horizontal',
        title: `${templateName}`,
        w: RelativePixels('w', .9),
        h: RelativePixels('h', .75),
        data: {
            Template_ID: templateId
        },
        afterLoad: function (elem) {
            attachOnDragListenersToGrid()
        },
        buttons: {
            'Close': {
                text: 'Close',
                priority: 'primary',
                style: 'background: #428BCA; color: #fff;',
                click: function () {
                    // save scroll position
                    SetCookie('hgScrollTop', $('.tablediv:first').scrollTop(), 1);
                    // apply filters
                    gridApplyFilters();
                }
            }
        }
    });

    function drag(ev) {
        var rowKfv = $(ev.target).closest('tr').attr('kfv')
        var rowOriginalSortOrder = getRowValueByColumnName(ev.target, 'Sort Order');
        ev.originalEvent.dataTransfer.setData("fieldKfv", rowKfv);
        ev.originalEvent.dataTransfer.setData("rowOriginalSortOrder", rowOriginalSortOrder);

    }

    function drop(ev) {
        ev.preventDefault();
        var draggedRowKfv = ev.originalEvent.dataTransfer.getData("fieldKfv");
        var rowNewSortOrder = getRowValueByColumnName(ev.target, 'Sort Order');

        var firstElementInDraggedRow = $(`tr[kfv='${draggedRowKfv}']`).eq(1).first();

        //var newSortOrder = $(getRowElementByColumnName(firstElementInDraggedRow, 'Sort Order')).text(parseInt(rowNewSortOrder) - 50);

        var jqxhr = $.getJSON("Process_Request.aspx", {
            action: "UpdateTemplateFieldSortOrder",
            fid: draggedRowKfv,
            newso: rowNewSortOrder - 50,
        })
            .done(function (data) {
                $(`#butRefresh${context}`).click();
            }).fail(function (data) {
                alert("Update failed.");
            });


    }

    function attachOnDragListenersToGrid() {
        $('#diaTemplateFields').on("dragstart", function (ev) {
            drag(ev)
        })

        $('#diaTemplateFields').on("dragover", function (ev) {
            ev.preventDefault()
        })

        $('#diaTemplateFields').on("drop", function (ev) {
            drop(ev)
        })
    }

}

// form related functions
function form(formID, formTemplateID, subjectID) {

    gridPop({
        context: 'form',
        load: 'Form.aspx',
        title: 'Form - Subject',
        w: RelativePixels('w', 1.0, 550),
        h: RelativePixels('h', 0.85),
        data: {
            fid: formID,
            ftid: formTemplateID,
            sid: subjectID
        },
        saveFunction: function () {
            gridValidate({
                id: 'form',
                onvalid: function (changes) {
                    var changes = '';
                    $('#tabform .hgResult').each(function () {
                        // determine what changed
                        var $resulte = $(this);
                        if (($resulte.attr('oresultkey') != $resulte.attr('resultkey') || $resulte.attr('oresult') != $resulte.attr('result'))) {
                            // create xml
                            changes += '<fv f="' + $resulte.attr('fn') + '" v="' + $resulte.attr('result') + '"/>,';
                        }
                    });

                    if (changes != '') {
                        changes = changes.substr(0, changes.length - 1);
                        changes = '<data>' + changes + '</data>';

                        //Encode
                        changes = changes.replace(/\</g, "-[_").replace(/\>/g, "_]-");

                        // post
                        $.ajax({
                            type: 'POST',
                            url: 'Form_Save',
                            async: true,
                            processData: false,
                            data: changes,
                            success: function (response) {
                                //o.success(response);
                                $('#diaform').dialog('destroy').remove();
                                ReloadPage();
                            },
                            error: function (response) {
                                //o.error(response);
                                gridBox({ boxType: 'message', type: 'danger', title: 'Form Save Error', body: response.message });
                            }
                        });
                    }
                    else {
                        // close form
                        $('#diaform').dialog('destroy').remove();
                    } 
                }
            });
        }
    });
}

function formNew(elem) {

    formID = 0;
    formTemplateID = $(elem).val();
    subjectID = $('.filter#Subject_ID').val();

    $(elem).prop("selectedIndex", 0).val();

    form(formID, formTemplateID, subjectID);
}

function formExisting(elem) {

    formID = $(elem).closest('tr').attr('kfv');
    formTemplateID = 0;
    subjectID = 0;

    form(formID, formTemplateID, subjectID);
}

// BI
function reportNew(elem) {
    gridEditorFormNew({ element: elem, w: RelativePixels('w', 1, 800), h: RelativePixels('h', .75, 800) });
}

/*
function _validateFormInputs() {
    let payload = {
        isValid: true,
        invalidFields: []
    }
    $(`form#${context} .form-input-wrapper`).each(function (index, inputWrapper) {
        var $inputWrapper = $(inputWrapper);
        var fieldName = $inputWrapper.find('label').attr('for')
        if ($inputWrapper.attr('fr') === "1") {
            let inputs = $inputWrapper.find('.form-input');
            if (inputsAreBlank(inputs) === true) {
                highglightRed(inputs);
                payload.isValid = false
                payload.invalidFields.push(fieldName)
            } else {
                removeRedHighligh(inputs);
            }
        }
    })
    return payload
}
function _highglightRed(inputsArray) {
    inputsArray.each(function (index, input) {
        var $input = $(input)
        $input.addClass('is-invalid')
    });
}

function _removeRedHighligh(inputsArray) {
    inputsArray.each(function (index, input) {
        var $input = $(input)
        $input.removeClass('is-invalid')
    });
}
function _inputsAreBlank(inputsArray) {
    var blank = true
    inputsArray.each(function (index, input) {
        var $input = $(input)
        if ($input.attr('type') === "checkbox") {
            if ($input.is(":checked")) {
                blank = false;
            }
        }
        else if ($input.val() !== "") {
            blank = false;
        }
    });
    return blank
}

function _vGridGetFieldValueByFieldName(fieldName) {
    let fieldElement = $(`td.vgLabel:contains('${fieldName}')`)
    return fieldElement.next().children(":first")[0];
}

function _getRowValueByColumnName(elem, columnName) {
    return getRowElementByColumnName(elem, columnName).innerText;
}

function _getRowElementByColumnName(elem, columnName) {
    let columnIndex = $(`.hgHeaderRow:last th:contains('${columnName}')`).index()
    return $(elem).closest('tr').children().eq(columnIndex)[0];
}

function _getRowKeyFieldValue(elem) {
    return $(elem).closest('tr').attr('kfv')
}

function _grabDialogByContext(context) {
    return $('#dia' + context);
}

function _reloadDialog(dialog, endpoint, data) {
    dialog.load(endpoint, data);
}

function _closeDialogFromContext(context) {
    $('#dia' + context).dialog('destroy').remove()
}

function _createTag(args) {
    let tag = document.createElement(args.tagName);
    let tagText = document.createTextNode(args.tagTextContent);

    args.attributes && Object.keys(args.attributes).forEach((attrKey) => {
        tag.setAttribute(attrKey, args.attributes[attrKey])
    })

    tag.appendChild(tagText);
    return tag
}

function _attachOnChangeToMultiselect() {

    $('#selCategories').change(function (e) {
        handleSelection(e)
    })
}

function _handleSelection(e) {
    var target = getEventTarget(e);
    if (target.tagName.toLowerCase() === 'input') {
        var targetIsAllCheckbox = target.nextSibling.data.trim() == 'All'
        var allCheckboxes = getAllCheckboxes()
        var allCheckboxSelected = getAllCheckBox().checked

        if (targetIsAllCheckbox) {
            setCheckboxesState(allCheckboxes, false)
            target.checked = true
        }
        if (allCheckboxSelected && !targetIsAllCheckbox) {
            allCheckboxes.each((index, checkbox) => {
                if (target.nextSibling.data.trim() != checkbox.nextSibling.data.trim())
                    checkbox.checked = false
            })
            target.checked = true
        }
    }
}

function _setCheckboxesState(checkboxes, state) {
    checkboxes.each((index, checkbox) => {
        checkbox.checked = state;
    })
}

function _isAllCheckboxesChecked() {
    var allAreChecked = true
    getAllCheckboxes().each((index, checkbox) => {
        if (checkbox.checked == false) {
            allAreChecked = false
        }
    })
    return allAreChecked
}

function _getAllCheckboxes() {
    return $($("#selCategories")[0]).find('input')
}

function _getAllCheckBox() {
    return getAllCheckboxes()[0]
}

function _getEventTarget(e) {
    e = e || window.event;
    return e.target || e.srcElement;
}

function _getEventType(e) {
    e = e || window.event;
    return e.type || e.type;
}

function _createTemplate() {

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

function templateFields(elem) {

    let context = 'TemplateFields'
    let templateId = $(elem).closest('tr').attr('kfv');
    let templateName = $(elem).closest('tr').find('td:first').text();

    gridPop({
        context: context,
        type: 'horizontal',
        title: `${templateName}`,
        w: RelativePixels('w', .9),
        h: RelativePixels('h', .75),
        data: {
            Template_ID: templateId
        },
        afterLoad: function (elem) {
            attachOnDragListenersToGrid()
        },
        buttons: {
            'Close': {
                text: 'Close',
                priority: 'primary',
                style: 'background: #428BCA; color: #fff;',
                click: function () {
                    // save scroll position
                    SetCookie('hgScrollTop', $('.tablediv:first').scrollTop(), 1);
                    // apply filters
                    gridApplyFilters();
                }
            }
        }
    });

    function drag(ev) {
        var rowKfv = $(ev.target).closest('tr').attr('kfv')
        var rowOriginalSortOrder = getRowValueByColumnName(ev.target, 'Sort Order');
        ev.originalEvent.dataTransfer.setData("fieldKfv", rowKfv);
        ev.originalEvent.dataTransfer.setData("rowOriginalSortOrder", rowOriginalSortOrder);

    }

    function drop(ev) {
        ev.preventDefault();
        var draggedRowKfv = ev.originalEvent.dataTransfer.getData("fieldKfv");
        var rowNewSortOrder = getRowValueByColumnName(ev.target, 'Sort Order');

        var firstElementInDraggedRow = $(`tr[kfv='${draggedRowKfv}']`).eq(1).first();

        //var newSortOrder = $(getRowElementByColumnName(firstElementInDraggedRow, 'Sort Order')).text(parseInt(rowNewSortOrder) - 50);

        var jqxhr = $.getJSON("Process_Request.aspx", {
            action: "UpdateTemplateFieldSortOrder",
            fid: draggedRowKfv,
            newso: rowNewSortOrder - 50,
        })
            .done(function (data) {
                $(`#butRefresh${context}`).click();
            }).fail(function (data) {
                alert("Update failed.");
            });


    }

    function attachOnDragListenersToGrid() {
        $('#diaTemplateFields').on("dragstart", function (ev) {
            drag(ev)
        })

        $('#diaTemplateFields').on("dragover", function (ev) {
            ev.preventDefault()
        })

        $('#diaTemplateFields').on("drop", function (ev) {
            drop(ev)
        })
    }

}

function _displayTemplateCategories(elem) {
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

function _displayChoicesEditor(elem, event) {

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

function _attachOnChangeToTypeSelect() {
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
*/
