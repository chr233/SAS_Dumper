using ArchiSteamFarm.Core;
using ArchiSteamFarm.NLog;
using ArchiSteamFarm.Steam.Integration;
using Chrxw.SAS_Dumper.Storage;
using System;
using System.Globalization;
using System.Net.Http;
using Chrxw.SAS_Dumper.Data;
using System.Collections.Concurrent;

namespace Chrxw.SAS_Dumper
{
    internal static class Utils
    {
        /// <summary>
        /// 当前语言代码
        /// </summary>
        internal static CultureInfo CurrentCulture => CultureInfo.CurrentCulture;

        /// <summary>
        /// 配置文件
        /// </summary>
        internal static Config SASConfig { get; set; } = null;

        /// <summary>
        /// Steam商店链接
        /// </summary>
        internal static Uri SteamStoreURL => ArchiWebHandler.SteamStoreURL;

        /// <summary>
        /// Steam社区链接
        /// </summary>
        internal static Uri SteamCommunityURL => ArchiWebHandler.SteamCommunityURL;

        /// <summary>
        /// 日志
        /// </summary>
        internal static ArchiLogger ASFLogger => ASF.ArchiLogger;

        /// <summary>
        /// 网络请求器
        /// </summary>
        internal static HttpClient Http = new();

        /// <summary>
        /// 机器人信息
        /// </summary>
        internal static ConcurrentDictionary<string, BotInfo> BotInfoDict = new();
    }
}
