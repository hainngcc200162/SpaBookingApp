document.getElementById("updateProductForm").addEventListener("submit", function (event) {
  event.preventDefault();

  var token = sessionStorage.getItem("Token");

  // Kiểm tra nếu không có token, chuyển hướng đến trang access denied
  if (!token) {
      window.location.href = "/error/AccessDenied.html";
      return;
  }

  var productId = document.getElementById("Product_Id").value;
  var productName = document.getElementById("Product_Name").value;
  var productPrice = document.getElementById("Product_Price").value;
  var quantityInStock = document.getElementById("Product_QuantityInStock").value;
  var productDescription = document.getElementById("Product_Description").value;
  var categoryId = document.getElementById("Product_CategoryId").value;
  var productPoster = document.getElementById("Product_Poster").files[0];

  var formData = new FormData();
  formData.append("Id", productId);
  formData.append("Name", productName);
  formData.append("Price", productPrice);
  formData.append("QuantityInStock", quantityInStock);
  formData.append("Description", productDescription);
  formData.append("CategoryId", categoryId);

  if (!productPoster && document.getElementById("Product_PosterName").value !== "") {
    formData.append("PosterName", document.getElementById("Product_PosterName").value);
  } else {
    formData.append("Poster", productPoster);
  }

  axios.put(`/api/SpaProduct/${productId}`, formData, {
    headers: {
      Authorization: "Bearer " + token,
      "Content-Type": "multipart/form-data",
    },
  })
    .then((response) => {
      if (response.data.success) {
        // Xử lý thành công, ví dụ: hiển thị thông báo, chuyển hướng trang, làm mới danh sách sản phẩm, v.v.
        alert("Cập nhật sản phẩm thành công");
        window.location.href = "/Products/Index";
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
