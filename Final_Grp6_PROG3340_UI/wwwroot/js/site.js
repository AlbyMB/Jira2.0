// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Task Collaboration App - Main JavaScript

// Placeholder for SignalR Connection
// TODO: Implement SignalR real-time updates
/*
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/taskHub")
    .build();

connection.on("TaskCreated", function (taskId) {
    // TODO: Handle task created event - refresh board or add task card
    console.log("Task created:", taskId);
});

connection.on("TaskUpdated", function (taskId) {
    // TODO: Handle task updated event - update task card
    console.log("Task updated:", taskId);
});

connection.on("TaskDeleted", function (taskId) {
    // TODO: Handle task deleted event - remove task card
    console.log("Task deleted:", taskId);
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});
*/

// Auto-dismiss alerts after 5 seconds
document.addEventListener('DOMContentLoaded', function () {
    const alerts = document.querySelectorAll('.alert:not(.alert-permanent)');
    alerts.forEach(function (alert) {
        setTimeout(function () {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        }, 5000);
    });
});

// Confirmation for delete actions
function confirmDelete(message) {
    return confirm(message || 'Are you sure you want to delete this item?');
}

// Utility function for API error handling
function handleApiError(response) {
    if (response.status === 401) {
        window.location.href = '/Auth/Login';
    } else {
        console.error('API Error:', response);
    }
}
