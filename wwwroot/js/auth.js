var document = {
    onload: function () {
        $('#BtnAddDoc').click(function () {
            $('#myModal').modal('show');
        });
    }
}

var fn = {
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

    KeyPressLength: function (fld, evt) {
        if (fld.value.length >= 10) {
            toastr.info('allow only 10 digits');
            return false;
        }
        else {
            return true;
        }
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
    }    
}