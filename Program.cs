using api_demo.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Supabase;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configure JSON serialization to ignore Supabase internal properties
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        
        // Ignore Supabase internal properties that cause serialization issues
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure Supabase
var supabaseConfig = new SupabaseConfig();
builder.Configuration.GetSection("Supabase").Bind(supabaseConfig);

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.Authority = $"{supabaseConfig.Url}/auth/v1";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"{supabaseConfig.Url}/auth/v1",
            ValidateAudience = true,
            ValidAudience = "authenticated",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(supabaseConfig.JwtSecret))
        };
    });

// Configure Authorization
builder.Services.AddAuthorization(options =>
{
    // Policy for admin access - check if user has Enterprise role in profiles table
    options.AddPolicy("AdminPolicy", policy =>
        policy.Requirements.Add(new AdminRequirement()));
    
    // Policy for expert access (Experts and Admins)
    options.AddPolicy("ExpertPolicy", policy =>
        policy.Requirements.Add(new ExpertRequirement()));
    
    // Policy for enterprise access (Enterprises and Admins)
    options.AddPolicy("EnterprisePolicy", policy =>
        policy.Requirements.Add(new EnterpriseRequirement()));
    
    // Policy for authenticated users
    options.AddPolicy("AuthenticatedUser", policy =>
        policy.RequireAuthenticatedUser());
});

// Register Authorization Handlers
builder.Services.AddScoped<IAuthorizationHandler, AdminAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ExpertAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, EnterpriseAuthorizationHandler>();

// Configure Supabase Client (use Service Role key to bypass RLS for server-side operations)
builder.Services.AddSingleton<Client>(provider =>
{
    var url = supabaseConfig.Url;
    // var key = supabaseConfig.AnonKey;
    var key = supabaseConfig.ServiceKey;
    
    var options = new SupabaseOptions
    {
        AutoRefreshToken = true,
        AutoConnectRealtime = true
    };
    
    var client = new Client(url, key, options);
    return client;
});

// Register Services
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IAcademicProductService, AcademicProductService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
