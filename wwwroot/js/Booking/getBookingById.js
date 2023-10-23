var token = sessionStorage.getItem("Token");
if (!token) {
    window.location.href = "/error/AccessDenied.html";
}

const apiService = axios.create({
    baseURL: '/api',
    timeout: 5000,
    headers: {
        'Authorization': `Bearer ${token}`
    }
});

async function fetchBookingById(bookingId) {
    try {
        const response = await apiService.get(`/Booking/${bookingId}`);
        if (response.status !== 200) {
            throw new Error('Network response was not ok');
        }
        const data = response.data;

        renderBooking(data);
        console.log(data);
    } catch (error) {
        console.error('Fetch error:', error);
    }
}

function renderBooking(booking) {
    const bookingDetailDiv = document.querySelector('.booking-detail');
    const provisionsDiv = document.querySelector('.provisions');

    const provisionsHTML = booking.data.provisions
        ? booking.data.provisions.map(provision => `
            <div class="provision">
                <p><strong>Provision ID:</strong> ${provision.id}</p>
                <p><strong>Provision Name:</strong> ${provision.name}</p> 
                <p><strong>Duration Minutes:</strong> ${provision.durationMinutes} minutes</p>
                <p><strong>Number Of Executions:</strong> ${provision.numberOfExecutions}</p>
                <p><strong>Description:</strong> ${provision.description}</p>
                <p><strong>Price: </strong>$ ${provision.price}.00</p>
                <p style="font-size: 16px; color: red;" ><strong>Remaining Executions:</strong> ${provision.remainingExecutions}</p>
            </div>
            <hr>
        `).join('')
        : '<p>No provisions available</p>';

        const totalProvisionPrice = booking.data.provisions
        ? booking.data.provisions.reduce((total, provision) => {
            // Check if staff ID is not 1, add $50
            if (booking.data.staffId !== 1) {
                total += provision.price + 50;
            } else {
                total += provision.price;
            }
            return total;
        }, 0)
        : 0;
        const staffNote = booking.data.staffId !== 1 ? '(50.00$ for Beauty Master)' : '';

        provisionsDiv.innerHTML = `
            <h2>Provisions:</h2>
            ${provisionsHTML}
            <p><strong>Total Provision Price: </strong>$ ${totalProvisionPrice}.00 ${staffNote}</p>
            <p class="red-message">*Please pay at the desk</p>
        `;

    bookingDetailDiv.innerHTML = `
        <p><strong>Booking ID:</strong> ${booking.data.id}</p>
        <p><strong>User Name:</strong> ${booking.data.userFirstName} ${booking.data.userLastName}</p>
        <p><strong>User Email:</strong> ${booking.data.userEmail}</p>
        <p><strong>User Phone Number:</strong> ${booking.data.userPhoneNumber}</p>
        <p><strong>Department Name:</strong> ${booking.data.departmentName}</p>
        <p><strong>Staff Name:</strong> ${booking.data.staffName}</p>
        <p><strong>Start Time:</strong> ${new Date(booking.data.startTime).toLocaleString()}</p>
        <p><strong>End Time:</strong> ${new Date(booking.data.endTime).toLocaleString()}</p>
        <p><strong>Status:</strong> ${booking.data.status}</p>
        <p><strong>Note:</strong> ${booking.data.note}</p>
    `;

    
}


window.addEventListener('load', () => {
    const urlParams = new URLSearchParams(window.location.search);
    const bookingId = urlParams.get('id');

    if (bookingId) {
        fetchBookingById(bookingId);
    } else {
        console.error('Booking ID not provided in URL.');
    }
});

document.getElementById('backButton').addEventListener('click', function () {
    history.back(); 
});