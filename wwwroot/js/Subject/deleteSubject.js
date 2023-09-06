document.getElementById("deleteSubjectForm").addEventListener("submit", function (event) {
    event.preventDefault();

    var subjectId = document.getElementById("Subject_Id").value;
    var token = sessionStorage.getItem("Token");

    // Kiểm tra nếu không có token, chuyển hướng đến trang access denied
    if (!token) {
        window.location.href = "/error/AccessDenied.html";
        return;
    }

    axios.delete(`/api/Subject/${subjectId}`, {
        headers: {
            Authorization: "Bearer " + token,
        },
    })
        .then((response) => {
            if (response.data.success) {
                // Xử lý thành công, ví dụ: hiển thị thông báo, chuyển hướng trang, v.v.
                alert("Subject deleted successfully");
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

