using System;
using System.Threading;

namespace Mids_Reborn.Core.Utils.Scopes;

/// <summary>
/// Manages event-suppression scopes with safe nesting and an override to force events enabled.
/// Use via composition: keep an instance as a field and call Suppress/Force/If as needed.
/// </summary>
public sealed class EventSuppressionController
{
    // # of active "suppress" scopes
    private int _suppressCount;
    // # of active "force enabled" scopes that override suppression
    private int _forceEnableCount;

    /// <summary>
    /// True when events should be suppressed given current nesting/overrides.
    /// </summary>
    public bool IsSuppressed => Volatile.Read(ref _suppressCount) > 0
                                && Volatile.Read(ref _forceEnableCount) == 0;

    /// <summary>
    /// Single entry point for conditional usage:
    /// suppress == true  -> begin a suppression scope (events OFF in scope)
    /// suppress == false -> begin a force-enabled scope (events ON in scope)
    /// </summary>
    public IDisposable Suppress(bool suppress) =>
        suppress ? BeginSuppressScope() : BeginForceEnableScope();

    /// <summary>
    /// Suppress only if <paramref name="condition"/> is true; otherwise no-op.
    /// Keeps call sites branch-free while preserving state.
    /// </summary>
    public IDisposable SuppressIf(bool condition) =>
        condition ? BeginSuppressScope() : NoOp.Instance;

    /// <summary>
    /// Explicit helper to force events enabled in the scope (overrides any suppression).
    /// </summary>
    public IDisposable ForceEnabled() => BeginForceEnableScope();

    /// <summary>
    /// Utility you can use in guards: if (!controller.ShouldRaiseEvents()) return;
    /// </summary>
    public bool ShouldRaiseEvents() => !IsSuppressed;

    // ---- private implementation ----

    private IDisposable BeginSuppressScope()
    {
        Interlocked.Increment(ref _suppressCount);
        return new Scope(() => Interlocked.Decrement(ref _suppressCount));
    }

    private IDisposable BeginForceEnableScope()
    {
        Interlocked.Increment(ref _forceEnableCount);
        return new Scope(() => Interlocked.Decrement(ref _forceEnableCount));
    }

    private sealed class Scope : IDisposable
    {
        private Action? _onDispose;
        public Scope(Action onDispose) => _onDispose = onDispose;
        public void Dispose() => Interlocked.Exchange(ref _onDispose, null)?.Invoke();
    }

    private sealed class NoOp : IDisposable
    {
        public static readonly NoOp Instance = new NoOp();
        private NoOp() { }
        public void Dispose() { /* no-op */ }
    }
}