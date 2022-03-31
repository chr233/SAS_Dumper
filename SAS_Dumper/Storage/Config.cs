using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Chrxw.SAS_Dumper.Storage
{
    /// <summary>应用配置</summary>
    internal sealed class Config
    {
        [JsonProperty(Required = Required.DisallowNull)]
        internal bool Enabled { get; set; } = false;

        [JsonProperty(Required = Required.DisallowNull)]
        internal string SASUrl { get; private set; } = "";

        [JsonProperty(Required = Required.DisallowNull)]
        internal string SASPasswd { get; private set; } = "";

        internal int FeedbackPeriod { get; private set; } = 60;

        [JsonConstructor]
        internal Config() { }
    }
}
