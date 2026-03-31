namespace BuildingBlocks.Observability.Options;

public sealed class ElasticLoggingOptions
{
    public const string SectionName = "ElasticLogging";

    public bool Enabled { get; set; } = true;

    public bool EnableConsoleSink { get; set; } = true;

    public string LogDirectory { get; set; } = "logs";

    public string MinimumLevel { get; set; } = "Information";

    public int RetainedFileCountLimit { get; set; } = 14;
}
