document.getElementById("createStaffForm").addEventListener("submit", function (event) {
    event.preventDefault();

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
        // Email is not in a valid format, show an error message
        alert("Invalid email format");
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
                console.log("Error: " + response.data.message);
                var parentElement = document.getElementById("createStaffForm");
                // Tạo alert và sử dụng nội dung từ response
                var alertElement = document.createElement("div");
                alertElement.className = "mb-3 alert alert-danger";
                alertElement.setAttribute("role", "alert");
                alertElement.textContent = "Error: " + response.data.message;
                // Thêm alert vào form
                parentElement.insertBefore(alertElement, parentElement.firstChild);
                alertElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
            }
        })
        .catch((error) => {
            window.location.href = "/Error/AccessDenied.html";
        });
});
