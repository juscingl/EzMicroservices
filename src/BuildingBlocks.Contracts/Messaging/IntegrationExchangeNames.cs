namespace BuildingBlocks.Contracts.Messaging;

/// <summary>
/// 平台消息交换机名称常量。
/// </summary>
public static class IntegrationExchangeNames
{
    /// <summary>
    /// 平台级 Topic Exchange，承载跨服务集成事件。
    /// </summary>
    public const string Platform = "eztrade.platform";
}
