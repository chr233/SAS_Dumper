using ArchiSteamFarm.Core;
using ArchiSteamFarm.Steam;
using Newtonsoft.Json;
using SAS_Dumper.Data;
using SteamKit2.Internal;
using System.Collections.Concurrent;
using System.Net;
using System.Text;
using static SteamKit2.GC.Dota.Internal.CMsgDOTABotDebugInfo;

namespace SAS_Dumper.SAS
{
    internal static class Command
    {
        /// <summary>
        /// 测试后台连通性
        /// </summary>
        /// <returns></returns>
        internal static async Task<string> ResponseSASTest()
        {
            HttpResponseMessage response = await WebRequests.TestSAS().ConfigureAwait(false);

            return FormatStaticResponse(string.Format(CurrentCulture, Langs.SASTest, response.StatusCode == HttpStatusCode.OK ? Langs.Success : Langs.Failure));
        }

        /// <summary>
        /// 控制自动上传
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        internal static string ResponseSASController(bool enable)
        {
            SASConfig.Enabled = enable;

            return FormatStaticResponse(string.Format(CurrentCulture, Langs.PluginState, SASConfig.Enabled ? Langs.Enabled : Langs.Disabled));
        }

        /// <summary>
        /// 手动汇报
        /// </summary>
        /// <returns></returns>
        internal static async Task<string> ResponseSASManualFeedback()
        {
            HashSet<Bot>? bots = Bot.GetBots("ASF");

            if (bots == null)
            {
                return FormatStaticResponse(string.Format(CurrentCulture, Langs.NoBotsAvilable));
            }

            var BotTokenCache = new ConcurrentDictionary<string, BotInfo>();

            foreach (var bot in bots.Where(x => x.IsConnectedAndLoggedOn))
            {
                var (_, accessToken) = await bot.ArchiWebHandler.CachedAccessToken.GetValue().ConfigureAwait(false);
                if (!string.IsNullOrEmpty(accessToken))
                {
                    BotTokenCache.TryAdd(
                        bot.BotName,
                        new BotInfo { SteamID=bot.SteamID, AccessToken=accessToken, }
                    );
                }
            }

            if (BotTokenCache.Any())
            {
                await WebRequests.SASFeedback(BotTokenCache).ConfigureAwait(false);
            }

            return FormatStaticResponse(string.Format(CurrentCulture, Langs.SASManual, BotTokenCache.Count));
        }

        /// <summary>
        /// 批量导出Token
        /// </summary>
        /// <returns></returns>
        internal static async Task<string> ResponseSASDump(string? desc = null)
        {
            HashSet<Bot>? bots = Bot.GetBots("ASF");

            if (bots == null)
            {
                return FormatStaticResponse(string.Format(CurrentCulture, Langs.NoBotsAvilable));
            }

            int count = 0;
            StringBuilder sb = new();

            foreach (var bot in bots.Where(x => x.IsConnectedAndLoggedOn))
            {
                var (_, accessToken) = await bot.ArchiWebHandler.CachedAccessToken.GetValue().ConfigureAwait(false);
                if (!string.IsNullOrEmpty(accessToken))
                {
                    count++;
                    if (!string.IsNullOrEmpty(desc))
                    {
                        sb.AppendLine($"{bot.SteamID}, {accessToken}, {desc}_{bot.BotName}");
                    }
                    else
                    {
                        sb.AppendLine($"{bot.SteamID}, {accessToken}, {bot.BotName}");
                    }
                }
            }

            if (count>0)
            {
                try
                {
                    string currentPath = MyLocation ?? ".";
                    string pluginFolder = Path.GetDirectoryName(currentPath) ?? ".";
                    string filePath = Path.Combine(pluginFolder, bot.BotName + ".json");

                    using var file = File.CreateText(filePath);

                    var setting = new JsonSerializerSettings {
                        DefaultValueHandling = DefaultValueHandling.Include
                    };

                    var json = JsonConvert.SerializeObject(inventory);
                    await file.WriteAsync(json).ConfigureAwait(false);

                    await file.FlushAsync().ConfigureAwait(false);

                    return bot.FormatBotResponse($"物品栏已保存至 {filePath}");
                }
                catch (Exception ex)
                {
                    ASFLogger.LogGenericException(ex);
                    return bot.FormatBotResponse($"导出物品栏遇到错误 {ex}");
                }




                await WebRequests.SASFeedback(BotTokenCache).ConfigureAwait(false);
            }
            else
            {
                return FormatStaticResponse(string.Format(CurrentCulture, Langs.SAD, count));
            }

            return FormatStaticResponse(string.Format(CurrentCulture, Langs.SASManual, BotTokenCache.Count));
        }



    }
}
