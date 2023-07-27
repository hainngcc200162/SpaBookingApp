document.addEventListener("DOMContentLoaded", function () {
  document.getElementById("loginForm").addEventListener("submit", function (event) {
    event.preventDefault();

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

          window.location.href = "/Products/Index";
          alert("Login Success!");
        } else {
          alert("Đăng nhập không thành công: " + response.data.message);
        }
      })
      .catch((error) => {
        console.error("Đã xảy ra lỗi:", error);
      });
  });
});
