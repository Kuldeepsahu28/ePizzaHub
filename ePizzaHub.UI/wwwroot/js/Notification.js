
function ToastSuccess(msg) {
    debugger
    toastr.success(msg, 'Success', {
        ProgressEvent: true,
        positionClass: 'toast-top-right',
        timeOut: 3000
    });
}