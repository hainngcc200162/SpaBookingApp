document.querySelector("form").addEventListener("submit", function (event) {
    event.preventDefault();
  
    var token = sessionStorage.getItem("Token");
  
    // Kiểm tra nếu không có token, chuyển hướng đến trang access denied
    if (!token) {
      window.location.href = "/error/AccessDenied.html";
      return;
    }
  
    var departmentId = document.getElementById("Department_Id").value;
    var departmentName = document.getElementById("Department_Name").value;
    var openingHours = document.getElementById("Department_OpeningHours").value;
    var description = document.getElementById("Department_Description").value;
  
    var data = {
      Id: departmentId,
      Name: departmentName,
      OpeningHours: openingHours,
      Description: description
    };
  
    axios.put(`/api/Department/${departmentId}`, data, {
      headers: {
        Authorization: "Bearer " + token,
      },
    })
      .then((response) => {
        if (response.data.success) {
          // Xử lý thành công, ví dụ: hiển thị thông báo, chuyển hướng trang, v.v.
          alert("Cập nhật phòng ban thành công");
          window.location.href = "/Departments/Index";
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
