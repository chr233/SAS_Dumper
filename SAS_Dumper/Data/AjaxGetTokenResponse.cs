using System.Text.Json.Serialization;

namespace SAS_Dumper.Data;

internal sealed record AjaxGetTokenResponse
{
    [JsonPropertyName("success")]
    public int Success { get; set; }

    [JsonPropertyName("data")]
    public TokenData? Data { get; set; }

    public sealed record TokenData
    {
        [JsonPropertyName("webapi_token")]
        public string? WebApiToken { get; set; }
    }
}
