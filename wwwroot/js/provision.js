// document.addEventListener("DOMContentLoaded", function () {
//     var token = sessionStorage.getItem("Token");

//     // Gửi yêu cầu GET để lấy danh sách dịch vụ (Provision)
//     axios.get("/api/provision/GetAll", {
//         headers: {
//             Authorization: "Bearer " + token,
//         },
//     })
//     .then((response) => {
//         if (response.data.success) {
//             // Xử lý thành công, hiển thị danh sách dịch vụ (Provision)
//             var provisions = response.data.data;

//             // Tạo chuỗi HTML cho các dòng của bảng
//             var rowsHtml = provisions.map(function (provision) {
//                 var posterHtml = provision.posterName ?
//                     `<img src="${provision.posterName}" alt="Poster" class="provision-image" />` :
//                     `<span class="no-poster">No poster available</span>`;

//                 return `
//                     <tr>
//                         <td>${provision.name}</td>
//                         <td>${provision.price.toString()}</td>
//                         <td>${provision.quantityInStock.toString()}</td>
//                         <td>${provision.categoryName}</td>
//                         <td>${posterHtml}</td>
//                         <td>
//                             <a href="/Provisions/GetProvisionById?id=${provision.id}" class="button secondary-button">Details</a>
//                             <a href="/Provisions/UpdateProvision?id=${provision.id}" class="button secondary-button">Update</a>
//                             <a href="/Provisions/DeleteProvision?id=${provision.id}" class="button secondary-button">Delete</a>
//                         </td>
//                     </tr>
//                 `;
//             });

//             // Thêm chuỗi HTML vào phần tử tbody của bảng
//             var tableBody = document.getElementById("provisionTableBody");
//             if (tableBody) {
//                 tableBody.innerHTML = rowsHtml.join("");
//                 tableBody.style.display = "table-row-group"; // Hiển thị tbody
//             } else {
//                 console.error("Không tìm thấy phần tử với ID 'provisionTableBody'.");
//             }
//         } else {
//             // Xử lý lỗi từ máy chủ, ví dụ: hiển thị thông báo lỗi
//             console.error("Lỗi máy chủ: " + response.data.message);
//         }
//     })
//     .catch((error) => {
//         // Xử lý lỗi kết nối hoặc lỗi xử lý trên máy chủ
//         console.error("Lỗi yêu cầu: " + error);
//     });
// });
