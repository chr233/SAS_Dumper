using Chrxw.SAS_Dumper.Data;
using Chrxw.SAS_Dumper.Localization;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using static Chrxw.SAS_Dumper.Utils;
using Newtonsoft.Json;
using System.Text;

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

            HashSet<string> botNames = new();

            foreach (string name in BotInfoDict.Keys)
            {
                if (BotInfoDict.TryGetValue(name, out BotInfo binfo))
                {
                    string steamID = binfo.SteamID.ToString();
                    string token = binfo.AccessToken;
                    payload.Add(new(3, StringComparer.Ordinal) {
                        { "steam_id", steamID },
                        { "access_token", token },
                        { "desc", name }
                    });
                    botNames.Add(name);
                };
            }

            if (payload.Count > 0)
            {
                string json = JsonConvert.SerializeObject(payload);

                HttpRequestMessage request = new(HttpMethod.Post, "/adv/bots/muli") {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };

                HttpResponseMessage response = await Http.SendAsync(request).ConfigureAwait(false);

                bool success = response.StatusCode == HttpStatusCode.OK;

                ASFLogger.LogGenericInfo(string.Format(CurrentCulture, Langs.SASFeedStatus, success ? Langs.Success : Langs.Failure));

                if (success)
                {
                    foreach (string name in botNames)
                    {
                        BotInfoDict.TryRemove(name, out BotInfo _);
                    }
                }
            }
        }
    }
}
