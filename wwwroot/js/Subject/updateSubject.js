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
    document.querySelector("form").addEventListener("submit", function (event) {
        event.preventDefault();
        hideAlerts();
        var token = sessionStorage.getItem("Token");

        // Kiểm tra nếu không có token, chuyển hướng đến trang access denied
        if (!token) {
            window.location.href = "/error/AccessDenied.html";
            return;
        }

        var subjectId = document.querySelector("#Subject_Id").value;
        var subjectName = document.querySelector("#Subject_Name").value;

        var data = {
            Id: subjectId,
            Name: subjectName,
            // Add other properties here for updating other fields
        };

        axios.put(`/api/Subject/${subjectId}`, data, {
            headers: {
                Authorization: "Bearer " + token,
            },
        })
            .then((response) => {
                if (response.data.success) {
                    // Xử lý thành công, ví dụ: hiển thị thông báo, chuyển hướng trang, v.v.
                    alert("Subject updated successfully!");
                    window.location.href = "/Subjects/Index";
                } else {
                    // Xử lý lỗi từ máy chủ, ví dụ: hiển thị thông báo lỗi
                    console.log("Lỗi: " + response.data.message);
                    alert(response.data.message);
                }
            })
            .catch((error) => {
                if (error.response) {
                    if (!alertDisplayed) {
                        var parentElement = document.querySelector("form");

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
});
