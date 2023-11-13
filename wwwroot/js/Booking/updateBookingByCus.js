var alertDisplayed = false;
var alertDisplayedDate = false;
var alertDisplayedDay = false;
var alertDisplayedDepartment = false;
var alertDisplayedProvision = false;
var alertDisplayedExe = false;
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
            checkbox.required = true;

            const provisionName = document.createTextNode(provision.name);

            const remainingExecutionsInput = document.createElement('input');
            remainingExecutionsInput.type = 'number'; // Input type number
            remainingExecutionsInput.className = 'form-control'; // Input type number
            remainingExecutionsInput.id = `remainingExecutions_${provision.id}`; // Unique ID for each input

            remainingExecutionsInput.name = 'remainingExecutions';
            remainingExecutionsInput.placeholder = 'Remaining Executions';

            remainingExecutionsInput.value = provision.numberOfExecutions || 0;

            remainingExecutionsInput.disabled = true;

            checkboxLabel.appendChild(checkbox);
            checkboxLabel.appendChild(provisionName);
            checkboxLabel.appendChild(remainingExecutionsInput); // Add remainingExecutions input
            provisionCheckboxes.appendChild(checkboxLabel);
        });

        fetchBookingData();
    } catch (error) {
        console.error('Fetch error:', error);
    }
}

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
            const provisionId = parseInt(checkbox.value);
            const remainingExecutionsInput = document.getElementById(`remainingExecutions_${provisionId}`);

            // Find the corresponding provision in the booking
            const provisionInBooking = booking.provisions.find(provision => provision.id === provisionId);

            // Set the value of the remainingExecutions input from the booking data
            if (provisionInBooking) {
                remainingExecutionsInput.value = provisionInBooking.remainingExecutions;
            }

            const label = document.createElement('label');
            label.textContent = 'Remaining Executions';
            label.htmlFor = `remainingExecutions_${provisionId}`;
            label.classList.add('label2');
            remainingExecutionsInput.parentNode.insertBefore(label, remainingExecutionsInput);
        });

        // Chuyển đổi giá trị startTime sang định dạng "yyyy-MM-ddThh:mm"
        const startTimeInput = document.getElementById('startTime');
        const startTimeServer = new Date(booking.startTime);

        // Cộng thêm 1 tiếng
        startTimeServer.setHours(startTimeServer.getHours() + 1);

        const year = startTimeServer.getFullYear();
        const month = (startTimeServer.getMonth() + 1).toString().padStart(2, '0');
        const day = startTimeServer.getDate().toString().padStart(2, '0');
        const hours = startTimeServer.getHours().toString().padStart(2, '0');
        const minutes = startTimeServer.getMinutes().toString().padStart(2, '0');
        const formattedStartTime = `${year}-${month}-${day}T${hours}:${minutes}`;

        // Set giá trị startTimeInput với giá trị đã được chuyển đổi
        startTimeInput.value = formattedStartTime;

        const noteInput = document.getElementById('note');
        noteInput.value = booking.note;

        // const statusSelect = document.getElementById('status');
        // statusSelect.value = booking.status;
    } catch (error) {
        console.error('Fetch error:', error);
    }
}


function hideAlerts() {
    var alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        alert.style.display = 'none';
    });

    alertDisplayed = false;
    alertDisplayedDate = false;
    alertDisplayedDay = false;
    alertDisplayedDepartment = false;
    alertDisplayedProvision = false;
    alertDisplayedExe = false;
}

