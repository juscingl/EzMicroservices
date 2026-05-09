namespace BuildingBlocks.Observability.Options;

/// <summary>
/// 日志输出配置，控制 ECS 格式日志的行为。
/// </summary>
public sealed class ElasticLoggingOptions
{
    /// <summary>
    /// 配置节名称。
    /// </summary>
    public const string SectionName = "ElasticLogging";

    /// <summary>
    /// 是否启用日志组件。
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 是否额外输出到控制台。
    /// </summary>
    public bool EnableConsoleSink { get; set; } = true;

    /// <summary>
    /// 日志文件目录。
    /// </summary>
    public string LogDirectory { get; set; } = "logs";

    /// <summary>
    /// 最低日志级别。
    /// </summary>
    public string MinimumLevel { get; set; } = "Information";

    /// <summary>
    /// 本地保留的滚动日志文件数量。
    /// </summary>
    public int RetainedFileCountLimit { get; set; } = 14;
}
