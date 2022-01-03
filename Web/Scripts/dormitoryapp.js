DormitoryApp = function () { }

DormitoryApp.prototype = {

    dataTable: function (args) {
        return DormitoryApp.DataTable(args);
    },
    request: function (args) {
        return new DormitoryApp.DataRequest(args);
    },
    deleteOperation: function (args) {
        return DormitoryApp.DeleteOperation(args); 
    },
    ajaxLoading: function (id, mainClass) {
        return DormitoryApp.AjaxLoading(id, mainClass);
    },
}


DormitoryApp.DeleteOperation = function (args) {

    var defaults = {
        title: "Delete",
        message: " Are you sure you want to delete?",
        cancelButtonText: "Cancel",
        yesButtonText: "Yes",
        url: undefined,
        data: undefined,
        grid: undefined,
        redirect: undefined,
        redirectUrl: '/Home/Index',
        cbfunction: undefined,
    };

    var prop = $.extend({}, defaults, args);


    var modalDiv = $("<div/>", { "class": "modal fade", id: "modalDelete", tabindex: "-1", "data-backdrop": "static", role: "dialog", "aria-hidden": "true" });
    var modalDialogDiv = $("<div/>", { "class": 'modal-dialog' });
    var modalContentDiv = $("<div/>", { "class": 'modal-content' });
    var modalHeaderDiv = $("<div/>", { "class": 'modal-header' });
    var modalCloseSpan = $("<span/>", { "aria-hidden": "true", text: "X" });
    var modalHeaderCloseButton = $("<button/>", {
        "class": 'close', type: 'button', "data-dismiss": "modal", "aria-label": 'Close'
        , click: function () {
            closePopUp(modalDiv);  
        }
    });
    var modalH4Title = $("<h4/>", { "class": "modal-title", text: prop.title });
    var modalBodyDiv = $("<div/>", { "class": 'modal-body', html: prop.message });
    var modalFooterDiv = $("<div/>", { "class": 'modal-footer' });
    var modalCancelButton = $("<button/>", {
        type: "button", "class": "btn btn-outline-dark ", "data-dismiss": "modal", text: " " + prop.cancelButtonText
        , click: function () {
            closePopUp(modalDiv); 
        }
    });
    var modalSaveButton = $("<button/>", {
        type: "button", id: "confirmDelete", "class": "btn btn-dark", text: " " + prop.yesButtonText,
        click: function () {

            document.DormitoryApp.request({
                method: prop.url,
                data: prop.data,
                loading: true,
                beforeSend: function () {
                    document.DormitoryApp.ajaxLoading('#confirmDelete', 'fa-save');
                },
                complete: function () {
                    document.DormitoryApp.ajaxLoading('#confirmDelete', 'fa-save');
                },
                success: function (result) {  
                    closePopUp(modalDiv);  
                    if (prop.redirect) {
                        location.href = prop.redirectUrl;
                    }
                    else {
                        if (prop.grid) {

                            if (prop.grid.length === 0) {
                                prop.grid.draw();
                            } else if (prop.grid.length > 0) {

                                for (var i = 0; i < prop.grid.length; i++) {
                                    prop.grid[i].draw();
                                }
                            }
                        }
                    }
                    if (prop.cbfunction) {
                        prop.cbfunction();
                    }
                }
            });
        }
    });


  
    

    var modalSaveButtonIcon = $("<i/>", { "class": "fa fa-check" });
    var modalCancelButtonIcon = $("<i/>", { "class": "fa fa-remove" });
     
    modalHeaderCloseButton.append(modalCloseSpan);
    modalHeaderDiv.append(modalH4Title , modalHeaderCloseButton );
    modalSaveButton.prepend(modalSaveButtonIcon);
    modalCancelButton.prepend(modalCancelButtonIcon);
    modalFooterDiv.append(modalCancelButton, modalSaveButton);
    modalContentDiv.append(modalHeaderDiv, modalBodyDiv, modalFooterDiv);
    modalDialogDiv.append(modalContentDiv);
    modalDiv.append(modalDialogDiv);
    $("#pageBodyContainer").append(modalDiv);
    $(modalDiv).modal("show");


};



