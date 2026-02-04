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
                if (context != null)
                {
                    context.Database.Migrate();
                }

                // Seed admin user
                if (context != null && !context.Users.Any(u => u.Username == "pbeuler"))
                {
                    var adminUser = new User
                    {
                        Username = "pbeuler",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("hornetPride#1"),
                        Email = "pbeuler@oahornets.org",
                        FirstName = "Peter",
                        LastName = "Beuler",
                        CreatedAt = DateTime.UtcNow
                    };

                    context.Users.Add(adminUser);
                    context.SaveChanges();
                }

                // Seed Sports
                if (context != null && !context.Sports.Any())
                {
                    SeedSports(context);
                }

                // Seed Events
                if (context != null && !context.Events.Any())
                {
                    SeedEvents(context);
                }
            }
        }

        private static void SeedSports(ApplicationDbContext context)
        {
            var sports = new[]
            {
                new Sport { Name = "Cross Country", Season = "Fall", DisplayOrder = 1 },
                new Sport { Name = "Indoor Track", Season = "Winter", DisplayOrder = 2 },
                new Sport { Name = "Outdoor Track", Season = "Spring", DisplayOrder = 3 },
                new Sport { Name = "Dash in the Dark", Season = "Special", DisplayOrder = 4 }
            };

            context.Sports.AddRange(sports);
            context.SaveChanges();
        }

        private static void SeedEvents(ApplicationDbContext context)
        {
            // Get sport IDs
            var crossCountry = context.Sports.First(s => s.Name == "Cross Country");
            var indoorTrack = context.Sports.First(s => s.Name == "Indoor Track");
            var outdoorTrack = context.Sports.First(s => s.Name == "Outdoor Track");
            var dashInTheDark = context.Sports.First(s => s.Name == "Dash in the Dark");

            var events = new List<Event>();

            // Cross Country Events
            events.AddRange(new[]
            {
                new Event { Name = "5K", SportId = crossCountry.Id, EventType = "Running", SortOrder = 1 },
                new Event { Name = "3 Mile", SportId = crossCountry.Id, EventType = "Running", SortOrder = 2 },
                new Event { Name = "2 Mile", SportId = crossCountry.Id, EventType = "Running", SortOrder = 3 }
            });

            // Indoor Track Events - Running
            events.AddRange(new[]
            {
                new Event { Name = "55m", SportId = indoorTrack.Id, EventType = "Running", SortOrder = 1 },
                new Event { Name = "200m", SportId = indoorTrack.Id, EventType = "Running", SortOrder = 2 },
                new Event { Name = "400m", SportId = indoorTrack.Id, EventType = "Running", SortOrder = 3 },
                new Event { Name = "800m", SportId = indoorTrack.Id, EventType = "Running", SortOrder = 4 },
                new Event { Name = "1600m", SportId = indoorTrack.Id, EventType = "Running", SortOrder = 5 },
                new Event { Name = "3200m", SportId = indoorTrack.Id, EventType = "Running", SortOrder = 6 },
                new Event { Name = "55m Hurdles", SportId = indoorTrack.Id, EventType = "Running", SortOrder = 7 }
            });

            // Indoor Track Events - Relays
            events.AddRange(new[]
            {
                new Event { Name = "4x200m Relay", SportId = indoorTrack.Id, EventType = "Relay", SortOrder = 10 },
                new Event { Name = "4x400m Relay", SportId = indoorTrack.Id, EventType = "Relay", SortOrder = 11 },
                new Event { Name = "4x800m Relay", SportId = indoorTrack.Id, EventType = "Relay", SortOrder = 12 },
                new Event { Name = "Distance Medley Relay", SportId = indoorTrack.Id, EventType = "Relay", SortOrder = 13 }
            });

            // Indoor Track Events - Field
            events.AddRange(new[]
            {
                new Event { Name = "Shot Put", SportId = indoorTrack.Id, EventType = "Field", SortOrder = 20 },
                new Event { Name = "Long Jump", SportId = indoorTrack.Id, EventType = "Field", SortOrder = 21 },
                new Event { Name = "High Jump", SportId = indoorTrack.Id, EventType = "Field", SortOrder = 22 },
                new Event { Name = "Pole Vault", SportId = indoorTrack.Id, EventType = "Field", SortOrder = 23 },
                new Event { Name = "Triple Jump", SportId = indoorTrack.Id, EventType = "Field", SortOrder = 24 }
            });

            // Outdoor Track Events - Running
            events.AddRange(new[]
            {
                new Event { Name = "100m", SportId = outdoorTrack.Id, EventType = "Running", SortOrder = 1 },
                new Event { Name = "200m", SportId = outdoorTrack.Id, EventType = "Running", SortOrder = 2 },
                new Event { Name = "400m", SportId = outdoorTrack.Id, EventType = "Running", SortOrder = 3 },
                new Event { Name = "800m", SportId = outdoorTrack.Id, EventType = "Running", SortOrder = 4 },
                new Event { Name = "1600m", SportId = outdoorTrack.Id, EventType = "Running", SortOrder = 5 },
                new Event { Name = "3200m", SportId = outdoorTrack.Id, EventType = "Running", SortOrder = 6 },
                new Event { Name = "100m Hurdles", SportId = outdoorTrack.Id, EventType = "Running", SortOrder = 7 },
                new Event { Name = "110m Hurdles", SportId = outdoorTrack.Id, EventType = "Running", SortOrder = 8 },
                new Event { Name = "300m Hurdles", SportId = outdoorTrack.Id, EventType = "Running", SortOrder = 9 },
                new Event { Name = "400m Hurdles", SportId = outdoorTrack.Id, EventType = "Running", SortOrder = 10 }
            });

            // Outdoor Track Events - Relays
            events.AddRange(new[]
            {
                new Event { Name = "4x100m Relay", SportId = outdoorTrack.Id, EventType = "Relay", SortOrder = 15 },
                new Event { Name = "4x400m Relay", SportId = outdoorTrack.Id, EventType = "Relay", SortOrder = 16 },
                new Event { Name = "4x800m Relay", SportId = outdoorTrack.Id, EventType = "Relay", SortOrder = 17 }
            });

            // Outdoor Track Events - Field
            events.AddRange(new[]
            {
                new Event { Name = "Shot Put", SportId = outdoorTrack.Id, EventType = "Field", SortOrder = 20 },
                new Event { Name = "Discus", SportId = outdoorTrack.Id, EventType = "Field", SortOrder = 21 },
                new Event { Name = "Javelin", SportId = outdoorTrack.Id, EventType = "Field", SortOrder = 22 },
                new Event { Name = "Long Jump", SportId = outdoorTrack.Id, EventType = "Field", SortOrder = 23 },
                new Event { Name = "High Jump", SportId = outdoorTrack.Id, EventType = "Field", SortOrder = 24 },
                new Event { Name = "Pole Vault", SportId = outdoorTrack.Id, EventType = "Field", SortOrder = 25 },
                new Event { Name = "Triple Jump", SportId = outdoorTrack.Id, EventType = "Field", SortOrder = 26 }
            });

            // Dash in the Dark Events
            events.AddRange(new[]
            {
                new Event { Name = "1 Mile", SportId = dashInTheDark.Id, EventType = "Running", SortOrder = 1 },
                new Event { Name = "5K", SportId = dashInTheDark.Id, EventType = "Running", SortOrder = 2 }
            });

            context.Events.AddRange(events);
            context.SaveChanges();
        }
    }
}
