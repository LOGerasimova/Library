using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Infrastructure.Persistence.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id)
            .HasColumnName("id");

        builder.Property(b => b.Title)
            .HasColumnName("title")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(b => b.Author)
            .HasColumnName("author")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(b => b.YearOfPublication)
            .HasColumnName("year_of_publication")
            .IsRequired();

        builder.Property(b => b.ContentsNode)
            .HasColumnName("contents_node")
            .HasColumnType("xml")
            .HasConversion(
                node => TableOfContentsXmlSerializer.Serialize(node),
                xml => TableOfContentsXmlSerializer.Deserialize(xml));
    }
}
