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
    var provisionNumberOfExecutions = parseInt(document.getElementById("Provision_NumberOfExecutions").value);
    var provisionNumberOfExecutions = document.getElementById("Provision_NumberOfExecutions").value;
    var provisionStatus = document.getElementById("Provision_Status").value;
    var provisionPoster = document.getElementById("Provision_Poster").files[0];

    var formData = new FormData();
    formData.append("Id", provisionId);
    formData.append("Name", provisionName);
    formData.append("Description", provisionDescription);
    formData.append("Price", provisionPrice);
    formData.append("DurationMinutes", provisionDuration);
    formData.append("NumberOfExecutions", provisionNumberOfExecutions);
    formData.append("Status", provisionStatus);
  
    if (!provisionPoster && document.getElementById("Provision_PosterName").value !== "") {
      formData.append("PosterName", document.getElementById("Provision_PosterName").value);
    } else {
      formData.append("Poster", provisionPoster);
    }

    console.log(formData);
  
    axios.put(`/api/Provision/${provisionId}`, formData, {
      headers: {
        Authorization: "Bearer " + token,
        "Content-Type": "multipart/form-data",
      },
    })
      .then((response) => {
        if (response.data.success) {
          // Xử lý thành công, ví dụ: hiển thị thông báo, chuyển hướng trang, v.v.
          alert("Provision updated successfully!");
          window.location.href = "/Provisions/Index";
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
  