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
    document.getElementById("createDepartmentForm").addEventListener("submit", function (event) {
        event.preventDefault();

        hideAlerts();

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
                    if (!alertDisplayed) {
                        var parentElement = document.getElementById("createDepartmentForm");

                        var alertElement = document.createElement("div");
                        alertElement.className = "mb-3 alert alert-danger";
                        alertElement.setAttribute("role", "alert");
                        alertElement.textContent = response.data.message;

                        parentElement.insertBefore(alertElement, parentElement.firstChild);
                        alertElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
                        alertDisplayed = true;
                    }
                }
            })
            .catch(error => {
                window.location.href = "/Error/AccessDenied.html";
            });
    });
});
