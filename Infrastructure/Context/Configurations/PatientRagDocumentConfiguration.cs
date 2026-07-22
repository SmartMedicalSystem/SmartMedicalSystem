using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    /// <summary>
    /// EF Core configuration for the RAG vector-store table.
    ///
    /// The embedding vector is persisted as an nvarchar(max) JSON array (see
    /// PatientRagDocument.EmbeddingJson) so this works unmodified on any supported SQL Server
    /// edition/version. If you are on SQL Server 2025+ and want native vector search, swap the
    /// EmbeddingJson column for a VECTOR(n) column here and push similarity search down to SQL
    /// via VECTOR_DISTANCE instead of the in-memory cosine similarity used by RagService.
    /// </summary>
    public class PatientRagDocumentConfiguration : IEntityTypeConfiguration<PatientRagDocument>
    {
        public void Configure(EntityTypeBuilder<PatientRagDocument> builder)
        {
            builder.ToTable("PatientRagDocuments");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.SourceType)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(d => d.Content)
                .HasMaxLength(8000)
                .IsRequired();

            builder.Property(d => d.EmbeddingJson)
                .HasColumnType("nvarchar(max)")
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
