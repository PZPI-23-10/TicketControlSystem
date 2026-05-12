using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TicketControlSystem.Data.Models;

namespace Ticket_control_system.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole<int>> roleManager)
    {
        await context.Database.MigrateAsync();

        foreach (var role in new[] { "Admin", "Owner", "User" })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(role));
            }
        }

        var admin = await EnsureUserAsync(userManager, "admin@ticket.local", "Administrator", "Admin");
        var owner = await EnsureUserAsync(userManager, "owner@ticket.local", "Organisation", "Owner");
        var user = await EnsureUserAsync(userManager, "user@ticket.local", "Guest", "User");

        if (!await context.Events.AnyAsync())
        {
            var concert = new Event
            {
                Name = "Концерт у центрі міста",
                EventType = EventType.Concert,
                StartTime = DateTime.UtcNow.AddDays(7).Date.AddHours(18),
                EndTime = DateTime.UtcNow.AddDays(7).Date.AddHours(21),
                OwnerId = owner.Id
            };

            var conference = new Event
            {
                Name = "IT конференція",
                EventType = EventType.Conference,
                StartTime = DateTime.UtcNow.AddDays(14).Date.AddHours(10),
                EndTime = DateTime.UtcNow.AddDays(14).Date.AddHours(17),
                OwnerId = admin.Id
            };

            context.Events.AddRange(concert, conference);
            await context.SaveChangesAsync();

            var standardTariff = new Tariff { EventId = concert.Id, Name = "Стандарт", Price = 350 };
            var vipTariff = new Tariff { EventId = concert.Id, Name = "VIP", Price = 800 };
            var conferenceTariff = new Tariff { EventId = conference.Id, Name = "Учасник", Price = 1200 };
            context.Tariffs.AddRange(standardTariff, vipTariff, conferenceTariff);
            await context.SaveChangesAsync();

            context.Devices.AddRange(
                new Device { EventId = concert.Id, Name = "Вхід A", Location = "Головний вхід", Status = Status.Active },
                new Device { EventId = concert.Id, Name = "Вхід B", Location = "Бічний вхід", Status = Status.Active },
                new Device { EventId = conference.Id, Name = "Реєстрація", Location = "Хол", Status = Status.Active });

            context.Tickets.AddRange(
                new Ticket
                {
                    TariffId = standardTariff.Id,
                    OwnerUserId = user.Id,
                    TicketUid = Guid.NewGuid().ToString(),
                    TicketOwnerName = user.UserName ?? user.Email ?? "Гість",
                    Status = "active",
                    ValidFrom = concert.StartTime.AddHours(-1),
                    ValidTo = concert.EndTime,
                    MaxUses = 1
                },
                new Ticket
                {
                    TariffId = vipTariff.Id,
                    OwnerUserId = user.Id,
                    TicketUid = Guid.NewGuid().ToString(),
                    TicketOwnerName = user.UserName ?? user.Email ?? "Гість",
                    Status = "active",
                    ValidFrom = concert.StartTime.AddHours(-1),
                    ValidTo = concert.EndTime,
                    MaxUses = 2
                });

            await context.SaveChangesAsync();
        }
    }

    private static async Task<User> EnsureUserAsync(
        UserManager<User> userManager,
        string email,
        string username,
        string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new User
            {
                UserName = username,
                Email = email,
                EmailConfirmed = true
            };
            var createResult = await userManager.CreateAsync(user, "123456");
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(string.Join("; ", createResult.Errors.Select(e => e.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            await userManager.AddToRoleAsync(user, role);
        }

        return user;
    }
}
