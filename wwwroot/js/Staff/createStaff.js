var alertDisplayed = false;
var alertDisplayedImg = false;
var alertDisplayedEmail = false;

function hideAlerts() {
    var alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        alert.style.display = 'none';
    });

    // Reset alert flags
    alertDisplayed = false;
    alertDisplayedImg = false;
    alertDisplayedEmail = false;
}
document.getElementById("createStaffForm").addEventListener("submit", function (event) {
    event.preventDefault();

    hideAlerts();
    // Kiểm tra xem tệp hình ảnh đã được chọn hay chưa
    var staffPoster = document.getElementById("imageInput").files[0];
    if (!staffPoster) {
        if (!alertDisplayedImg) {
            var parentElement = document.getElementById("createStaffForm");
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

    var token = sessionStorage.getItem("Token");

    if (!token) {
        window.location.href = "/error/AccessDenied.html";
        return;
    }

    var staffName = document.getElementById("Staff_Name").value;
    var staffGender = document.getElementById("Staff_Gender").value;
    var staffEmail = document.getElementById("Staff_Email").value;
    var staffDescription = document.getElementById("Staff_Description").value;
    var staffPoster = document.getElementById("imageInput").files[0];

    // Email validation regular expression
    var emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;

    if (!emailPattern.test(staffEmail)) {
        if (!alertDisplayedEmail) {
            var parentElement = document.getElementById("createStaffForm");
            var alertElement = document.createElement("div");
            alertElement.className = "mb-3 alert alert-danger";
            alertElement.setAttribute("role", "alert");
            alertElement.textContent = "Invalid email format";
            parentElement.insertBefore(alertElement, parentElement.firstChild);
            alertElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
            alertDisplayedEmail = true;
        }
        return; // Don't proceed with the form submission
    }

    var formData = new FormData();
    formData.append("Name", staffName);
    formData.append("Gender", staffGender);
    formData.append("Email", staffEmail);
    formData.append("Description", staffDescription);
    formData.append("Poster", staffPoster);

    axios
        .post("/api/Staff", formData, {
            headers: {
                Authorization: "Bearer " + token,
                "Content-Type": "multipart/form-data",
            },
        })
        .then((response) => {
            if (response.data.success) {
                alert("Staff created successfully");
                window.location.href = "/Staffs/Index";
            } else {
                if (!alertDisplayed) {
                    var parentElement = document.getElementById("createStaffForm");

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
