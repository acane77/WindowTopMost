namespace MelinaWindowTopmost.Models;

public enum AppBackdrop
{
    None,
    Mica,
    Acrylic,
    Tabbed
}

public static class AppBackdropSupport
{
    public static bool SupportsMicaAndTabbed =>
        OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000);

    public static AppBackdrop DefaultBackdrop =>
        SupportsMicaAndTabbed ? AppBackdrop.Mica : AppBackdrop.Acrylic;

    public static AppBackdrop Normalize(AppBackdrop backdrop)
    {
        if (!SupportsMicaAndTabbed && backdrop is AppBackdrop.Mica or AppBackdrop.Tabbed)
        {
            return AppBackdrop.Acrylic;
        }

        return backdrop;
    }
}
