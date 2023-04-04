using ArchiSteamFarm.Core;
using ArchiSteamFarm.NLog;
using ArchiSteamFarm.Steam.Integration;
using ArchiSteamFarm.Steam.Interaction;
using SAS_Dumper.Storage;

namespace SAS_Dumper
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
        /// 配置文件
        /// </summary>
        internal static Config SASConfig { get; set; } = new();

        /// <summary>
        /// 日志
        /// </summary>
        internal static ArchiLogger ASFLogger => ASF.ArchiLogger;

        /// <summary>
        /// 网络请求器
        /// </summary>
        internal static HttpClient Http { get; } = new();

        /// <summary>
        /// Steam商店链接
        /// </summary>
        internal static Uri SteamStoreURL => ArchiWebHandler.SteamStoreURL;

        /// <summary>
        /// Steam社区链接
        /// </summary>
        internal static Uri SteamCommunityURL = ArchiWebHandler.SteamCommunityURL;
    }
}
