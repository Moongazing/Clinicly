namespace Moongazing.Kernel.Localization.Abstractions;

public interface ILocalizationService
{
    ICollection<string>? AcceptLocales { get; set; }

    Task<string> GetLocalizedAsync(string key, string? keySection = null);

    Task<string> GetLocalizedAsync(string key, ICollection<string> acceptLocales, string? keySection = null);
}
