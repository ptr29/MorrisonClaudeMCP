namespace KateMorrisonMCP.Ingestion.Engine;

/// <summary>
/// Summary statistics from an ingestion run
/// </summary>
public class IngestionSummary
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Duration => EndTime - StartTime;

    public int FilesProcessed { get; set; }
    public int TagsProcessed { get; set; }
    public int RecordsInserted { get; set; }
    public int RecordsUpdated { get; set; }
    public int RecordsDeleted { get; set; } // Orphan cleanup
    public int Errors { get; set; }

    public List<string> ErrorMessages { get; set; } = new();

    public override string ToString()
    {
        return $@"
Ingestion Summary
================
Duration: {Duration.TotalSeconds:F2} seconds
Files processed: {FilesProcessed}
Tags processed: {TagsProcessed}
Records inserted: {RecordsInserted}
Records updated: {RecordsUpdated}
Records deleted (orphans): {RecordsDeleted}
Errors: {Errors}
";
    }
}
