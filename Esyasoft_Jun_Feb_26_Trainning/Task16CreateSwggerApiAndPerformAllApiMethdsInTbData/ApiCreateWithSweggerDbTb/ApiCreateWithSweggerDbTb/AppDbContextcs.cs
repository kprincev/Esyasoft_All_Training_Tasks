using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using static ApiCreateWithSweggerDbTb.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Consumer> Consumers { get; set; }
}
