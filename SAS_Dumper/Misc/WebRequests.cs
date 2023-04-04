using ArchiSteamFarm.Steam;
using ArchiSteamFarm.Steam.Integration;

namespace SAS_Dumper.Misc
{
    internal static class WebRequests
    {
        /// <summary>
        /// 关注或者取关鉴赏家
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="clanId"></param>
        /// <param name="isFollow"></param>
        /// <returns></returns>
        internal static async Task FollowCurator(Bot bot, string clanId)
        {
            Uri request = new(SteamStoreURL, "/curators/ajaxfollow");
            Uri referer = new(SteamStoreURL, $"curator/{clanId}");

            Dictionary<string, string> data = new(3) {
                { "clanid", clanId },
                { "follow",  "1" },
            };

            await bot.ArchiWebHandler.UrlPostWithSession(request, data: data, referer: referer).ConfigureAwait(false);
        }

        /// <summary>
        /// 加入指定群组
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        internal static async Task JoinGroup(Bot bot, string groupName)
        {
            Uri request = new(SteamCommunityURL, $"/groups/{groupName}");

            Dictionary<string, string> data = new(2, StringComparer.Ordinal)
            {
                    { "action", "join" },
                };
            await bot.ArchiWebHandler.UrlPostToHtmlDocumentWithSession(request, data: data, referer: SteamCommunityURL, session: ArchiWebHandler.ESession.CamelCase).ConfigureAwait(false);
        }
    }
}
