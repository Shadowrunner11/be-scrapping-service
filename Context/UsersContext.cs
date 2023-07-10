
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
        
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // It would be a good idea to move the connection string to user secrets
        options.UseSqlServer(this._configuration.GetConnectionString("SqlConnection"));
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
