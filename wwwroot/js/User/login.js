var alertDisplayed = false;
function hideAlerts() {
    var alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        alert.style.display = 'none';
    });

    // Reset alert flags
    alertDisplayed = false;
}

document.addEventListener("DOMContentLoaded", function () {
  document.getElementById("loginForm").addEventListener("submit", function (event) {
    event.preventDefault();
    hideAlerts();
    var email = document.getElementById("email").value;
    var password = document.getElementById("password").value;

    axios.post("/auth/login", { email: email, password: password })
      .then((response) => {
        if (response.data.success) {
          var token = response.data.data;

          // Lưu token và lastname vào sessionStorage
          sessionStorage.setItem("Token", token);
          sessionStorage.setItem("email", email);
          
          axios.defaults.headers.common["Authorization"] = "Bearer " + token;
          alert("Login Success!");
          window.location.href = "/UserManagement/Profile";
          
        } else {
          alert("Đăng nhập không thành công: " + response.data.message);
        }
      })
      .catch((error) => {
        if (error.response) {
          if (!alertDisplayed) {
            var parentElement = document.getElementById("loginForm");
  
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
});
