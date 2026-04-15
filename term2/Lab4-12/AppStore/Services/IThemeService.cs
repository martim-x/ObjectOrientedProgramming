namespace Project.Services;

public interface IThemeService
{
    void SetTheme(bool isDark);
    bool IsDark { get; }
}
