namespace SAS_Dumper.Data;

internal sealed record BotInfo
{
    internal ulong SteamID { get; set; }
    internal string AccessToken { get; set; } = null!;
}
