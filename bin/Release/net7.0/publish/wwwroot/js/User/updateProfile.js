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

  // Populate form fields with existing user profile data
  axios.get("/auth/profile")
      .then(function (response) {
          console.log("Profile API response:", response);

          var result = response.data;
          if (result.success) {
              console.log("Profile data:", result.data);

              var userProfile = result.data;
              document.getElementById("firstName").value = userProfile.firstName;
              document.getElementById("lastName").value = userProfile.lastName;
              document.getElementById("email").value = userProfile.email;
              document.getElementById("phoneNumber").value = userProfile.phoneNumber;
          } else {
              console.log("Profile API response indicates failure, redirecting to access denied");
              window.location.href = "/error/AccessDenied.html";
          }
      })
      .catch(function (error) {
          console.error("An error occurred while making the request:", error);
          window.location.href = "/error/AccessDenied.html";
      });

    // Handle form submission
    var updateProfileForm = document.getElementById("updateProfileForm");
    var passwordModal = document.getElementById("passwordModal");
    var passwordInput = document.getElementById("passwordInput");
    var passwordSubmit = document.getElementById("passwordSubmit");
    var closeModal = document.getElementById("closeModal");

    updateProfileForm.addEventListener("submit", function (event) {
      event.preventDefault();

      // Show the password modal
      passwordModal.style.display = "block";
  });

  passwordSubmit.addEventListener("click", function () {
      var password = passwordInput.value;

      if (password === "") {
          alert("Please enter your password.");
          return;
      }

      // Close the modal
      passwordModal.style.display = "none";

      var updatedProfile = {
          FirstName: document.getElementById("firstName").value,
          LastName: document.getElementById("lastName").value,
          Email: document.getElementById("email").value,
          PhoneNumber: document.getElementById("phoneNumber").value
      };

      axios.put("/auth/updateprofile?password=" + encodeURIComponent(password), updatedProfile)
          .then(function (response) {
              console.log("Update Profile API response:", response);

              var result = response.data;
              if (result.success) {
                  console.log("Profile updated successfully");
                  alert("Profile updated successfully!");
                  window.location.href = "/UserManagement/Profile";
              } else {
                  console.log("Update Profile API response indicates failure");
                  alert("Failed to update profile. Please try again.");
              }
          })
          .catch(function (error) {
              console.error("An error occurred while updating the profile:", error);
              alert("An error occurred while updating the profile. Please try again.");
          });
  });

  closeModal.addEventListener("click", function () {
      // Close the modal without taking any action
      passwordModal.style.display = "none";
  });
});
