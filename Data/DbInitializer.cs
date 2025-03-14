using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OaeCrosstrackApi.Models;

namespace OaeCrosstrackApi.Data
{
    public static class DbInitializer
    {
        public static void SeedData(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                
                // Apply migrations if they aren't applied
                context.Database.Migrate();
                
                // In DbInitializer.cs
                if (!context.Users.Any(u => u.Username == "pbeuler"))
                {
                    // Create the site admin user
                    var adminUser = new User
                    {
                        Username = "pbeuler",
                        // Make sure to use the same password as your PostgreSQL user
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("hornetPride#1"),
                        Email = "pbeuler@oahornets.org",
                        FirstName = "Peter",
                        LastName = "Beuler",
                        CreatedAt = DateTime.UtcNow
                    };
                    
                    context.Users.Add(adminUser);
                    context.SaveChanges();
                }
            }
        }
    }
}