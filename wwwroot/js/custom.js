// function selectImage() {
//     document.getElementById("imageInput").click();
// }

// document.getElementById("imageInput").addEventListener("change", function () {
//     var input = this;
//     var imageFileName = document.getElementById("imageFileName");
//     var selectedImageName = document.getElementById("selectedImageName");
//     var previewImage = document.getElementById("previewImage");

//     if (input.files && input.files[0]) {
//         var reader = new FileReader();

//         reader.onload = function (e) {
//             imageFileName.value = input.files[0].name;
//             selectedImageName.textContent = input.files[0].name;
//             previewImage.src = e.target.result;
//             previewImage.style.display = "block";
//         };

//         reader.readAsDataURL(input.files[0]);
//     }
// });
