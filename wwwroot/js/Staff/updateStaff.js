document.getElementById("updateStaffForm").addEventListener("submit", function (event) {
    event.preventDefault();
  
    var token = sessionStorage.getItem("Token");
  
    // Kiểm tra nếu không có token, chuyển hướng đến trang access denied
    if (!token) {
      window.location.href = "/error/AccessDenied.html";
      return;
    }
  
    var staffId = document.getElementById("Staff_Id").value;
    var staffName = document.getElementById("Staff_Name").value;
    var staffDescription = document.getElementById("Staff_Description").value;
    var staffGender = document.getElementById("Staff_Gender").value;
    var staffEmail = document.getElementById("Staff_Email").value;
    var staffPoster = document.getElementById("Staff_Poster").files[0];
  
    var formData = new FormData();
    formData.append("Id", staffId);
    formData.append("Name", staffName);
    formData.append("Description", staffDescription);
    formData.append("Gender", staffGender);
    formData.append("Email", staffEmail);
  
    if (!staffPoster && document.getElementById("Staff_PosterName").value !== "") {
      formData.append("PosterName", document.getElementById("Staff_PosterName").value);
    } else {
      formData.append("Poster", staffPoster);
    }
  
    axios.put(`/api/Staff/${staffId}`, formData, {
      headers: {
        Authorization: "Bearer " + token,
        "Content-Type": "multipart/form-data",
      },
    })
      .then((response) => {
        if (response.data.success) {
          // Xử lý thành công, ví dụ: hiển thị thông báo, chuyển hướng trang, v.v.
          alert("Staff updated successfully!");
          window.location.href = "/Staffs/Index";
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
