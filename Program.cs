using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.IdentityModel.Tokens;

using be_scrapping_service.Context;
using be_scrapping_service.Service;
using be_scrapping_service.Entity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ScrapingContext>();

// Add services to the container.
builder.Services.AddScoped<TokenService, TokenService>();

builder.Services.AddCors(op=>
{
    op.AddPolicy(
        name: "*",
        policy => 
        {
            policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
        }
    );
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

var secret =  Encoding.UTF8.GetBytes(
    "a5e5a0e4bc817ef2774d841c0d300847b3212d96a49a3fe849ecf65d51dadb11a5e5a0e4bc817ef2774d841c0d300847b3212d96a49a3fe849ecf65d51dadb11a5e5a0e4bc817ef2774d841c0d300847b3212d96a49a3fe849ecf65d51dadb11"
);
Console.WriteLine(secret.Length);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {   
            // TODO: checj valid issuer and audience
            RoleClaimType = "roles",
            NameClaimType = "name",
            ClockSkew = TimeSpan.Zero,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "https://localhost:*",
            ValidAudience = "be_scrapping_service",
            // TODO: read this from enviroments
            IssuerSigningKey = new SymmetricSecurityKey(secret),
        };
    });
    

builder.Services
    .AddIdentityCore<User>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    })
    .AddEntityFrameworkStores<ScrapingContext>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("*");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
