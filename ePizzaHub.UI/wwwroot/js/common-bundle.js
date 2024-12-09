// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(document).ready(function () {
   
    RefereshCartQuantity();
})

function RefereshCartQuantity() {
  
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: '/Cart/GetCartCount',
        success: function (data) {
            $("#cartCounter").text(data);
            $("#cartCounter2").text(data);
            if (data > 0) {
                $("#view-cart").css("display","block");
            } else {
                $("#view-cart").css("display", "none");
            }
           
        },
        error: function (result) {
        },
    });
}


function ToastSuccess(msg) {
    debugger
    toastr.success(msg, 'Success', {
        ProgressEvent: true,
        positionClass: 'toast-top-right',
        timeOut: 3000
    });
}