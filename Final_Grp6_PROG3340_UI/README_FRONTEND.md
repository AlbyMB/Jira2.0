# Task Collaboration App - Frontend

A mini Jira clone built with ASP.NET Core MVC (.NET 8).

## Features Implemented

### Authentication
- ? Login page with username/password
- ? Registration page
- ? Google OAuth placeholder (ready for integration)
- ? JWT token storage in cookies
- ? Logout functionality

### Task Management
- ? **Task Board (Kanban)** - 5 columns: To Do, Development, Review, Merge, Done
- ? **Task Details** - View full task information
- ? **Create Task** - Form with title, description, status, and assignment
- ? **Edit Task** - Update task information
- ? **Delete Task** - Remove tasks with confirmation
- ? **My Tasks** - View tasks created by current user
- ? **Assigned Tasks** - View tasks assigned to current user

### UI/UX
- ? Bootstrap 5 styling with custom Kanban board CSS
- ? Responsive design for mobile and desktop
- ? Bootstrap Icons integration
- ? Success/Error alerts with auto-dismiss
- ? JIRA-inspired color scheme and layout

## Project Structure

```
Final_Grp6_PROG3340_UI/
??? Controllers/
?   ??? AuthController.cs          # Authentication (login, register, logout)
?   ??? TasksController.cs         # Task CRUD operations and views
?   ??? HomeController.cs          # Home page redirects
??? Models/
?   ??? ViewModels/
?       ??? TaskViewModel.cs       # Task data model
?       ??? UserViewModel.cs       # User data model
?       ??? LoginViewModel.cs      # Login form model
?       ??? RegisterViewModel.cs   # Registration form model
?       ??? TaskBoardViewModel.cs  # Kanban board data model
??? Services/
?   ??? ApiClient.cs               # HTTP client wrapper with auth
?   ??? JwtService.cs              # JWT token generation
?   ??? AuthApiService.cs          # Auth API calls
?   ??? TaskApiService.cs          # Task API calls
?   ??? UserApiService.cs          # User API calls
??? Views/
?   ??? Auth/
?   ?   ??? Login.cshtml           # Login page
?   ?   ??? Register.cshtml        # Registration page
?   ??? Tasks/
?   ?   ??? Board.cshtml           # Kanban board view
?   ?   ??? Details.cshtml         # Task details view
?   ?   ??? Create.cshtml          # Create task form
?   ?   ??? Edit.cshtml            # Edit task form
?   ?   ??? MyTasks.cshtml         # My tasks list
?   ?   ??? AssignedTasks.cshtml   # Assigned tasks list
?   ?   ??? _TaskCard.cshtml       # Task card partial
?   ??? Shared/
?       ??? _Layout.cshtml         # Main layout with navigation
??? Hubs/
?   ??? TaskHub.cs                 # SignalR hub (placeholder)
??? wwwroot/
    ??? css/site.css               # Custom styles including Kanban
    ??? js/site.js                 # JavaScript utilities and SignalR placeholder

## Backend API Endpoints

The frontend calls these backend REST API endpoints:

### Auth
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `POST /api/auth/google` - Google OAuth (placeholder)
- `GET /api/auth/me` - Get current user

### Tasks
- `GET /api/tasks` - Get all tasks
- `GET /api/tasks/{id}` - Get task by ID
- `POST /api/tasks` - Create new task
- `PUT /api/tasks/{id}` - Update task
- `DELETE /api/tasks/{id}` - Delete task
- `GET /api/tasks/my` - Get tasks created by current user
- `GET /api/tasks/assigned` - Get tasks assigned to current user

### Users
- `GET /api/users` - Get all users (for assignment dropdown)
- `GET /api/users/{id}` - Get user by ID

## Configuration

Update `appsettings.json` with your JWT configuration:

```json
{
  "Jwt": {
    "Key": "your-secret-key-here",
    "Issuer": "your-issuer",
    "Audience": "your-audience"
  }
}
```

Update the API base URL in `Program.cs`:

```csharp
builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri("https://localhost:7044/api/");
});
```

## Running the Application

1. Ensure the backend API is running on `https://localhost:7044`
2. Run the frontend:
   ```bash
   dotnet run
   ```
3. Navigate to `https://localhost:<port>` in your browser
4. You'll be redirected to the Task Board (or Login if not authenticated)

## Future Enhancements (TODO)

### SignalR Integration
The project includes placeholders for real-time updates:
- `Hubs/TaskHub.cs` - SignalR hub (ready for implementation)
- `wwwroot/js/site.js` - SignalR client code (commented out)

To enable SignalR:
1. Install SignalR package: `dotnet add package Microsoft.AspNetCore.SignalR.Client`
2. Update `Program.cs` to add SignalR services and map hub
3. Uncomment SignalR JavaScript code in `site.js`
4. Implement hub methods in `TaskHub.cs`

### Google OAuth
- Complete OAuth flow in `AuthController.GoogleLogin()` and `GoogleCallback()`
- Call backend `/api/auth/google` endpoint with OAuth token

## Technologies Used

- **ASP.NET Core 8.0** - MVC framework
- **Bootstrap 5** - UI framework
- **Bootstrap Icons** - Icon library
- **jQuery** - JavaScript library (for Bootstrap)
- **HttpClient** - API communication
- **JWT** - Authentication tokens

## Error Handling

The frontend includes comprehensive error handling:
- 401 (Unauthorized) - Redirects to login page
- 400/500 errors - Displays Bootstrap alerts
- Validation errors - Displayed inline with form fields
- Success messages - Shown as dismissible alerts

## Styling

The app uses a JIRA-inspired design with:
- Color-coded status badges
- Kanban board with scrollable columns
- Card-based task display
- Responsive layout for all screen sizes
- Smooth animations and transitions
