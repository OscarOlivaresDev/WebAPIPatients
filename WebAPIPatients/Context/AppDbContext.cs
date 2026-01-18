using Microsoft.EntityFrameworkCore;
using WebAPIPatients.Models;

namespace WebAPIPatients.Context
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext>options): base(options)
        {
            
        }

        public DbSet<Patient> Patients { get; set; }
    }
}
