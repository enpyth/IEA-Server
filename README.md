# Supabase CRUD API Demo

This is a .NET 9 Web API project that demonstrates CRUD operations with Supabase using JWT authentication.

## Features

- **Full CRUD operations** for profiles
- **JWT authentication** with Supabase
- **Role-based authorization** (admin, expert, visitor, enterprise)
- **Row Level Security (RLS)** integration
- **OpenAPI/Swagger** documentation

## Prerequisites

- .NET 9 SDK
- Supabase project with the following table structure:

```sql
create table public.profiles (
  id uuid primary key references auth.users(id) on delete cascade,
  role text check (role in ('expert', 'visitor', 'enterprise')) not null,
  email text unique not null
);
```

## Setup

1. **Clone the repository**
   ```bash
   git clone <your-repo>
   cd api-demo
   ```

2. **Configure Supabase credentials**
   
   Update `appsettings.json` with your Supabase project details:
   ```json
   {
     "Supabase": {
       "Url": "https://your-project.supabase.co",
       "AnonKey": "your-anon-key",
       "ServiceKey": "your-service-key"
     }
   }
   ```

3. **Restore packages**
   ```bash
   dotnet restore
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

## API Endpoints

### Public Endpoints
- `GET /openapi/v1.json` - OpenAPI specification
- `GET /swagger` - Swagger UI (development only)

### Protected Endpoints (Require JWT)

#### Profiles Management (Admin only)
- `GET /api/profiles` - List all profiles
- `GET /api/profiles/{id}` - Get profile by ID
- `POST /api/profiles` - Create new profile
- `PUT /api/profiles/{id}` - Update profile
- `DELETE /api/profiles/{id}` - Delete profile

#### Current User Operations
- `GET /api/me/profile` - Get current user's profile
- `PUT /api/me/profile` - Update current user's profile

## Testing

### 1. Get a JWT Token from Supabase

Use your frontend application or Supabase dashboard to authenticate and get a JWT token.

### 2. Test with Swagger UI

1. Navigate to `https://localhost:5001/swagger` (or your configured port)
2. Click "Authorize" button
3. Enter your JWT token in the format: `Bearer <your-jwt-token>`
4. Test the endpoints

### 3. Test with curl

```bash
# Get current user profile
curl -H "Authorization: Bearer <your-jwt-token>" \
     https://localhost:5001/api/me/profile

# Update current user profile
curl -X PUT \
     -H "Authorization: Bearer <your-jwt-token>" \
     -H "Content-Type: application/json" \
     -d '{"role":"expert","email":"newemail@example.com"}' \
     https://localhost:5001/api/me/profile
```

### 4. Test with Postman

1. Import the OpenAPI specification from `/openapi/v1.json`
2. Set the Authorization header: `Bearer <your-jwt-token>`
3. Test the endpoints

## Security Features

- **JWT Validation**: Tokens are validated against Supabase
- **Role-based Access**: Different endpoints require different roles
- **RLS Integration**: Database-level security through Supabase policies
- **Input Validation**: Model validation using Data Annotations

## Project Structure

```
api-demo/
├── Controllers/
│   ├── ProfilesController.cs    # Full CRUD operations
│   └── MeController.cs         # Current user operations
├── Models/
│   ├── Profile.cs              # Profile entity
│   └── ProfileDto.cs           # DTOs for create/update
├── Services/
│   ├── IProfileService.cs      # Service interface
│   ├── ProfileService.cs       # Service implementation
│   └── SupabaseConfig.cs       # Configuration model
├── Program.cs                   # Application configuration
└── appsettings.json            # Configuration file
```

## Troubleshooting

### Common Issues

1. **JWT Validation Errors**
   - Ensure your Supabase URL is correct
   - Check that the JWT token is valid and not expired
   - Verify the token was issued by your Supabase project

2. **Database Connection Issues**
   - Verify your Supabase credentials
   - Check that the profiles table exists
   - Ensure RLS policies are properly configured

3. **Authorization Errors**
   - Check that the user has the required role
   - Verify the JWT contains the necessary claims
   - Ensure the user exists in the profiles table

### Debug Mode

Enable detailed logging by setting the log level in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  }
}
```

## Next Steps

- Add more entities (experts, enterprises)
- Implement pagination for list endpoints
- Add search and filtering capabilities
- Implement caching
- Add unit and integration tests
- Set up CI/CD pipeline

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License.
