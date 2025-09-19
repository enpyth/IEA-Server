## Step-by-Step: Build the ASP.NET Core API from the Class Diagram

This guide turns the class diagram in `docs/Design_DataModel.md` into a concrete, controller-based ASP.NET Core API. Follow the steps in order. Do not execute commands here; use it as a checklist.

### 0) Prerequisites and Scope
- Framework: ASP.NET Core 9 (already configured in `api-demo.csproj`).
- Packages present: `NSwag.AspNetCore`, `Microsoft.EntityFrameworkCore.InMemory`.
- Required packages to add:
  - PostgreSQL provider: `Npgsql.EntityFrameworkCore.PostgreSQL`
  - Code generation tools: `Microsoft.VisualStudio.Web.CodeGeneration.Design`
  - EF Core tools: `Microsoft.EntityFrameworkCore.Tools` (for migrations)
- Optional packages (add later if needed):
  - Authentication/JWT: `Microsoft.AspNetCore.Authentication.JwtBearer`
  - Fluent validation: `FluentValidation.AspNetCore`
  - AutoMapper: `AutoMapper.Extensions.Microsoft.DependencyInjection` (for DTO mapping)

### 1) Translate the Class Diagram to Domain Models
Create entities that reflect the diagram:
- `User` (base), specialized roles via a `Role` enum: `Visitor`, `AuthenticatedUser`, `Enterprise`, `Expert`, `Admin`.
- `CollaborationRequest` with fields: `Id (Guid)`, `SenderId (Guid)`, `ReceiverId (Guid)`, `Details (string)`, `Status (enum: Pending/Approved/Rejected)`, `CreatedAt (DateTime)`, `UpdatedAt (DateTime)`.
- `AcademicProduct` with fields: `Id (Guid)`, `ExpertId (Guid)`, `Achievements (JSONB)`, `Title (string)`, `Description (string)`, `CreatedAt (DateTime)`.

Recommended files under `Models/`:
- `Models/User.cs` - Base user entity with Role enum
- `Models/Role.cs` - Role enumeration
- `Models/CollaborationRequest.cs` - Collaboration request entity
- `Models/CollaborationStatus.cs` - Status enumeration
- `Models/AcademicProduct.cs` - Academic product entity

**Entity Design Notes:**
- Use `Guid` for all primary keys
- Add `CreatedAt`/`UpdatedAt` timestamps for audit trails
- Use navigation properties for EF relationships
- Configure JSONB for PostgreSQL `Achievements` field

### 2) Define DTOs and Contracts
Avoid over-posting and stabilize your public API:
- `Dtos/Users/UserDto.cs`, `CreateUserDto.cs`, `UpdateUserRoleDto.cs`
- `Dtos/Collaboration/CollaborationRequestDto.cs`, `CreateCollaborationRequestDto.cs`, `ReviewCollaborationRequestDto.cs`
- `Dtos/AcademicProducts/AcademicProductDto.cs`, `CreateAcademicProductDto.cs`, `UpdateAcademicProductDto.cs`

DTO tips:
- Keep IDs as `Guid`.
- For `Achievements`, use `Dictionary<string, object>` or strongly-typed structure.

### 3) Persistence: DbContext and Mapping
Add `Data/AppDbContext.cs`:
- `DbSet<User> Users`
- `DbSet<CollaborationRequest> CollaborationRequests`
- `DbSet<AcademicProduct> AcademicProducts`
- Configure relationships: `CollaborationRequest` has `SenderId` and `ReceiverId` foreign keys to `User`.
- Configure PostgreSQL-specific settings (JSONB for `AcademicProducts.Achievements`).
- Add `OnModelCreating` to configure entity relationships and constraints.
- Seed minimal data if needed for dev.

In `Program.cs` register EF Core:
- Use PostgreSQL: `AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connectionString))`.
- Connection string should point to your Supabase PostgreSQL instance.
- Add migrations: `dotnet ef migrations add Initial` and `dotnet ef database update`.

