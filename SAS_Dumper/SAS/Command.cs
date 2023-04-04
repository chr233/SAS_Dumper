using ArchiSteamFarm.Core;
using ArchiSteamFarm.Steam;
using Newtonsoft.Json;
using SAS_Dumper.Data;
using SteamKit2.Internal;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

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

            return FormatStaticResponse(string.Format( Langs.SASTest, response.StatusCode == HttpStatusCode.OK ? Langs.Success : Langs.Failure));
        }

        /// <summary>
        /// 控制自动上传
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        internal static string ResponseSASController(bool enable)
        {
            SASConfig.Enabled = enable;

            return FormatStaticResponse(string.Format( Langs.PluginState, SASConfig.Enabled ? Langs.Enabled : Langs.Disabled));
        }

        /// <summary>
        /// 手动刷新Token
        /// </summary>
        /// <returns></returns>
        internal static async Task<string> ResponseSASFresh(IDictionary<string, BotInfo> botTokens)
        {
            var bots = Bot.GetBots("ASF");

            botTokens.Clear();

            if (bots == null || !bots.Any())
            {
                return FormatStaticResponse(string.Format( Langs.NoBotsAvilable));
            }

            foreach (var bot in bots.Where(x => x.IsConnectedAndLoggedOn))
            {
                var (_, accessToken) = await bot.ArchiWebHandler.CachedAccessToken.GetValue().ConfigureAwait(false);
                if (!string.IsNullOrEmpty(accessToken))
                {
                    botTokens.TryAdd(
                        bot.BotName,
                        new BotInfo { SteamID=bot.SteamID, AccessToken=accessToken, }
                    );
                }
            }

            return FormatStaticResponse(string.Format( Langs.SASManual, botTokens.Count));
        }


        /// <summary>
        /// 手动汇报
        /// </summary>
        /// <returns></returns>
        internal static async Task<string> ResponseSASManualFeedback(IDictionary<string, BotInfo> botTokens)
        {
            if (botTokens.Any())
            {
                await WebRequests.SASFeedback(botTokens).ConfigureAwait(false);
            }

            return FormatStaticResponse(string.Format( Langs.SASManual, botTokens.Count));
        }

        /// <summary>
        /// 批量导出Token
        /// </summary>
        /// <returns></returns>
        internal static async Task<string> ResponseSASDump(IDictionary<string, BotInfo> botTokens, string? desc = null)
        {
            var bots = Bot.GetBots("ASF");

            if (bots == null || !bots.Any())
            {
                return FormatStaticResponse(string.Format( Langs.NoBotsAvilable));
            }

            if (!string.IsNullOrEmpty(desc))
            {
                desc=desc.Replace('-', '_').Replace(' ', '_');
                if (!desc.EndsWith('_'))
                {
                    desc += '_';
                }
            }
            else
            {
                desc="";
            }

            int count = 0;
            StringBuilder sb = new();

            foreach (var (botName, botInfo) in botTokens)
            {
                if (!string.IsNullOrEmpty(botInfo.AccessToken))
                {
                    count++;
                    sb.AppendLine($"{botInfo.SteamID}, {botInfo.AccessToken}, {desc}{botName}");
                }
            }

            if (count>0)
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

                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
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

                    return FormatStaticResponse(string.Format(Langs.TokenDumpSuccess, filePath));
                }
                catch (Exception ex)
                {
                    ASFLogger.LogGenericException(ex);
                    return FormatStaticResponse(string.Format(Langs.TokenDumpFailed, ex));
                }
            }
            else
            {
                return FormatStaticResponse(string.Format( Langs.NoBotsAvilable));
            }
        }
    }
}
