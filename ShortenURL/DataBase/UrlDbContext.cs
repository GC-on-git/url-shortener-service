using Microsoft.EntityFrameworkCore;
using ShortenURL.Models;

namespace ShortenURL.DataBase{

public class UrlDbContext : DbContext
{
    public UrlDbContext(DbContextOptions<UrlDbContext> options) : base(options)
    {
    }
    public DbSet<UrlMapping> UrlMappings { get; set; }
}
}