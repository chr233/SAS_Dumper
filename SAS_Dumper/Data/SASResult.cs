
using System.Text.Json.Serialization;

namespace SAS_Dumper.Data;

internal sealed class SASResult
{
    [JsonPropertyName("code")]
    internal int Code { get; set; }

    [JsonPropertyName("msg")]
    internal string Message { get; set; } = "";

    [JsonPropertyName("result")]
    internal List<List<string>> Result { get; set; } = [];
}
