namespace Cobalt.SalesAgentSchedule.Services;

/// <summary>
/// Per-circuit (Scoped) light/dark theme state. HeadOutlet only picks up
/// HeadContent rendered from routed page components, not from layout
/// components, so the actual &lt;link&gt; swap lives in a small component
/// included on each page (see ThemeStylesheet.razor) rather than in
/// MainLayout — this service is what lets that stay in sync with the
/// toggle button in the sidebar.
/// </summary>
public class ThemeService
{
    public bool IsDark { get; private set; }

    public event Action? OnChange;

    public void Toggle()
    {
        IsDark = !IsDark;
        OnChange?.Invoke();
    }
}
