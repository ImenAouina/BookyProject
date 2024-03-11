#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;
namespace Booky.Models;

public class MyContext : DbContext
{
    public MyContext(DbContextOptions options) : base(options) { }
    
    public DbSet<Book> Books { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Like> Likes { get; set; }
}