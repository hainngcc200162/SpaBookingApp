document.addEventListener("DOMContentLoaded", function () {
    var token = sessionStorage.getItem("Token");

    // Gửi yêu cầu GET để lấy danh sách sản phẩm
    axios.get("/api/product/GetAll", {
        headers: {
            Authorization: "Bearer " + token,
        },
    })
    .then((response) => {
        if (response.data.success) {
            // Xử lý thành công, hiển thị danh sách sản phẩm
            var products = response.data.data;

            // Tạo chuỗi HTML cho các dòng của bảng
            var rowsHtml = products.map(function (product) {
                var posterHtml = product.posterName ?
                    `<img src="${product.posterName}" alt="Poster" class="product-image" />` :
                    `<span class="no-poster">No poster available</span>`;

                return `
                    <tr>
                        <td>${product.name}</td>
                        <td>${product.price.toString()}</td>
                        <td>${product.quantityInStock.toString()}</td>
                        <td>${product.categoryName}</td>
                        <td>${posterHtml}</td>
                        <td>
                            <a href="/Products/GetProductById?id=${product.id}" class="button secondary-button">Details</a>
                            <a href="/Products/UpdateProduct?id=${product.id}" class="button secondary-button">Update</a>
                            <a href="/Products/DeleteProduct?id=${product.id}" class="button secondary-button">Delete</a>
                        </td>
                    </tr>
                `;
            });

            // Thêm chuỗi HTML vào phần tử tbody của bảng
            var tableBody = document.getElementById("productTableBody");
            if (tableBody) {
                tableBody.innerHTML = rowsHtml.join("");
                tableBody.style.display = "table-row-group"; // Hiển thị tbody
            } else {
                console.error("Không tìm thấy phần tử với ID 'productTableBody'.");
            }
        } else {
            // Xử lý lỗi từ máy chủ, ví dụ: hiển thị thông báo lỗi
            console.error("Lỗi máy chủ: " + response.data.message);
        }
    })
    .catch((error) => {
        // Xử lý lỗi kết nối hoặc lỗi xử lý trên máy chủ
        console.error("Lỗi yêu cầu: " + error);
    });
});