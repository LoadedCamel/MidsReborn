using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace mrbControls.My.Resources
{
    // Token: 0x02000002 RID: 2
    [CompilerGenerated]
    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [DebuggerNonUserCode]
    [StandardModule]
    [HideModuleName]
    internal sealed class Resources
    {
        // Token: 0x04000001 RID: 1
        private static ResourceManager resourceMan;

        // Token: 0x17000001 RID: 1
        // (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager
        {
            get
            {
                if (!ReferenceEquals(resourceMan, null))
                    return resourceMan;
                var resourceManager = new ResourceManager("midsControls.Resources", typeof(Resources).Assembly);
                resourceMan = resourceManager;
                return resourceMan;
            }
        }

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x06000002 RID: 2 RVA: 0x0000209C File Offset: 0x0000029C
        // (set) Token: 0x06000003 RID: 3 RVA: 0x000020B3 File Offset: 0x000002B3
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture { get; set; }

        // Token: 0x04000002 RID: 2
    }
}