using Microsoft.AspNetCore.Identity;

namespace appdev_sprint.Data;

// Seeds the ClinicAdmin role + one demo clinic login (dev/demo only).
// Contract: role name "ClinicAdmin" is frozen — admin pages authorize on it.
public static class IdentitySeed
{
    public const string ClinicAdminRole = "ClinicAdmin";
    public const string DemoAdminEmail = "admin@cebudental.example";
    public const string DemoAdminPassword = "Admin123!";

    public static async Task InitializeAsync(IServiceProvider services)
    {
        var roles = services.GetRequiredService<RoleManager<IdentityRole>>();
        var users = services.GetRequiredService<UserManager<ApplicationUser>>();

        if (await roles.FindByNameAsync(ClinicAdminRole) is null)
            await roles.CreateAsync(new IdentityRole(ClinicAdminRole));

        var admin = await users.FindByEmailAsync(DemoAdminEmail);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = DemoAdminEmail,
                Email = DemoAdminEmail,
                EmailConfirmed = true,
                DisplayName = "Demo Clinic Admin",
            };
            await users.CreateAsync(admin, DemoAdminPassword);
        }

        if (!await users.IsInRoleAsync(admin, ClinicAdminRole))
            await users.AddToRoleAsync(admin, ClinicAdminRole);
    }
}