// Call the API to update a booking for customer
function updateBookingForCustomer() {
    hideAlerts();
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
    const endTime = '2023-08-26T05:04:09.579Z';
    const selectedStatus = "Waiting";
    const note = noteInput.value;

    // Create an array to store provisionRemainingExecutions
    const provisionRemainingExecutions = [];

    // Iterate over selectedProvisionIds to build provisionRemainingExecutions
    selectedProvisionIds.forEach(provisionId => {
        // Get the corresponding remainingExecutions input element
        const remainingExecutionsInput = document.getElementById(`remainingExecutions_${provisionId}`);
        const remainingExecutions = parseInt(remainingExecutionsInput.value);

        // Create an object for provisionRemainingExecutions
        const provisionRemainingExecution = {
            provisionId: provisionId,
            remainingExecutions: remainingExecutions
        };

        // Add it to the array
        provisionRemainingExecutions.push(provisionRemainingExecution);
    });

    console.log('Selected Staff:', selectedStaffId);
    console.log('Selected Department:', selectedDepartmentId);
    console.log('Selected Provisions:', selectedProvisionIds);
    console.log('Start Time:', startTime);

    console.log('Note:', note);

    const startTimeDate = new Date(startTime);

    // Check if the start time is in the past
    if (startTimeDate < new Date()) {
        if (!alertDisplayedDate) {
             parentElement = document.getElementById("updateForm");
            // Tạo alert và sử dụng nội dung từ response
             alertElement = document.createElement("div");
            alertElement.className = "mb-3 alert alert-danger";
            alertElement.setAttribute("role", "alert");
            alertElement.textContent = "Cannot book in the past. Please select a future date.";
            // Thêm alert vào form
            parentElement.insertBefore(alertElement, parentElement.firstChild);
            window.scrollTo({ top: 0, behavior: 'smooth' });
            alertDisplayedDate = true;
        }
        return; // Stop further execution
    }

    const startHour = startTimeDate.getHours();
    if (startHour >= 20 || startHour < 7) {
        if (!alertDisplayedExe) {
            var parentElement = document.getElementById("updateForm");
            var alertElement = document.createElement("div");
            alertElement.className = "mb-3 alert alert-danger";
            alertElement.setAttribute("role", "alert");
            alertElement.textContent = "Cannot book between 8 PM and 7 AM the next day.";
            parentElement.insertBefore(alertElement, parentElement.firstChild);
            window.scrollTo({ top: 0, behavior: 'smooth' });
            alertDisplayedExe = true;
        }
        return; // Stop further execution
    }

    if (!selectedDepartmentId) {
        if (!alertDisplayedDepartment) {
             parentElement = document.getElementById("updateForm");
            // Tạo alert và sử dụng nội dung từ response
            var alertElement = document.createElement("div");
            alertElement.className = "mb-3 alert alert-danger";
            alertElement.setAttribute("role", "alert");
            alertElement.textContent = "Please select a department.";
            // Thêm alert vào form
            parentElement.insertBefore(alertElement, parentElement.firstChild);
            window.scrollTo({ top: 0, behavior: 'smooth' });
            alertDisplayedDepartment = true;
        }
        return;
    }

    if (selectedProvisionIds.length === 0) {
        if (!alertDisplayedProvision) {
            var parentElement = document.getElementById("updateForm");
            // Tạo alert và sử dụng nội dung từ response
            var alertElement = document.createElement("div");
            alertElement.className = "mb-3 alert alert-danger";
            alertElement.setAttribute("role", "alert");
            alertElement.textContent = "Please select at least one provision.";
            // Thêm alert vào form
            parentElement.insertBefore(alertElement, parentElement.firstChild);
            window.scrollTo({ top: 0, behavior: 'smooth' });
            alertDisplayedProvision = true;
        }
        return;
    }

    

    // Kiểm tra nếu không chọn thời gian
    if (!startTime || isNaN(startTimeDate.getTime())) {
        if (!alertDisplayedDate) {
            var parentElement = document.getElementById("updateForm");
            var alertElement = document.createElement("div");
            alertElement.className = "mb-3 alert alert-danger";
            alertElement.setAttribute("role", "alert");
            alertElement.textContent = "Please select a valid future date and time.";
            parentElement.insertBefore(alertElement, parentElement.firstChild);
            window.scrollTo({ top: 0, behavior: 'smooth' });
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
        note: note,
        provisionRemainingExecutions: provisionRemainingExecutions // Include provisionRemainingExecutions in the request body
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
            alert("Booking Updated Successfully");
            window.location.href = '/UserManagement/IndexCusBooking';
        })
        .catch(error => {
            // Xử lý lỗi khi yêu cầu Fetch không thành công
            if (error.response) {
                if (!alertDisplayed) {
                    var parentElement = document.getElementById("updateForm");
                    var alertElement = document.createElement("div");
                    alertElement.className = "mb-3 alert alert-danger";
                    alertElement.setAttribute("role", "alert");
                    alertElement.textContent = error.response.data.message;
                    console.log("1"+error.response.data.message);

                    parentElement.insertBefore(alertElement, parentElement.firstChild);
                    window.scrollTo({ top: 0, behavior: 'smooth' });

                    alertDisplayed = true;
                }
            } else {
                // Xử lý lỗi mạng hoặc lỗi không xác định
                console.log("Network Error or Unknown Error: " + error.message);
            }
        });
}

