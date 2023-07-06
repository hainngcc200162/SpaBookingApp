function selectImage() {
    document.getElementById("imageInput").click();
}

document.getElementById("imageInput").addEventListener("change", function (event) {
    var input = event.target;
    var fileName = input.files[0].name;
    var imageFileName = document.getElementById("imageFileName");
    var previewImage = document.getElementById("previewImage");

    imageFileName.value = fileName;
    previewImage.src = URL.createObjectURL(input.files[0]);
    previewImage.style.display = "block";
    document.getElementById("selectedImageName").innerText = fileName;
});