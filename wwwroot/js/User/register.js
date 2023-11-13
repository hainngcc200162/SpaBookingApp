document.getElementById("registerForm").addEventListener("submit", function (event) {
    event.preventDefault();

    var firstName = document.getElementById("firstName").value;
    var lastName = document.getElementById("lastName").value;
    var email = document.getElementById("email").value;
    var phoneNumber = document.getElementById("phoneNumber").value;
    var password = document.getElementById("password").value;
    var confirmPassword = document.getElementById("confirmPassword").value;

    // Email validation regular expression
    var emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;

    // Phone number validation regular expression (assuming 10 digits for a basic example)
    var phonePattern = /^\d{10}$/;

    // Password validation regular expression (at least one lowercase, one uppercase, one special character, one digit, and minimum length of 8)
    var passwordPattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;

    if (password !== confirmPassword) {
        var registerAlert = document.getElementById("registerAlert");
        registerAlert.textContent = "Passwords do not match.";
        registerAlert.classList.add("alert-danger");
        registerAlert.style.display = "block";
        return;
    }

    if (!emailPattern.test(email)) {
        var registerAlert = document.getElementById("registerAlert");
        registerAlert.textContent = "Invalid email format.";
        registerAlert.classList.add("alert-danger");
        registerAlert.style.display = "block";
        return;
    }

    if (!phonePattern.test(phoneNumber)) {
        var registerAlert = document.getElementById("registerAlert");
        registerAlert.textContent = "Invalid phone number format (should be 10 digits).";
        registerAlert.classList.add("alert-danger");
        registerAlert.style.display = "block";
        return;
    }

    if (!passwordPattern.test(password)) {
        var registerAlert = document.getElementById("registerAlert");
        registerAlert.textContent = "Password must have at least one lowercase letter, one uppercase letter, one special character, one digit, and a minimum length of 8.";
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
            if (error.response) {
                var registerAlert = document.getElementById("registerAlert");
                registerAlert.textContent = error.response.data.message;
                registerAlert.classList.add("alert-danger");
                registerAlert.style.display = "block";

            } else {
                console.log("Network Error or Unknown Error: " + error.message);
            }
        });
});