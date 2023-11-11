var alertDisplayed = false;
var alertDisplayedDate = false;
var alertDisplayedDay = false;
var alertDisplayedDepartment = false;
var alertDisplayedProvision = false;
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

        const provisionCheckboxes = document.getElementById('provisionCheckboxes');

        provisionResponse.data.data.forEach(provision => {
            const checkboxLabel = document.createElement('label');
            checkboxLabel.classList.add('checkbox-label');

            const checkbox = document.createElement('input');
            checkbox.type = 'checkbox';
            checkbox.value = provision.id;
            checkbox.name = 'selectedProvisions';
            checkbox.classList.add('checkbox');
            checkbox.required = true;

            const provisionName = document.createTextNode(provision.name);

            checkboxLabel.appendChild(checkbox);
            checkboxLabel.appendChild(provisionName);
            provisionCheckboxes.appendChild(checkboxLabel);
        });

    } catch (error) {
        console.error('Fetch error:', error);
    }
}
function hideAlerts() {
    var alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        alert.style.display = 'none';
    });

    // Reset alert flags
    alertDisplayed = false;
    alertDisplayedDate = false;
    alertDisplayedDepartment = false;
    alertDisplayedProvision = false;
}

// Call the API to create a booking
function createBooking() {
    hideAlerts();
    const staffSelect = document.getElementById('staffSelect');
    const departmentSelect = document.getElementById('departmentSelect');
    const provisionCheckboxes = document.getElementsByClassName('checkbox');
    const startTimeInput = document.getElementById('startTime');
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
    console.log('Status:', selectedStatus);
    console.log('Note:', note);

    // Convert the start time input to a Date object
    const startTimeDate = new Date(startTime);

    // Check if the start time is in the past
    if (startTimeDate < new Date()) {
        if (!alertDisplayedDate) {
            var parentElement = document.getElementById("AddBooking");
            // Tạo alert và sử dụng nội dung từ response
            var alertElement = document.createElement("div");
            alertElement.className = "mb-3 alert alert-danger";
            alertElement.setAttribute("role", "alert");
            alertElement.textContent = "Cannot book in the past. Please select a future date.";
            // Thêm alert vào form
            parentElement.insertBefore(alertElement, parentElement.firstChild);
             
            alertDisplayedDate = true;
        }
        return; // Stop further execution
    }

    if (!selectedDepartmentId) {
        if (!alertDisplayedDepartment) {
            var parentElement = document.getElementById("AddBooking");
            // Tạo alert và sử dụng nội dung từ response
            var alertElement = document.createElement("div");
            alertElement.className = "mb-3 alert alert-danger";
            alertElement.setAttribute("role", "alert");
            alertElement.textContent = "Please select a department.";
            // Thêm alert vào form
            parentElement.insertBefore(alertElement, parentElement.firstChild);
             
            alertDisplayedDepartment = true;
        }
        return;
    }

    if (selectedProvisionIds.length === 0) {
        if (!alertDisplayedProvision) {
            var parentElement = document.getElementById("AddBooking");
            // Tạo alert và sử dụng nội dung từ response
            var alertElement = document.createElement("div");
            alertElement.className = "mb-3 alert alert-danger";
            alertElement.setAttribute("role", "alert");
            alertElement.textContent = "Please select at least one provision.";
            // Thêm alert vào form
            parentElement.insertBefore(alertElement, parentElement.firstChild);
             
            alertDisplayedProvision = true;
        }
        return;
    }

    // Kiểm tra nếu không chọn thời gian
    if (!startTime || isNaN(startTimeDate.getTime())) {
        if (!alertDisplayedDate) {
            var parentElement = document.getElementById("AddBooking");
            var alertElement = document.createElement("div");
            alertElement.className = "mb-3 alert alert-danger";
            alertElement.setAttribute("role", "alert");
            alertElement.textContent = "Please select a valid future date and time.";
            parentElement.insertBefore(alertElement, parentElement.firstChild);
             
            alertDisplayedDate = true;
        }
        return;
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
            sendEmailNotification(response.data);
            console.log('Booking created:', response.data);
        })
        .catch(error => {
            if (error.response) {
                if (!alertDisplayed) {
                    var parentElement = document.getElementById("AddBooking");
                    // Tạo alert và sử dụng nội dung từ response
                    var alertElement = document.createElement("div");
                    alertElement.className = "mb-3 alert alert-danger";
                    alertElement.setAttribute("role", "alert");
                    alertElement.textContent = error.response.data.message;
                    // Thêm alert vào form
                    parentElement.insertBefore(alertElement, parentElement.firstChild);
                     
                    alertDisplayed = true;
                }
            } else {
                console.log("Network Error or Unknown Error: " + error.message);
            }
        });
}
window.addEventListener('load', async () => {
    await fetchAndPopulateData();

    document.getElementById('createButton').addEventListener('click', createBooking);
});
