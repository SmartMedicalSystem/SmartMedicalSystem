using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    /// <summary>
    /// EF Core configuration for the RAG vector-store table.
    ///
    /// The embedding is persisted using SQL Server 2025's native VECTOR(n) type via EF Core 10's
    /// built-in SqlVector support, which lets similarity search be pushed down to the database
    /// with EF.Functions.VectorDistance ("cosine") instead of being computed in memory.
    /// The dimension (1536) must match whatever embedding model IMedicalAIClient.EmbedAsync uses -
    /// changing embedding models to a different dimensionality requires a migration that alters
    /// this column (and re-indexing every existing document).
    /// </summary>
    public class PatientRagDocumentConfiguration : IEntityTypeConfiguration<PatientRagDocument>
    {
        public void Configure(EntityTypeBuilder<PatientRagDocument> builder)
        {
            builder.ToTable("PatientRagDocuments");

            builder.HasKey(d => d.Id);

            // Store enum as string in database for readability and compatibility with existing text values
            builder.Property(d => d.SourceType)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(d => d.Content)
                .HasMaxLength(8000)
                .IsRequired();

            builder.Property(d => d.EmbeddingVector)
                .HasColumnType("vector(1536)")
                .IsRequired();

            builder.Property(d => d.EmbeddingModel)
                .HasMaxLength(200);

            builder.HasOne(d => d.Patient)
                .WithMany()
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.PatientResultRef)
                .WithMany()
                .HasForeignKey(d => d.PatientResultId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(d => d.PatientId);
            builder.HasIndex(d => d.PatientResultId);
            builder.HasIndex(d => d.SourceType);

            builder.HasQueryFilter(d => !d.IsDeleted);
        }
    }
}
