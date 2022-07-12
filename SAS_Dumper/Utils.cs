using ArchiSteamFarm.Core;
using ArchiSteamFarm.NLog;
using ArchiSteamFarm.Steam.Integration;
using ArchiSteamFarm.Steam.Interaction;
using Chrxw.SAS_Dumper.Data;
using Chrxw.SAS_Dumper.Storage;
using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Net.Http;

namespace Chrxw.SAS_Dumper
{
    internal static class Utils
    {
        /// <summary>
        /// 格式化Bot输出
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        internal static string FormatStaticResponse(string response)
        {
            return Commands.FormatStaticResponse(response);
        }

        /// <summary>
        /// 当前语言代码
        /// </summary>
        internal static CultureInfo CurrentCulture => CultureInfo.CurrentCulture;

        /// <summary>
        /// 配置文件
        /// </summary>
        internal static Config SASConfig { get; set; } = null;

        /// <summary>
        /// 日志
        /// </summary>
        internal static ArchiLogger ASFLogger => ASF.ArchiLogger;

        /// <summary>
        /// 网络请求器
        /// </summary>
        internal static HttpClient Http { get; } = new();

        /// <summary>
        /// 机器人信息
        /// </summary>
        internal static ConcurrentDictionary<string, BotInfo> BotInfoDict { get; } = new();
    }
}
