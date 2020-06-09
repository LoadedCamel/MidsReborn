
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer.My.Resources
{
    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [HideModuleName]
    [CompilerGenerated]
    [StandardModule]
    [DebuggerNonUserCode]
    internal sealed class Resources
    {
        static ResourceManager resourceMan;


        [EditorBrowsable(EditorBrowsableState.Advanced)]
        private static CultureInfo Culture { get; set; }

        internal static Bitmap Gradient => (Bitmap) RuntimeHelpers.GetObjectValue(ResourceManager.GetObject(nameof(Gradient), Culture));

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        private static ResourceManager ResourceManager =>
            resourceMan ??= new ResourceManager("Hero_Designer.Resources", typeof(Resources).Assembly);
    }
}
