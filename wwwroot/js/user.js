var On = {
    body: function (P) {
        //var Tkn = $('#RequestVerificationToken').val();
        //fn.postData('/Api/GUFM', {
        //    ID: P            
        //}, Tkn).then(data => {
        //    var obj = JSON.parse(data);
        //    console.log(obj)
        //});

       

        $("#jstree-ajax").jstree({
            core: {
                data: {
                    url: '/Api/GUFM',
                    type: 'POST',
                    dataType: 'json', // IF IN JSON FORMAT                    
                    contentType: 'application/json',
                    data: JSON.stringify({ ID: P })
                },
                "themes": {
                    "variant": "large"
                }
            },
            plugins: ['types', 'state'],
            types: {
                default: { icon: 'bx bx-folder' },
                form: { icon: 'bx bx-book-content text-info' },
                css: { icon: 'bx bxl-css3 text-info' },
                img: { icon: 'bx bx-image text-success' },
                js: { icon: 'bx bxl-nodejs text-warning' }
            }
        })
    }  
}

var UserEvents = {
    
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
    }
}