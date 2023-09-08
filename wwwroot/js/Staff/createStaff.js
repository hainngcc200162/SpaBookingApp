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
                alert(response.data.message);
            }
        })
        .catch((error) => {
            window.location.href = "/Error/AccessDenied.html";
        });
});
