using Microsoft.EntityFrameworkCore;
using SharpMail.Entities;

namespace SharpMail.EntityConfigs;

public class mailConfig : IEntityTypeConfiguration<Mail>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Mail> builder)
    {
        builder.ToTable("tb_mail");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.AccountId)
            .HasColumnName("account_id")
            .IsRequired();

        builder.Property(x => x.Uid)
            .HasColumnName("uid")
            .IsRequired();

        builder.Property(x => x.Read)
            .HasColumnName("read")
            .HasDefaultValue(false);
        
        builder.Property(x => x.Type)
            .HasColumnName("type")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(x => x.Text)
            .HasColumnName("text");
        
        builder.Property(x => x.Subject)
            .HasColumnName("subject");
        
        builder.Property(x => x.From)
            .HasColumnName("from");
        
        builder.Property(x => x.To)
            .HasColumnName("to");
        
        builder.Property(x => x.Date)
            .HasColumnName("date");
        
        builder.Property(x => x.Content)
            .HasColumnName("content");

        builder.Property(x => x.CreateTime)
            .HasColumnName("create_time");
        
        builder.HasOne(m => m.Account)
            .WithMany(a => a.Mails)
            .HasForeignKey(m => m.AccountId);
    }
}