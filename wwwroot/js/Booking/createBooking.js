var token = sessionStorage.getItem("Token");
if (!token) {
    window.location.href = "/error/AccessDenied.html";
}

// Fetch staff, department, and provision data from respective APIs and populate dropdowns
async function fetchAndPopulateData() {
    try {
        // Fetch staff data
        const largePageSize = 10000; // Choose a suitable large value
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

        // Fetch provision data
        const provisionResponse = await axios.get('/api/Provision/GetAll', {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });
        // Inside the fetchAndPopulateData function

        // Inside the fetchAndPopulateData function

        const provisionCheckboxes = document.getElementById('provisionCheckboxes');

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

    } catch (error) {
        console.error('Fetch error:', error);
    }
}

// Call the API to create a booking
function createBooking() {
    const staffSelect = document.getElementById('staffSelect');
    const departmentSelect = document.getElementById('departmentSelect');
    const provisionCheckboxes = document.getElementsByClassName('checkbox');
    const startTimeInput = document.getElementById('startTime');
    // const statusSelect = document.getElementById('status');
    const noteInput = document.getElementById('note');

    const selectedStaffId = staffSelect.value;
    const selectedDepartmentId = departmentSelect.value;
    const selectedProvisionIds = Array.from(provisionCheckboxes)
        .filter(checkbox => checkbox.checked)
        .map(checkbox => parseInt(checkbox.value));
            const startTime = startTimeInput.value;
    const selectedStatus = "Waiting";
    const note = noteInput.value;

    console.log('Selected Staff:', selectedStaffId);
    console.log('Selected Department:', selectedDepartmentId);
    console.log('Selected Provisions:', selectedProvisionIds);
    console.log('Start Time:', startTime);
    // console.log('Status:', selectedStatus);
    console.log('Note:', note);

    // Convert the start time input to a Date object
    const startTimeDate = new Date(startTime);

    // Check if the start time is in the past
    if (startTimeDate < new Date()) {
        alert('Cannot book in the past. Please select a future date.');
        return; // Stop further execution
    }

    const requestBody = {
        provisionIds: selectedProvisionIds,
        departmentId: selectedDepartmentId,
        staffId: selectedStaffId,
        startTime: startTime,
        status: selectedStatus,
        note: note
    };

    axios.post('/api/Booking', requestBody, {
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        }
        
    }
    )
    .then(response => {
        // Redirect to Home/Index page
        window.location.href = '/Home/Index';

        // Show success message using an alert
        alert('Booking created successfully.');
        console.log('Booking created:', response.data);
    })
    .catch(error => {
        // Handle error, e.g. show error message to user
        console.error('Error creating booking:', error);
    });
}
window.addEventListener('load', async () => {
    await fetchAndPopulateData(); // Chờ fetchAndPopulateData hoàn thành

    // Sau khi fetchAndPopulateData hoàn thành, gắn sự kiện click
    document.getElementById('createButton').addEventListener('click', createBooking);
});
