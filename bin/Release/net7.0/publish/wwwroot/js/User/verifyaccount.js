document.addEventListener("DOMContentLoaded", function () {
    // Lấy giá trị email từ sessionStorage
    var email = sessionStorage.getItem("TempEmail");

    // Hiển thị giá trị email trong trường email của form
    document.getElementById("email").value = email;

    // Xử lý khi người dùng gửi form
    document.getElementById("verifyAccountForm").addEventListener("submit", function (event) {
        event.preventDefault();

        var verificationCode = document.getElementById("verificationCode").value;
        var formData = {
            Email: email,
            VerificationCode: verificationCode
        };

        // Gửi dữ liệu bằng phương thức POST
        axios.post("/auth/verifyaccount", formData)
            .then(function (response) {
                if (response.data.success && response.data.data) {
                    // Nếu xác minh thành công, chuyển hướng đến trang Login
                    alert("Verify Account Success!");
                    window.location.href = "/UserManagement/Login";
                } else {
                    // Nếu xác minh thất bại, hiển thị thông báo lỗi
                    var alertElement = document.createElement("div");
                    alertElement.className = "alert alert-danger";
                    alertElement.setAttribute("role", "alert");
                    alertElement.textContent = "Account verification failed. Please check the verification code and try again.";
                    document.getElementById("verifyAccountForm").appendChild(alertElement);
                }
            })
            .catch(function (error) {
                // Xử lý lỗi nếu có
                console.error("An error occurred while verifying the account:", error);
                var alertElement = document.createElement("div");
                alertElement.className = "alert alert-danger";
                alertElement.setAttribute("role", "alert");
                alertElement.textContent = "An error occurred while verifying the account. Please try again.";
                document.getElementById("verifyAccountForm").appendChild(alertElement);
            });
    });
});