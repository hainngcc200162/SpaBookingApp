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
                alert("Successfully created subject");
                window.location.href = "/Subjects/Index";
            } else {
                console.log("Error: " + response.data.message);
                // Tạo một thẻ khoảng cách (ví dụ: div)
                var spaceElement = document.createElement("div");
                spaceElement.style.height = "0px";
                var parentElement = document.getElementById("createSubjectForm");
                parentElement.insertBefore(spaceElement, parentElement.firstChild);
                var alertElement = document.createElement("div");
                alertElement.className = "mb-3 alert alert-danger";
                alertElement.setAttribute("role", "alert");
                alertElement.textContent = "This subject already exists, please create another subject.";
                parentElement.insertBefore(alertElement, parentElement.firstChild);
            }
        })
        .catch(error => {
            window.location.href = "/Error/AccessDenied.html";
        });
});
