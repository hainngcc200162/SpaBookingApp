document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("createCategoryForm").addEventListener("submit", function (event) {
        event.preventDefault();

        var token = sessionStorage.getItem("Token");

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
                    console.log("Error: " + response.data.message);
                }
            })
            .catch(error => {
                window.location.href = "/Error/AccessDenied.html";
            });
    });
});
