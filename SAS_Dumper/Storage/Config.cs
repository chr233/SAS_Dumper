using Newtonsoft.Json;

namespace SAS_Dumper.Storage
{
    /// <summary>
    /// 应用配置
    /// </summary>
    public sealed record Config
    {
        /// <summary>
        /// 自动汇报
        /// </summary>
        [JsonProperty(Required = Required.DisallowNull)]
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// 后台地址
        /// </summary>
        [JsonProperty(Required = Required.DisallowNull)]
        public string SASUrl { get; private set; } = "";

        /// <summary>
        /// 后台连接密码
        /// </summary>
        [JsonProperty(Required = Required.DisallowNull)]
        public string SASPasswd { get; private set; } = "";

        /// <summary>
        /// 上报周期
        /// </summary>
        [JsonProperty(Required = Required.DisallowNull)]
        public int FeedbackPeriod { get; private set; } = 30;
    }
}
