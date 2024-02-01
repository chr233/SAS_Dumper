using ArchiSteamFarm.Steam;
using ArchiSteamFarm.Steam.Integration;

namespace SAS_Dumper.Misc;

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
        var request = new Uri(SteamStoreURL, "/curators/ajaxfollow");
        var referer = new Uri(SteamStoreURL, $"curator/{clanId}");

        var data = new Dictionary<string, string>(3)
        {
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
        var request = new Uri(SteamCommunityURL, $"/groups/{groupName}");

        var data = new Dictionary<string, string>(2)
        {
                { "action", "join" },
        };
        await bot.ArchiWebHandler.UrlPostToHtmlDocumentWithSession(request, data: data, referer: SteamCommunityURL, session: ArchiWebHandler.ESession.CamelCase).ConfigureAwait(false);
    }
}
