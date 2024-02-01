using Newtonsoft.Json;

namespace SAS_Dumper.Data;

internal sealed record AjaxGetTokenResponse
{
    [JsonProperty("success")]
    public int Success { get; set; }

    [JsonProperty("data")]
    public TokenData? Data { get; set; }

    public sealed record TokenData
    {
        [JsonProperty("webapi_token")]
        public string? WebApiToken { get; set; }
    }
}
