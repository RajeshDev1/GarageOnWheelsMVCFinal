$(document).ready(function () {
    $('#country').attr('disabled', true);
    $('#state').attr('disabled', true);
    $('#city').attr('disabled', true);
    $('#area').attr('disabled', true);
    LoadCountries();

    $('#country').change(function () {
        var countryId = $(this).val();
        if (countryId > 0) {
            LoadStates(countryId);
        }
        else {
            alert("Select Country");
            $('#state').empty();
            $('#city').empty();
            $('#area').empty();
            $('#state').attr('disabled', true);
            $('#city').attr('disabled', true);
            $('#area').attr('disabled', true);
            $('#state').append('<option>--Select State--</option>');
            $('#city').append('<option>--Select City--</option>');
            $('#area').append('<option>--Select Area--</option>');
        }
    });


    $('#state').change(function () {
        var stateId = $(this).val();
        if (stateId > 0) {
            LoadCities(stateId);
        }
        else {
            alert("Select State");
            $('#city').attr('disabled', true);
            $('#area').attr('disabled', true);
            $('#city').append('<option>--Select City--</option>');
            $('#area').append('<option>--Select Area--</option>');
        }
    });


    $('#city').change(function () {
        var cityId = $(this).val();
        if (cityId > 0) {
            LoadAreas(cityId);
        }
        else {
            alert("Select City");
            $('#area').attr('disabled', true);
            $('#area').append('<option>--Select Area--</option>');
        }
    });

});

function LoadCountries() {
    $('#country').empty();

    $.ajax({
        url: '/Location/GetCountries',
        success: function (response) {
            if (response != null && response != undefined && response.length > 0) {
                $('#country').attr('disabled', false);
                $('#country').append('<option>--Select Country--</option>');
                $('#state').append('<option>--Select State--</option>');
                $('#city').append('<option>--Select City--</option>');
                $('#area').append('<option>--Select Area--</option>');
                $.each(response, function (i, data) {
                    $('#country').append('<option value=' + data.Id + '>' + data.Name + '</option>');
                });
            }
            else {
                $('#country').attr('disabled', true);
                $('#state').attr('disabled', true);
                $('#city').attr('disabled', true);
                $('#area').attr('disabled', true);
                $('#country').append('<option>--Country not Available--</option>');
                $('#state').append('<option>--State not Available--</option>');
                $('#city').append('<option>--City not Available--</option>');
                $('#area').append('<option>--Area not Available--</option>');
            }
        },
        error: function (error) {
            alert("Error Comes here ", error.statusText);
        }
    });
}


function LoadStates(countryId) {
    $('#state').empty();
    $('#city').empty();
    $('#area').empty();
    $('#city').attr('disabled', true);
    $('#area').attr('disabled', true);

    $.ajax({
        url: '/Location/GetStates?Id=' + countryId,
        success: function (response) {
            if (response != null && response != undefined && response.length > 0) {
                $('#state').attr('disabled', false);
                $('#state').append('<option>--Select State--</option>');
                $('#city').append('<option>--Select City--</option>');
                $('#area').append('<option>--Select Area--</option>');
                $.each(response, function (i, data) {
                    $('#state').append('<option value=' + data.Id + '>' + data.Name + '</option>');
                });
            }
            else {
                $('#state').attr('disabled', true);
                $('#city').attr('disabled', true);
                $('#area').attr('disabled', true);
                $('#state').append('<option>--State not Available--</option>');
                $('#city').append('<option>--City not Available--</option>');
                $('#area').append('<option>--Area not Available--</option>');
            }
        },
        error: function (error) {
            alert("Error Comes here ", error.statusText);
        }
    });
}


function LoadCities(stateId) {
    $('#city').empty();
    $('#area').empty();
    $('#area').attr('disabled', true);


    $.ajax({
        url: '/Location/GetCities?Id=' + stateId,
        success: function (response) {
            if (response != null && response != undefined && response.length > 0) {
                $('#city').attr('disabled', false);
                $('#city').append('<option>--Select City--</option>');
                $('#area').append('<option>--Select Area--</option>');
                $.each(response, function (i, data) {
                    $('#city').append('<option value=' + data.Id + '>' + data.Name + '</option>');
                });
            }
            else {
                $('#city').attr('disabled', true);
                $('#area').attr('disabled', true);
                $('#city').append('<option>--City not Available--</option>');
                $('#area').append('<option>--Area not Available--</option>');
            }
        },
        error: function (error) {
            alert("Error Comes here ", error.statusText);
        }
    });
}



function LoadAreas(cityId) {
    $('#area').empty();

    $.ajax({
        url: '/Location/GetAreas?Id=' + cityId,
        success: function (response) {
            if (response != null && response != undefined && response.length > 0) {
                $('#area').attr('disabled', false);
                $('#area').append('<option>--Select Area--</option>');
                $.each(response, function (i, data) {
                    $('#area').append('<option value=' + data.Id + '>' + data.Name + '</option>');
                });
            }
            else {
                $('#area').attr('disabled', true);
                $('#area').append('<option>--Area not Available--</option>');
            }
        },
        error: function (error) {
            alert("Error Comes here ", error.statusText);
        }
    });
  
}