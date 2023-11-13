var alertDisplayed = false;
var alertDisplayedImg = false;
function hideAlerts() {
    var alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        alert.style.display = 'none';
    });

    // Reset alert flags
    alertDisplayed = false;
    alertDisplayedImg = false;
}

document.getElementById("createProvisionForm").addEventListener("submit", function (event) {
    event.preventDefault();
    hideAlerts();
    var token = sessionStorage.getItem("Token");

    if (!token) {
        window.location.href = "/error/AccessDenied.html";
        return;
    }

    var provisionPoster = document.getElementById("imageInput").files[0];
    if (!provisionPoster) {
        if (!alertDisplayedImg) {
            var parentElement = document.getElementById("createProvisionForm");
            var alertElement = document.createElement("div");
            alertElement.className = "mb-3 alert alert-danger";
            alertElement.setAttribute("role", "alert");
            alertElement.textContent = "Please select an image before creating.";
            parentElement.insertBefore(alertElement, parentElement.firstChild);
            alertElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
            alertDisplayedImg = true;
        }
        return;
    }

    var provisionName = document.getElementById("Provision_Name").value;
    var provisionPrice = document.getElementById("Provision_Price").value;
    var provisionDuration = document.getElementById("Provision_DurationMinutes").value;
    var provisionNumberOfExecutions = document.getElementById("Provision_NumberOfExecutions").value;
    var provisionDescription = document.getElementById("Provision_Description").value;
    var provisionStatus = document.getElementById("Provision_Status").value;
    var provisionPoster = document.getElementById("imageInput").files[0];

    var formData = new FormData();
    formData.append("Name", provisionName);
    formData.append("Price", provisionPrice);
    formData.append("DurationMinutes", provisionDuration);
    formData.append("NumberOfExecutions", provisionNumberOfExecutions);   
    formData.append("Description", provisionDescription);
    formData.append("Status", provisionStatus);
    formData.append("Poster", provisionPoster);

    axios
        .post("/api/Provision", formData, {
            headers: {
                Authorization: "Bearer " + token,
                "Content-Type": "multipart/form-data",
            },
        })
        .then((response) => {
            if (response.data.success) {
                alert("Provision created successfully");
                window.location.href = "/Provisions/Index";
            } else {
                if (!alertDisplayed) {
                    var parentElement = document.getElementById("createProvisionForm");

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
        .catch((error) => {
            window.location.href = "/Error/AccessDenied.html";
        });
});
