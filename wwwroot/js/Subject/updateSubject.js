document.addEventListener("DOMContentLoaded", function () {
    document.querySelector("form").addEventListener("submit", function (event) {
        event.preventDefault();

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
                }
            })
            .catch((error) => {
                // Xử lý lỗi kết nối hoặc lỗi xử lý trên máy chủ
                console.log("Lỗi: " + error);
                window.location.href = "/Error/AccessDenied.html";
            });
    });
});
