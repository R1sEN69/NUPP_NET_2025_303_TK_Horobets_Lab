using Microsoft.AspNetCore.Identity; 
using Microsoft.EntityFrameworkCore;
using Transport.Infrastructure.Data; 
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TransportDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); 

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{

    options.Password.RequireDigit = true;    
    options.Password.RequireLowercase = true;    
    options.Password.RequireUppercase = true;      
    options.Password.RequireNonAlphanumeric = true; 
    options.Password.RequiredLength = 8;           
    options.User.RequireUniqueEmail = true;      
})
.AddEntityFrameworkStores<TransportDbContext>() 
.AddDefaultTokenProviders();


var jwtSettings = builder.Configuration.GetSection("JwtSettings");

var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("SecretKey not found in configuration.");

builder.Services.AddAuthentication(options =>
{

    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true, 

        ValidIssuer = jwtSettings["ValidIssuer"], 
        ValidAudience = jwtSettings["ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});


builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Transport API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header, 
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); 


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); 

app.Run();