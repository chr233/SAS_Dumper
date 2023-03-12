using ArchiSteamFarm.Steam;
using System.Net;
namespace SAS_Dumper.SAS
{
    internal static class Command
    {
        internal static async Task<string> ResponseSASTest()
        {
            HttpResponseMessage response = await WebRequests.TestSAS().ConfigureAwait(false);

            return FormatStaticResponse(string.Format(CurrentCulture, Langs.SASTest, response.StatusCode == HttpStatusCode.OK ? Langs.Success : Langs.Failure));
        }

        internal static string ResponseSASController(bool enable)
        {
            SASConfig.Enabled = enable;

            return FormatStaticResponse(string.Format(CurrentCulture, Langs.PluginState, SASConfig.Enabled ? Langs.Enabled : Langs.Disabled));
        }

        internal static async Task<string> ResponseSASManualFeedbackAsync()
        {
            HashSet<Bot>? bots = Bot.GetBots("ASF");

            if (bots == null)
            {
                return FormatStaticResponse(string.Format(CurrentCulture, Langs.NoBotsAvilable));
            }

            foreach (Bot bot in bots)
            {
                if (bot.IsConnectedAndLoggedOn)
                {
                    var (_, accessToken) = await bot.ArchiWebHandler.CachedAccessToken.GetValue().ConfigureAwait(false);
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        BotInfoDict.TryAdd(bot.BotName, new(bot.SteamID, accessToken));
                    }
                }
            }

            int count = BotInfoDict.Count;
            if (count > 0)
            {
                await WebRequests.SASFeedback().ConfigureAwait(false);
            }

            return FormatStaticResponse(string.Format(CurrentCulture, Langs.SASManual, count));
        }
    }
}
