using Newtonsoft.Json;

using SAS_Dumper.Data;

using System.Net;
using System.Text;

namespace SAS_Dumper.SAS;

internal static class WebRequests
{
    /// <summary>
    /// 测试后端
    /// </summary>
    /// <returns></returns>
    internal static async Task<HttpResponseMessage> TestSAS()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/adv/bots/");

        var response = await Http.SendAsync(request).ConfigureAwait(false);

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

        HashSet<Dictionary<string, string>> payload = [];

        foreach (var (name, botInfo) in botTokens)
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

            var request = new HttpRequestMessage(HttpMethod.Post, "/adv/bots/muli")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = await Http.SendAsync(request).ConfigureAwait(false);

            bool success = response.StatusCode == HttpStatusCode.OK;

            string rawResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (success)
            {
                var result = JsonConvert.DeserializeObject<SASResult>(rawResponse);

                if (result != null)
                {
                    int succCount = 0, failCount = 0;

                    foreach (var res in result.Result)
                    {
                        if (res.Count == 4)
                        {
                            string steamId = res[0];
                            string desc = res[2];
                            string state = res[3];

                            if (state != "添加失败")
                            {
                                succCount++;
                            }
                            else
                            {
                                failCount++;
                                ASFLogger.LogGenericWarning(string.Format(Langs.SASAddBotFailed, steamId, state));
                            }
                        }
                    }

                    ASFLogger.LogGenericInfo(string.Format(Langs.SASFeedStatus, payload.Count, succCount, failCount));
                }
                else
                {
                    ASFLogger.LogGenericWarning(string.Format(Langs.SASFailed, response.StatusCode, rawResponse));
                }
            }
            else
            {
                ASFLogger.LogGenericWarning(string.Format(Langs.SASFailed, response.StatusCode, rawResponse));
            }
        }
    }
}
