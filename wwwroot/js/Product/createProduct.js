document.getElementById("createProductForm").addEventListener("submit", function (event) {
    event.preventDefault();

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
        .then((response) => {
            if (response.data.success) {
                alert("Product added successfully");
                window.location.href = "/Products/Index";
            } else {
                console.log("Error: " + response.data.message);
            }
        })
        .catch((error) => {
            window.location.href = "/Error/AccessDenied.html";
        });
    });

