using ArchiSteamFarm.Steam;
using Newtonsoft.Json;
using SAS_Dumper.Data;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace SAS_Dumper.SAS;

internal static class Command
{
    /// <summary>
    /// 测试后台连通性
    /// </summary>
    /// <returns></returns>
    internal static async Task<string?> ResponseSASTest()
    {
        var response = await WebRequests.TestSAS().ConfigureAwait(false);

        return FormatStaticResponse(Langs.SASTest, response.StatusCode == HttpStatusCode.OK ? Langs.Success : Langs.Failure);
    }

    /// <summary>
    /// 控制自动上传
    /// </summary>
    /// <param name="enable"></param>
    /// <returns></returns>
    internal static string? ResponseSASController(bool enable)
    {
        SASConfig.Enabled = enable;

        return FormatStaticResponse(Langs.PluginState, SASConfig.Enabled ? Langs.Enabled : Langs.Disabled);
    }

    /// <summary>
    /// 手动刷新Token
    /// </summary>
    /// <returns></returns>
    internal static async Task<string?> ResponseSASFresh()
    {
        var bots = Bot.GetBots("ASF");

        BotTokenCache.Clear();

        if (bots == null || bots.Count == 0)
        {
            return FormatStaticResponse(Langs.NoBotsAvilable);
        }

        foreach (var bot in bots.Where(x => x.IsConnectedAndLoggedOn))
        {
            var accessToken = bot.AccessToken;
            if (!string.IsNullOrEmpty(accessToken))
            {
                BotTokenCache.TryAdd(
                    bot.BotName,
                    new BotInfo { SteamID = bot.SteamID, AccessToken = accessToken }
                );
            }
        }

        await WebRequests.SASFeedback(BotTokenCache).ConfigureAwait(false);

        return FormatStaticResponse(Langs.SASManual, BotTokenCache.Count);
    }

    /// <summary>
    /// 手动汇报
    /// </summary>
    /// <returns></returns>
    internal static async Task<string?> ResponseSASManualFeedback()
    {
        if (!BotTokenCache.IsEmpty)
        {
            await WebRequests.SASFeedback(BotTokenCache).ConfigureAwait(false);
        }

        return FormatStaticResponse(Langs.SASManual, BotTokenCache.Count);
    }

    /// <summary>
    /// 批量导出Token
    /// </summary>
    /// <returns></returns>
    internal static async Task<string?> ResponseSASDump(string? desc = null)
    {
        var bots = Bot.GetBots("ASF");

        if (bots == null || bots.Count == 0)
        {
            return FormatStaticResponse(string.Format(Langs.NoBotsAvilable));
        }

        if (!string.IsNullOrEmpty(desc))
        {
            desc = desc.Replace('-', '_').Replace(' ', '_');
            if (!desc.EndsWith('_'))
            {
                desc += '_';
            }
        }
        else
        {
            desc = "";
        }

        int count = 0;
        StringBuilder sb = new();

        foreach (var (botName, botInfo) in BotTokenCache)
        {
            if (!string.IsNullOrEmpty(botInfo.AccessToken))
            {
                count++;
                sb.AppendLine($"{botInfo.SteamID}, {botInfo.AccessToken}, {desc}{botName}");
            }
        }

        if (count > 0)
        {
            try
            {
                string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string filePath = Path.Combine(folderPath, $"SAS_{DateTime.Now:yyyy-MM-dd}.txt");

                using var file = File.CreateText(filePath);

                var setting = new JsonSerializerSettings {
                    DefaultValueHandling = DefaultValueHandling.Include
                };

                await file.WriteAsync(sb).ConfigureAwait(false);
                await file.FlushAsync().ConfigureAwait(false);

                try
                {
                    var p = new Process {
                        StartInfo =
                        {
                            FileName = "explorer",
                            WorkingDirectory = folderPath,
                            Arguments = "/select,"+ filePath
                        }
                    };
                    p.Start();
                }
                catch (Exception ex)
                {
                    ASFLogger.LogGenericException(ex);
                }

                return FormatStaticResponse(Langs.TokenDumpSuccess, filePath);
            }
            catch (Exception ex)
            {
                ASFLogger.LogGenericException(ex);
                return FormatStaticResponse(Langs.TokenDumpFailed, ex);
            }
        }
        else
        {
            return FormatStaticResponse(string.Format(Langs.NoBotsAvilable));
        }
    }

    internal static async void RefreshTokens(object? _)
    {
        var botNames = BotTokenCache.Keys.ToList();

        int changeCount = 0;

        var diffBotInfo = new Dictionary<string, BotInfo>();

        foreach (string botName in botNames)
        {
            if (BotTokenCache.TryGetValue(botName, out var botInfo))
            {
                var bot = Bot.GetBot(botName);

                if (bot != null && bot.IsConnectedAndLoggedOn)
                {
                    var accessToken = bot.AccessToken;
                    if (!string.IsNullOrEmpty(accessToken) && botInfo.AccessToken != accessToken)
                    {
                        botInfo.AccessToken = accessToken;
                        diffBotInfo.Add(botName, botInfo);
                        changeCount++;
                    }
                }
            }
        }

        if (changeCount > 0)
        {
            ASFLogger.LogGenericInfo(string.Format("更新了 {0} 个Token", changeCount));
            await WebRequests.SASFeedback(diffBotInfo).ConfigureAwait(false);
        }
    }
}