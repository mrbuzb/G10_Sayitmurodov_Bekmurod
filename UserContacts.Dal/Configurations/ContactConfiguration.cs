using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserContacts.Dal.Entities;

namespace UserContacts.Dal.Configurations;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("Contacts");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.FirstName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(c => c.LastName)
               .HasMaxLength(100);

        builder.Property(c => c.PhoneNumber)
               .HasMaxLength(50);

        builder.Property(c => c.Email)
               .HasMaxLength(100);

        builder.Property(c => c.Address)
               .HasMaxLength(200);

        builder.Property(c => c.CreatedAt)
               .HasDefaultValueSql("GETDATE()");
    }
}
