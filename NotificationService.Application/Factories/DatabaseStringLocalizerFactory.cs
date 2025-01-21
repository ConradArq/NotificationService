using Microsoft.Extensions.Localization;
using NotificationService.Application.Interfaces.Services;

namespace NotificationService.Application.Factories
{
    public class DatabaseStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly ILocalizationService _localizationService;

        public DatabaseStringLocalizerFactory(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return new DatabaseStringLocalizer(_localizationService);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new DatabaseStringLocalizer(_localizationService);
        }
    }

    public class DatabaseStringLocalizer : IStringLocalizer
    {
        private readonly ILocalizationService _localizationService;

        public DatabaseStringLocalizer(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        public LocalizedString this[string name]
        {
            get
            {
                string value = _localizationService.GetLocalizedString(name);
                return new LocalizedString(name, value ?? name, value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                string value = _localizationService.GetLocalizedString(name);
                return new LocalizedString(name, string.Format(value ?? name, arguments), value == null);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return _localizationService.GetAllLocalizedStrings()
                .Select(kv => new LocalizedString(kv.Key, kv.Value, false));
        }
    }
}
