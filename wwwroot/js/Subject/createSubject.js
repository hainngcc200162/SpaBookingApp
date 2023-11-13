var alertDisplayed = false;
function hideAlerts() {
    var alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        alert.style.display = 'none';
    });

    // Reset alert flags
    alertDisplayed = false;
}

document.getElementById("createSubjectForm").addEventListener("submit", function (event) {
    event.preventDefault();
    hideAlerts();
    var token = sessionStorage.getItem("Token");

    if (!token) {
        window.location.href = "/error/AccessDenied.html";
        return;
    }

    var subjectName = document.getElementById("Subject_Name").value;

    axios
        .post("/api/Subject", { name: subjectName }, {
            headers: {
                Authorization: "Bearer " + token,
                "Content-Type": "application/json",
            },
        })
        .then(response => {
            if (response.data.success) {
                alert("Successfully created subject");
                window.location.href = "/Subjects/Index";
            } else {
                if (!alertDisplayed) {
                    var parentElement = document.getElementById("createSubjectForm");

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
