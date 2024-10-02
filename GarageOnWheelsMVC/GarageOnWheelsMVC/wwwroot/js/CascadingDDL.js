$(document).ready(function () {

    var countryid = $('#country').data('countryid');
    var stateid = $('#state').data('stateid');
    var cityid = $('#city').data('cityid');
    var areaid = $('#area').data('areaid');
    LoadCountries(countryid);
    console.log(countryid);
   
    $('#country').change(function () {
        var countryId = $(this).val();
        if (countryId > 0) {
            LoadStates(countryId, stateid);
        }      
    });


    $('#state').change(function () {
        var stateId = $(this).val();
        if (stateId > 0) {
            LoadCities(stateId, cityid);
        }
    });


    $('#city').change(function () {
        var cityId = $(this).val();
        if (cityId > 0) {
            LoadAreas(cityId,areaid);
        }
      
    });

});

function LoadCountries(countryid) {
    $.ajax({
        url: '/Location/GetCountries',
        success: function (response) {
            if (response != null && response != undefined && response.length > 0) {
                $('#country').attr('disabled', false);
                $('#country').append('<option value="">Select Country</option>');
                $('#state').append('<option value="">Select State</option>');
                $('#city').append('<option value="">Select City</option>');
                $('#area').append('<option value="">Select Area</option>');

                var countryExists = false;
                $.each(response, function (i, data) {
                    $('#country').append('<option value=' + data.Id + '>' + data.Name + '</option>');
                    if (data.Id == countryid) {
                        countryExists = true;
                    }
                });

                if (countryExists) {
                    $('#country').val(countryid).trigger('change');
                } else {
                    $('#country').val(''); // Default to "Select Country"
                }

            } else {
                $('#country').attr('disabled', true);
                $('#state').attr('disabled', true);
                $('#city').attr('disabled', true);
                $('#area').attr('disabled', true);
            }
        },
        error: function (error) {
            alert("Error Comes here ", error.statusText);
        }
    });
}



function LoadStates(countryId, stateid) {
    $('#state').empty();
    $('#city').empty();
    $('#area').empty();

    $.ajax({
        url: '/Location/GetStates?Id=' + countryId,
        success: function (response) {
            if (response != null && response != undefined && response.length > 0) {
                $('#state').attr('disabled', false);
                $('#state').append('<option value="">Select State</option>');
                $('#city').append('<option value="">Select City</option>');
                $('#area').append('<option value="">Select Area</option>');

                var stateExists = false;
                $.each(response, function (i, data) {
                    $('#state').append('<option value=' + data.Id + '>' + data.Name + '</option>');
                    if (data.Id == stateid) {
                        stateExists = true;
                    }
                });

                if (stateExists) {
                    $('#state').val(stateid).trigger('change');
                } else {
                    $('#state').val(''); // Default to "Select State"
                }
            } else {
                $('#state').attr('disabled', true);
                $('#city').attr('disabled', true);
                $('#area').attr('disabled', true);
            }
        },
        error: function (error) {
            alert("Error Comes here ", error.statusText);
        }
    });
}


function LoadCities(stateId, cityid) {
    $('#city').empty();
    $('#area').empty();

    $.ajax({
        url: '/Location/GetCities?Id=' + stateId,
        success: function (response) {
            if (response != null && response != undefined && response.length > 0) {
                $('#city').attr('disabled', false);
                $('#city').append('<option value="">Select City</option>');
                $('#area').append('<option value="">Select Area</option>');

                var cityExists = false;
                $.each(response, function (i, data) {
                    $('#city').append('<option value=' + data.Id + '>' + data.Name + '</option>');
                    if (data.Id == cityid) {
                        cityExists = true;
                    }
                });

                if (cityExists) {
                    $('#city').val(cityid).trigger('change');
                } else {
                    $('#city').val(''); // Default to "Select City"
                }
            } else {
                $('#city').attr('disabled', true);
                $('#area').attr('disabled', true);
            }
        },
        error: function (error) {
            alert("Error Comes here ", error.statusText);
        }
    });
}



function LoadAreas(cityId, areaid) {
    $('#area').empty();

    $.ajax({
        url: '/Location/GetAreas?Id=' + cityId,
        success: function (response) {
            if (response != null && response != undefined && response.length > 0) {
                $('#area').attr('disabled', false);
                $('#area').append('<option value="">Select Area</option>');

                var areaExists = false;
                $.each(response, function (i, data) {
                    $('#area').append('<option value=' + data.Id + '>' + data.Name + '</option>');
                    if (data.Id == areaid) {
                        areaExists = true;
                    }
                });

                if (areaExists) {
                    $('#area').val(areaid);
                } else {
                    $('#area').val(''); // Default to "Select Area"
                }
            } else {
                $('#area').attr('disabled', true);
            }
        },
        error: function (error) {
            alert("Error Comes here ", error.statusText);
        }
    });
}
