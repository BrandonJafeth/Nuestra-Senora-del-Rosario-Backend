using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.AppDbContext
{
    public partial class AppDbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureAdministrativeEntities(modelBuilder);
            ConfigureInformativeEntities(modelBuilder);
        }
    }
}
