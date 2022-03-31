using Chrxw.SAS_Dumper.Localization;
using System.Threading.Tasks;
using static Chrxw.SAS_Dumper.Utils;
namespace Chrxw.SAS_Dumper.SAS
{
    internal static class Command
    {
        internal static async Task<string> ResponseSASTest()
        {
            return await WebRequests.TestSAS().ConfigureAwait(false);
        }

        internal static string ResponseSASController(bool enable)
        {
            SASConfig.Enabled = enable;

            return string.Format(CurrentCulture, Langs.PluginState, SASConfig.Enabled ? Langs.Enabled : Langs.Disabled);
        }
    }
}