**DbContext Configuration Example:**
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Configure JSONB for Achievements
    modelBuilder.Entity<AcademicProduct>()
        .Property(e => e.Achievements)
        .HasColumnType("jsonb");
    
    // Configure relationships
    modelBuilder.Entity<CollaborationRequest>()
        .HasOne(c => c.Sender)
        .WithMany()
        .HasForeignKey(c => c.SenderId);
}
```

### 4) Roles and Authorization (PostgreSQL RLS Placeholder)
**Note**: Authorization will be handled by PostgreSQL Row Level Security (RLS) policies in Supabase. Create placeholder interfaces for now.

Create authorization interfaces in `Services/`:
- `IAuthorizationService` - placeholder for RLS-based authorization
- `IRoleService` - placeholder for role management via Supabase Auth
- `IUserContextService` - placeholder for getting current user context from Supabase JWT

Model roles in `User` entity:
- `enum Role { Visitor, AuthenticatedUser, Enterprise, Expert, Admin }` on `User`.
- Store role in database but authorization decisions will be made by RLS policies.

Policies (placeholder for future implementation):
- `AdminOnly` for admin actions (e.g., verify users).
- `EnterpriseOrExpert` for creating collaboration requests.
- `OwnerOrAdmin` for modifying owned resources.

In `Program.cs`:
- Register placeholder services: `builder.Services.AddScoped<IAuthorizationService, PlaceholderAuthorizationService>();`
- Add authorization middleware: `app.UseAuthorization();`
- **Later**: Replace placeholders with Supabase Auth integration when RLS policies are ready.

### 5) Services Layer (Business Logic)
Create interfaces and implementations in `Services/`:
- `IUserService`/`UserService`: user CRUD, role updates, verify user (admin capability).
- `ICollaborationService`/`CollaborationService`: create request, approve, reject, list by sender/receiver, query by status.
- `IAcademicProductService`/`AcademicProductService`: CRUD for products owned by experts.

**Service Implementation Guidelines:**
- Use async/await for all I/O operations
- Inject `AppDbContext` and use repository pattern if needed
- Implement proper error handling and validation
- Use AutoMapper for entity-DTO mapping
- Add logging for business operations

Register these with DI in `Program.cs`:
- `builder.Services.AddScoped<IUserService, UserService>();` etc.
- `builder.Services.AddAutoMapper(typeof(Program));` (if using AutoMapper)

### 6) Controllers and Routes (Using dotnet-aspnet-codegenerator)
**Use scaffolding to generate controllers automatically:**

Install scaffolding tools:
```bash
dotnet tool install -g dotnet-aspnet-codegenerator
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
```

Generate controllers using scaffolding:
```bash
# Generate UsersController
dotnet aspnet-codegenerator controller -name UsersController -api -m User -dc AppDbContext -outDir Controllers

# Generate CollaborationRequestsController  
dotnet aspnet-codegenerator controller -name CollaborationRequestsController -api -m CollaborationRequest -dc AppDbContext -outDir Controllers

