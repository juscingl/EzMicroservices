namespace BuildingBlocks.Search.Options;

public sealed class ElasticsearchOptions
{
    public const string SectionName = "Elasticsearch";

    public bool Enabled { get; set; } = true;

    public string Uri { get; set; } = "http://localhost:9200";

    public string DefaultIndexPrefix { get; set; } = "eztrade";

    public string? UserName { get; set; }

    public string? Password { get; set; }
}
