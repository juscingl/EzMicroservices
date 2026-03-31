namespace BuildingBlocks.Nacos.Options;

public sealed class NacosOptions
{
    public const string SectionName = "Nacos";

    public bool Enabled { get; set; } = true;

    public bool LoadConfiguration { get; set; } = true;

    public bool RegisterService { get; set; } = true;

    public string ServerAddress { get; set; } = "http://localhost:8848";

    public string NamespaceId { get; set; } = "public";

    public string Group { get; set; } = "DEFAULT_GROUP";

    public string ClusterName { get; set; } = "DEFAULT";

    public string? ServiceName { get; set; }

    public string? ConfigDataId { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string? InstanceIp { get; set; }

    public int InstancePort { get; set; }

    public Dictionary<string, string> Metadata { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public int RequestTimeoutSeconds { get; set; } = 10;
}
