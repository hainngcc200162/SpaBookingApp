var token = sessionStorage.getItem("Token");
if (!token) {
    window.location.href = "/error/AccessDenied.html";
}


let searchInfo = {
    searchBy: "",
    fromDate: "",
    toDate: ""
};

let currentPage = 0;
let totalPages = 1;

async function fetchData(pageIndex, searchBy = "", fromDate = "", toDate = "") {
    try {
        const queryParams = new URLSearchParams({
            pageIndex: pageIndex,
            searchBy: searchInfo.searchBy,
            fromDate: searchInfo.fromDate,
            toDate: searchInfo.toDate
        });

        const apiUrl = `/api/Booking/GetAll${queryParams ? `?${queryParams}` : ''}`;

        const response = await axios.get(apiUrl, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        if (!response || !response.data) {
            throw new Error('Error fetching data');
        }

        if (!response.data.success) {
            console.error('Error:', response.data.message);
            return;
        }
        const data = response.data;
        
        currentPage = data.pageInformation.pageIndex; 
        totalPages = data.pageInformation.totalPages;

        renderTable(data.data);
        updatePagination(currentPage, totalPages);

    } catch (error) {
        console.error('Fetch error:', error);
    }
}

function updatePagination(currentPage, totalPages) {
    const paginationDiv = document.getElementById("pagination");
    paginationDiv.innerHTML = "";

    const firstLink = document.createElement('a');
    firstLink.href = "#";
    firstLink.innerText = "First";
    firstLink.addEventListener("click", function () {
        fetchData(0);
    });
    paginationDiv.appendChild(firstLink);

    if (currentPage > 0) {
        const prevLink = document.createElement('a');
        prevLink.href = "#";
        prevLink.innerText = "Previous";
        prevLink.addEventListener("click", function () {
            fetchData(currentPage - 1);
        });
        paginationDiv.appendChild(prevLink);
    }

    const maxIntermediatePages = 5; 
    let startPage = Math.max(0, currentPage - Math.floor(maxIntermediatePages / 2));
    const endPage = Math.min(totalPages - 1, startPage + maxIntermediatePages - 1);

    if (endPage - startPage < maxIntermediatePages - 1) {
        startPage = Math.max(0, endPage - maxIntermediatePages + 1);
    }

    for (let i = startPage; i <= endPage; i++) {
        const pageIndex = i;
        const link = document.createElement('a');
        link.href = "#";
        link.innerText = i + 1;

        link.addEventListener("click", function () {
            fetchData(pageIndex);
        });

        paginationDiv.appendChild(link);
    }

    if (currentPage < totalPages - 1) {
        const nextLink = document.createElement('a');
        nextLink.href = "#";
        nextLink.innerText = "Next";
        nextLink.addEventListener("click", function () {
            fetchData(currentPage + 1);
        });
        paginationDiv.appendChild(nextLink);
    }

    const lastLink = document.createElement('a');
    lastLink.href = "#";
    lastLink.innerText = "Last";
    lastLink.addEventListener("click", function () {
        fetchData(totalPages - 1);
    });
    paginationDiv.appendChild(lastLink);
}

function renderTable(bookings) {
    const tableBody = document.querySelector('#bookingTable tbody');
    tableBody.innerHTML = '';

    for (const booking of bookings) {
        const row = document.createElement('tr');
        row.innerHTML = `
                <td>${booking.id}</td>
                <td>${booking.userFirstName} ${booking.userLastName}</td>
                <td>${booking.departmentName}</td>
                <td>${booking.staffName}</td>
                <td>${new Date(booking.startTime).toLocaleString()}</td>
                <td>${new Date(booking.endTime).toLocaleString()}</td>
                <td>${booking.status}</td>
                <td>
                    <a href="GetBookingById?id=${booking.id}" class="button secondary-button"><i class="fa-solid fa-eye fa-fade fa-xl"></i></a>&nbsp;
                    <a href="UpdateBooking?id=${booking.id}" class="button secondary-button"><i class="fa-solid fa-pen-to-square fa-beat fa-xl"></i></a>
                </td>
            `;
        tableBody.appendChild(row);
    }
}

function onSearchButtonClick() {
    searchInfo.searchBy = document.getElementById('searchInput').value;
    searchInfo.fromDate = document.getElementById('fromDate').value;
    searchInfo.toDate = document.getElementById('toDate').value;

    fetchData(0);
}

document.getElementById('searchButton').addEventListener('click', onSearchButtonClick);

window.addEventListener('load', () => {
    const storedSearchInfo = sessionStorage.getItem("searchInfo");
    if (storedSearchInfo) {
        searchInfo = JSON.parse(storedSearchInfo);
        document.getElementById('searchInput').value = searchInfo.searchBy;
        document.getElementById('fromDate').value = searchInfo.fromDate;
        document.getElementById('toDate').value = searchInfo.toDate;
    }
    fetchData(0);
});
