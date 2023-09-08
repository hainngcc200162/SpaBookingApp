document.getElementById("registerForm").addEventListener("submit", function (event) {
    event.preventDefault();

    var firstName = document.getElementById("firstName").value;
    var lastName = document.getElementById("lastName").value;
    var email = document.getElementById("email").value;
    var phoneNumber = document.getElementById("phoneNumber").value;
    var password = document.getElementById("password").value;
    var confirmPassword = document.getElementById("confirmPassword").value;

    if (password !== confirmPassword) {
        var registerAlert = document.getElementById("registerAlert");
        registerAlert.textContent = "Passwords do not match.";
        registerAlert.classList.add("alert-danger");
        registerAlert.style.display = "block";
        return;
    }

    axios.post("/auth/register", {
        FirstName: firstName,
        LastName: lastName,
        Email: email,
        PhoneNumber: phoneNumber,
        Password: password,
        ConfirmPassword: confirmPassword
    })
    .then((response) => {
        if (response.data.success) {
            // Handle successful registration, e.g., redirect to a success page
            alert("Registration successful!");

            // Save the email to sessionStorage
            sessionStorage.setItem("TempEmail", email);

            // Redirect to the verification page with the email as a query parameter
            window.location.href = "/UserManagement/VerifyAccount?email=" + encodeURIComponent(email);
        } else {
            var registerAlert = document.getElementById("registerAlert");
            registerAlert.textContent = "Registration failed: " + response.data.message;
            registerAlert.classList.add("alert-danger");
            registerAlert.style.display = "block";
        }
    })
    .catch((error) => {
        var registerAlert = document.getElementById("registerAlert");
        registerAlert.textContent = "Registration Failed! An error occurred while processing the request, You can use another email to register again.";
        registerAlert.classList.add("alert-danger");
        registerAlert.style.display = "block";
    });
});