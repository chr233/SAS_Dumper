using Newtonsoft.Json;

namespace SAS_Dumper.Data;

internal sealed class SASResult
{
    [JsonProperty(PropertyName = "code")]
    internal int Code { get; set; }

    [JsonProperty(PropertyName = "msg")]
    internal string Message { get; set; } = "";

    [JsonProperty(PropertyName = "result")]
    internal List<List<string>> Result { get; set; } = [];
}
