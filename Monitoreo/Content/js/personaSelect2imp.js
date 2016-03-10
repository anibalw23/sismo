$(document).ready(function () {
    var personaUrl = '/Persona/GetPersona';
    var personasUrl = '/Persona/GetPersonas';
    var pageSize = 20;

    if ((typeof personaType == 'undefined')) personaType = 'Persona';

    $('#' + personaType + 'Id').select2(
    {
        placeholder: 'Digíte el nombre o Cédula',       
        //selectOnBlur: true,
        //Does the user have to enter any data before sending the ajax request
        minimumInputLength: 1,
        allowClear: true,
        ajax: {
            //How long the user has to pause their typing before sending the next request
            quietMillis: 150,
            //The url of the json service
            url: personasUrl,
            dataType: 'json',
            //Our search term and what page we are on
            data: function (term, page) {
                return {
                    pageSize: pageSize,
                    pageNum: page,
                    searchTerm: term,
                    type: personaType
                };
            },
            results: function (data, page) {
                //Used to determine whether or not there are more results available,
                //and if requests for more data should be sent in the infinite scrolling
                var more = (page * pageSize) < data.Total;
                return { results: data.Results, more: more };
            }
        },

        //createSearchChoice: function (term, data) {
        //    if ($(data).filter(function () {
        //      return this.text.localeCompare(term) === 0;
        //    }).length === 0) {
        //        return { id: term, text: term };
        //    }
        //},



        initSelection: function (element, callback) {
            $.getJSON(personaUrl + '?id=' + (element.val()) + '&type=' + personaType, null, function (data) {
                callback(data);
            });
        }
    });
});