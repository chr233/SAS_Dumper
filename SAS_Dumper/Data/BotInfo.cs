namespace Chrxw.SAS_Dumper.Data
{
    internal sealed class BotInfo
    {
        public BotInfo(ulong steamID, string accessToken)
        {
            SteamID = steamID;
            AccessToken = accessToken;
        }
        internal ulong SteamID { get; set; }
        internal string AccessToken { get; set; }
    }
}
