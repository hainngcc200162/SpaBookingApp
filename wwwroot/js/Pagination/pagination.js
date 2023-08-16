function updatePagination(currentPage, totalPages) {
    const paginationDiv = document.getElementById("pagination");
    paginationDiv.innerHTML = "";

    // Tạo liên kết "First"
    const firstLink = document.createElement('a');
    firstLink.href = "#";
    firstLink.innerText = "First";
    firstLink.addEventListener("click", function () {
        fetchData(0);
    });
    paginationDiv.appendChild(firstLink);

    // Tạo liên kết "Previous"
    if (currentPage > 0) {
        const prevLink = document.createElement('a');
        prevLink.href = "#";
        prevLink.innerText = "Previous";
        prevLink.addEventListener("click", function () {
            fetchData(currentPage - 1);
        });
        paginationDiv.appendChild(prevLink);
    }

    // Tạo liên kết cho các trang
    for (let i = 0; i < totalPages; i++) {
        const pageIndex = i;
        const link = document.createElement('a');
        link.href = "#";
        link.innerText = i + 1;

        link.addEventListener("click", function () {
            fetchData(pageIndex);
        });

        paginationDiv.appendChild(link);
    }

    // Tạo liên kết "Next"
    if (currentPage < totalPages - 1) {
        const nextLink = document.createElement('a');
        nextLink.href = "#";
        nextLink.innerText = "Next";
        nextLink.addEventListener("click", function () {
            fetchData(currentPage + 1);
        });
        paginationDiv.appendChild(nextLink);
    }

    // Tạo liên kết "Last"
    const lastLink = document.createElement('a');
    lastLink.href = "#";
    lastLink.innerText = "Last";
    lastLink.addEventListener("click", function () {
        fetchData(totalPages - 1);
    });
    paginationDiv.appendChild(lastLink);
}
