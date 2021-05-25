function createNewSubject() {

    let saveFunction = function () {
        let categoryId = $('#selChoices').val();
        $('#dia' + 'ChooseASubjectCategory').dialog('destroy').remove();
        gridPop({
            type: 'vertical',
            context: 'CreateASubject',
            title: 'Create A Subject',
            w: RelativePixels('w', .6, 500),
            h: RelativePixels('h', .7, 400),
        })
    } 

    gridPop({
        type: 'vertical',
        context: 'ChooseASubjectCategory',
        title: 'Please Choose a Subject Category',
        w: RelativePixels('w', .6, 500),
        h: RelativePixels('h', .3, 400),
        saveFunction: saveFunction
    })
}