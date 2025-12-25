using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BlazorApp1.Data
{
    public class AppDbContext : DbContext
    {
        // Este constructor permite que Program.cs le pase la configuración correcta
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<RegistroLed> Registros { get; set; }
    }
}