var alertDisplayed = false;
function hideAlerts() {
  var alerts = document.querySelectorAll('.alert');
  alerts.forEach(alert => {
    alert.style.display = 'none';
  });

  // Reset alert flags
  alertDisplayed = false;
}
document.getElementById("updateProvisionForm").addEventListener("submit", function (event) {
  event.preventDefault();
  hideAlerts();
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
      if (error.response) {
        if (!alertDisplayed) {
          var parentElement = document.getElementById("updateProvisionForm");

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
