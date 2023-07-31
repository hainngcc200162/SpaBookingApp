document
    .getElementById("createProductForm")
    .addEventListener("submit", function (event) {
        event.preventDefault();

        var productName = document.getElementById("Product_Name").value;
        var productPrice = document.getElementById("Product_Price").value;
        var quantityInStock = document.getElementById(
            "Product_QuantityInStock"
        ).value;
        var productDescription = document.getElementById(
            "Product_Description"
        ).value;
        var categoryId = document.getElementById("Product_CategoryId").value;
        var productPoster = document.getElementById("imageInput").files[0];

        var formData = new FormData();
        formData.append("Name", productName);
        formData.append("Price", productPrice);
        formData.append("QuantityInStock", quantityInStock);
        formData.append("Description", productDescription);
        formData.append("CategoryId", categoryId);
        formData.append("Poster", productPoster);

        var token = sessionStorage.getItem("Token");

        axios
            .post("/api/Product", formData, {
                headers: {
                    Authorization: "Bearer " + token,
                    "Content-Type": "multipart/form-data",
                },
            })
            .then((response) => {
                if (response.data.success) {
                    // Xử lý thành công, ví dụ: hiển thị thông báo, chuyển hướng trang, làm mới danh sách sản phẩm, v.v.
                    alert("Thêm sản phẩm thành công");
                    window.location.href = "/Products";
                } else {
                    // Xử lý lỗi từ máy chủ, ví dụ: hiển thị thông báo lỗi
                    console.log("Lỗi: " + response.data.message);
                }
            })
            .catch((error) => {
                // Xử lý lỗi kết nối hoặc lỗi xử lý trên máy chủ
                window.location.href = "/Error/AccessDenied.html";
            });
    });

