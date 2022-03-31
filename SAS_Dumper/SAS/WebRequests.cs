using Chrxw.SAS_Dumper.Data;
using Chrxw.SAS_Dumper.Localization;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using static Chrxw.SAS_Dumper.Utils;

namespace Chrxw.SAS_Dumper.SAS
{
    internal static class WebRequests
    {

        internal static async Task<string> TestSAS()
        {
            HttpRequestMessage request = new(HttpMethod.Get, "/adv/bots/");

            HttpResponseMessage response = await Http.SendAsync(request).ConfigureAwait(false);

            return string.Format(CurrentCulture, Langs.SASTest, response.StatusCode == HttpStatusCode.OK ? Langs.Success : Langs.Failure);
        }

        internal static void DoSASFeedback(object _)
        {
            if (SASConfig.Enabled)
            {
                Task.Run(async () => {
                    await SASFeedback().ConfigureAwait(false);
                });
            }
        }


        internal static async Task SASFeedback()
        {
            HashSet<Dictionary<string, string>> payload = new();

            foreach (string name in BotInfoDict.Keys)
            {
                if (BotInfoDict.TryRemove(name, out BotInfo binfo))
                {
                    string steamID = binfo.SteamID.ToString();
                    string token = binfo.AccessToken;
                    payload.Add(new(3, StringComparer.Ordinal) {
                        { "steam_id", steamID },
                        { "access_token", token },
                        { "desc", name }
                    });
                };
            }

            if(payload.Count == 0)
            {
                return Task.CompletedTask;
            }

            HttpRequestMessage request = new(HttpMethod.Post, "/adv/bots/muli") {
                Content = JsonContent.Create(payload)
            };

            HttpResponseMessage response = await Http.SendAsync(request).ConfigureAwait(false);

            bool success = response.StatusCode == HttpStatusCode.OK;

            ASFLogger.LogGenericInfo(string.Format(CurrentCulture, Langs.SASFeedStatus, success ? Langs.Success : Langs.Failure));
        }
    }
}
