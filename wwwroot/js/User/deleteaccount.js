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
    document.getElementById("deleteAccountForm").addEventListener("submit", function (event) {
        event.preventDefault();

        var password = document.getElementById("password").value;
        var confirmPassword = document.getElementById("confirmPassword").value;

        if (password !== confirmPassword) {
            // Kiểm tra xác nhận mật khẩu
            displayResult("Passwords do not match.", "alert-danger");
        } else {
            // Gửi dữ liệu bằng phương thức DELETE
            axios.delete("/auth/deleteaccount", {
                data: {
                    Password: password,
                    ConfirmPassword: confirmPassword
                }
            })
            .then(function (response) {
                if (response.data.success && response.data.data) {
                    // Nếu xóa tài khoản thành công, hiển thị thông báo thành công
                    alert("Account deleted successfully.");
                    window.location.href = "/UserManagement/Login";
                } else {
                    // Nếu xóa tài khoản thất bại, hiển thị thông báo lỗi
                    displayResult(response.data.message, "alert-danger");
                }
            })
            .catch(function (error) {
                // Xử lý lỗi nếu có
                console.error("An error occurred while deleting the account:", error);
                displayResult("An error occurred while deleting the account. Please try again.", "alert-danger");
            });
        }
    });

    function displayResult(message, alertClass) {
        var resultElement = document.getElementById("deleteAccountResult");
        resultElement.textContent = message;
        resultElement.className = "alert " + alertClass;
        resultElement.style.display = "block";
    }
});