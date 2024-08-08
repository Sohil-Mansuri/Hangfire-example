using Microsoft.EntityFrameworkCore;

namespace ScheduleJob
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions option) : base(option) { }
       
        public DbSet<Currency> Currency { get; set; }
    }
}
