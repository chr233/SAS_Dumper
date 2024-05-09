using ArchiSteamFarm.Core;
using ArchiSteamFarm.Plugins.Interfaces;
using ArchiSteamFarm.Steam;



using SAS_Dumper.Data;

using SteamKit2;
using System.ComponentModel;
using System.Composition;
using System.Reflection;
using System.Text.Json;

namespace SAS_Dumper;

[Export(typeof(IPlugin))]
internal sealed class SAS_Dumper : IASF, IBotCommand2, IBotConnection, IBot
{
    public string Name => "SAS Dumper";
    public Version Version => Assembly.GetExecutingAssembly().GetName().Version ?? throw new ArgumentNullException(nameof(Version));

    private bool ASFEBridge;

    private Timer? FeedBackTimer { get; set; }

    private Timer? RefreshTokenTimer { get; set; }

    /// <summary>
    /// ASF启动事件
    /// </summary>
    /// <param name="additionalConfigProperties"></param>
    /// <returns></returns>
    public Task OnASFInit(IReadOnlyDictionary<string, JsonElement>? additionalConfigProperties = null)
    {
        PluginConfig? config = null;

        if (additionalConfigProperties != null)
        {
            foreach (var (configProperty, configValue) in additionalConfigProperties)
            {
                try
                {
                    if (configProperty == "SASConfig" && configValue.ValueKind == JsonValueKind.Object)
                    {
                        config = configValue.Deserialize<PluginConfig>();
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
                        try
                        {
                            await SAS.WebRequests.SASFeedback(BotTokenCache).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            ASFLogger.LogGenericException(ex);
                            ASFLogger.LogGenericError("网络请求失败");
                        }
                    }
                }, null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(config.FeedbackPeriod)
            );

            ASFLogger.LogGenericInfo(string.Format(Langs.PluginState, SASConfig.Enabled ? Langs.Enabled : Langs.Disabled));

            OnlineMode = true;
        }
        else
        {
            ASFLogger.LogGenericInfo(string.Format(Langs.ReadConfigError));
        }

        RefreshTokenTimer = new Timer(
            SAS.Command.RefreshTokens,
            null,
            TimeSpan.FromHours(1),
            TimeSpan.FromHours(1)
        );

        SASConfig = config ?? new();

        return Task.CompletedTask;
    }

    /// <summary>
    /// 插件加载事件
    /// </summary>
    /// <returns></returns>
    public Task OnLoaded()
    {
        ASFLogger.LogGenericInfo(Langs.PluginContact);
        ASFLogger.LogGenericInfo(Langs.PluginInfo);

        var flag = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        var handler = typeof(SAS_Dumper).GetMethod(nameof(ResponseCommand), flag);

        const string pluginId = nameof(SAS_Dumper);
        const string cmdPrefix = "SAS";
        const string? repoName = "chr233/SAS_Dumper";

        ASFEBridge = AdapterBridge.InitAdapter(Name, pluginId, cmdPrefix, repoName, handler);

        if (ASFEBridge)
        {
            ASFLogger.LogGenericDebug(Langs.ASFEnhanceRegisterSuccess);
        }
        else
        {
            ASFLogger.LogGenericInfo(Langs.ASFEnhanceRegisterFailed);
            ASFLogger.LogGenericWarning(Langs.PluginStandalongMode);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 获取插件信息
    /// </summary>
    private static string? PluginInfo => string.Format("{0} {1}", nameof(SAS_Dumper), MyVersion);

    /// <summary>
    /// 处理命令
    /// </summary>
    /// <param name="access"></param>
    /// <param name="cmd"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static Task<string?>? ResponseCommand(EAccess access, string cmd, string message, string[] args)
    {
        int argLength = args.Length;
        return argLength switch {
            0 => throw new InvalidOperationException(nameof(args.Length)),
            1 => cmd switch //不带参数
            {
                //PluginInfo
                "SASDUMPER" when access >= EAccess.Master =>
                    Task.FromResult(PluginInfo),
                //SAS
                "SASTEST" when OnlineMode && access >= EAccess.Master =>
                    SAS.Command.ResponseSASTest(),
                "SASON" when OnlineMode && access >= EAccess.Master =>
                    Task.FromResult(SAS.Command.ResponseSASController(true)),
                "SASTOFF" when OnlineMode && access >= EAccess.Master =>
                    Task.FromResult(SAS.Command.ResponseSASController(false)),
                "SASFRESH" when access >= EAccess.Master =>
                    SAS.Command.ResponseSASFresh(),
                "SASMANUAL" when OnlineMode && access >= EAccess.Master =>
                    SAS.Command.ResponseSASManualFeedback(),
                "SASDUMP" when access >= EAccess.Master =>
                    SAS.Command.ResponseSASDump(null),

                _ => null,
            },
            _ => cmd switch //带参数
            {
                //SAS
                "SASDUMP" when access >= EAccess.Master =>
                    SAS.Command.ResponseSASDump(Utilities.GetArgsAsText(message, 1)),

                _ => null,
            },
        };
    }

    /// <summary>
    /// 处理命令事件
    /// </summary>
    /// <param name="bot"></param>
    /// <param name="access"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    /// <param name="steamID"></param>
    /// <returns></returns>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<string?> OnBotCommand(Bot bot, EAccess access, string message, string[] args, ulong steamID = 0)
    {
        if (ASFEBridge)
        {
            return null;
        }

        if (!Enum.IsDefined(access))
        {
            throw new InvalidEnumArgumentException(nameof(access), (int)access, typeof(EAccess));
        }

        try
        {
            var cmd = args[0].ToUpperInvariant();

            if (cmd.StartsWith("SAS."))
            {
                cmd = cmd[4..];
            }

            var task = ResponseCommand(access, cmd, message, args);
            if (task != null)
            {
                return await task.ConfigureAwait(false);
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            _ = Task.Run(async () => {
                await Task.Delay(500).ConfigureAwait(false);
                Utils.ASFLogger.LogGenericException(ex);
            }).ConfigureAwait(false);

            return ex.StackTrace;
        }
    }

    /// <summary>
    /// Bot成功登录
    /// </summary>
    /// <param name="bot"></param>
    /// <returns></returns>
    public Task OnBotLoggedOn(Bot bot)
    {
        var accessToken = bot.AccessToken;

        if (!string.IsNullOrEmpty(accessToken))
        {
            BotTokenCache.TryAdd(
                bot.BotName,
                new BotInfo { SteamID = bot.SteamID, AccessToken = accessToken }
            );
        }
        else
        {
            ASFLogger.LogGenericWarning(string.Format(Langs.FetchTokenFailure, bot.BotName));
        }

        return Task.CompletedTask;
        //_ = Task.Run(async () => await Misc.Handler.OnBotLoggedOn(bot).ConfigureAwait(false));
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
