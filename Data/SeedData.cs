using Microsoft.AspNetCore.Identity;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { "Student", "Staff" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            string staffEmail = "priya@iubat.edu";
            if (await userManager.FindByEmailAsync(staffEmail) == null)
            {
                var staffUser = new ApplicationUser
                {
                    UserName = staffEmail,
                    Email = staffEmail,
                    FirstName = "Priya",
                    LastName = "IUBAT",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(staffUser, "P@ssw0rd");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(staffUser, "Staff");
                }
                else
                {
                    Console.WriteLine($"[Seed] Failed to create {staffEmail}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            // Cleanup legacy staff maya@iubat.com (replaced by priya@iubat.edu per latest requirement)
            var legacyEmail = "maya@iubat.com";
            var legacyUser = await userManager.FindByEmailAsync(legacyEmail);
            if (legacyUser != null)
            {
                var delResult = await userManager.DeleteAsync(legacyUser);
                Console.WriteLine(delResult.Succeeded
                    ? $"[Seed] Removed legacy staff {legacyEmail} (migrated to {staffEmail})."
                    : $"[Seed] Failed to remove legacy {legacyEmail}: {string.Join(", ", delResult.Errors.Select(e => e.Description))}");
            }
        }
    }
}
