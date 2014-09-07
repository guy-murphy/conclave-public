$(document).ready(function () {
    $('.editable').editable({
        // we're going to set up the editable elements so
        // that if they specify a data-update attribe
        // that value of that attribute will be used as a selector
        // for those elements that should have their text value
        // set to the new value of the editable element
        //
        // association/edit.xsl relies upon this functionality
        // and serves as an example
        success: function (response, value) {
            var update = $(this).data('editableContainer').options.update;
            if (update != undefined) {
                $(update).text(value);
            }
        }
    });
});