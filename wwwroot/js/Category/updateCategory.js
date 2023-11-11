document.querySelector("form").addEventListener("submit", function (event) {
  event.preventDefault();

  var token = sessionStorage.getItem("Token");

  if (!token) {
    window.location.href = "/error/AccessDenied.html";
    return;
  }

  var categoryId = document.getElementById("Category_Id").value;
  var categoryName = document.getElementById("Category_Name").value;

  var data = {
    Id: categoryId,
    Name: categoryName,
  };

  axios.put(`/api/Category/${categoryId}`, data, {
    headers: {
      Authorization: "Bearer " + token,
    },
  })
    .then((response) => {
      if (response.data.success) {
        alert("Category updated successfully!");
        window.location.href = "/Categories/Index";
      } else {
        console.log("Lỗi: " + response.data.message);
        var parentElement = document.getElementById("form");
        // Tạo alert và sử dụng nội dung từ response
        var alertElement = document.createElement("div");
        alertElement.className = "mb-3 alert alert-danger";
        alertElement.setAttribute("role", "alert");
        alertElement.textContent = "Error: " + error.response.data.message;
        // Thêm alert vào form
        parentElement.insertBefore(alertElement, parentElement.firstChild);
        alertElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
      }
    })
    .catch(error => {
      if (error.response) {
        var parentElement = document.querySelector("form");

        var alertElement = document.createElement("div");
        alertElement.className = "mb-3 alert alert-danger";
        alertElement.setAttribute("role", "alert");
        alertElement.textContent = error.response.data.message;

        parentElement.insertBefore(alertElement, parentElement.firstChild);
      } else {
        console.log("Network Error or Unknown Error: " + error.message);
      }
    });
});
