var alertDisplayed = false;
function hideAlerts() {
    var alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        alert.style.display = 'none';
    });

    // Reset alert flags
    alertDisplayed = false;
}
document.querySelector("form").addEventListener("submit", function (event) {
  event.preventDefault();
  hideAlerts();
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
      if (error.response) {
        if (!alertDisplayed) {
          var parentElement = document.querySelector("form");

          var alertElement = document.createElement("div");
          alertElement.className = "mb-3 alert alert-danger";
          alertElement.setAttribute("role", "alert");
          alertElement.textContent = error.response.data.message;

          parentElement.insertBefore(alertElement, parentElement.firstChild);
          alertDisplayed = true;
        }

      } else {
        console.log("Network Error or Unknown Error: " + error.message);
      }
    });
});
