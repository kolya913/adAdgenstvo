using adAdgenstvo.Models;
using Microsoft.EntityFrameworkCore;

public class MDBContext : DbContext
{
    public MDBContext(DbContextOptions<MDBContext> options) : base(options)
    {
    }

    public DbSet<Position> Positions { get; set; }
    public DbSet<Role> Role { get; set; }
    public DbSet<Worker> Worker { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<OrderService> OrderServices { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Place> Places { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Internet> Internet { get; set; }
    public DbSet<ServicePlaceInternet> ServicePlaceInternet { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Define relationships, constraints, etc. if needed
        modelBuilder.Entity<Worker>()
            .HasOne(w => w.Role)
            .WithMany()
            .HasForeignKey(w => w.IdRole);

        modelBuilder.Entity<Worker>()
            .HasOne(w => w.Position)
            .WithMany()
            .HasForeignKey(w => w.IdPosition);

        modelBuilder.Entity<Client>()
            .HasOne(c => c.Role)
            .WithMany()
            .HasForeignKey(c => c.IdRole);

        // Define other relationships as needed

        base.OnModelCreating(modelBuilder);
    }
}
