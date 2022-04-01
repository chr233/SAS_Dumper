using ArchiSteamFarm.Plugins.Interfaces;
using ArchiSteamFarm.Steam;
using Chrxw.SAS_Dumper.Localization;
using Chrxw.SAS_Dumper.Storage;
using Newtonsoft.Json.Linq;
using SteamKit2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using static Chrxw.SAS_Dumper.Utils;

namespace Chrxw.SAS_Dumper
{
    [Export(typeof(IPlugin))]
    internal sealed class SAS_Dumper : IASF, IBotCommand2, IBotConnection
    {
        public string Name => nameof(SAS_Dumper);
        public Version Version => typeof(SAS_Dumper).Assembly.GetName().Version ?? throw new ArgumentNullException(nameof(Version));

        private bool PluginEnabled { get; set; } = false;

        internal Timer TaskTimer { get; private set; } = null!;

        /// <summary>
        /// ASF启动事件
        /// </summary>
        /// <param name="additionalConfigProperties"></param>
        /// <returns></returns>
        public Task OnASFInit(IReadOnlyDictionary<string, JToken> additionalConfigProperties = null)
        {
#pragma warning disable CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
            Config? config = null;
#pragma warning restore CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。

            if (additionalConfigProperties != null)
            {
                foreach ((string configProperty, JToken configValue) in additionalConfigProperties)
                {
                    try
                    {
                        if (configProperty == "SASConfig")
                        {
                            config = configValue.ToObject<Config>();
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        ASFLogger.LogGenericException(e);
                        ASFLogger.LogGenericWarning(string.Format(CurrentCulture, Langs.ReadConfigError));
                    }
                }
            }

            if (config != null)
            {
                Http.BaseAddress = new(config.SASUrl);
                Http.DefaultRequestHeaders.Add("auth", config.SASPasswd);
                TaskTimer = new(SAS.WebRequests.DoSASFeedback, null, TimeSpan.Zero, TimeSpan.FromSeconds(config.FeedbackPeriod));
            }
            else
            {
                ASFLogger.LogGenericWarning(string.Format(CurrentCulture, Langs.ReadConfigError));
            }

            PluginEnabled = config != null;

            SASConfig = config ?? new();

            ASFLogger.LogGenericInfo(string.Format(CurrentCulture, Langs.PluginState, SASConfig.Enabled ? Langs.Enabled : Langs.Disabled));

            return Task.CompletedTask;
        }

        /// <summary>
        /// 插件加载事件
        /// </summary>
        /// <returns></returns>
        public Task OnLoaded()
        {
            ASFLogger.LogGenericInfo(string.Format(CurrentCulture, Langs.PluginVer, nameof(SAS_Dumper), Version.Major, Version.Minor, Version.Build, Version.Revision));
            ASFLogger.LogGenericInfo(string.Format(CurrentCulture, Langs.PluginContact));

            return Task.CompletedTask;
        }

        /// <summary>
        /// 处理命令事件
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="access"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<string> OnBotCommand(Bot bot, EAccess access, string message, string[] args, ulong steamID = 0)
        {
            if (!Enum.IsDefined(access))
            {
                throw new InvalidEnumArgumentException(nameof(access), (int)access, typeof(EAccess));
            }

            switch (args.Length)
            {
                case 0:
                    throw new InvalidOperationException(nameof(args.Length));
                case 1: //不带参数
                    switch (args[0].ToUpperInvariant())
                    {
                        //Other
                        case "SASDUMPER" when access > EAccess.Master:
                        case "SASD" when access > EAccess.Master:
                            return Other.Command.ResponseSASDumperVersion();

                        //SAS
                        case "SASTEST" when PluginEnabled && access > EAccess.Master:
                            return await SAS.Command.ResponseSASTest().ConfigureAwait(false);

                        case "SASON" when PluginEnabled && access > EAccess.Master:
                            return SAS.Command.ResponseSASController(true);

                        case "SASTOFF" when PluginEnabled && access > EAccess.Master:
                            return SAS.Command.ResponseSASController(false);

                        default:
                            return null;
                    }

                default: //带参数
                    return null;

            }
        }

        /// <summary>
        /// Bot成功登录
        /// </summary>
        /// <param name="bot"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        async Task IBotConnection.OnBotLoggedOn(Bot bot)
        {
#pragma warning disable CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
            (bool success, string? accessToken) = await bot.ArchiWebHandler.CachedAccessToken.GetValue().ConfigureAwait(false);
#pragma warning restore CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
            if (success)
            {
                BotInfoDict.TryAdd(bot.BotName, new(bot.SteamID, accessToken));
            }
            else
            {
                ASFLogger.LogGenericWarning(string.Format(CurrentCulture, Langs.FetchTokenFailure, bot.BotName));
            }
        }

        /// <summary>
        /// Bot断开连接
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task OnBotDisconnected(Bot bot, EResult reason)
        {
            return Task.CompletedTask;
        }
    }
}
