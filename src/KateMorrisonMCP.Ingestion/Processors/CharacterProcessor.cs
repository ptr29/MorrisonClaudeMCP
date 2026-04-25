using KateMorrisonMCP.Data;
using KateMorrisonMCP.Ingestion.Helpers;
using KateMorrisonMCP.Ingestion.Parsing;
using KateMorrisonMCP.Ingestion.SchemaManagement;

namespace KateMorrisonMCP.Ingestion.Processors;

/// <summary>
/// Processes character tags
/// </summary>
public class CharacterProcessor : ITagProcessor
{
    private readonly DatabaseContext _db;

    public string TagType => "character";
    public int Priority => 1; // Characters must be created first (no dependencies)

    public CharacterProcessor(DatabaseContext db)
    {
        _db = db;
    }

    public async Task<int> ProcessAsync(CanonicalTag tag)
    {
        var fullName = tag.GetRequired("name");

        // Check if character exists
        var existingId = await _db.QuerySingleOrDefaultAsync<int?>(
            "SELECT id FROM characters WHERE LOWER(full_name) = LOWER(@FullName)",
            new { FullName = fullName });

        // Parse height
        int? heightInches = null;
        if (tag.HasField("height"))
        {
            heightInches = HeightParser.Parse(tag.GetOptional("height"));
        }

        // Parse distinctive features (comma-delimited → JSON array)
        string? distinctiveFeatures = null;
        if (tag.HasField("distinctive_features"))
        {
            distinctiveFeatures = JsonArrayHelper.ToJsonArray(tag.GetOptional("distinctive_features"));
        }

        if (existingId.HasValue)
        {
            // Update existing character
            await _db.ExecuteAsync(@"
                UPDATE characters SET
                    preferred_name = COALESCE(@PreferredName, preferred_name),
                    age = COALESCE(@Age, age),
                    birthday = COALESCE(@Birthday, birthday),
                    birth_year = COALESCE(@BirthYear, birth_year),
                    height_inches = COALESCE(@HeightInches, height_inches),
                    weight_lbs = COALESCE(@WeightLbs, weight_lbs),
                    build = COALESCE(@Build, build),
                    hair_color = COALESCE(@HairColor, hair_color),
                    hair_length = COALESCE(@HairLength, hair_length),
                    eye_color = COALESCE(@EyeColor, eye_color),
                    distinctive_features = COALESCE(@DistinctiveFeatures, distinctive_features),
                    occupation = COALESCE(@Occupation, occupation),
                    employer = COALESCE(@Employer, employer),
                    job_title = COALESCE(@JobTitle, job_title),
                    work_location = COALESCE(@WorkLocation, work_location),
                    work_schedule_type = COALESCE(@WorkScheduleType, work_schedule_type),
                    phone = COALESCE(@Phone, phone),
                    email = COALESCE(@Email, email),
                    character_type = COALESCE(@CharacterType, character_type),
                    is_alive = COALESCE(@IsAlive, is_alive),
                    source_file = @SourceFile,
                    updated_at = datetime('now')
                WHERE id = @Id",
                new
                {
                    Id = existingId.Value,
                    PreferredName = tag.GetOptional("preferred_name"),
                    Age = tag.GetOptionalInt("age"),
                    Birthday = tag.GetOptional("birthday"),
                    BirthYear = tag.GetOptionalInt("birth_year"),
                    HeightInches = heightInches,
                    WeightLbs = tag.GetOptionalInt("weight"),
                    Build = tag.GetOptional("build"),
                    HairColor = tag.GetOptional("hair_color"),
                    HairLength = tag.GetOptional("hair_length"),
                    EyeColor = tag.GetOptional("eye_color"),
                    DistinctiveFeatures = distinctiveFeatures,
                    Occupation = tag.GetOptional("occupation"),
                    Employer = tag.GetOptional("employer"),
                    JobTitle = tag.GetOptional("job_title"),
                    WorkLocation = tag.GetOptional("work_location"),
                    WorkScheduleType = tag.GetOptional("work_schedule_type"),
                    Phone = tag.GetOptional("phone"),
                    Email = tag.GetOptional("email"),
                    CharacterType = tag.GetOptional("character_type"),
                    IsAlive = tag.GetOptionalInt("is_alive"),
                    SourceFile = tag.SourceFile
                });

            return existingId.Value;
        }
        else
        {
            // Insert new character
            await _db.ExecuteAsync(@"
                INSERT INTO characters (
                    full_name, preferred_name, age, birthday, birth_year,
                    height_inches, weight_lbs, build, hair_color, hair_length, eye_color,
                    distinctive_features, occupation, employer, job_title, work_location,
                    work_schedule_type, phone, email, character_type, is_alive,
                    source_file, created_at, updated_at
                ) VALUES (
                    @FullName, @PreferredName, @Age, @Birthday, @BirthYear,
                    @HeightInches, @WeightLbs, @Build, @HairColor, @HairLength, @EyeColor,
                    @DistinctiveFeatures, @Occupation, @Employer, @JobTitle, @WorkLocation,
                    @WorkScheduleType, @Phone, @Email, @CharacterType, @IsAlive,
                    @SourceFile, datetime('now'), datetime('now')
                )",
                new
                {
                    FullName = fullName,
                    PreferredName = tag.GetOptional("preferred_name"),
                    Age = tag.GetOptionalInt("age"),
                    Birthday = tag.GetOptional("birthday"),
                    BirthYear = tag.GetOptionalInt("birth_year"),
                    HeightInches = heightInches,
                    WeightLbs = tag.GetOptionalInt("weight"),
                    Build = tag.GetOptional("build"),
                    HairColor = tag.GetOptional("hair_color"),
                    HairLength = tag.GetOptional("hair_length"),
                    EyeColor = tag.GetOptional("eye_color"),
                    DistinctiveFeatures = distinctiveFeatures,
                    Occupation = tag.GetOptional("occupation"),
                    Employer = tag.GetOptional("employer"),
                    JobTitle = tag.GetOptional("job_title"),
                    WorkLocation = tag.GetOptional("work_location"),
                    WorkScheduleType = tag.GetOptional("work_schedule_type"),
                    Phone = tag.GetOptional("phone"),
                    Email = tag.GetOptional("email"),
                    CharacterType = tag.GetOptional("character_type") ?? "secondary",
                    IsAlive = tag.GetOptionalInt("is_alive") ?? 1,
                    SourceFile = tag.SourceFile
                });

            var newId = await _db.QuerySingleOrDefaultAsync<int>(
                "SELECT last_insert_rowid()");

            return newId;
        }
    }
}
