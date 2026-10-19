using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Domain.Entities;
using System.Reflection;

namespace SmartRestaurant.Infrastructure.Contexts
{
    //este es el contexto de la base de datos que se utilizará para interactuar con la base de datos.
    public class SmartRestaurantDbContext : DbContext
    {
        public SmartRestaurantDbContext(DbContextOptions<SmartRestaurantDbContext> options) : base(options)
        {
        }

        // Aquí irán los DbSets de las entidades.
        public DbSet<Reservation> Reservations => Set<Reservation>();

        public DbSet<Table> Tables => Set<Table>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Aplica automáticamente todas las configuraciones de Fluent API que estén en el ensamblado
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}