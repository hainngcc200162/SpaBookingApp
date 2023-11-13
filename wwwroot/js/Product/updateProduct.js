var alertDisplayed = false;
function hideAlerts() {
  var alerts = document.querySelectorAll('.alert');
  alerts.forEach(alert => {
    alert.style.display = 'none';
  });

  // Reset alert flags
  alertDisplayed = false;
}
document.getElementById("updateProductForm").addEventListener("submit", function (event) {
  event.preventDefault();
  hideAlerts();
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
        alert("Product updated successfully!");
        window.location.href = "/Products/Index";
      } else {
        // Xử lý lỗi từ máy chủ, ví dụ: hiển thị thông báo lỗi
        console.log("Lỗi: " + response.data.message);
        alert(response.data.message);
      }
    })
    .catch(error => {
      if (error.response) {
        if (!alertDisplayed) {
          var parentElement = document.getElementById("updateProductForm");

          var alertElement = document.createElement("div");
          alertElement.className = "mb-3 alert alert-danger";
          alertElement.setAttribute("role", "alert");
          alertElement.textContent = error.response.data.message;

          parentElement.insertBefore(alertElement, parentElement.firstChild);
          alertElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
          alertDisplayed = true;
        }
      } else {
        // Xử lý lỗi mạng hoặc lỗi không xác định
        console.log("Network Error or Unknown Error: " + error.message);
      }
    });
});