# Generate AcademicProductsController
dotnet aspnet-codegenerator controller -name AcademicProductsController -api -m AcademicProduct -dc AppDbContext -outDir Controllers
```

**Expected generated endpoints:**

Users (`UsersController`)
- `GET /api/users` (paged)
- `GET /api/users/{id}`
- `POST /api/users` (create)
- `PUT /api/users/{id}` (update)
- `DELETE /api/users/{id}` (delete)

Collaboration Requests (`CollaborationRequestsController`)
- `GET /api/collaborationrequests` (list)
- `GET /api/collaborationrequests/{id}`
- `POST /api/collaborationrequests` (create)
- `PUT /api/collaborationrequests/{id}` (update)
- `DELETE /api/collaborationrequests/{id}` (delete)

Academic Products (`AcademicProductsController`)
- `GET /api/academicproducts` (list)
- `GET /api/academicproducts/{id}`
- `POST /api/academicproducts` (create)
- `PUT /api/academicproducts/{id}` (update)
- `DELETE /api/academicproducts/{id}` (delete)

**Post-scaffolding modifications needed:**
- Add custom actions (e.g., `approve`, `reject` for collaboration requests)
- Add authorization attributes: `[Authorize(Policy = "...")]`
- Modify routes if needed: `[Route("api/[controller]")]`
- Replace direct DbContext access with service layer calls
- Add proper error handling and validation
- Add XML comments for Swagger documentation
- Implement proper HTTP status codes and response types
- Add pagination support for list endpoints

### 7) Validation and Error Handling
- Add data annotations on DTOs (e.g., `[Required]`, `[EmailAddress]`).
- Optional: integrate FluentValidation.
- Implement global exception handling middleware to return RFC 7807 ProblemDetails.

### 8) Query, Paging, Sorting, Filtering
- Introduce query models: `GET /api/users?page=1&pageSize=20&sort=-createdAt&role=Expert`.
- Validate `pageSize` limits; default reasonable values (e.g., max 100).
- Create `QueryParameters` base class for common pagination properties.
- Implement filtering by role, status, date ranges, etc.
- Add `[FromQuery]` parameters for complex filtering.

### 9) OpenAPI/Swagger
- Ensure XML comments are enabled in `csproj` and `Program.cs` to include them in Swagger.
- Describe response types, error schemas, and authorization requirements.
- Keep example payloads in Swashbuckle/NSwag annotations or `ProducesResponseType`.

### 10) Authentication (Supabase Integration)
- **Current**: Use placeholder authentication for development.
- **Future**: Integrate Supabase Auth:
  - Configure JWT bearer authentication with Supabase public key.
  - Map Supabase JWT claims to application roles.
  - Implement `IUserContextService` to extract user info from JWT.
  - Configure CORS for Supabase frontend integration.

### 11) Testing Strategy
- Unit tests for services (business rules: approve/reject transitions, ownership checks).
- Integration tests for controllers using `WebApplicationFactory`.
- Seed test data per test run for determinism.

### 12) Data Migration Path (PostgreSQL/Supabase)
- Already configured for PostgreSQL with `Npgsql.EntityFrameworkCore.PostgreSQL`.
- Connection string should point to your Supabase PostgreSQL instance.
- Create migrations: `dotnet ef migrations add Initial` and `dotnet ef database update`.
- Configure RLS policies in Supabase dashboard for authorization.

### 13) Remove Template WeatherForecast API (if still present)
- Delete `Controllers/WeatherForecastController.cs`.
- Delete `WeatherForecast.cs`.
- Update or remove `/weatherforecast` in `api-demo.http`.

### 14) Documentation and Examples
- Update `README.md` with:
  - ER/class diagram link (`docs/Design_DataModel.md`).
  - Endpoint list and example requests/responses.
  - Auth and role policy descriptions.

### 15) Minimal Entity and DTO Sketches (for reference)

**Entity Example:**
```csharp
public class CollaborationRequest
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public string Details { get; set; } = string.Empty;
    public CollaborationStatus Status { get; set; } = CollaborationStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public User Sender { get; set; } = null!;
    public User Receiver { get; set; } = null!;
}
```

**DTO Example:**
```csharp
public class CreateCollaborationRequestDto
{
    [Required]
    public Guid ReceiverId { get; set; }
    
    [Required]
    [StringLength(1000, MinimumLength = 10)]
    public string Details { get; set; } = string.Empty;
}
```

**Controller Action Example:**
```csharp
[HttpPost]
[Authorize(Policy = "EnterpriseOrExpert")]
[ProducesResponseType(typeof(CollaborationRequestDto), 201)]
[ProducesResponseType(400)]
public async Task<ActionResult<CollaborationRequestDto>> Create([FromBody] CreateCollaborationRequestDto dto)
{
    var created = await _collaborationService.CreateAsync(User, dto);
    return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
}
```

**Service Interface Example:**
```csharp
public interface ICollaborationService
{
    Task<CollaborationRequestDto> CreateAsync(ClaimsPrincipal user, CreateCollaborationRequestDto dto);
    Task<CollaborationRequestDto?> GetByIdAsync(Guid id);
    Task<PagedResult<CollaborationRequestDto>> GetByUserAsync(Guid userId, QueryParameters parameters);
    Task<bool> ApproveAsync(Guid id, ClaimsPrincipal user);
    Task<bool> RejectAsync(Guid id, ClaimsPrincipal user);
}
```

Use this guide as the backbone for your implementation, checking off each section as you build out the API.

