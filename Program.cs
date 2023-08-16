global using AutoMapper;

global using Stripe;
global using SpaBookingApp.Models;

global using SpaBookingApp.Dtos.User;
global using SpaBookingApp.Dtos.SpaProduct;
global using SpaBookingApp.Dtos.Category;
global using SpaBookingApp.Dtos.Provision;
global using SpaBookingApp.Dtos.Department;
global using SpaBookingApp.Dtos.Staff;
global using SpaBookingApp.Dtos.Contact;
global using SpaBookingApp.Dtos.Subject;
global using SpaBookingApp.Dtos.Booking;

global using SpaBookingApp.Helpter;

global using SpaBookingApp.Services;

global using SpaBookingApp.Services.CategoryService;
global using SpaBookingApp.Services.ProvisionService;
global using SpaBookingApp.Services.EmailService;
global using SpaBookingApp.Services.CartService;
global using SpaBookingApp.Services.OrderService;
global using SpaBookingApp.Services.DepartmentService;
global using SpaBookingApp.Services.StaffService;
global using SpaBookingApp.Services.ContactService;
global using SpaBookingApp.Services.SubjectService;
global using SpaBookingApp.Services.BookingService;
global using SpaBookingApp.Services.SpaProductService;

global using Microsoft.EntityFrameworkCore;
global using SpaBookingApp.Data;
global using System.ComponentModel.DataAnnotations;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;




var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));
var key=builder.Configuration.GetValue<string>("StripeSettings:SecretKey");

builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddLogging();


// Add services to the container.

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
builder.Services.AddHostedService<DeleteUnverifiedAccounts>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First()); // Giải quyết xung đột với phương thức đầu tiên
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = """Standard Authorization header using the Bearer scheme. Example: "bearer {token}" """,
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddScoped<ISpaProductService, SpaProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProvisionService, ProvisionService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IBookingService, BookingService>();


builder.Services.AddScoped<IAuthRepository, AuthRepository>();

builder.Services.AddSession();
// builder.Services.AddScoped<JwtMiddleware>();
builder.Services.AddSingleton<RequestDelegate>(provider => provider.GetRequiredService<IApplicationBuilder>().Build());


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
builder.Services.AddHttpContextAccessor();


// Định tuyến API
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
});

// Thêm middleware xác thực và kiểm tra quyền truy cập
// app.UseMiddleware<JwtMiddleware>();
app.UseMiddleware<RedirectMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    var dbContext = services.GetRequiredService<DataContext>();
    var authRepository = services.GetRequiredService<IAuthRepository>();

    await dbContext.Database.MigrateAsync();
    await authRepository.SeedAdminUser();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();


app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapRazorPages();
});

app.MapRazorPages();

app.Run();
