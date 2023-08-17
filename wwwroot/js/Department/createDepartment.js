document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("createDepartmentForm").addEventListener("submit", function (event) {
        event.preventDefault();

        var token = sessionStorage.getItem("Token");

        if (!token) {
            window.location.href = "/error/AccessDenied.html";
            return;
        }

        var departmentName = document.getElementById("Department_Name").value;
        var openingHours = document.getElementById("Department_OpeningHours").value;
        var description = document.getElementById("Department_Description").value;

        axios
            .post("/api/Department", {
                name: departmentName,
                openingHours: openingHours,
                description: description
            }, {
                headers: {
                    Authorization: "Bearer " + token,
                    "Content-Type": "application/json",
                },
            })
            .then(response => {
                if (response.data.success) {
                    alert("Department created successfully");
                    window.location.href = "/Departments/Index";
                } else {
                    console.log("Error: " + response.data.message);
                }
            })
            .catch(error => {
                window.location.href = "/Error/AccessDenied.html";
            });
    });
});
