using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MomentaryMessages.Data;
using MomentaryMessages.Data.Services;
using MomentaryMessages.Helper;

namespace MomentaryMessages
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);

      // Add services to the container.
      var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
      builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
      builder.Services.AddScoped<SecretViewLogsService>();
      builder.Services.AddDatabaseDeveloperPageExceptionFilter();
      builder.Services
        .AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();
      builder.Services.AddControllersWithViews();

      // Email
      builder.Services.Configure<SecretLinkSettings>(x =>
      {
        x.LinksExpireAfterXHours = builder.Configuration.GetValue<int>("SecretLinkSettings:ExpireAfterXHours");
        x.LinksCanBeClickedXNumberOfTimes = builder.Configuration.GetValue<int>("SecretLinkSettings:CanBeClickedXNumberOfTimes");
      });

      var app = builder.Build();

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
      app.UseAuthorization();
      app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
      app.MapRazorPages();

      #region DB Initialize

      // Seed initial data
      using (var scope = app.Services.CreateScope())
      {
        // Roles
        const string c_adminRole = "Admin";
        const string c_userRole = "User";
        var roles = new[] { c_adminRole, c_userRole };
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in roles)
        {
          if (await roleManager.RoleExistsAsync(role))
            continue;

          await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Users
        // - Admins
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var adminUserNames = new[] { "admin" };
        await CreateUsers(userManager, adminUserNames, c_adminRole);

        // - Default
        var userNames = new[] { "user" };
        await CreateUsers(userManager, userNames, c_userRole);
      }

      #endregion DB Initialize

      app.Run();
    }

    private static async Task CreateUsers(UserManager<IdentityUser> userManager, string[] userNames, string role)
    {
      const string c_userPassword = "Test123$";
      foreach (var userName in userNames)
      {
        if (await userManager.FindByNameAsync(userName) != null)
          continue;

        var userEmail = $"{userName}@{role.ToLower()}.com";
        var user = new IdentityUser()
        {
          UserName = userEmail,
          Email = userEmail,
          EmailConfirmed = true
        };
        await userManager.CreateAsync(user, c_userPassword);
        await userManager.AddToRoleAsync(user, role);
      }
    }
  }
}
