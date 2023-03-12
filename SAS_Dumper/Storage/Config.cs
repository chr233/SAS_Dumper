using Newtonsoft.Json;

namespace SAS_Dumper.Storage
{
    /// <summary>应用配置</summary>
    public sealed record Config
    {
        [JsonProperty(Required = Required.DisallowNull)]
        public bool Enabled { get; set; } = false;

        [JsonProperty(Required = Required.DisallowNull)]
        public string SASUrl { get; private set; } = "";

        [JsonProperty(Required = Required.DisallowNull)]
        public string SASPasswd { get; private set; } = "";

        [JsonProperty(Required = Required.DisallowNull)]
        public int FeedbackPeriod { get; private set; } = 30;
    }
}
