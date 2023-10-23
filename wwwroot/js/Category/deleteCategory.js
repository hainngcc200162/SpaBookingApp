document.getElementById("deleteCategoryForm").addEventListener("submit", function (event) {
    event.preventDefault();

    var categoryId = document.getElementById("Category_Id").value;

    var token = sessionStorage.getItem("Token");

    if (!token) {
        window.location.href = "/error/AccessDenied.html";
        return;
    }

    axios.delete(`/api/Category/${categoryId}`, {
        headers: {
            Authorization: "Bearer " + token,
        },
    })
        .then((response) => {
            if (response.data.success) {
                alert("Category deleted successfully");
                window.location.href = "/Categories/Index";
            } else {
                console.log("Error: " + response.data.message);
            }
        })
        .catch((error) => {
            console.log("Error: " + error);
            window.location.href = "/Error/AccessDenied.html";
        });
});
