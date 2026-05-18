function ShowImagePreview(imageUploader, previewImage) {
    if (imageUploader.files && imageUploader.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $(previewImage).attr('src', e.target.result);
        }
        reader.readAsDataURL(imageUploader.files[0]);
    }
}

function uploadFile(fileUploader) {
    if (fileUploader.files && fileUploader.files[0]) {
        var reader = new FileReader();
        reader.readAsDataURL(fileUploader.files[0]);
    }
}