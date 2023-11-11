document.getElementById("deleteDepartmentForm").addEventListener("submit", function (event) {
    event.preventDefault();

    var departmentId = document.getElementById("Department_Id").value;

    var token = sessionStorage.getItem("Token");

    if (!token) {
        window.location.href = "/error/AccessDenied.html";
        return;
    }

    axios.delete(`/api/Department/${departmentId}`, {
        headers: {
            Authorization: "Bearer " + token,
        },
    })
        .then((response) => {
            if (response.data.success) {
                // Xử lý thành công, ví dụ: hiển thị thông báo, chuyển hướng trang, làm mới danh sách phòng ban, v.v.
                alert("Department deleted successfully");
                window.location.href = "/Departments/Index";
            } else {
                // Xử lý lỗi từ máy chủ, ví dụ: hiển thị thông báo lỗi
                console.log("Error: " + response.data.message);
            }
        })
        .catch((error) => {
            // Xử lý lỗi kết nối hoặc lỗi xử lý trên máy chủ
            console.log("Error: " + error);
            window.location.href = "/Error/AccessDenied.html";
        });
});
