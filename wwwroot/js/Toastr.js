
function ShowToastrError(Title, Message) {
    toastr.error(Title, Message, {
        closeButton: true,
        progressBar: true,
        newestOnTop: false,
        showDuration: '100',
        hideDuration: '100',
        timeOut: '3000',
    });
}
function ShowToastrSuccess(Title, Message) {
    toastr.success(Title, Message, {
        closeButton: true,
        progressBar: true,
        newestOnTop: false,
        showDuration: '100',
        hideDuration: '100',
        timeOut: '3000',
    });
}