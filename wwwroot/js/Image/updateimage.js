function displayPreviewImage(event) {
    var input = event.target;
    var reader = new FileReader();

    reader.onload = function () {
        var imgElement = document.getElementById('previewImage');
        var currentImage = document.getElementById('currentImage');
        
        if (currentImage) {
            currentImage.src = reader.result;
            currentImage.style.display = 'block';
        }
        
        imgElement.style.display = 'none';
    };

    reader.readAsDataURL(input.files[0]);
}
