using KateMorrisonMCP.Ingestion.Parsing;

namespace KateMorrisonMCP.Ingestion.Processors;

/// <summary>
/// Interface for tag processors that convert canonical tags to database records
/// </summary>
public interface ITagProcessor
{
    /// <summary>
    /// The tag type this processor handles (character, location, schedule, etc.)
    /// </summary>
    string TagType { get; }

    /// <summary>
    /// Processing priority for dependency ordering (1 = first, 6 = last)
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Processes a tag and upserts it to the database
    /// Returns the record ID (either newly inserted or existing updated record)
    /// </summary>
    Task<int> ProcessAsync(CanonicalTag tag);
}
