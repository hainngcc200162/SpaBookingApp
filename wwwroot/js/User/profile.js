document.addEventListener("DOMContentLoaded", function () {
    console.log("DOMContentLoaded event fired");

    var token = sessionStorage.getItem("Token");
    console.log("Token:", token);

    if (!token) {
        console.log("No token found, redirecting to access denied");
        window.location.href = "/error/AccessDenied.html";
        return;
    }

    axios.defaults.headers.common["Authorization"] = "Bearer " + token;

    axios.get("/auth/profile")
        .then(function (response) {
            console.log("Profile API response:", response);

            var result = response.data;
            if (result.success) {
                console.log("Profile data:", result.data);

                // Handle success and update UI
                var userProfile = result.data;
                document.getElementById("firstName").innerText = userProfile.firstName;
                document.getElementById("lastName").innerText = userProfile.lastName;
                document.getElementById("email").innerText = userProfile.email;
                document.getElementById("phoneNumber").innerText = userProfile.phoneNumber;
                document.getElementById("role").innerText = userProfile.role;
                document.getElementById("createdAt").innerText = new Date(userProfile.createdAt).toLocaleDateString();
            } else {
                console.log("Profile API response indicates failure, redirecting to access denied");
                // Handle failure and redirect to error page
                window.location.href = "/error/AccessDenied.html";
            }
        })
        .catch(function (error) {
            console.error("An error occurred while making the request:", error);
            // Handle request error and redirect to error page
            window.location.href = "/error/AccessDenied.html";
        });
});
// document.getElementById("updateProfileButton").addEventListener("click", function () {
//     window.location.href = "/UserManagement/ProfileUpdate";
// });