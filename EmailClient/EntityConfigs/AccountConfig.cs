using EmailClient.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmailClient.EntityConfigs;

public class AccountConfig : IEntityTypeConfiguration<Account>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("tb_account");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        
        builder.Property(x => x.Email)
            .HasColumnName("email")
            .IsRequired();

        builder.Property(x => x.Password)
            .HasColumnName("password");

        builder.Property(x => x.SmtpHost)
            .HasColumnName("smtp_host");

        builder.Property(x => x.SmtpPort)
            .HasColumnName("smtp_port");

        builder.Property(x => x.SmtpSsl)
            .HasColumnName("smtp_ssl")
            .HasDefaultValue(false);
        
        builder.Property(x => x.Pop3Host)
            .HasColumnName("pop3_host");
        
        builder.Property(x => x.Pop3Port)
            .HasColumnName("pop3_port");
        
        builder.Property(x => x.Pop3Ssl)
            .HasColumnName("pop3_ssl")
            .HasDefaultValue(false);
        
        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.CreateTime)
            .HasColumnName("create_time");
        
        builder.Property(x => x.UpdateTime)
            .HasColumnName("update_time");
        
        builder.Property(x => x.LastLoginTime)
            .HasColumnName("last_login_time");
        
    }
}