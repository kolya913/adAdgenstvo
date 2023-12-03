using adAdgenstvo.Models;
using Microsoft.EntityFrameworkCore;
using adAdgenstvo.Models.DataBaseModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

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
}
