using Microsoft.EntityFrameworkCore;

namespace Mess.Data;

public class AppDatabaseContext : DbContext
{
    
    public DbSet<Group> Groups { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Entry> Entries { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        Config.GetFromEnvironment();
        optionsBuilder.UseSqlite(Config.Instance?.DbConnectionString ?? new Config().DbConnectionString, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
    }
}