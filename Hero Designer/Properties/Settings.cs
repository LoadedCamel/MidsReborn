using System.CodeDom.Compiler;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace Hero_Designer.Properties
{
    [CompilerGenerated]
    [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed class Settings : ApplicationSettingsBase
    {
        private static readonly Settings DefaultInstance = (Settings) Synchronized(new Settings());


        public static Settings Default
        {
            get
            {
                var defaultInstance2 = DefaultInstance;
                return defaultInstance2;
            }
        }
    }
}