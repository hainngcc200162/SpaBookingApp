var alertDisplayed = false;
var alertDisplayedImg = false;
function hideAlerts() {
    var alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        alert.style.display = 'none';
    });

    // Reset alert flags
    alertDisplayed = false;
    alertDisplayedImg = false;
}
document.getElementById("createProductForm").addEventListener("submit", function (event) {
    event.preventDefault();
    hideAlerts();
    // Kiểm tra xem tệp hình ảnh đã được chọn hay chưa
    var productPoster = document.getElementById("imageInput").files[0];
    if (!productPoster) {
        if (!alertDisplayedImg) {
            var parentElement = document.getElementById("createProductForm");
            var alertElement = document.createElement("div");
            alertElement.className = "mb-3 alert alert-danger";
            alertElement.setAttribute("role", "alert");
            alertElement.textContent = "Please select an image before creating.";
            parentElement.insertBefore(alertElement, parentElement.firstChild);
            alertElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
            alertDisplayedImg = true;
        }
        return;
    }

    var token = sessionStorage.getItem("Token");

    // Kiểm tra nếu không có token, chuyển hướng đến trang access denied
    if (!token) {
        window.location.href = "/error/AccessDenied.html";
        return;
    }

    var productName = document.getElementById("SpaProduct_Name").value;
    var productPrice = document.getElementById("SpaProduct_Price").value;
    var quantityInStock = document.getElementById("SpaProduct_QuantityInStock").value;
    var productDescription = document.getElementById("SpaProduct_Description").value;
    var categoryId = document.getElementById("SpaProduct_CategoryId").value;
    var productPoster = document.getElementById("imageInput").files[0];

    var formData = new FormData();
    formData.append("Name", productName);
    formData.append("Price", productPrice);
    formData.append("QuantityInStock", quantityInStock);
    formData.append("Description", productDescription);
    formData.append("CategoryId", categoryId);
    formData.append("Poster", productPoster);


    axios
        .post("/api/SpaProduct", formData, {
            headers: {
                Authorization: "Bearer " + token,
                "Content-Type": "multipart/form-data",
            },
        })
        .then(response => {
            if (response.data.success) {
                alert("Product created successfully");
                window.location.href = "/Products/Index";
            } else {
                if (!alertDisplayed) {
                    var parentElement = document.getElementById("createProductForm");

                    var alertElement = document.createElement("div");
                    alertElement.className = "mb-3 alert alert-danger";
                    alertElement.setAttribute("role", "alert");
                    alertElement.textContent = response.data.message;

                    parentElement.insertBefore(alertElement, parentElement.firstChild);
                    alertElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
                    alertDisplayed = true;
                }
            }
        })
        .catch((error) => {
            window.location.href = "/Error/AccessDenied.html";
        });
});

