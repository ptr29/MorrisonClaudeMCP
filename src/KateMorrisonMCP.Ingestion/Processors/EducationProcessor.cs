using KateMorrisonMCP.Data;
using KateMorrisonMCP.Ingestion.Helpers;
using KateMorrisonMCP.Ingestion.Parsing;

namespace KateMorrisonMCP.Ingestion.Processors;

/// <summary>
/// Processes education tags
/// </summary>
public class EducationProcessor : ITagProcessor
{
    private readonly DatabaseContext _db;
    private readonly CharacterLookup _characterLookup;

    public string TagType => "education";
    public int Priority => 4; // Education depends on characters

    public EducationProcessor(DatabaseContext db, CharacterLookup characterLookup)
    {
        _db = db;
        _characterLookup = characterLookup;
    }

    public async Task<int> ProcessAsync(CanonicalTag tag)
    {
        var characterName = tag.GetRequired("character");
        var institution = tag.GetRequired("institution");
        var degreeType = tag.GetRequired("degree_type");

        // Look up character_id
        var characterId = await _characterLookup.GetRequiredIdAsync(characterName, tag.SourceFile);

        // Check if education record exists
        var existingId = await _db.QuerySingleOrDefaultAsync<int?>(
            @"SELECT id FROM education
              WHERE character_id = @CharacterId
              AND LOWER(institution) = LOWER(@Institution)
              AND LOWER(degree_type) = LOWER(@DegreeType)",
            new { CharacterId = characterId, Institution = institution, DegreeType = degreeType });

        if (existingId.HasValue)
        {
            // Update existing education record
            await _db.ExecuteAsync(@"
                UPDATE education SET
                    field_of_study = COALESCE(@FieldOfStudy, field_of_study),
                    graduation_year = COALESCE(@GraduationYear, graduation_year),
                    honors = COALESCE(@Honors, honors),
                    notes = COALESCE(@Notes, notes),
                    source_file = @SourceFile
                WHERE id = @Id",
                new
                {
                    Id = existingId.Value,
                    FieldOfStudy = tag.GetOptional("field_of_study"),
                    GraduationYear = tag.GetOptionalInt("graduation_year"),
                    Honors = tag.GetOptional("honors"),
                    Notes = tag.GetOptional("notes"),
                    SourceFile = tag.SourceFile
                });

            return existingId.Value;
        }
        else
        {
            // Insert new education record
            await _db.ExecuteAsync(@"
                INSERT INTO education (
                    character_id, institution, degree_type, field_of_study,
                    graduation_year, honors, notes, source_file
                ) VALUES (
                    @CharacterId, @Institution, @DegreeType, @FieldOfStudy,
                    @GraduationYear, @Honors, @Notes, @SourceFile
                )",
                new
                {
                    CharacterId = characterId,
                    Institution = institution,
                    DegreeType = degreeType,
                    FieldOfStudy = tag.GetOptional("field_of_study"),
                    GraduationYear = tag.GetOptionalInt("graduation_year"),
                    Honors = tag.GetOptional("honors"),
                    Notes = tag.GetOptional("notes"),
                    SourceFile = tag.SourceFile
                });

            var newId = await _db.QuerySingleOrDefaultAsync<int>(
                "SELECT last_insert_rowid()");

            return newId;
        }
    }
}
