
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using be_scrapping_service.Entity;
namespace be_scrapping_service.Context;

public class UsersContext : IdentityUserContext<User>
{
    private readonly IConfiguration _configuration;

  
    public UsersContext (DbContextOptions<UsersContext> options, IConfiguration configuration)
        : base(options)
    {
            this._configuration = configuration;
    }

     public DbSet<History> Histories { get; set; }

        
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer(this._configuration.GetConnectionString("SqlConnection"));
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
