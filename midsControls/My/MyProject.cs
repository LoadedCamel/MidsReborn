using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.VisualBasic.MyServices.Internal;

namespace midsControls.My
{
    // Token: 0x02000005 RID: 5
    [GeneratedCode("MyTemplate", "8.0.0.0")]
    [StandardModule]
    [HideModuleName]
    internal sealed class MyProject
    {
        // Token: 0x04000003 RID: 3
        private static readonly ThreadSafeObjectProvider<MyComputer> m_ComputerObjectProvider =
            new ThreadSafeObjectProvider<MyComputer>();

        // Token: 0x04000004 RID: 4
        private static readonly ThreadSafeObjectProvider<MyApplication> m_AppObjectProvider =
            new ThreadSafeObjectProvider<MyApplication>();

        // Token: 0x04000005 RID: 5
        private static readonly ThreadSafeObjectProvider<User> m_UserObjectProvider =
            new ThreadSafeObjectProvider<User>();

        // Token: 0x04000006 RID: 6
        private static readonly ThreadSafeObjectProvider<MyWebServices> m_MyWebServicesObjectProvider =
            new ThreadSafeObjectProvider<MyWebServices>();

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x06000007 RID: 7 RVA: 0x000020D8 File Offset: 0x000002D8
        [HelpKeyword("My.Computer")]
        internal static MyComputer Computer
        {
            [DebuggerHidden] get => m_ComputerObjectProvider.GetInstance;
        }

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x06000008 RID: 8 RVA: 0x000020F4 File Offset: 0x000002F4
        [HelpKeyword("My.Application")]
        internal static MyApplication Application
        {
            [DebuggerHidden] get => m_AppObjectProvider.GetInstance;
        }

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x06000009 RID: 9 RVA: 0x00002110 File Offset: 0x00000310
        [HelpKeyword("My.User")]
        internal static User User
        {
            [DebuggerHidden] get => m_UserObjectProvider.GetInstance;
        }

        // Token: 0x17000006 RID: 6
        // (get) Token: 0x0600000A RID: 10 RVA: 0x0000212C File Offset: 0x0000032C
        [HelpKeyword("My.WebServices")]
        internal static MyWebServices WebServices
        {
            [DebuggerHidden] get => m_MyWebServicesObjectProvider.GetInstance;
        }

        // Token: 0x02000006 RID: 6
        [MyGroupCollection("System.Web.Services.Protocols.SoapHttpClientProtocol", "Create__Instance__",
            "Dispose__Instance__", "")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal sealed class MyWebServices
        {
            // Token: 0x06000013 RID: 19 RVA: 0x0000221E File Offset: 0x0000041E
            [EditorBrowsable(EditorBrowsableState.Never)]
            [DebuggerHidden]
            public MyWebServices()
            {
            }

            // Token: 0x0600000D RID: 13 RVA: 0x0000217C File Offset: 0x0000037C
            [EditorBrowsable(EditorBrowsableState.Never)]
            [DebuggerHidden]
            // ReSharper disable once BaseObjectEqualsIsObjectEquals
            public override bool Equals(object o)
            {
                return base.Equals(RuntimeHelpers.GetObjectValue(o));
            }

            // Token: 0x0600000E RID: 14 RVA: 0x0000219C File Offset: 0x0000039C
            [EditorBrowsable(EditorBrowsableState.Never)]
            [DebuggerHidden]
            public override int GetHashCode()
            {
                // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
                return base.GetHashCode();
            }

            // Token: 0x0600000F RID: 15 RVA: 0x000021B4 File Offset: 0x000003B4
            [DebuggerHidden]
            [EditorBrowsable(EditorBrowsableState.Never)]
            internal new Type GetType()
            {
                return typeof(MyWebServices);
            }

            // Token: 0x06000010 RID: 16 RVA: 0x000021D0 File Offset: 0x000003D0
            [EditorBrowsable(EditorBrowsableState.Never)]
            [DebuggerHidden]
            public override string ToString()
            {
                return base.ToString();
            }

            // Token: 0x06000011 RID: 17 RVA: 0x000021E8 File Offset: 0x000003E8
            [DebuggerHidden]
            private static T Create__Instance__<T>(T instance) where T : new()
            {
                var result = instance == null ? Activator.CreateInstance<T>() : instance;
                return result;
            }

            // Token: 0x06000012 RID: 18 RVA: 0x00002214 File Offset: 0x00000414
            [DebuggerHidden]
            private void Dispose__Instance__<T>(ref T instance)
            {
                if (instance == null) throw new ArgumentNullException(nameof(instance));
                instance = default;
            }
        }

        // Token: 0x02000007 RID: 7
        [EditorBrowsable(EditorBrowsableState.Never)]
        [ComVisible(false)]
        internal sealed class ThreadSafeObjectProvider<T> where T : new()
        {
            // Token: 0x04000007 RID: 7
            private readonly ContextValue<T> m_Context;

            // Token: 0x06000015 RID: 21 RVA: 0x00002270 File Offset: 0x00000470
            [DebuggerHidden]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public ThreadSafeObjectProvider()
            {
                m_Context = new ContextValue<T>();
            }

            // Token: 0x17000007 RID: 7
            // (get) Token: 0x06000014 RID: 20 RVA: 0x0000222C File Offset: 0x0000042C
            internal T GetInstance
            {
                [DebuggerHidden]
                get
                {
                    var t = m_Context.Value;
                    if (t != null)
                        return t;
                    t = Activator.CreateInstance<T>();
                    m_Context.Value = t;
                    return t;
                }
            }
        }
    }
}