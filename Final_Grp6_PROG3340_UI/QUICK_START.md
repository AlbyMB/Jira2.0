# ?? Quick Start Guide - Task Collaboration App

## Prerequisites
- ? .NET 8 SDK installed
- ? Backend API running on `https://localhost:7044`
- ? Visual Studio 2022 or VS Code

## 1. Configure JWT Settings

Add to `appsettings.json`:
```json
{
  "Jwt": {
    "Key": "your-super-secret-key-minimum-32-characters-for-security",
    "Issuer": "TaskCollaborationApp",
    "Audience": "TaskCollaborationApp"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

## 2. Verify Backend API URL

In `Program.cs`, ensure this matches your backend:
```csharp
builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri("https://localhost:7044/api/");
});
```

## 3. Run the Application

```bash
dotnet run
```

Or press **F5** in Visual Studio.

## 4. Test the Application

### A. Registration Flow
1. Navigate to `https://localhost:<port>`
2. You'll be redirected to the Task Board
3. If not authenticated, click **Login**
4. Click **Register here**
5. Fill in:
   - Full Name
   - Email
   - Username
   - Password (min 6 characters)
6. Click **Register**
7. You'll see success message
8. Login with your credentials

### B. Login Flow
1. Go to `/Auth/Login`
2. Enter username and password
3. Click **Login**
4. You'll be redirected to the Task Board

### C. Google Login Flow
1. On login page, click **Login with Google**
2. Follow Google OAuth flow
3. You'll be redirected back to the app
4. Currently logs you in with Google account
5. **To complete:** Implement backend `/api/auth/google` endpoint

### D. Task Management
1. **View Board:** `/Tasks/Board`
   - See all tasks in 5 columns
   - Click any task card to view details

2. **Create Task:** Click "Create Task" button
   - Enter title (required)
   - Enter description (optional)
   - Select status
   - Assign to user (dropdown)
   - Click "Create Task"

3. **Edit Task:** On task details page, click "Edit Task"
   - Modify any field
   - Click "Save Changes"

4. **Delete Task:** On task details page, click "Delete Task"
   - Confirm deletion
   - Task removed from board

5. **My Tasks:** Click "My Tasks" in navigation
   - See all tasks you created

6. **Assigned Tasks:** Click "Assigned Tasks" in navigation
   - See all tasks assigned to you

## 5. Navigation Overview

**Top Navigation Bar:**
- ?? **Dashboard** ? Task Board (Kanban view)
- ? **Create Task** ? Create new task form
- ?? **My Tasks** ? Tasks I created
- ? **Assigned Tasks** ? Tasks assigned to me
- ?? **User Menu** (dropdown)
  - Displays current user name
  - **Logout** option

## 6. Keyboard Shortcuts & Tips

- **Responsive Design:** Works on mobile, tablet, and desktop
- **Auto-dismiss Alerts:** Success/error messages disappear after 5 seconds
- **Card Hover:** Task cards lift on hover for better UX
- **Breadcrumbs:** Navigate back easily from task pages

## 7. Troubleshooting

### Issue: "Cannot connect to API"
**Solution:** Ensure backend API is running on `https://localhost:7044`

### Issue: "401 Unauthorized"
**Solution:** 
- Check JWT configuration in `appsettings.json`
- Ensure JWT Key matches backend
- Try logging out and logging in again

### Issue: "Users dropdown is empty"
**Solution:** Backend must have users in database. Register some users first.

### Issue: "Google login not working"
**Solution:** 
- Verify Google OAuth credentials in `Program.cs`
- Ensure redirect URI is configured in Google Console: `https://localhost:<port>/signin-oidc`
- Backend needs to implement `/api/auth/google` endpoint

## 8. Default Routes

| Route | Description |
|-------|-------------|
| `/` | Redirects to Task Board |
| `/Tasks/Board` | Kanban board (default) |
| `/Tasks/Create` | Create task form |
| `/Tasks/Details/{id}` | Task details |
| `/Tasks/Edit/{id}` | Edit task form |
| `/Tasks/MyTasks` | My created tasks |
| `/Tasks/AssignedTasks` | Tasks assigned to me |
| `/Auth/Login` | Login page |
| `/Auth/Register` | Registration page |
| `/Auth/GoogleLogin` | Google OAuth trigger |

## 9. Testing Checklist

- [ ] Register new user
- [ ] Login with credentials
- [ ] View task board
- [ ] Create new task
- [ ] Edit existing task
- [ ] Delete task
- [ ] View task details
- [ ] Check "My Tasks"
- [ ] Check "Assigned Tasks"
- [ ] Logout
- [ ] Try Google login
- [ ] Test mobile responsiveness

## 10. Next Steps

### Enable SignalR (Real-time Updates)
1. Install package:
   ```bash
   dotnet add package Microsoft.AspNetCore.SignalR.Client
   ```

2. Update `Program.cs`:
   ```csharp
   // Add before var app = builder.Build();
   builder.Services.AddSignalR();
   
   // Add after app.MapControllerRoute();
   app.MapHub<Final_Grp6_PROG3340_UI.Hubs.TaskHub>("/taskHub");
   ```

3. Uncomment SignalR code in `wwwroot/js/site.js`

### Complete Google OAuth Backend
1. Implement `/api/auth/google` endpoint in backend
2. Uncomment code in `Controllers/AuthController.cs` ? `GoogleCallback()`
3. Test full OAuth flow

## 11. Development Tips

- **Hot Reload:** Enable in Visual Studio for faster development
- **Browser DevTools:** Check Network tab for API calls
- **Console Logs:** Check browser console for errors
- **API Logs:** Backend logs show all API requests
- **Debug Mode:** Set breakpoints in controllers and services

## 12. Production Considerations

Before deploying:
- [ ] Change JWT secret key to production value
- [ ] Update Google OAuth redirect URIs
- [ ] Enable HTTPS enforcement
- [ ] Add proper error pages
- [ ] Implement rate limiting
- [ ] Add logging and monitoring
- [ ] Set secure cookie policies
- [ ] Review CORS settings
- [ ] Add health checks

## ?? You're Ready!

The complete frontend is now running. Test all features and enjoy your Task Collaboration App!

For detailed documentation, see:
- `README_FRONTEND.md` - Full feature documentation
- `API_CONTRACTS.md` - Backend API specifications
- `IMPLEMENTATION_SUMMARY.md` - Complete implementation details
