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
          alert(response.data.message);
        }
      })
      .catch(error => {
        if (error.response) {
            if (error.response.status === 400) {
                alert("Bad Request: " + error.response.data.message);
            } else if (error.response.status === 401) {
                alert("Unauthorized: " + error.response.data.message);
            } else if (error.response.status === 404) {
                alert("Not Found: " + error.response.data.message);
            } else {
                console.log("HTTP Error: " + error.response.status);
            }
        } else {
            console.log("Network Error or Unknown Error: " + error.message);
        }
    });
  });
  