using dangNhapDangKy.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;         // Yêu cầu chữ số
    options.Password.RequiredLength = 1;           // Độ dài tối thiểu của mật khẩu
    options.Password.RequireLowercase = false;      // Yêu cầu chữ thường
    options.Password.RequireUppercase = false;      // Yêu cầu chữ hoa
    options.Password.RequireNonAlphanumeric = false; // Yêu cầu ký tự đặc biệt
});

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();


var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Ensure "user" role exists
    if (!await roleManager.RoleExistsAsync("user"))
    {
        await roleManager.CreateAsync(new IdentityRole("user"));
    }

    // Ensure "admin" role exists
    if (!await roleManager.RoleExistsAsync("admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("admin"));
    }

    // Check if there is an admin user
    var adminUser = await userManager.FindByNameAsync("admin");

    if (adminUser == null)
    {
        // Create admin user if not exists
        var newAdminUser = new IdentityUser("admin@admin.com");
        newAdminUser.Email = "admin@admin.com";

        var result = await userManager.CreateAsync(newAdminUser, "admin");

        if (result.Succeeded)
        {
            // Add admin role to admin user
            await userManager.AddToRoleAsync(newAdminUser, "admin");
        }
        else
        {
            // Handle errors if needed
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
