using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace SharpMail.Entities;

/// <summary>
/// 数据库管理类
/// </summary>
public class EmailClientContext : DbContext
{
    public EmailClientContext(DbContextOptions<EmailClientContext> options)
        : base(options)
    {
        Database.Migrate();
    }
    
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Mail> Mails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}