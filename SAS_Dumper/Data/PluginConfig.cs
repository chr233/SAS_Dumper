namespace SAS_Dumper.Data;

/// <summary>
/// 应用配置
/// </summary>
public sealed record PluginConfig
{
    /// <summary>
    /// 自动汇报
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// 后台地址
    /// </summary>
    public string SASUrl { get; private set; } = "";

    /// <summary>
    /// 后台连接密码
    /// </summary>
    public string SASPasswd { get; private set; } = "";

    /// <summary>
    /// 上报周期
    /// </summary>
    public int FeedbackPeriod { get; private set; } = 30;
}
