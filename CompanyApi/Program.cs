using CompanyApi;
using CompanyApi.Abstractions;
using CompanyApi.Data;
using CompanyApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<CompanyDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Add services
builder.Services.AddScoped<ICompanyService, CompanyServices>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Company API",
        Version = "v1",
        Description = "A simple API for managing company information"
    });
    // Configureaz? Swagger pentru API Key
    c.AddSecurityDefinition("ApiKey", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "API Key needed to access the endpoints. X-Api-Key: My_API_Key",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = "X-Api-Key",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "X-Api-Key",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            new string[] {}
        }
    });
});
// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Company API V1");
        c.RoutePrefix = string.Empty; // Set Swagger as the root page
    }); ;
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
;

app.UseHttpsRedirection();
//app.UseAuthentication();
app.UseMiddleware<ApiKeyMiddleware>(); // Custom API Key middleware
app.UseAuthorization();

app.MapControllers();
// Ensure database is created and migrated
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CompanyDbContext>();
    context.Database.EnsureCreated();
}

//app.MapCompanyEndpoints();
app.Run();
