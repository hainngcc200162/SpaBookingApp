document.getElementById("updateProvisionForm").addEventListener("submit", function (event) {
    event.preventDefault();
  
    var token = sessionStorage.getItem("Token");
  
    // Kiểm tra nếu không có token, chuyển hướng đến trang access denied
    if (!token) {
      window.location.href = "/error/AccessDenied.html";
      return;
    }
  
    var provisionId = document.getElementById("Provision_Id").value;
    var provisionName = document.getElementById("Provision_Name").value;
    var provisionDescription = document.getElementById("Provision_Description").value;
    var provisionPrice = document.getElementById("Provision_Price").value;
    var provisionDuration = document.getElementById("Provision_DurationMinutes").value;
    var provisionStatus = document.getElementById("Provision_Status").value;
    var provisionPoster = document.getElementById("Provision_Poster").files[0];
  
    var formData = new FormData();
    formData.append("Id", provisionId);
    formData.append("Name", provisionName);
    formData.append("Description", provisionDescription);
    formData.append("Price", provisionPrice);
    formData.append("DurationMinutes", provisionDuration);
    formData.append("Status", provisionStatus);
  
    if (!provisionPoster && document.getElementById("Provision_PosterName").value !== "") {
      formData.append("PosterName", document.getElementById("Provision_PosterName").value);
    } else {
      formData.append("Poster", provisionPoster);
    }
  
    axios.put(`/api/Provision/${provisionId}`, formData, {
      headers: {
        Authorization: "Bearer " + token,
        "Content-Type": "multipart/form-data",
      },
    })
      .then((response) => {
        if (response.data.success) {
          // Xử lý thành công, ví dụ: hiển thị thông báo, chuyển hướng trang, v.v.
          alert("Cập nhật dịch vụ thành công");
          window.location.href = "/Provisions/Index";
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
  