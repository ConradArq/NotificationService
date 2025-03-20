namespace NotificationService.Application.Interfaces.Services
{
    public interface ILocalizationService
    {
        Dictionary<string, string> GetAllLocalizedStrings();
        string GetLocalizedString(string name);
    }
}