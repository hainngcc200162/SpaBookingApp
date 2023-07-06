document.addEventListener("DOMContentLoaded", function () {
  document
    .getElementById("loginForm")
    .addEventListener("submit", function (event) {
      event.preventDefault();

      var username = document.getElementById("username").value;
      var password = document.getElementById("password").value;

      axios
        .post("/auth/login", { username: username, password: password })
        .then((response) => {
          if (response.data.success) {
            var token = response.data.data;

            // Lưu token vào sessionStorage
            sessionStorage.setItem("Token", token);
            // console.log("Token:", token);
            // console.log("Headers:", axios.defaults.headers.common);
            // Gán token vào header "Authorization" với giá trị "Bearer"
            axios.defaults.headers.common["Authorization"] = "Bearer " + token;
            // console.log("Headers:", axios.defaults.headers.common);
            // Tiếp tục với các thao tác khác hoặc chuyển hướng đến trang khác
            window.location.href = "/Products/Index";
            sessionStorage.setItem("username", username);
            alert("Login Success!");
            // Hiển thị Username trong header
            // console.log(username);
          } else {
            alert("Đăng nhập không thành công: " + response.data.message);
          }
        })
        .catch((error) => {
          console.error("Đã xảy ra lỗi:", error);
        });
    });
});
