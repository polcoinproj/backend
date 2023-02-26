using Microsoft.EntityFrameworkCore;
using backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

IConfigurationSection jwtConfig = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer((options) =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtConfig.GetValue<string>("Issuer"),
        ValidateAudience = true,
        ValidAudience = jwtConfig.GetValue<string>("Audience"),
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        RequireExpirationTime = true,
        IssuerSigningKey = new SymmetricSecurityKey(File.ReadAllBytes(jwtConfig.GetValue<string>("Key")!))
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationContext>((options) => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")!));
builder.Services.AddTransient<EmailChecker>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped(typeof(FileService<>));
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(builder.Configuration.GetSection("Storage").GetValue<string>("Path")!),
    RequestPath = "/static",
    ServeUnknownFileTypes = true,
});

app.Run();
