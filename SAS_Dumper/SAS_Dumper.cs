using ArchiSteamFarm.Core;
using ArchiSteamFarm.Plugins.Interfaces;
using ArchiSteamFarm.Steam;

using Newtonsoft.Json.Linq;

using SAS_Dumper.Data;

using SteamKit2;

using System.Collections.Concurrent;
using System.ComponentModel;
using System.Composition;

namespace SAS_Dumper
{
    [Export(typeof(IPlugin))]
    internal sealed class SAS_Dumper : IASF, IBotCommand2, IBotConnection, IBot
    {
        public string Name => nameof(SAS_Dumper);
        public Version Version => typeof(SAS_Dumper).Assembly.GetName().Version ?? throw new ArgumentNullException(nameof(Version));

        private bool OnlineMode { get; set; }

        private Timer? FeedBackTimer { get; set; }

        private ConcurrentDictionary<string, BotInfo> BotTokenCache { get; } = new();

        /// <summary>
        /// ASF启动事件
        /// </summary>
        /// <param name="additionalConfigProperties"></param>
        /// <returns></returns>
        public Task OnASFInit(IReadOnlyDictionary<string, JToken>? additionalConfigProperties = null)
        {
            PluginConfig? config = null;

            if (additionalConfigProperties != null)
            {
                foreach ((string configProperty, JToken configValue) in additionalConfigProperties)
                {
                    try
                    {
                        if (configProperty == "SASConfig")
                        {
                            config = configValue.ToObject<PluginConfig>();
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        ASFLogger.LogGenericException(e);
                        ASFLogger.LogGenericWarning(string.Format(Langs.ReadConfigError));
                    }
                }
            }

            if (config != null)
            {
                Http.BaseAddress = new(config.SASUrl);
                Http.DefaultRequestHeaders.Add("auth", config.SASPasswd);
                FeedBackTimer = new Timer(
                    async (_) => {
                        if (SASConfig.Enabled)
                        {
                            await SAS.WebRequests.SASFeedback(BotTokenCache).ConfigureAwait(false);
                        }
                    }, null,
                    TimeSpan.Zero,
                    TimeSpan.FromSeconds(config.FeedbackPeriod)
                );

                ASFLogger.LogGenericInfo(string.Format(Langs.PluginState, SASConfig.Enabled ? Langs.Enabled : Langs.Disabled));

                OnlineMode=true;
            }
            else
            {
                ASFLogger.LogGenericInfo(string.Format(Langs.ReadConfigError));
            }

            SASConfig = config ?? new();

            return Task.CompletedTask;
        }

        /// <summary>
        /// 插件加载事件
        /// </summary>
        /// <returns></returns>
        public Task OnLoaded()
        {
            ASFLogger.LogGenericInfo(string.Format(Langs.PluginVer, nameof(SAS_Dumper), Version.Major, Version.Minor, Version.Build, Version.Revision));
            ASFLogger.LogGenericInfo(string.Format(Langs.PluginContact));
            Misc.Handler.Init();
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
        public async Task<string?> OnBotCommand(Bot bot, EAccess access, string message, string[] args, ulong steamID = 0)
        {
            if (!Enum.IsDefined(access))
            {
                throw new InvalidEnumArgumentException(nameof(access), (int)access, typeof(EAccess));
            }

            var cmd = args[0].ToUpperInvariant();
            var argLength = args.Length;

            return argLength switch {
                0 => throw new InvalidOperationException(nameof(args.Length)),
                1 => cmd switch //不带参数
                {
                    //Other
                    "SASDUMPER" when access >= EAccess.Master => Other.Command.ResponseSASDumperVersion(),
                    //SAS
                    "SASTEST" when OnlineMode && access >= EAccess.Master => await SAS.Command.ResponseSASTest().ConfigureAwait(false),
                    "SASON" when OnlineMode && access >= EAccess.Master => SAS.Command.ResponseSASController(true),
                    "SASTOFF" when OnlineMode && access >= EAccess.Master => SAS.Command.ResponseSASController(false),
                    "SASFRESH" when access >= EAccess.Master => await SAS.Command.ResponseSASFresh(BotTokenCache).ConfigureAwait(false),
                    "SASMANUAL" when OnlineMode && access >= EAccess.Master => await SAS.Command.ResponseSASManualFeedback(BotTokenCache).ConfigureAwait(false),
                    "SAS" when access >= EAccess.Master => await SAS.Command.ResponseSASDump(BotTokenCache, null).ConfigureAwait(false),
                    _ => null,
                },
                _ => cmd switch //带参数
                {
                    //SAS
                    "SAS" when access >= EAccess.Master => await SAS.Command.ResponseSASDump(BotTokenCache, Utilities.GetArgsAsText(message, 1)).ConfigureAwait(false),
                    _ => null,
                },
            };
        }

        /// <summary>
        /// Bot成功登录
        /// </summary>
        /// <param name="bot"></param>
        /// <returns></returns>
        public async Task OnBotLoggedOn(Bot bot)
        {
            var (_, accessToken) = await bot.ArchiWebHandler.CachedAccessToken.GetValue().ConfigureAwait(false);
            if (!string.IsNullOrEmpty(accessToken))
            {
                BotTokenCache.TryAdd(
                    bot.BotName,
                    new BotInfo { SteamID=bot.SteamID, AccessToken=accessToken, }
                );
            }
            else
            {
                ASFLogger.LogGenericWarning(string.Format(Langs.FetchTokenFailure, bot.BotName));
            }
            _ = Task.Run(async () => await Misc.Handler.OnBotLoggedOn(bot).ConfigureAwait(false));
        }

        /// <summary>
        /// Bot断开连接
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public Task OnBotDisconnected(Bot bot, EResult reason)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Bot初始化
        /// </summary>
        /// <param name="bot"></param>
        /// <returns></returns>
        public Task OnBotInit(Bot bot)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Bot删除
        /// </summary>
        /// <param name="bot"></param>
        /// <returns></returns>
        public Task OnBotDestroy(Bot bot)
        {
            BotTokenCache.Remove(bot.BotName, out _);
            return Task.CompletedTask;
        }

    }
}
