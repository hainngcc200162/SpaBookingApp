
document.getElementById("createCategoryForm").addEventListener("submit", function (event) {
    event.preventDefault();

    var token = sessionStorage.getItem("Token");

    // Kiểm tra nếu không có token, chuyển hướng đến trang access denied
    if (!token) {
        window.location.href = "/error/AccessDenied.html";
        return;
    }

    var categoryName = document.getElementById("Category_Name").value;

    axios
        .post("/api/Category", { name: categoryName }, {
            headers: {
                Authorization: "Bearer " + token,
                "Content-Type": "application/json",
            },
        })
        .then(response => {
            if (response.data.success) {
                alert("Category created successfully");
                window.location.href = "/Categories/Index";
            } else {
                var parentElement = document.getElementById("createCategoryForm");
                // Tạo alert và sử dụng nội dung từ response
                var alertElement = document.createElement("div");
                alertElement.className = "mb-3 alert alert-danger";
                alertElement.setAttribute("role", "alert");
                alertElement.textContent = "Error: " + response.data.message;
                // Thêm alert vào form
                parentElement.insertBefore(alertElement, parentElement.firstChild);
                alertElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
                console.log("Error: " + response.data.message);
                
            }
        })
        .catch(error => {
            window.location.href = "/Error/AccessDenied.html";
        });
});
