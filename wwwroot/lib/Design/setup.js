
//$('#flexSwitchCheckChecked').change(function () {
//    if (this.checked) {
//        $('#password').prop('disabled', true);
//    }
//    else {
//        $('#password').prop('disabled', false);
//    }
//});



$('#enableProjectForm').submit(function (event) {
    event.preventDefault();
    if (fn.checkValidation()) {
        var pname = $('#ProjectName').val();
        var pcode = $('#ProjectCode').val();
        var email = $('#email').val();
        var city = $("#projectcity").val();
        var Tkn = $('#RequestVerificationToken').val();
        if (fn.Email(email)) {
            if (city != null) {
                fn.postData('/Api/PS', {
                    ID: ID,
                    CityName: CN,
                    IsActive: A,
                    Token: Tkn
                }, Tkn).then(data => {
                    var obj = JSON.parse(data);
                    if (!obj.IsError) {
                        $('#ModalCity').modal('hide');
                        Tables.City();
                    }
                    fn.alertaction(obj)
                });
            }
            else {
                n.alert('danger', 'Please select any city');
            }
        } else {
            fn.alert('danger', 'Please check email address');
        }
    }
    else {
        fn.alert('danger', 'Please fill all required fields.');
    }
});

$("#project-step-1").click(function () {
    $('#header1').removeClass('active');
    $('#header1').addClass('crossed');
    $('#header2').addClass('active');
    $('#header2').removeClass('crossed');
    $('#project-details-validation').removeClass('active dstepper-block');
    $('#project-info-validation').addClass('active dstepper-block');
});

$("#project-step-back-1").click(function () {

    $('#header1').removeClass('crossed');
    $('#header1').addClass('active');
    $('#header2').addClass('crossed');
    $('#header2').removeClass('active');
    $('#project-details-validation').addClass('active dstepper-block');
    $('#project-info-validation').removeClass('active dstepper-block');
});