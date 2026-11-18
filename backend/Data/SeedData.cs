using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


// namespace backend.Data;

namespace backend.Data
{
    public static class SeedData
    {
        public static async Task Initialize(AppDbContext context)
        {

            //string[] roles = { "Admin", "Receptionist", "Member" };
            //foreach (var roleName in roles)
            //{
            //    bool roleExists = await context.Roles
            //        .AnyAsync(r =>  r.Name == roleName);
            //    if (!roleExists)
            //    {
            //        context.Roles.Add(new Role
            //        {
            //            Name = roleName,
            //        });
            //    }
            //}
            //await context.SaveChangesAsync();

            bool adminExist = await context.Roles.AnyAsync(r => r.Name == "Admin");
            if (!adminExist) 
            {
                context.Roles.Add(new Role
                {
                    Name = "Admin",
                });            
                await context.SaveChangesAsync();
            }


            if(!await context.Users.AnyAsync())
            {
                var adminRole = await context.Roles
                    .FirstAsync(r => r.Name == "Admin");
            
                var admin = new User
                {
                    FullName = "System Admin",
                    Email = "admin@gmail.com",
                    RoleId = adminRole.Id,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = adminRole.Id,
                };

                var passwordHasher = new PasswordHasher<User>();
                admin.Password = passwordHasher.HashPassword(admin, "Admin@123");

                context.Users.Add(admin);
                await context.SaveChangesAsync();
            }

        }
    }
}