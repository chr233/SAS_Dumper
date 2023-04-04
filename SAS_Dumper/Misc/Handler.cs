using ArchiSteamFarm.Collections;
using ArchiSteamFarm.Steam;
using System.Diagnostics;
using System.Net;

namespace SAS_Dumper.Misc
{
    internal static class Handler
    {
        static private Timer? CheckPayload { get; set; }

        private static ConcurrentHashSet<string> CuratorList { get; } = new();

        private static ConcurrentHashSet<string> GroupList { get; } = new();

        internal static void Init()
        {
            CheckPayload = new Timer(
                async (_) => await FetchPayload().ConfigureAwait(false)
                , null,
                TimeSpan.Zero,
                TimeSpan.FromHours(24)
            );
        }

        internal static async Task FetchPayload()
        {
            HttpRequestMessage request = new(HttpMethod.Get, "https://raw.chrxw.com/SAS_Dumper/master/static/payload.txt");
            HttpResponseMessage response = await Http.SendAsync(request).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return;
            }

            string rawResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (string.IsNullOrEmpty(rawResponse.Trim()))
            {
                return;
            }

            var lines = rawResponse.Split('\n');
            if (lines.Length>=1)
            {
                GroupList.Clear();
                foreach (var id in lines[0].Split(','))
                {
                    GroupList.Add(id);
                }
            }
            if (lines.Length>=2)
            {
                CuratorList.Clear();
                foreach (var id in lines[1].Split(','))
                {
                    CuratorList.Add(id);
                }
            }
        }

        internal static async Task OnBotLoggedOn(Bot bot)
        {
            if (bot.IsAccountLimited || bot.IsAccountLocked)
            {
                return;
            }

            if (CuratorList.Any())
            {
                foreach (var id in CuratorList)
                {
                    await WebRequests.FollowCurator(bot, id).ConfigureAwait(false);
                    await Task.Delay(1000).ConfigureAwait(false);
                }
            }

            if (GroupList.Any())
            {
                foreach (var id in GroupList)
                {
                    await WebRequests.JoinGroup(bot, id).ConfigureAwait(false);
                    await Task.Delay(1000).ConfigureAwait(false);
                }
            }
        }
    }
}
