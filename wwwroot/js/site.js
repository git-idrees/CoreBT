var table;


var Onload = {
    body: function () {
        $('.btn.btn-label-secondary').click(function () {
            $(this).find('form').trigger('reset');
        });
        $('#form_li').click(function () {
            var res = $(this).hasClass('open');
            if (!res) {
                var t = $(this).attr('data-id');
                fn.postData('/Api/GFM', {
                    ID: t,
                }, "").then(data => {
                    var obj = JSON.parse(data);
                    if (!obj.IsError) {
                        var a = "";
                        $.each(obj.data, function () {
                            a = a + ("<a href='/Form/Document?Type=" + this.Id + "' class='menu-link'><div> " + this.Name + " </div></a>")
                        });
                        $('#form_a').html(a);
                    }
                });
            }
        });
    },
    Set: function (eman, edoc) {
        $('#eman').val(eman);
        $('#edoc').val(edoc);
    },
    Setup: function () {
        fn.getData('/Api/GCL').then(data => {
            $('#projectcity').select2({
                data: data
            });
        });
    },
    Editsetup: function () {

    },
    projectList: function () {
        var alert = fn.getUrlParameter('alert');
        if (alert) {
            fn.alert('success', 'Action Completed.');
        }
    }
}

var Events = {
    Document: function (P) {
        $('#AddDocument').click(function () {
            $('#ID').val('0');
            $('#DName').val('');
            $('#ModalDocument').modal({ backdrop: 'static', keyboard: false });
            $('#ModalDocument').modal('show');
        });
        $('#enableDocumentForm').submit(function (event) {
            event.preventDefault();
            if (fn.checkValidation()) {
                var ID = $('#ID').val();
                var DN = $('#DName').val();
                var A = $('#IsActive').is(":checked");
                var Tkn = $('#RequestVerificationToken').val();
                fn.postData('/Api/SDT', {
                    ID: ID,
                    DName: DN,
                    IsActive: A,
                    Token: Tkn,
                    ProjectID: P
                }, Tkn).then(data => {
                    var obj = JSON.parse(data);
                    if (!obj.IsError) {
                        $('#ModalDocument').modal('hide');
                        Tables.Document(P);
                    }
                    fn.alertaction(obj)
                });
            }
            else {
                fn.alert('info', 'Please fill all required fields.')
            }
        })
    },
    DocumentTable: function () {
        $('.edit-document').click(function () {
            var response = $('#Documentlist').DataTable().row($(this).closest('tr')).data();
            fn.populateEditModal('#enableDocumentForm', response, "ModalDocument");
            response.IsActive === true ? $('.switch-input.required-entry').prop('checked', true) : $('.switch-input.required-entry').prop('checked', false);
            $('#ModalDocument').modal({ backdrop: 'static', keyboard: false });
            $('#ModalDocument').modal('show');
        });
        $('.delete-document').click(function () {
            var ID = $(this).attr('data-delete');
            var P = $(this).attr('data-pid');
            var Tkn = $('#RequestVerificationToken').val();
            Swal.fire({
                title: 'Are you sure?',
                text: "You will be able to revert this!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, delete it!',
                customClass: {
                    confirmButton: 'btn btn-primary me-1',
                    cancelButton: 'btn btn-label-secondary'
                },
                buttonsStyling: false
            }).then(function (result) {
                if (result.value) {
                    fn.postData('/Api/DDT', {
                        ID: ID
                    }, Tkn).then(data => {
                        if (!data.IsError) {
                            Swal.close();
                            Tables.Document(P);
                        }
                        fn.alertaction(data)
                    });
                }
            });
        });
    },
    Role: function (P) {
        $('#selectAll').change(function () {
            if (this.checked) {
                $('.form-check-input.check').attr('checked', true); // Checks it               
            }
            else {
                $('.form-check-input.check').attr('checked', false); // Unchecks it
            }
        });
        $('#AddRole').click(function () {
            $('#selectAll').attr('checked', false);
            Tables.FillFormlist(P, 0);
            $('#ID').val('0');
            $('#Name').val('');
            $('#ModalRole').modal({ backdrop: 'static', keyboard: false });
            $('#ModalRole').modal('show');
        });
        $('#enableRoleForm').submit(function (event) {
            event.preventDefault();
            if (fn.checkValidation()) {
                var temp = table.rows().data();
                var tempdata = [];
                var ID = $('#ID').val();
                var N = $('#Name').val();
                $.each(temp, function (i, dat) {
                    var check = $('#len' + dat.FormID)
                    tempdata.push({
                        FormID: dat.ID,
                        IsView: $('#IsView_' + dat.ID).is(":checked") == true ? true : false,
                        IsEdit: $('#IsEdit_' + dat.ID).is(":checked") == true ? true : false,
                        IsDelete: $('#IsDelete_' + dat.ID).is(":checked") == true ? true : false,
                        IsDisable: $('#IsDisable_' + dat.ID).is(":checked") == true ? true : false,
                    });
                });
                if (tempdata.length > 0) {
                    $.ajax({
                        type: 'POST',
                        url: '../MasterData/SRs',
                        data: { temp: tempdata, ID: ID, PID: P, NM: N },
                        success: function (json) {
                            var obj = JSON.parse(json);
                            if (!obj.IsError) {
                                $('#ModalRole').modal('hide');
                                Tables.Role(P);
                            }
                            fn.alertaction(obj)
                        }
                    });
                }
                else {
                    fn.alert('danger', 'Please select any field.')
                    return false;
                }
            }
            else {
                fn.alert('info', 'Please fill all required fields.')
            }
        })
    },
    RoleTable: function (P) {
        $('.edit-role').click(function () {
            var response = $('#Rolelist').DataTable().row($(this).closest('tr')).data();
            console.log(response)
            fn.populateEditModal('#enableRoleForm', response, "ModalRole");
            Tables.FillFormlist(P, response.ID);
            $('#ModalRole').modal({ backdrop: 'static', keyboard: false });
            $('#ModalRole').modal('show');
        });
        $('.delete-role').click(function () {
            var ID = $(this).attr('data-delete');
            var P = $(this).attr('data-pid');
            var Tkn = $('#RequestVerificationToken').val();
            Swal.fire({
                title: 'Are you sure?',
                text: "You will be able to revert this!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, delete it!',
                customClass: {
                    confirmButton: 'btn btn-primary me-1',
                    cancelButton: 'btn btn-label-secondary'
                },
                buttonsStyling: false
            }).then(function (result) {
                if (result.value) {
                    fn.postData('/Api/DR', {
                        ID: ID
                    }, Tkn).then(data => {
                        if (!data.IsError) {
                            Swal.close();
                            Tables.Role(P);
                        }
                        fn.alertaction(data)
                    });
                }
            });
        });
    },
    AccountL2: function (P) {
        $('#AddAccountL2').click(function () {
            $('#ID').val('0');
            $('#AccountName').val('');
            $('#ModalAccountL2').modal({ backdrop: 'static', keyboard: false });
            $('#ModalAccountL2').modal('show');
        });
        $('#enableAccountL2Form').submit(function (event) {
            event.preventDefault();
            if (fn.checkValidation()) {
                var ID = $('#ID').val();
                var N = $('#AccountName').val();
                var A = $('#IsActive').is(":checked");
                var Tkn = $('#RequestVerificationToken').val();
                fn.postData('/Api/SAL2', {
                    ID: ID,
                    AccountName: N,
                    IsActive: A,
                    Token: Tkn,
                    ProjectID: P
                }, Tkn).then(data => {
                    var obj = JSON.parse(data);
                    if (!obj.IsError) {
                        $('#ModalAccountL2').modal('hide');
                        Tables.AccountL2(P);
                    }
                    fn.alertaction(obj)
                });
            }
            else {
                fn.alert('info', 'Please fill all required fields.')
            }
        })
    },
    AccountL2Table: function () {
        $('.edit-AccountL2').click(function () {
            var response = $('#AccountL2list').DataTable().row($(this).closest('tr')).data();
            fn.populateEditModal('#enableAccountL2Form', response, "ModalAccountL2");
            response.IsActive === true ? $('.switch-input.required-entry').prop('checked', true) : $('.switch-input.required-entry').prop('checked', false);
            $('#ModalAccountL2').modal({ backdrop: 'static', keyboard: false });
            $('#ModalAccountL2').modal('show');
        });
        $('.delete-AccountL2').click(function () {
            var ID = $(this).attr('data-delete');
            var P = $(this).attr('data-pid');
            var Tkn = $('#RequestVerificationToken').val();
            Swal.fire({
                title: 'Are you sure?',
                text: "You will be able to revert this!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, delete it!',
                customClass: {
                    confirmButton: 'btn btn-primary me-1',
                    cancelButton: 'btn btn-label-secondary'
                },
                buttonsStyling: false
            }).then(function (result) {
                if (result.value) {
                    fn.postData('/Api/DAL2', {
                        ID: ID
                    }, Tkn).then(data => {
                        if (!data.IsError) {
                            Swal.close();
                            Tables.AccountL2(P);
                        }
                        fn.alertaction(data)
                    });
                }
            });
        });
    },
    DropdownL1: function (P) {
        fn.postData('/Api/GAL2DD', {
            ID: P
        }, '').then(data => {
            var obj = JSON.parse(data);
            $('#ddAccountL2').select2({
                data: obj
            });
        });
    },
    DropdownL2: function (ID) {
        fn.postData('/Api/GAL2DD', {
            ID: ID
        }, '').then(data => {
            var obj = JSON.parse(data);
            $('#ddAccountL2').select2({
                dropdownParent: $("#ModalAccountL3"),
                data: obj
            });
        });
    },
    AccountL3: function (P) {
        Events.DropdownL2(P);
        $('#AddAccountL3').click(function () {
            $('#ID').val('0');
            $('#AccountName').val('');
            $('#ddAccountL2 ').val($('#ddAccountL2  option:eq(0)').val()).trigger('change');
            $('#ModalAccountL3').modal({ backdrop: 'static', keyboard: false });
            $('#ModalAccountL3').modal('show');
            //$('#backDropModal').modal('show');
        });
        $('#enableAccountL3Form').submit(function (event) {
            event.preventDefault();
            if (fn.checkValidation()) {
                var ID = $('#ID').val();
                var N = $('#AccountName').val();
                var L2 = $("#ddAccountL2").val();
                var A = $('#IsActive').is(":checked");
                var Tkn = $('#RequestVerificationToken').val();
                fn.postData('/Api/SAL3', {
                    ID: ID,
                    AccountName: N,
                    IsActive: A,
                    Token: Tkn,
                    AccountL2ID: L2
                }, Tkn).then(data => {
                    var obj = JSON.parse(data);
                    if (!obj.IsError) {
                        $('#ModalAccountL3').modal('hide');
                        Tables.AccountL3(P);
                    }
                    fn.alertaction(obj)
                });
            }
            else {
                fn.alert('info', 'Please fill all required fields.')
            }
        })
    },
    AccountL3Table: function (P) {
        $('.edit-AccountL3').click(function () {
            var response = $('#AccountL3list').DataTable().row($(this).closest('tr')).data();
            fn.populateEditModal('#enableAccountL3Form', response, "ModalAccountL3");
            response.IsActive === true ? $('.switch-input.required-entry').prop('checked', true) : $('.switch-input.required-entry').prop('checked', false);
            $('#ddAccountL2').val(response.L2ID).trigger('change');
            $('#ModalAccountL3').modal({ backdrop: 'static', keyboard: false });
            $('#ModalAccountL3').modal('show');
        });
        $('.delete-AccountL3').click(function () {
            var ID = $(this).attr('data-delete');
            var P = $(this).attr('data-pid');
            var Tkn = $('#RequestVerificationToken').val();
            Swal.fire({
                title: 'Are you sure?',
                text: "You will be able to revert this!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, delete it!',
                customClass: {
                    confirmButton: 'btn btn-primary me-1',
                    cancelButton: 'btn btn-label-secondary'
                },
                buttonsStyling: false
            }).then(function (result) {
                if (result.value) {
                    fn.postData('/Api/DAL3', {
                        ID: ID
                    }, Tkn).then(data => {
                        if (!data.IsError) {
                            Swal.close();
                            Tables.AccountL3(P);
                        }
                        fn.alertaction(data)
                    });
                }
            });
        });
    },
    City: function () {
        $('#Addcity').click(function () {
            $('#ModalCity').modal({ backdrop: 'static', keyboard: false });
            $('#ModalCity').modal('show');
        });
        $('#enableCityForm').submit(function (event) {
            event.preventDefault();
            if (fn.checkValidation()) {
                var ID = $('#ID').val();
                var CN = $('#CityName').val();
                var A = $('#IsActive').is(":checked");
                var Tkn = $('#RequestVerificationToken').val();
                fn.postData('/Api/SC', {
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
                fn.alert('info', 'Please fill all required fields.')
            }
        })
    },
    CityTable: function () {
        $('.edit-city').click(function () {
            var response = $('#projectlist').DataTable().row($(this).closest('tr')).data();
            console.log(response)
            fn.populateEditModal('#enableCityForm', response, "ModalCity");
            response.IsActive === true ? $('.switch-input.required-entry').prop('checked', true) : $('.switch-input.required-entry').prop('checked', false);
            $('#ModalCity').modal({ backdrop: 'static', keyboard: false });
            $('#ModalCity').modal('show');
        });
        $('.delete-city').click(function () {
            var ID = $(this).attr('data-delete');
            var Tkn = $('#RequestVerificationToken').val();
            Swal.fire({
                title: 'Are you sure?',
                text: "You will be able to revert this!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, delete it!',
                customClass: {
                    confirmButton: 'btn btn-primary me-1',
                    cancelButton: 'btn btn-label-secondary'
                },
                buttonsStyling: false
            }).then(function (result) {
                if (result.value) {
                    fn.postData('/Api/DC', {
                        ID: ID
                    }, Tkn).then(data => {
                        if (!data.IsError) {
                            Swal.close();
                            Tables.City();
                        }
                        fn.alertaction(data)
                    });
                }
            });
        });
    },
    ProjectTable: function () {
        $('.edit-project').click(function () {
            var response = $('#projectlist').DataTable().row($(this).closest('tr')).data();
            fn.populateEditModal('#enableProjectForm', response, "ModalProject");
            response.IsActive === true ? $('.switch-input.required-entry').prop('checked', true) : $('.switch-input.required-entry').prop('checked', false);
            $('#ModalCity').modal({ backdrop: 'static', keyboard: false });
            $('#ModalCity').modal('show');
        });
    },
    Setup: function () {
        $('#enableProjectForm').submit(function (event) {
            event.preventDefault();
            if (fn.checkValidation()) {
                var email = $('#email').val();
                if (fn.Email(email)) {
                    var pname = $('#ProjectName').val();
                    var pcode = $('#ProjectCode').val();
                    var city = $("#projectcity").val();
                    var Tkn = $('#RequestVerificationToken').val();
                    fn.postData('/Api/PS', {
                        pname: pname,
                        pcode: pcode,
                        city: city,
                        Token: Tkn,
                        email: email
                    }, Tkn).then(data => {
                        var obj = JSON.parse(data);
                        if (!obj.IsError) {
                            window.location.replace('https://localhost:7278/Project/List?alert=true');
                        }
                        fn.alertaction(obj)
                    });
                }
                else {
                    fn.alert('danger', 'Invalid email address');
                }
            }
            else {
                fn.alert('info', 'Please fill all required fields.');
            }
        });
    },
    User: function (P) {
        Events.DDRole(P);
        $('#AddUser').click(function () {
            $('#ID').val('');
            $('#Name').val('');
            $('#Email').val('');
            $('#Password').val('');
            $('#EmpID').val('');
            $('.switch-input.required-entry').prop('checked', false)
            $('#ModalUser').modal({ backdrop: 'static', keyboard: false });
            $('#ModalUser').modal('show');
        });
        $('#enableUserForm').submit(function (event) {
            event.preventDefault();
            if (fn.checkValidation()) {
                var ID = $('#ID').val();
                var FullName = $('#Name').val();
                var Email = $('#Email').val();
                var Pass = $('#Password').val();
                var EmpID = $('#EmpID').val();
                var RID = $("#ddRole").val();
                var Tkn = $('#RequestVerificationToken').val();
                fn.postData('/Api/SUsr', {
                    ID: ID,
                    ProjectID: P,
                    FullName: FullName,
                    Email: Email,
                    Pass: Pass,
                    RID: RID,
                    EmpID: EmpID
                }, Tkn).then(data => {
                    var obj = JSON.parse(data);
                    console.log(obj)
                    if (!obj.IsError) {
                        $('#ModalUser').modal('hide');
                        Tables.User();
                        $('#ID').val('');
                        $('#Name').val('');
                        $('#Email').val('');
                        $('#Password').val('');
                        $('#EmpID').val('');
                    }
                    fn.alertaction(obj)
                });

            }
            else {
                fn.alert('info', 'Please fill all required fields.')
            }
        })
    },
    UserTable: function (P) {
        $('.edit-user').click(function () {
            var response = $('#Userlist').DataTable().row($(this).closest('tr')).data();
            console.log(response)
            fn.populateEditModal('#enableUserForm', response, "ModalUser");           
            $('#ModalUser').modal({ backdrop: 'static', keyboard: false });
            $('#ModalUser').modal('show');
        });
        $('.delete-user').click(function () {
            var ID = $(this).attr('data-delete');
            var P = $(this).attr('data-pid');
            var Tkn = $('#RequestVerificationToken').val();
            Swal.fire({
                title: 'Are you sure?',
                text: "You will be able to revert this!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, delete it!',
                customClass: {
                    confirmButton: 'btn btn-primary me-1',
                    cancelButton: 'btn btn-label-secondary'
                },
                buttonsStyling: false
            }).then(function (result) {
                if (result.value) {
                    fn.postData('/Api/DR', {
                        ID: ID
                    }, Tkn).then(data => {
                        if (!data.IsError) {
                            Swal.close();
                            Tables.User(P);
                        }
                        fn.alertaction(data)
                    });
                }
            });
        });
    },
    DDRole: function (P) {
        fn.postData('/Api/GRsL', {
            ID: P
        }, '').then(data => {
            var obj = JSON.parse(data);
            if (obj.length > 0) {
                $('#ddRole').empty().trigger('change');
                $('#ddRole').prop('disabled', false);
                $('#ddRole').select2({
                    dropdownParent: $("#ModalUser"),
                    data: obj
                });
            } else {
                $('#ddRole').empty().trigger('change');
                $('#ddRole').prop('disabled', true);
            }
        });
    },

    /*Form*/
    FormDDOnchanges: function (P) {
        $('#ddL2').on('select2:selecting', function (e) {
            Events.DDL3(e.params.args.data.id);
        });
        //$('#ddL2').on('select2:selecting', function (e) {
        //    Tables.Form(P, par1, e.params.args.data.id, par3);
        //});
        //$('#ddD').on('select2:selecting', function (e) {
        //    Tables.Form(P, par1, par2, e.params.args.data.id);
        //});
    },
    DDL2: function (P, call) {
        if (call) {
            $('#AddForm').attr('data-has', 1);
            fn.postData('/Api/GAL2DD', {
                ID: P
            }, '').then(data => {
                var obj = JSON.parse(data);
                if (obj.length > 0) {
                    $('#ddL2').prop('disabled', false);
                    $('#ddL2').select2({
                        dropdownParent: $("#ModalForm"),
                        data: obj
                    });
                    var L2 = $('#ddL2').val();
                    if (L2.length > 0) {
                        fn.postData('/Api/GAL3DD', {
                            ID: L2
                        }, '').then(data => {
                            var obj = JSON.parse(data);
                            if (obj.length > 0) {
                                $('#ddL3').prop('disabled', false);
                                $('#ddL3').select2({
                                    dropdownParent: $("#ModalForm"),
                                    data: obj
                                });

                            } else {
                                $('#ddL3').prop('disabled', true);
                            }
                        });
                    } else {
                        $('#ddL3').prop('disabled', true);
                    }
                } else {
                    $('#ddL2').prop('disabled', true);
                }
            });
        }
        else {
            $('#AddForm').attr('data-has', 0);
            $('#ddL2').empty().trigger('change');
            $('#ddL2').prop('disabled', true);
            $('#ddL3').empty().trigger('change');
            $('#ddL3').prop('disabled', true);
        }

        fn.postData('/Api/GD', {
            ID: P
        }, '').then(data => {
            var obj = JSON.parse(data);
            if (obj.length > 0) {
                $('#ddD').prop('disabled', false);
                $('#ddD').select2({
                    dropdownParent: $("#ModalForm"),
                    data: obj
                });
            } else {
                $('#ddD').prop('disabled', true);
            }
        });
    },
    DDL3: function (L2) {
        if (L2.length > 0) {
            fn.postData('/Api/GAL3DD', {
                ID: L2
            }, '').then(data => {
                var obj = JSON.parse(data);
                if (obj.length > 0) {
                    $('#ddL3').empty().trigger('change');
                    $('#ddL3').prop('disabled', false);
                    $('#ddL3').select2({
                        dropdownParent: $("#ModalForm"),
                        data: obj
                    });

                } else {
                    $('#ddL3').empty().trigger('change');
                    $('#ddL3').prop('disabled', true);
                }
            });
        } else {
            $('#ddL3').empty().trigger('change');
            $('#ddL3').prop('disabled', true);
        }
    },
    Form: function (P) {
        var Tkn = $('#RequestVerificationToken').val();
        fn.postData('/Api/CL', {
            ID: P
        }, Tkn).then(data => {
            var obj = JSON.parse(data);
            $('#AddForm').click(function () {
                $('#ddD').val(0).trigger('change');
                $('#ID').val('0');
                $('#Name').val('');
                $('#ModalForm').modal({ backdrop: 'static', keyboard: false });
                $('#ModalForm').modal('show');
            });
            $('#enableFormForm').submit(function (event) {
                event.preventDefault();
                if (fn.checkValidation()) {
                    var ID = $('#ID').val();
                    var N = $('#Name').val();
                    var L2 = $('#ddL2').val() == null ? 0 : $('#ddL2').val();
                    var L3 = $('#ddL3').val() == null ? 0 : $('#ddL3').val();
                    var dat = $('#ddD').select2('data');
                    var D = dat[0].id;
                    var LevelName = dat[0].text;
                    var A = $('#IsActive').is(":checked");
                    var Tkn = $('#RequestVerificationToken').val();
                    fn.postData('/Api/SFs', {
                        ID: ID,
                        L1: P,
                        L2: L2,
                        L3: L3,
                        D: D,
                        Name: N,
                        LevelName: LevelName,
                        IsActive: A
                    }, Tkn).then(data => {
                        var obj = JSON.parse(data);
                        if (!obj.IsError) {
                            $('#ModalForm').modal('hide');
                            Tables.FormList(P);
                        }
                        fn.alertaction(obj)
                    });
                }
                else {
                    $('#Name').focus();
                    fn.alert('info', 'Please fill all required fields.')
                }
            })
            Events.FormDDOnchanges(P)
            Tables.FormList(P);
            if (obj[0].Levels > 0) {
                Events.DDL2(P, true);
            } else {
                Events.DDL2(P, false);
            }
        });
    },
    FormTable: function (P) {
        $('.edit-form').click(function () {
            var response = $('#Formslist').DataTable().row($(this).closest('tr')).data();
            $('#ID').val(response.ID)
            if (response.Level = 1) {
                if (response.SubLevelID != 0) {
                    $('#ddD').val(response.SubLevelID).trigger('change');
                    $('#ddD').prop('disabled', false);
                }
                else {
                    $('#ddD').val(0).trigger('change');
                    $('#ddD').prop('disabled', false);
                }
            }
            else if (response.Level = 2) {
                $('#ddL2').val(response.LevelID).trigger('change');
                $('#ddL2').prop('disabled', false);

                if (response.SubLevelID != 0) {
                    $('#ddD').val(response.SubLevelID).trigger('change');
                    $('#ddD').prop('disabled', false);
                }
                else {
                    $('#ddD').val(0).trigger('change');
                    $('#ddD').prop('disabled', false);
                }
            }
            else if (response.Level = 3) {
                $('#ddL3').empty().trigger('change');
                $('#ddL3').prop('disabled', true);
            }
            response.IsActive === true ? $('.switch-input.required-entry').prop('checked', true) : $('.switch-input.required-entry').prop('checked', false);
            $('#Name').val(response.Name);
            $('#ModalForm').modal({ backdrop: 'static', keyboard: false });
            $('#ModalForm').modal('show');
        });
        $('.disable-form').click(function () {
            var ID = $(this).attr('data-delete');
            var Tkn = $('#RequestVerificationToken').val();
            Swal.fire({
                title: 'Are you sure?',
                text: "You will be able to revert this!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, delete it!',
                customClass: {
                    confirmButton: 'btn btn-primary me-1',
                    cancelButton: 'btn btn-label-secondary'
                },
                buttonsStyling: false
            }).then(function (result) {
                if (result.value) {
                    fn.postData('/Api/DF', {
                        ID: ID
                    }, Tkn).then(data => {
                        if (!data.IsError) {
                            Swal.close();
                            Tables.FormList(P);
                        }
                        fn.alertaction(data)
                    });
                }
            });
        });
    },
    FormDefaultAction: function (P) {
        var TempID = $('#TempID').val();
        fn.postData('/Api/PDF', {
            ID: P,
            TempID: TempID
        }, '').then(data => {
            var obj = JSON.parse(data);
            Events.DDL2(P, true);
            Tables.FillFieldsData(obj);
        });
    }
}

var Tables = {
    FillFormlist(P, ID) {
        fn.postData('/Api/RAL', {
            ID: P,
            RID: ID
        }, "").then(data => {
            var obj = JSON.parse(data);
            if (!obj.IsError) {
                table = $('#RoleAccesslist').DataTable({
                    destroy: true,
                    info: true,
                    searching: false,
                    paging: true,
                    pageLength: 15,
                    bLengthChange: false,
                    data: obj,
                    "columnDefs": [
                        { "width": "10%", "targets": 1 },
                        { "width": "10%", "targets": 2 },
                        { "width": "10%", "targets": 3 },
                        { "width": "10%", "targets": 4 }
                    ],
                    "columns": [
                        { "data": "Name", "autoWidth": true, "sortable": false, },
                        {
                            mRender: function (data, type, full) {
                                if (full.IsView === true) {
                                    return ' <div class="form-check d-flex justify-content-center" > <input class="form-check-input check" id="IsView_' + full.ID + '" type="checkbox" Checked /></div > '
                                } else {
                                    return ' <div class="form-check d-flex justify-content-center" > <input class="form-check-input check" id="IsView_' + full.ID + '" type="checkbox" /></div > '
                                }
                            },
                            "sortable": false, "autoWidth": false
                        },
                        {
                            mRender: function (data, type, full) {
                                if (full.IsEdit === true) {
                                    return ' <div class="form-check d-flex justify-content-center" > <input class="form-check-input check" id="IsEdit_' + full.ID + '" type="checkbox" Checked /></div > '
                                } else {
                                    return ' <div class="form-check d-flex justify-content-center" > <input class="form-check-input check" id="IsEdit_' + full.ID + '" type="checkbox" /></div > '
                                }
                            },
                            "sortable": false, "autoWidth": false
                        },
                        {
                            mRender: function (data, type, full) {
                                if (full.IsDelete === true) {
                                    return ' <div class="form-check d-flex justify-content-center" > <input class="form-check-input check" id="IsDelete_' + full.ID + '" type="checkbox" Checked /></div > '
                                } else {
                                    return ' <div class="form-check d-flex justify-content-center" > <input class="form-check-input check" id="IsDelete_' + full.ID + '" type="checkbox" /></div > '
                                }
                            },
                            "sortable": false, "autoWidth": false
                        }, {
                            mRender: function (data, type, full) {
                                if (full.IsDisable === true) {
                                    return ' <div class="form-check d-flex justify-content-center" > <input class="form-check-input" id="IsDisable_' + full.ID + '" type="checkbox" Checked /></div > '
                                } else {
                                    return ' <div class="form-check d-flex justify-content-center" > <input class="form-check-input" id="IsDisable_' + full.ID + '" type="checkbox" /></div > '
                                }
                            },
                            "sortable": false, "autoWidth": false
                        }

                    ]
                });
            }
        });


    },
    City: function () {
        fetch('/Api/GCD')
            .then(response => response.json()
            ).then(data => {
                $('#projectlist').DataTable({
                    destroy: true,
                    info: false,
                    bLengthChange: false,
                    data: data,
                    "columnDefs": [
                        { "width": "15%", "targets": 0 },
                    ],
                    drawCallback: function () {
                        Events.CityTable();
                    },
                    "columns": [
                        { "data": "CityId", "autoWidth": true, "sortable": true, },
                        { "data": "CityName", "autoWidth": true, "sortable": true, },
                        { "data": "OrderID", "autoWidth": true, "sortable": true, },
                        {
                            "data": "IsActive",
                            mRender: function (data, type, full) {
                                if (full.IsActive === true) {
                                    return ' <span class="badge  bg-label-primary m-1"> Active </span>'
                                } else {
                                    return '<span class="badge  bg-label-danger m-1"> In-Active </span>'
                                }
                            },
                            "sortable": false, "autoWidth": false
                        },
                        {
                            mRender: function (data, type, row) {
                                if (row.IsActive === true) {
                                    return ' <span class="text-nowrap"><button class="btn btn-sm btn-icon me-2 edit-city" ><i class="bx bx-edit"></i></button><button class="btn btn-sm btn-icon delete-city" data-delete="' + row.CityId + '"><i class="bx bx-trash"></i></button></span>';
                                } else {
                                    return ' <button type="button" class="btn btn-outline-primary edit-city"> <i class="bx bx-left-arrow-alt"></i> Make Active </button >';
                                }
                            },
                            "sortable": false, "autoWidth": false
                        }
                    ]
                });
            }
            ).catch(error => {
                // handle the error
            });
    },
    Document: function (ID) {
        var Tkn = $('#RequestVerificationToken').val();
        fn.postData('/Api/GDTL', {
            ID: ID
        }, Tkn).then(data => {
            var obj = JSON.parse(data);
            $('#Documentlist').DataTable({
                destroy: true,
                info: false,
                bLengthChange: false,
                data: obj,
                "columnDefs": [
                    { "width": "15%", "targets": 0 },
                ],
                drawCallback: function () {
                    Events.DocumentTable();
                },
                "columns": [
                    { "data": "ID", "autoWidth": true, "sortable": true, },
                    { "data": "DName", "autoWidth": true, "sortable": true, },
                    {
                        "data": "IsActive",
                        mRender: function (data, type, full) {
                            if (full.IsActive === true) {
                                return ' <span class="badge  bg-label-primary m-1"> Active </span>'
                            } else {
                                return '<span class="badge  bg-label-danger m-1"> In-Active </span>'
                            }
                        },
                        "sortable": false, "autoWidth": false
                    },
                    {
                        mRender: function (data, type, row) {
                            if (row.IsActive === true) {
                                return ' <span class="text-nowrap"><button class="btn btn-sm btn-icon me-2 edit-document" ><i class="bx bx-edit"></i></button><button class="btn btn-sm btn-icon delete-document" data-delete="' + row.ID + '" data-pid="' + row.PID + '"><i class="bx bx-trash"></i></button></span>';
                            } else {
                                return ' <button type="button" class="btn btn-outline-primary edit-document"> <i class="bx bx-left-arrow-alt"></i> Make Active </button >';
                            }
                        },
                        "sortable": false, "autoWidth": false
                    }
                ]
            });
        });
    },
    Role: function (P) {
        var Tkn = $('#RequestVerificationToken').val();
        fn.postData('/Api/GRL', {
            ID: P
        }, Tkn).then(data => {
            var obj = JSON.parse(data);
            $('#Rolelist').DataTable({
                destroy: true,
                info: false,
                bLengthChange: false,
                data: obj,
                "columnDefs": [
                    { "width": "15%", "targets": 0 },
                ],
                drawCallback: function () {
                    Events.RoleTable(P);
                },
                "columns": [
                    { "data": "ID", "autoWidth": true, "sortable": true, },
                    { "data": "Name", "autoWidth": true, "sortable": true, },
                    {
                        "data": "IsActive",
                        mRender: function (data, type, full) {
                            if (full.IsActive === true) {
                                return ' <span class="badge  bg-label-primary m-1"> Active </span>'
                            } else {
                                return '<span class="badge  bg-label-danger m-1"> In-Active </span>'
                            }
                        },
                        "sortable": false, "autoWidth": false
                    },
                    {
                        mRender: function (data, type, row) {
                            if (row.IsActive === true) {
                                return ' <span class="text-nowrap"><button class="btn btn-sm btn-icon me-2 edit-role" ><i class="bx bx-edit"></i></button><button class="btn btn-sm btn-icon delete-role" data-delete="' + row.ID + '" data-pid="' + row.PID + '"><i class="bx bx-trash"></i></button></span>';
                            } else {
                                return ' <button type="button" class="btn btn-outline-primary edit-role"> <i class="bx bx-left-arrow-alt"></i> Make Active </button >';
                            }
                        },
                        "sortable": false, "autoWidth": false
                    }
                ]
            });
        });
    },
    FormList: function (P) {

        var Tkn = $('#RequestVerificationToken').val();
        fn.postData('/Api/GFSL', {
            ID: P
        }, Tkn).then(data => {
            var obj = JSON.parse(data);
            $('#Formslist').DataTable({
                destroy: true,
                info: false,
                bLengthChange: false,
                data: obj,
                "columnDefs": [
                    { "width": "15%", "targets": 0 },
                ],
                drawCallback: function () {
                    Events.FormTable(P);
                },
                "columns": [
                    { "data": "ID", "autoWidth": true, "sortable": true, },
                    { "data": "Name", "autoWidth": true, "sortable": true, },
                    { "data": "Level", "autoWidth": true, "sortable": true, },
                    { "data": "LevelName", "autoWidth": true, "sortable": true, },
                    {
                        "data": "IsActive",
                        mRender: function (data, type, full) {
                            if (full.IsActive === true) {
                                return ' <span class="badge  bg-label-primary m-1"> Unlock </span>'
                            } else {
                                return '<span class="badge  bg-label-info m-1"> Lock </span>'
                            }
                        },
                        "sortable": false, "autoWidth": false
                    },
                    {
                        mRender: function (data, type, row) {
                            if (row.IsActive === true) {
                                return ' <span class="text-nowrap"><button title="Edit this form" class="btn btn-sm btn-icon me-2 edit-form" ><i class="bx bx-edit"></i></button><button class="btn btn-sm btn-icon disable-form" data-delete="' + row.ID + '" title="lock this form"><i class="bx bx-lock"></i></button></span>';
                            } else {
                                return ' <button type="button" class="btn btn-outline-primary edit-form"> <i class="bx bx-lock-open"></i> Make Unlock </button >';
                            }
                        },
                        "sortable": false, "autoWidth": false
                    }
                ]
            });
        });
    },
    AccountL2: function (ID) {
        var Tkn = $('#RequestVerificationToken').val();
        fn.postData('/Api/GAL2L', {
            ID: ID
        }, Tkn).then(data => {
            var obj = JSON.parse(data);
            $('#AccountL2list').DataTable({
                destroy: true,
                info: false,
                bLengthChange: false,
                data: obj,
                "columnDefs": [
                    { "width": "15%", "targets": 0 },
                ],
                drawCallback: function () {
                    Events.AccountL2Table();
                },
                "columns": [
                    { "data": "ID", "autoWidth": true, "sortable": true, },
                    { "data": "AccountName", "autoWidth": true, "sortable": true, },
                    {
                        "data": "IsActive",
                        mRender: function (data, type, full) {
                            if (full.IsActive === true) {
                                return ' <span class="badge  bg-label-primary m-1"> Active </span>'
                            } else {
                                return '<span class="badge  bg-label-danger m-1"> In-Active </span>'
                            }
                        },
                        "sortable": false, "autoWidth": false
                    },
                    {
                        mRender: function (data, type, row) {
                            if (row.IsActive === true) {
                                return ' <span class="text-nowrap"><button class="btn btn-sm btn-icon me-2 edit-AccountL2" ><i class="bx bx-edit"></i></button><button class="btn btn-sm btn-icon delete-AccountL2" data-delete="' + row.ID + '" data-pid="' + row.PID + '"><i class="bx bx-trash"></i></button></span>';
                            } else {
                                return ' <button type="button" class="btn btn-outline-primary edit-AccountL2"> <i class="bx bx-left-arrow-alt"></i> Make Active </button >';
                            }
                        },
                        "sortable": false, "autoWidth": false
                    }
                ]
            });
        });
    },
    AccountL3: function (ID) {
        var Tkn = $('#RequestVerificationToken').val();
        fn.postData('/Api/GAL3L', {
            ID: ID
        }, Tkn).then(data => {
            var obj = JSON.parse(data);
            $('#AccountL3list').DataTable({
                destroy: true,
                info: false,
                bLengthChange: false,
                data: obj,
                "columnDefs": [
                    { "width": "15%", "targets": 0 },
                ],
                drawCallback: function () {
                    Events.AccountL3Table(ID);
                },
                "columns": [
                    { "data": "ID", "autoWidth": true, "sortable": true, },
                    { "data": "AccountName", "autoWidth": true, "sortable": true, },
                    { "data": "L2Name", "autoWidth": true, "sortable": true, },
                    {
                        "data": "IsActive",
                        mRender: function (data, type, full) {
                            if (full.IsActive === true) {
                                return ' <span class="badge  bg-label-primary m-1"> Active </span>'
                            } else {
                                return '<span class="badge  bg-label-danger m-1"> In-Active </span>'
                            }
                        },
                        "sortable": false, "autoWidth": false
                    },
                    {
                        mRender: function (data, type, row) {
                            if (row.IsActive === true) {
                                return ' <span class="text-nowrap"><button class="btn btn-sm btn-icon me-2 edit-AccountL3" ><i class="bx bx-edit"></i></button><button class="btn btn-sm btn-icon delete-AccountL3" data-delete="' + row.ID + '" data-pid="' + row.L2ID + '"><i class="bx bx-trash"></i></button></span>';
                            } else {
                                return ' <button type="button" class="btn btn-outline-primary edit-AccountL3"> <i class="bx bx-left-arrow-alt"></i> Make Active </button >';
                            }
                        },
                        "sortable": false, "autoWidth": false
                    }
                ]
            });
        });
    },
    Project: function () {
        fetch('/Api/GPL')
            .then(response => response.json()
            ).then(data => {
                $('#projectlist').DataTable({
                    destroy: true,
                    info: false,
                    bLengthChange: false,
                    data: data,
                    drawCallback: function () {
                        Events.CityTable();
                    },
                    "columns": [
                        {
                            "data": "ProjectName",
                            mRender: function (data, type, row) {
                                return ' <div class="d-flex justify-content-start align-items-center"><div class="avatar-wrapper"><div class="avatar avatar-sm me-3"><span class="avatar-initial rounded-circle bg-label-primary">' + row.ProjectCode + '</span></div></div><div class="d-flex flex-column"><a href="/Project/Detail?access=' + row.ProjectID + '" ><span class="fw-semibold">' + row.ProjectName + '</span></a><small class="text-muted">admin@riyadbank.com</small></div></div>'
                            },
                            "sortable": false, "autoWidth": false
                        },
                        { "data": "City", "autoWidth": true, "sortable": true, },
                        {
                            "data": "IsActive",
                            mRender: function (data, type, full) {
                                if (full.IsActive === true) {
                                    return ' <span class="badge  bg-label-primary m-1"> Active </span>'
                                } else {
                                    return '<span class="badge  bg-label-danger m-1"> In-Active </span>'
                                }
                            },
                            "sortable": false, "autoWidth": false
                        },
                        {
                            mRender: function (data, type, row) {
                                if (row.IsActive === true) {
                                    return ' <span class="text-nowrap"><button class="btn btn-sm btn-icon me-2 edit-project" ><i class="bx bx-edit"></i></button><button class="btn btn-sm btn-icon delete-project" data-delete="' + row.ProjectID + '"><i class="bx bx-trash"></i></button></span>';
                                } else {
                                    return ' <button type="button" class="btn btn-outline-primary edit-project"> <i class="bx bx-left-arrow-alt"></i> Make Active </button >';
                                }
                            },
                            "sortable": false, "autoWidth": false
                        }
                    ]
                });
            }
            ).catch(error => {
                // handle the error
            });
    },
    Form: function (L1, L2, L3, DocID) {
        var tempId = $('#TempID').val();
        if (L1.toString().length > 0) {
            var Tkn = $('#RequestVerificationToken').val();
            fn.postData('/Api/GF', {
                L1: L1.toString(),
                L2: L2.toString(),
                L3: L3.toString(),
                DocID: DocID.toString(),
                TempID: tempId
            }, Tkn).then(data => {
                var obj = JSON.parse(data);
                Tables.FillFieldsData(obj);
            });
        }

    },
    Fields: function () {
        var tempId = $('#TempID').val();
        var FID = $('#FormID').val();
        if (tempId.length > 0) {
            var Tkn = $('#RequestVerificationToken').val();
            fn.postData('/Api/LF', {
                ID: tempId,
                FID: FID
            }, Tkn).then(data => {
                var obj = JSON.parse(data);
                $('#fieldslist').DataTable({
                    destroy: true,
                    info: true,
                    searching: false,
                    paging: false,
                    pageLength: 15,
                    bLengthChange: false,
                    data: obj,
                    "columnDefs": [
                        { "width": "20%", "targets": 0 },
                    ],
                    "columns": [
                        {
                            mRender: function (data, type, row) {
                                return '<input id="' + row.FieldID + '" type="checkbox" class="dt-checkboxes form-check-input">'
                            },
                            "sortable": false, "autoWidth": false
                        },
                        { "data": "Name", "autoWidth": true, "sortable": false, }
                    ]
                });
                $('#ModalFields').modal({ backdrop: 'static', keyboard: false });
                $('#ModalFields').modal('show');
            });
        }
    },
    FillFieldsData(obj) {
        if (obj.length > 0) {
            if (!obj[0].FormName == "") {
                $('#FormName').val(obj[0].FormName)
                $('#FormID').val(obj[0].FromID)
            }
            else {
                $('#FormName').val('')
                $('#FormID').val(0)
            }
        }
        //$('#FormName').val(obj)
        table = $('#FormFieldslist').DataTable({
            destroy: true,
            info: true,
            searching: false,
            paging: false,
            pageLength: 15,
            bLengthChange: false,
            data: obj,
            "columnDefs": [
                { "width": "5%", "targets": 0 },
                { "width": "15%", "targets": 1 },
                { "width": "18%", "targets": 2 },
                { "width": "10%", "targets": 4 },
                { "width": "10%", "targets": 5 }
            ],
            drawCallback: function () {
                Events.FormTable();
            },
            "columns": [
                { "data": "OrderID", "autoWidth": true, "sortable": false, },
                { "data": "Name", "autoWidth": true, "sortable": false, },
                {
                    mRender: function (data, type, row) {
                        return '<input type="text" class="form-control" id="fn' + row.FieldID + '" placeholder="Any Name" value="' + row.FieldDisplayName + '" >'
                    },
                    "sortable": false, "autoWidth": false
                },
                {
                    mRender: function (data, type, row) {
                        if (row.FieldID != 4 && row.FieldID != 5) {
                            switch (row.DataTypeID) {
                                case 1:
                                    return '<select id = "smallSelect_' + row.FieldID + '" class="form-select form-select-sm" ><option value="1" selected>Number With Characters</option><option value="2">Number Only</option><option value="3">Characters Only</option><option value="4">Date Time</option><option value="5">Bool</option></select >'
                                    break;
                                case 2:
                                    return '<select id = "smallSelect_' + row.FieldID + '" class="form-select form-select-sm" ><option value="1" >Number With Characters</option><option value="2" selected>Number Only</option><option value="3">Characters Only</option><option value="4">Date Time</option><option value="5">Bool</option></select >'
                                    break;
                                case 3:
                                    return '<select id = "smallSelect_' + row.FieldID + '" class="form-select form-select-sm" ><option value="1" >Number With Characters</option><option value="2" >Number Only</option><option value="3" selected>Characters Only</option><option value="4">Date Time</option><option value="5">Bool</option></select >'
                                    break;
                                case 5:
                                    return '<select id = "smallSelect_' + row.FieldID + '" class="form-select form-select-sm" ><option value="1" >Number With Characters</option><option value="2">Number Only</option><option value="3">Characters Only</option><option value="4">Date Time</option><option value="5" selected>Bool</option></select >'
                                    break;
                                default:
                            }
                        }
                        else {
                            return '<select id = "smallSelect" class="form-select form-select-sm" ><option value="4" selected>Date Time</option></select >'
                        }
                    },
                    "sortable": false, "autoWidth": false
                },
                {
                    mRender: function (data, type, row) {
                        return '<input type="text" class="form-control" id="len' + row.FieldID + '" placeholder="Any Name" value="' + row.Length + '" >'
                    },
                    "sortable": false, "autoWidth": false
                },
                {
                    "data": "IsActive",
                    mRender: function (data, type, full) {
                        if (full.Status === true) {
                            return ' <div class="form-check d-flex justify-content-center" > <input class="form-check-input" id="check_' + full.FieldID + '" type="checkbox" Checked /></div > '
                        } else {
                            return ' <div class="form-check d-flex justify-content-center" > <input class="form-check-input" id="check_' + full.FieldID + '" type="checkbox" /></div > '
                        }
                    },
                    "sortable": false, "autoWidth": false
                },
                {
                    mRender: function (data, type, row) {

                        return '<button type="button" class="btn rounded-pill btn-icon btn-outline-primary mt-1 add-funtion" view-funtion" data-id="' + row.FieldID + '" title="Add Function on this Field" > <i class="bx bx-plus"></i></button> | <button title="View Existing Function on this Field" type="button" class="btn btn-outline-info view-funtion" data-id="' + row.FieldID + '" > <i class="bx bx-left-arrow-alt"></i> View func</button >';
                    },
                    "sortable": false, "autoWidth": false
                }
            ]
        });
    },
    User: function (P) {
        var Tkn = $('#RequestVerificationToken').val();
        fn.postData('/Api/GUsL', {
            ID: P,           
        }, Tkn).then(data => {
            var obj = JSON.parse(data);
            $('#Userlist').DataTable({
                destroy: true,
                info: false,
                bLengthChange: false,
                data: obj,
                "columnDefs": [
                    { "width": "15%", "targets": 0 },
                ],
                drawCallback: function () {
                    Events.UserTable();
                },
                "columns": [
                    { "data": "EmpID", "autoWidth": true, "sortable": true, },
                    { "data": "Name", "autoWidth": true, "sortable": true, },
                    { "data": "Role", "autoWidth": true, "sortable": true, },
                    {
                        "data": "IsActive",
                        mRender: function (data, type, full) {
                            if (full.IsActive === true) {
                                return ' <span class="badge  bg-label-primary m-1"> Un-Lock </span>'
                            } else {
                                return '<span class="badge  bg-label-danger m-1"> Lock </span>'
                            }
                        },
                        "sortable": false, "autoWidth": false
                    },
                    {
                        mRender: function (data, type, row) {
                            if (row.IsActive === true) {
                                return ' <span class="text-nowrap"><button class="btn btn-sm btn-icon me-2 edit-user" ><i class="bx bx-edit"></i></button><button class="btn btn-sm btn-icon disable-user" data-delete="' + row.ID + '"><i class="bx bx-lock"></i></button></span>';
                            } else {
                                return ' <button type="button" class="btn btn-outline-primary edit-user"> <i class="bx bx-lock-alt"></i> Make Unlock </button >';
                            }
                        },
                        "sortable": false, "autoWidth": false
                    }
                ]
            });
        });           
    }
}

var fn = {
    postData: async function (url = '', data = {}, token) {

        // Default options are marked with *
        const response = await fetch(url, {
            method: 'POST', // *GET, POST, PUT, DELETE, etc.
            mode: 'cors', // no-cors, *cors, same-origin
            // cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
            // credentials: 'same-origin', // include, *same-origin, omit
            headers: {
                'Accept': 'application/json',
                'RequestVerificationToken': token,
                'Content-Type': 'application/json'

                // 'Content-Type': 'application/x-www-form-urlencoded',
            },
            // redirect: 'follow', // manual, *follow, error
            // referrerPolicy: 'no-referrer', // no-referrer, *no-referrer-when-downgrade, origin, origin-when-cross-origin, same-origin, strict-origin, strict-origin-when-cross-origin, unsafe-url
            body: JSON.stringify(data) // body data type must match "Content-Type" header

        });
        if (response.ok) {
            return response.json(); // parses JSON response into native JavaScript objects
        }
        else {
            return response.status
        }
    },

    getData: async function (url = '') {
        // Default options are marked with *
        const response = await fetch(url);
        if (response.ok) {
            return response.json(); // parses JSON response into native JavaScript objects
        }
        else {
            return response.status
        }
    },

    checkValidation: function () {
        var reqlength = $('.required-entry').length;
        var value = $('.required-entry').filter(function () {
            return this.value !== '';
        });
        if (value.length >= 0 && (value.length !== reqlength)) {
            return false;
        } else {
            return true;
        }
    },

    alertaction: function (res) {
        toastr.options = {
            "closeButton": true,
            "progressBar": true,
            "preventDuplicates": false,
            "positionClass": "toast-top-right",
            "onclick": null,
            "showDuration": "300",
            "titleClass": "toast-title"
        }
        if (res.IsError) {
            toastr.error(res.Message);
        } else {
            toastr.success('Action completed');
        }
    },

    alert: function (type, message) {


        toastr.options = {
            "closeButton": true,
            "progressBar": true,
            "preventDuplicates": false,
            "onclick": null,
            "showDuration": "300",
            "titleClass": "toast-title"

        }
        switch (type) {
            case 'info':
                toastr.info(message, { toastClass: 'yourclass ngx-toastr' });
                break;
            case 'success':
                toastr.success(message);
                break;
            case 'warning':
                toastr.warning(message);
                $('#toast-container').addClass('bs-toast animate__bounce');
                break;
            case 'danger':
                toastr.error(message);
                break;
        }
    },

    setDataTableFilters: function (data) {
        var obj = {};
        obj.PageLenght = data.length;
        obj.Start = data.start;
        obj.Draw = data.draw;
        return obj;
    },

    createData: function (formid) {
        var $form = $("#" + formid + " input[type=text],#" + formid + " input[type = date],#" + formid + " input[type = tel], #" + formid + " textarea, #" + formid + " input[type=hidden],#" + formid + " input[type=password],#" + formid + " input[type=email], #" + formid + " input[type=number]"),
            _len = $form.length, data = {};
        if (_len > 0) {
            for (i = 0; i < _len; i++) {

                data[$($form[i]).attr('data-id')] = $("#" + formid + " [data-id='" + $($form[i]).attr('data-id') + "'").val();
            }
        }
        return data;
    },

    createJson: function (formid) {
        var $form = $("#" + formid + " input[type=text], #" + formid + " input[type=number]"),
            _len = $form.length, data = {};
        if (_len > 0) {
            for (i = 0; i < _len; i++) {
                data[$($form[i]).attr('id')] = $($form[i]).val();
            }
        }
        return data;
    },

    createDataOnChange: function (formid) {
        var $form = $("#" + formid + ' input[data-change="1"]')
        _len = $form.length, data = {};
        var val;
        if (_len > 0) {
            for (i = 0; i < _len; i++) {
                val = $("#" + formid + " [data-id='" + $($form[i]).attr('data-id') + "'").val();
                data[$($form[i]).attr('data-id')] = val + "|P"
            }
        }
        return data;
    },

    populateEditModal: function (fomid, data, id, callback) {

        var __data = data;
        var sData = Object.keys(__data).map(function (key) {
            return [key, __data[key]];
        });
        var data_len = sData.length;
        for (i = 0; i < data_len; i++) {
            if ($(fomid + " [data-id='" + sData[i][0] + "']").attr("data-obj") === 'dropdown') {
                $(fomid + " ." + sData[i][0]).dropdown('set selected', sData[i][1]);
                // $(fomid + " ." + sData[i][0]).attr("data-value", sData[i][1]);                
                if (typeof callback === 'function') {
                    // callback("." + sData[i][0], sData[i][1]);
                }
            }
            if ($(fomid + " [data-id='" + sData[i][0] + "']").attr("data-obj") === 'label') {
                $(fomid + " [data-id='" + sData[i][0] + "']").html(sData[i][1]);
            }
            else {
                $(fomid + " [data-id='" + sData[i][0] + "']").val(sData[i][1]);
            }
        }
    },

    KeyPressOrder: function (fld, evt) {
        var arabicCharUnicodeRange = /[\u0600-\u06FF]/;
        var key = evt.which;
        // 0 = numpad
        // 8 = backspace
        // 32 = space
        if (key === 8 || key === 0 || key === 32 || key === 16 || key === 17 || key === 18) {
            return true;
        }
        var str = String.fromCharCode(key);
        if (arabicCharUnicodeRange.test(str)) {
            return true;
        }
        else {
            toastr.info('allow only arabic letters');
            return false;
        }
    },

    Characters: function (ev8t, _len) {
        var regex = /^[a-zA-Z]+$/;
        var key = ev8t.keyCode || ev8t.which;
        if (key === 8 || key === 0 || key === 32 || key === 16 || key === 17 || key === 18) {
            return true;
        }
        var str = String.fromCharCode(key);
        if (regex.test(str)) {
            if (ev8t.target.value.length >= _len) {
                toastr.info('limit exceeded');
                return false;
            }
            else {
                return true;
            }
        }
        else {
            toastr.info('allow only characters');
            return false;
        }
    },

    Length: function (ev8t, _len) {
        if (ev8t.target.value.length >= _len) {
            toastr.info('limit exceeded');
            return false;
        }
        else {
            return true;
        }
    },

    Email: function (_str) {
        var validRegex = /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/;
        if (validRegex.test(_str)) { return true; }
        else { return false; }
    },

    getAction: function () {
        var location = window.location.pathname;
        var action = location.split('/')[2];
        return action.toLowerCase();
    },

    getController: function () {
        var location = window.location.pathname;
        var controller = location.split('/')[1];
        return controller.toLowerCase();
    },

    getUrlParameter: function getUrlParameter(sParam) {
        var sPageURL = window.location.search.substring(1),
            sURLVariables = sPageURL.split('&'),
            sParameterName,
            i;

        for (i = 0; i < sURLVariables.length; i++) {
            sParameterName = sURLVariables[i].split('=');

            if (sParameterName[0] === sParam) {
                return sParameterName[1] === undefined ? true : decodeURIComponent(sParameterName[1]);
            }
        }
        return false;
    }
}