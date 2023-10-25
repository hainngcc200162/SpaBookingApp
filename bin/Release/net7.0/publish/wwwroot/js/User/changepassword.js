document.addEventListener("DOMContentLoaded", function () {
    // Lấy giá trị token từ sessionStorage
    var token = sessionStorage.getItem("Token");

    // Kiểm tra nếu không có token, chuyển hướng đến trang access denied
    if (!token) {
        window.location.href = "/error/AccessDenied.html";
        return;
    }

    // Thêm token vào header của request
    axios.defaults.headers.common["Authorization"] = "Bearer " + token;

    // Xử lý khi người dùng gửi form
    document.getElementById("changePasswordForm").addEventListener("submit", function (event) {
        event.preventDefault();

        var oldPassword = document.getElementById("oldPassword").value;
        var newPassword = document.getElementById("newPassword").value;
        var confirmNewPassword = document.getElementById("confirmNewPassword").value;

        if (newPassword !== confirmNewPassword) {
            // Kiểm tra xác nhận mật khẩu mới
            displayResult("Passwords do not match.", "alert-danger");
        } else {
            // Gửi dữ liệu bằng phương thức POST
            axios.post("/auth/changepassword", {
                OldPassword: oldPassword,
                NewPassword: newPassword,
                ConfirmNewPassword: confirmNewPassword
            })
            .then(function (response) {
                if (response.data.success && response.data.data) {
                    // Nếu thay đổi mật khẩu thành công, hiển thị thông báo thành công
                    alert("Password changed successfully.");
                    window.location.href = "/UserManagement/Profile";
                } else {
                    // Nếu thay đổi mật khẩu thất bại, hiển thị thông báo lỗi

                    displayResult(response.data.message, "alert-danger");
                }
            })
            .catch(function (error) {
                // Xử lý lỗi nếu có
                console.error("An error occurred while changing the password:", error);
                displayResult("An error occurred while changing the password. Please try again.", "alert-danger");
            });
        }
    });

    function displayResult(message, alertClass) {
        var resultElement = document.getElementById("changePasswordResult");
        resultElement.textContent = message;
        resultElement.className = "alert " + alertClass;
        resultElement.style.display = "block";
    }
});