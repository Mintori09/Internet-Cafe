using InternetCafeManagementSystem.Models;

namespace InternetCafeManagementSystem.Data;

public static class DatabaseSeeder
{
    public static void Seed(ComputerDbContext context)
    {
        // Add admin user if not exists
        if (!context.Users.Any(u => u.Username == "admin"))
        {
            context.Users.Add(new User
            {
                Username = "admin",
                Password = "admin123",
                FullName = "Administrator",
                Balance = 1000000,
                IsAdmin = true
            });
        }

        // Add sample users if not exists
        if (!context.Users.Any(u => u.Username == "user1"))
        {
            var users = new List<User>
            {
                new User { Username = "user1", Password = "123456", FullName = "Nguyễn Văn A", Balance = 500000, IsAdmin = false },
                new User { Username = "user2", Password = "123456", FullName = "Trần Thị B", Balance = 300000, IsAdmin = false },
                new User { Username = "user3", Password = "123456", FullName = "Lê Văn C", Balance = 200000, IsAdmin = false },
                new User { Username = "user4", Password = "123456", FullName = "Phạm Thị D", Balance = 100000, IsAdmin = false },
                new User { Username = "user5", Password = "123456", FullName = "Hoàng Văn E", Balance = 150000, IsAdmin = false }
            };
            context.Users.AddRange(users);
        }

        // Add computers if not exists
        if (!context.Computers.Any())
        {
            var computers = new List<Computer>
            {
                new Computer { Name = "PC-001", Status = true, PricePerHour = 10000 },
                new Computer { Name = "PC-002", Status = true, PricePerHour = 10000 },
                new Computer { Name = "PC-003", Status = true, PricePerHour = 10000 },
                new Computer { Name = "PC-004", Status = true, PricePerHour = 10000 },
                new Computer { Name = "PC-005", Status = false, PricePerHour = 10000 },
                new Computer { Name = "PC-006", Status = true, PricePerHour = 10000 },
                new Computer { Name = "PC-007", Status = true, PricePerHour = 10000 },
                new Computer { Name = "PC-008", Status = true, PricePerHour = 10000 },
                new Computer { Name = "PC-009", Status = false, PricePerHour = 10000 },
                new Computer { Name = "PC-010", Status = true, PricePerHour = 10000 }
            };
            context.Computers.AddRange(computers);
        }

        // Add sample sessions if not exists
        if (!context.UserSessions.Any())
        {
            var users = context.Users.ToList();
            var computers = context.Computers.ToList();
            var random = new Random();

            var sessions = new List<UserSession>();
            for (int i = 0; i < 20; i++)
            {
                var user = users[random.Next(users.Count)];
                var computer = computers[random.Next(computers.Count)];
                var startTime = DateTime.Now.AddHours(-random.Next(1, 24));
                var endTime = startTime.AddHours(random.Next(1, 5));

                sessions.Add(new UserSession
                {
                    UserId = user.Id,
                    ComputerId = computer.Id,
                    StartTime = startTime,
                    EndTime = endTime,
                    TotalCost = random.Next(10000, 50000)
                });
            }
            context.UserSessions.AddRange(sessions);
        }

        context.SaveChanges();
    }
}