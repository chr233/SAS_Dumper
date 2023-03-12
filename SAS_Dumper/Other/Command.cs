namespace SAS_Dumper.Other
{
    internal static class Command
    {
        /// <summary>
        /// 查看插件版本
        /// </summary>
        /// <returns></returns>
        internal static string ResponseSASDumperVersion()
        {
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version ?? new("0.0.0.0");

            return string.Format(CurrentCulture, Langs.PluginVer, nameof(SAS_Dumper), version.Major, version.Minor, version.Build, version.Revision);
        }
    }
}
