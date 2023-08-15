document.getElementById("createProvisionForm").addEventListener("submit", function (event) {
    event.preventDefault();

    var token = sessionStorage.getItem("Token");

    if (!token) {
        window.location.href = "/error/AccessDenied.html";
        return;
    }

    var provisionName = document.getElementById("Provision_Name").value;
    var provisionPrice = document.getElementById("Provision_Price").value;
    var provisionDuration = document.getElementById("Provision_DurationMinutes").value;
    var provisionDescription = document.getElementById("Provision_Description").value;
    var provisionStatus = document.getElementById("Provision_Status").value;
    var provisionPoster = document.getElementById("imageInput").files[0];

    var formData = new FormData();
    formData.append("Name", provisionName);
    formData.append("Price", provisionPrice);
    formData.append("DurationMinutes", provisionDuration);
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
                console.log("Error: " + response.data.message);
            }
        })
        .catch((error) => {
            window.location.href = "/Error/AccessDenied.html";
        });
});
