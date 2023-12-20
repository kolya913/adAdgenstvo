using adAdgenstvo.Models.DataBaseModels;
using Microsoft.EntityFrameworkCore;

public class MDBContext : DbContext
{
    public MDBContext(DbContextOptions<MDBContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Position> Positions { get; set; } = null!;
    public DbSet<ServiceType> ServiceTypes { get; set; } = null!;
    public DbSet<Service> Services { get; set; } = null!;
    public DbSet<PriceService> PriceService { get; set; } = null!;
    public DbSet<Order> Order { get; set; } = null!;
    public DbSet<OrderServicePrice> OrderServicePrice { get; set; } = null!;
    public DbSet<LayoutProject> Projects { get; set; } = null!;
    public DbSet<LayoutPhoto> Photos { get; set; } = null!;
    public DbSet<Contract> Contract { get; set; } = null!;
}
