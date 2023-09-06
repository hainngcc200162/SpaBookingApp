document.querySelector("form").addEventListener("submit", function (event) {
    event.preventDefault();
  
    var token = sessionStorage.getItem("Token");
  
    // Kiểm tra nếu không có token, chuyển hướng đến trang access denied
    if (!token) {
      window.location.href = "/error/AccessDenied.html";
      return;
    }
  
    var categoryId = document.getElementById("Category_Id").value;
    var categoryName = document.getElementById("Category_Name").value;
  
    var data = {
      Id: categoryId,
      Name: categoryName,
    };
  
    axios.put(`/api/Category/${categoryId}`, data, {
      headers: {
        Authorization: "Bearer " + token,
      },
    })
      .then((response) => {
        if (response.data.success) {
          // Xử lý thành công, ví dụ: hiển thị thông báo, chuyển hướng trang, v.v.
          alert("Category updated successfully!");
          window.location.href = "/Categories/Index";
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
  