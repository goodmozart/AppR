
function ShowToastr(Title, Message) {
    toastr.error(Title, Message, {
        closeButton: true,
        progressBar: true,
        newestOnTop: false,
        showDuration: '100',
        hideDuration: '100',
        timeOut: '3000',
    });
}