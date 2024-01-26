namespace SAS_Dumper.Data;

internal sealed record BotInfo
{
    public ulong SteamID { get; set; }
    public string AccessToken { get; set; } = null!;

    public DateTime ExpiredAt { get; set; }
}
