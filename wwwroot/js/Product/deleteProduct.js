document.getElementById("deleteProductForm").addEventListener("submit", function (event) {
  event.preventDefault();

  var productId = document.getElementById("Product_Id").value;

  var token = sessionStorage.getItem("Token");

  axios.delete(`/api/Product/${productId}`, {
      headers: {
          Authorization: "Bearer " + token,
      },
  })
  .then((response) => {
      if (response.data.success) {
          // Xử lý thành công, ví dụ: hiển thị thông báo, chuyển hướng trang, làm mới danh sách sản phẩm, v.v.
          alert("Product deleted successfully");
          window.location.href = "/Products/Index";
      } else {
          // Xử lý lỗi từ máy chủ, ví dụ: hiển thị thông báo lỗi
          console.log("Error: " + response.data.message);
      }
  })
  .catch((error) => {
      // Xử lý lỗi kết nối hoặc lỗi xử lý trên máy chủ
      console.log("Error: " + error);
      window.location.href = "/Error/AccessDenied.html";
  });
});
