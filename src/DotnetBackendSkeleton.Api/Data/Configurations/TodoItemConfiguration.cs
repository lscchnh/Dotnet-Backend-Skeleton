using DotnetBackendSkeleton.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotnetBackendSkeleton.Api.Data.Configurations;

public sealed class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.ToTable("todo_items");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .HasMaxLength(TodoItem.TitleMaxLength)
            .IsRequired();

        // Stored as text: readable in the database and robust to enum reordering.
        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.HasIndex(t => t.CreatedAt);
    }
}
