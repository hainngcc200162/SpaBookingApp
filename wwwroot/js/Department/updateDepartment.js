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
          alert("Deparment updated successfully!");
          window.location.href = "/Departments/Index";
        } else {
          // Xử lý lỗi từ máy chủ, ví dụ: hiển thị thông báo lỗi
          console.log("Lỗi: " + response.data.message);
          alert(response.data.message);
        }
      })
      .catch(error => {
        // Xử lý lỗi khi yêu cầu Fetch không thành công
        if (error.response) {
            if (error.response.status === 400) {
                // Xử lý lỗi 400 Bad Request
                alert("Bad Request: " + error.response.data.message);
            } else if (error.response.status === 401) {
                // Xử lý lỗi 401 Unauthorized
                alert("Unauthorized: " + error.response.data.message);
            } else if (error.response.status === 404) {
                // Xử lý lỗi 404 Not Found
                alert("Not Found: " + error.response.data.message);
            } else {
                // Xử lý các lỗi HTTP khác
                console.log("HTTP Error: " + error.response.status);
            }
        } else {
            // Xử lý lỗi mạng hoặc lỗi không xác định
            console.log("Network Error or Unknown Error: " + error.message);
        }
    });
  });
