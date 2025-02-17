using Infrastructure.Persistence.AppDbContext;
using Microsoft.EntityFrameworkCore;
using System;

namespace MyProject.Tests.Helpers
{
    public static class AppDbContextInMemoryFactory
    {
        public static AppDbContext Create(string databaseName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            var context = new AppDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }
    }
}
