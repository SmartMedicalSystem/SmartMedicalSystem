using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmbeddingVector : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop legacy columns if present (IF EXISTS checks so this migration doesn't fail
            // when applied to a database that was already partially migrated by hand).
            migrationBuilder.Sql(@"
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'EmbeddingJson' AND Object_ID = OBJECT_ID(N'dbo.PatientRagDocuments'))
BEGIN
    ALTER TABLE dbo.PatientRagDocuments DROP COLUMN EmbeddingJson;
END
");

            migrationBuilder.Sql(@"
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'Embedding' AND Object_ID = OBJECT_ID(N'dbo.PatientRagDocuments'))
BEGIN
    ALTER TABLE dbo.PatientRagDocuments DROP COLUMN Embedding;
END
");

            // The old JSON-encoded embeddings are not valid VECTOR data and can't be converted in
            // place - every row has to be re-embedded and re-indexed anyway (RagService.IndexAsync
            // / the AI report pipeline will do this automatically the next time each patient's
            // reports are generated). Purge the now-orphaned rows so the column below can be added
            // as NOT NULL, matching PatientRagDocumentConfiguration.
            migrationBuilder.Sql(@"
IF EXISTS (SELECT * FROM sys.tables WHERE Name = N'PatientRagDocuments')
BEGIN
    DELETE FROM dbo.PatientRagDocuments;
END
");

            // Requires SQL Server 2025+ (or an Azure SQL tier with VECTOR support). This column
            // is required, not a portable fallback: PatientRagDocument.EmbeddingVector is typed as
            // Microsoft.Data.SqlTypes.SqlVector<float>, which only maps to the native VECTOR type.
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'EmbeddingVector' AND Object_ID = OBJECT_ID(N'dbo.PatientRagDocuments'))
BEGIN
    ALTER TABLE dbo.PatientRagDocuments ADD EmbeddingVector VECTOR(1536) NOT NULL;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (SELECT * FROM sys.columns WHERE Name = N'EmbeddingVector' AND Object_ID = OBJECT_ID(N'dbo.PatientRagDocuments'))
BEGIN
    ALTER TABLE dbo.PatientRagDocuments DROP COLUMN EmbeddingVector;
END
");

            // Restore the legacy JSON column so a down-migration leaves a working schema.
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'EmbeddingJson' AND Object_ID = OBJECT_ID(N'dbo.PatientRagDocuments'))
BEGIN
    ALTER TABLE dbo.PatientRagDocuments ADD EmbeddingJson nvarchar(max) NOT NULL CONSTRAINT DF_PatientRagDocuments_EmbeddingJson DEFAULT (N'');
END
");
        }
    }
}
