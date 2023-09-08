document.getElementById("createSubjectForm").addEventListener("submit", function (event) {
    event.preventDefault();

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
                alert("Subject created successfully");
                window.location.href = "/Subjects/Index";
            } else {
                console.log("Error: " + response.data.message);
                alert(response.data.message);
            }
        })
        .catch(error => {
            window.location.href = "/Error/AccessDenied.html";
        });
});
