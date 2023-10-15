using System.Text;

namespace SAS_Dumper.Other
{
    internal static class Command
    {
        /// <summary>
        /// 查看插件版本
        /// </summary>
        /// <returns></returns>
        internal static string? ResponseSASDumperVersion()
        {
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version ?? new("0.0.0.0");

            StringBuilder sb = new();
            sb.AppendLine(string.Format(Langs.PluginVer, nameof(SAS_Dumper), version.Major, version.Minor, version.Build, version.Revision));
            sb.AppendLine("插件发布地址: https://github.com/chr233/SAS_Dumper/releases");

            return FormatStaticResponse(sb.ToString());
        }
    }
}