// Function to update booking status to "Cancelled"
function cancelBooking() {
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
    const endTime = '2023-08-26T05:04:09.579Z';
    const selectedStatus = "Cancelled";
    const note = noteInput.value;

    // Create an array to store provisionRemainingExecutions
    const provisionRemainingExecutions = [];

    // Iterate over selectedProvisionIds to build provisionRemainingExecutions
    selectedProvisionIds.forEach(provisionId => {
        // Get the corresponding remainingExecutions input element
        const remainingExecutionsInput = document.getElementById(`remainingExecutions_${provisionId}`);
        const remainingExecutions = parseInt(remainingExecutionsInput.value);

        // Create an object for provisionRemainingExecutions
        const provisionRemainingExecution = {
            provisionId: provisionId,
            remainingExecutions: remainingExecutions
        };

        // Add it to the array
        provisionRemainingExecutions.push(provisionRemainingExecution);
    });

    const requestBody = {
        provisionIds: selectedProvisionIds,
        departmentId: selectedDepartmentId,
        staffId: selectedStaffId,
        startTime: startTime,
        status: selectedStatus,
        note: note,
        provisionRemainingExecutions: provisionRemainingExecutions // Include provisionRemainingExecutions in the request body
    };

    const urlParams = new URLSearchParams(window.location.search);
    const bookingId = urlParams.get('id');

    console.log('Selected Staff:', selectedStaffId);
    console.log('Selected Department:', selectedDepartmentId);
    console.log('Selected Provisions:', selectedProvisionIds);
    console.log('Start Time:', startTime);
    console.log('Status', selectedStatus);
    // Use window.confirm() to confirm the booking cancellation
    const confirmation = window.confirm("Are you sure you want to cancel this booking?");

    if (confirmation) {
        // User confirmed to cancel the booking
        axios.put(`/api/Booking/${bookingId}`, requestBody, {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        })
            .then(response => {
                // Handle success, e.g., display a success message to the user
                console.log('Booking updated:', response.data);
                alert("Booking successfully canceled.");
                window.location.href = "/UserManagement/IndexCusBooking";
            })
            .catch(error => {
                // Xử lý lỗi khi yêu cầu Fetch không thành công
                if (error.response) {
                    if (!alertDisplayed) {
                        var parentElement = document.getElementById("updateForm");
                        var alertElement = document.createElement("div");
                        alertElement.className = "mb-3 alert alert-danger";
                        alertElement.setAttribute("role", "alert");
                        alertElement.textContent = error.response.data.message;
                        console.log("1"+error.response.data.message);
    
                        parentElement.insertBefore(alertElement, parentElement.firstChild);
                        alertDisplayed = true;
                    }
                } else {
                    // Xử lý lỗi mạng hoặc lỗi không xác định
                    console.log("Network Error or Unknown Error: " + error.message);
                }
            });
    } else {
        // User canceled the booking cancellation
        console.log('Booking cancellation was canceled by the user.');
    }
}

// Populate dropdowns and set up event listener
window.addEventListener('load', () => {
    fetchAndPopulateData();

    document.getElementById('updateButton').addEventListener('click', updateBookingForCustomer);

    // Thêm sự kiện nghe cho nút "Cancelled"
    document.getElementById('cancelButton').addEventListener('click', cancelBooking);
});