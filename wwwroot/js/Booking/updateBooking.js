var token = sessionStorage.getItem("Token");
if (!token) {
    window.location.href = "/error/AccessDenied.html";
}

// Fetch staff, department, provision data and populate dropdowns
async function fetchAndPopulateData() {
    try {
        // Fetch staff data
        const largePageSize = 10000;
        const staffResponse = await axios.get(`/api/Staff/GetAll?pageIndex=0&pageSize=${largePageSize}`, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });
        const staffSelect = document.getElementById('staffSelect');
        staffResponse.data.data.forEach(staff => {
            const option = document.createElement('option');
            option.value = staff.id;
            option.textContent = staff.name;
            staffSelect.appendChild(option);
        });

        // Fetch department data
        const departmentResponse = await axios.get(`/api/Department/GetAll?pageIndex=0&pageSize=${largePageSize}`, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });
        const departmentSelect = document.getElementById('departmentSelect');
        departmentResponse.data.data.forEach(department => {
            const option = document.createElement('option');
            option.value = department.id;
            option.textContent = department.name;
            departmentSelect.appendChild(option);
        });

        const provisionCheckboxes = document.getElementById('provisionCheckboxes');

        // Fetch provision data
        const provisionResponse = await axios.get('/api/Provision/GetAll', {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });
        
        provisionResponse.data.data.forEach(provision => {
            const checkboxLabel = document.createElement('label');
            checkboxLabel.classList.add('checkbox-label');

            const checkbox = document.createElement('input');
            checkbox.type = 'checkbox';
            checkbox.value = provision.id;
            checkbox.name = 'selectedProvisions';
            checkbox.classList.add('checkbox');

            const provisionName = document.createTextNode(provision.name);

            checkboxLabel.appendChild(checkbox);
            checkboxLabel.appendChild(provisionName);
            provisionCheckboxes.appendChild(checkboxLabel);
        });

        fetchBookingData();
    } catch (error) {
        console.error('Fetch error:', error);
    }
}

// Fetch booking data to prepopulate form fields
async function fetchBookingData() {
    try {
        const urlParams = new URLSearchParams(window.location.search);
        const bookingId = urlParams.get('id');
        if (!bookingId) {
            console.error('Booking ID not provided in URL.');
            return;
        }

        const response = await axios.get(`/api/Booking/${bookingId}`, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });
        if (response.status !== 200) {
            throw new Error('Network response was not ok');
        }

        const booking = response.data.data;

        const staffSelect = document.getElementById('staffSelect');
        staffSelect.value = booking.staffId;

        const departmentSelect = document.getElementById('departmentSelect');
        departmentSelect.value = booking.departmentId;

        const provisionCheckboxes = document.getElementsByClassName('checkbox');
        const selectedProvisionIds = booking.provisions.map(provision => provision.id);
        Array.from(provisionCheckboxes).forEach(checkbox => {
            checkbox.checked = selectedProvisionIds.includes(parseInt(checkbox.value));
        });

        const startTimeInput = document.getElementById('startTime');
        startTimeInput.value = new Date(booking.startTime).toISOString().substring(0, 16);

        const statusSelect = document.getElementById('status');
        statusSelect.value = booking.status;

        const noteInput = document.getElementById('note');
        noteInput.value = booking.note;
    } catch (error) {
        console.error('Fetch error:', error);
    }
}

// Call the API to update a booking
function updateBooking() {
    const staffSelect = document.getElementById('staffSelect');
    const departmentSelect = document.getElementById('departmentSelect');
    const provisionCheckboxes = document.getElementsByClassName('checkbox');
    const startTimeInput = document.getElementById('startTime');
    const statusSelect = document.getElementById('status');
    const noteInput = document.getElementById('note');

    const selectedStaffId = staffSelect.value;
    const selectedDepartmentId = departmentSelect.value;
    const selectedProvisionIds = Array.from(provisionCheckboxes)
        .filter(checkbox => checkbox.checked)
        .map(checkbox => parseInt(checkbox.value));
    const startTime = startTimeInput.value;
    const selectedStatus = statusSelect.value;
    const note = noteInput.value;

    console.log('Selected Staff:', selectedStaffId);
    console.log('Selected Department:', selectedDepartmentId);
    console.log('Selected Provisions:', selectedProvisionIds);
    console.log('Start Time:', startTime);
    console.log('Status:', selectedStatus);
    console.log('Note:', note);

    const requestBody = {
        provisionIds: selectedProvisionIds,
        departmentId: selectedDepartmentId,
        staffId: selectedStaffId,
        startTime: startTime,
        status: selectedStatus,
        note: note
    };

    const urlParams = new URLSearchParams(window.location.search);
    const bookingId = urlParams.get('id');

    axios.put(`/api/Booking/${bookingId}`, requestBody, {
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        }
    })
    .then(response => {
        // Handle success, e.g. show success message to user
        console.log('Booking updated:', response.data);
        alert("Booking updated successfully");
        window.location.href = '/Bookings/Index';
    })
    .catch(error => {
        // Handle error, e.g. show error message to user
        console.error('Error updating booking:', error);
    });
}

// Populate dropdowns and set up event listener
window.addEventListener('load', () => {
    fetchAndPopulateData();

    document.getElementById('updateButton').addEventListener('click', updateBooking);
});
