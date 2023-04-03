using Newtonsoft.Json;
using SAS_Dumper.Data;
using System.Collections.Concurrent;
using System.Net;
using System.Text;

namespace SAS_Dumper.SAS
{
    internal static class WebRequests
    {
        /// <summary>
        /// 测试后端
        /// </summary>
        /// <returns></returns>
        internal static async Task<HttpResponseMessage> TestSAS()
        {
            HttpRequestMessage request = new(HttpMethod.Get, "/adv/bots/");

            HttpResponseMessage response = await Http.SendAsync(request).ConfigureAwait(false);

            return response;
        }

        /// <summary>
        /// 上传账号信息
        /// </summary>
        /// <param name="botTokens"></param>
        /// <returns></returns>
        internal static async Task SASFeedback(IDictionary<string, BotInfo> botTokens)
        {
            if (!SASConfig.Enabled)
            {
                return;
            }

            HashSet<Dictionary<string, string>> payload = new();

            foreach(var (name,botInfo) in botTokens)
            {
                string steamID = botInfo.SteamID.ToString();
                string token = botInfo.AccessToken;
                payload.Add(new(3, StringComparer.Ordinal) {
                        { "steam_id", steamID },
                        { "access_token", token },
                        { "desc", name }
                    });
            }

            if (payload.Count > 0)
            {
                string json = JsonConvert.SerializeObject(payload);

                HttpRequestMessage request = new(HttpMethod.Post, "/adv/bots/muli") {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };

                HttpResponseMessage response = await Http.SendAsync(request).ConfigureAwait(false);

                bool success = response.StatusCode == HttpStatusCode.OK;

                string rawResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (success)
                {
                    SASResult? result = JsonConvert.DeserializeObject<SASResult>(rawResponse);

                    if (result != null)
                    {
                        int succCount = 0, failCount = 0;

                        foreach (List<string> res in result.Result)
                        {
                            if (res.Count == 4)
                            {
                                string name = res[0];
                                string state = res[3];

                                if (state != "添加失败")
                                {
                                    succCount++;
                                }
                                else
                                {
                                    failCount++;
                                    ASFLogger.LogGenericWarning(string.Format(CurrentCulture, Langs.SASAddBotFailed, name));
                                }
                            }
                        }

                        ASFLogger.LogGenericInfo(string.Format(CurrentCulture, Langs.SASFeedStatus, payload.Count, succCount, failCount));
                    }
                    else
                    {
                        ASFLogger.LogGenericWarning(string.Format(CurrentCulture, Langs.SASFailed, response.StatusCode, rawResponse));
                    }
                }
                else
                {
                    ASFLogger.LogGenericWarning(string.Format(CurrentCulture, Langs.SASFailed, response.StatusCode, rawResponse));
                }

            }
        }
    }
}
