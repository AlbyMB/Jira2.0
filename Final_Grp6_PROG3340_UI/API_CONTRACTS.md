# API Data Transfer Objects (DTOs)

This document describes the expected request/response formats for the backend API.

## Authentication Endpoints

### POST /api/auth/login
**Request:**
```json
{
  "username": "string",
  "password": "string"
}
```

**Response (200 OK):**
```json
{
  "token": "string (JWT token)"
}
```

### POST /api/auth/register
**Request:**
```json
{
  "name": "string",
  "email": "string",
  "username": "string",
  "password": "string"
}
```

**Response (200 OK):**
```json
{
  "message": "Registration successful"
}
```

### GET /api/auth/me
**Response (200 OK):**
```json
{
  "id": 1,
  "name": "John Doe",
  "email": "john@example.com",
  "username": "johndoe",
  "role": "User"
}
```

## Task Endpoints

### GET /api/tasks
**Response (200 OK):**
```json
[
  {
    "id": 1,
    "title": "Task Title",
    "description": "Task Description",
    "status": "ToDo",
    "createdById": 1,
    "createdByName": "John Doe",
    "assignedToId": 2,
    "assignedToName": "Jane Smith",
    "isArchived": false,
    "createdAt": "2025-01-01T10:00:00Z",
    "updatedAt": "2025-01-01T10:00:00Z",
    "archivedAt": null
  }
]
```

### GET /api/tasks/{id}
**Response (200 OK):**
```json
{
  "id": 1,
  "title": "Task Title",
  "description": "Task Description",
  "status": "Development",
  "createdById": 1,
  "createdByName": "John Doe",
  "assignedToId": 2,
  "assignedToName": "Jane Smith",
  "isArchived": false,
  "createdAt": "2025-01-01T10:00:00Z",
  "updatedAt": "2025-01-01T10:00:00Z",
  "archivedAt": null
}
```

### POST /api/tasks
**Request:**
```json
{
  "title": "New Task",
  "description": "Task description",
  "status": "ToDo",
  "assignedToId": 2
}
```

**Response (201 Created):**
```json
{
  "id": 1,
  "title": "New Task",
  "description": "Task description",
  "status": "ToDo",
  "createdById": 1,
  "createdByName": "John Doe",
  "assignedToId": 2,
  "assignedToName": "Jane Smith",
  "isArchived": false,
  "createdAt": "2025-01-01T10:00:00Z",
  "updatedAt": "2025-01-01T10:00:00Z",
  "archivedAt": null
}
```

### PUT /api/tasks/{id}
**Request:**
```json
{
  "title": "Updated Task",
  "description": "Updated description",
  "status": "Review",
  "assignedToId": 3
}
```

**Response (200 OK):**
```json
{
  "id": 1,
  "title": "Updated Task",
  "description": "Updated description",
  "status": "Review",
  "createdById": 1,
  "createdByName": "John Doe",
  "assignedToId": 3,
  "assignedToName": "Bob Johnson",
  "isArchived": false,
  "createdAt": "2025-01-01T10:00:00Z",
  "updatedAt": "2025-01-01T11:00:00Z",
  "archivedAt": null
}
```

### DELETE /api/tasks/{id}
**Response (204 No Content)**

### GET /api/tasks/my
Returns all tasks created by the authenticated user.

**Response (200 OK):**
```json
[
  {
    "id": 1,
    "title": "My Task",
    "description": "Description",
    "status": "Development",
    "createdById": 1,
    "createdByName": "John Doe",
    "assignedToId": 2,
    "assignedToName": "Jane Smith",
    "isArchived": false,
    "createdAt": "2025-01-01T10:00:00Z",
    "updatedAt": "2025-01-01T10:00:00Z",
    "archivedAt": null
  }
]
```

### GET /api/tasks/assigned
Returns all tasks assigned to the authenticated user.

**Response (200 OK):**
```json
[
  {
    "id": 2,
    "title": "Assigned Task",
    "description": "Description",
    "status": "ToDo",
    "createdById": 2,
    "createdByName": "Jane Smith",
    "assignedToId": 1,
    "assignedToName": "John Doe",
    "isArchived": false,
    "createdAt": "2025-01-01T10:00:00Z",
    "updatedAt": "2025-01-01T10:00:00Z",
    "archivedAt": null
  }
]
```

## User Endpoints

### GET /api/users
**Response (200 OK):**
```json
[
  {
    "id": 1,
    "name": "John Doe",
    "email": "john@example.com",
    "username": "johndoe",
    "role": "User"
  },
  {
    "id": 2,
    "name": "Jane Smith",
    "email": "jane@example.com",
    "username": "janesmith",
    "role": "User"
  }
]
```

### GET /api/users/{id}
**Response (200 OK):**
```json
{
  "id": 1,
  "name": "John Doe",
  "email": "john@example.com",
  "username": "johndoe",
  "role": "User"
}
```

## Task Status Enum

The `status` field can have the following values:
- `"ToDo"` (0)
- `"Development"` (1)
- `"Review"` (2)
- `"Merge"` (3)
- `"Done"` (4)

## Authentication

All task and user endpoints (except auth endpoints) require a Bearer token in the Authorization header:

```
Authorization: Bearer <JWT_TOKEN>
```

The frontend automatically handles this via the `ApiClient` service.

## Error Responses

### 400 Bad Request
```json
{
  "message": "Validation error message"
}
```

### 401 Unauthorized
```json
{
  "message": "Unauthorized"
}
```

### 404 Not Found
```json
{
  "message": "Resource not found"
}
```

### 500 Internal Server Error
```json
{
  "message": "Internal server error"
}
```