DormitoryApp.DataTable = function (args) {
    var callCount = 0;
    var defaults = {
        id: "table",
        url: "",
        customSearch: "",
        sortIndex: 0,
        sortDirection: "asc",
        columns: [],
        columnDefs: [],
        searchButton: undefined,
        ajaxCompleteCallback: undefined,
    };
    var properties = $.extend({}, defaults, args);




    return $("#" + properties.id).DataTable({

        processing: true,
        serverSide: true,
        orderMulti: false,
        dom: "<<t>lip>",
        paginate: {
            previous: "Prev",
            next: "Next",
            last: "Last",
            first: "First"
        },
        pagingType: "full_numbers", 
        lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]],
        columns: properties.columns,
        columnDefs: properties.columnDefs,
        select: {
            style: 'multi',
            selector: 'td:first-child'
        },
        order: [[properties.sortIndex, properties.sortDirection]], 
        ajax: {
            url: properties.url,
            type: "POST",
            datatype: "json",
            complete: function (result) {
                if (callCount > 0) {
                    if (properties.searchButton) {
                        if (properties.searchButton.isLoading()) {
                            properties.searchButton.stop();
                        }
                    }
                } else
                    callCount++; 
            }, 
        }
    });
};


DormitoryApp.AjaxLoading = function (id, mainClass) {
    $(id).attr('disabled', function (i, v) { return !v; });
    $(id + " i").toggleClass(mainClass).toggleClass("fa-spin").toggleClass("fa-spinner");
}

DormitoryApp.Notifications = function (args) {
    var options = {
        closeButton: false,
        debug: false,
        newestOnTop: false,
        progressBar: true,
        positionClass: "toast-top-right",
        preventDuplicates: false,
        onclick: null,
        showDuration: "300",
        hideDuration: "1000",
        timeOut: "3000",
        extendedTimeOut: "1000",
        showEasing: "swing",
        hideEasing: "linear",
        showMethod: "fadeIn",
        hideMethod: "fadeOut"
    }

    var properties = $.extend({}, options, args);

    toastr.options = properties;
    toastr[args.type](args.message, args.title);
}


DormitoryApp.DataRequest = function (args) {

    var defaults = {
        method: null,
        dataType: "json",
        type: "POST",
        beforeSend: null,
        complete: null,
        cache: false,
        data: {},
        async: true,
        success: function () { },
        error: function () { },
        contentType: "application/json; charset=utf-8",
        processData: true
    };

    var properties = $.extend({}, defaults, args);
    $.ajax({
        type: properties.type,
        dataType: (properties.dataType !== undefined) ? properties.dataType : "json",
        contentType: (properties.contentType !== undefined) ? properties.contentType : "application/json; charset=utf-8",
        processData: (properties.processData !== undefined) ? properties.processData : true,
        beforeSend: (args.loading !== undefined && args.loading === true) ? properties.beforeSend : "",
        complete: (args.loading !== undefined && args.loading === true) ? properties.complete : "",
        async: properties.async,
        url: properties.method,
        data: properties.contentType !== false ? JSON.stringify(properties.data) : data,
        success: function (data) {
            
            if (properties.dataType === "json") {
                data = data.hasOwnProperty("d") ? data.d : data;

                if (data.NotifyTypeName !== "error")
                    properties.success(data);
                else
                    properties.error(data);


                if (data.NotifyTypeName !== "success" && data.NotifyTypeName !== undefined) {
                    DormitoryApp.Notifications({ type: data.NotifyTypeName, message: data.Message });
                }
            } else {
                properties.success(data);
           } 


        },
        error: function (jqXhr, textStatus, errorThrown) {

            //if (jqXhr && jqXhr.responseText.indexOf("/Account/Login") >= 0) {
            //    window.location = "/Account/Login";
            //} else {
            //    var errorText = jqXhr.statusText;
            //    var status = jqXhr.status;
            //    properties.error(errorText);
            //    informationMessage(errorText, textStatus, errorThrown, status);
            //}

        }
    });
}
