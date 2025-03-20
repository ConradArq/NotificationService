using NotificationService.Application.Interfaces.Services;
using NotificationService.Domain.Interfaces.Repositories;

namespace NotificationService.Application.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LocalizationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public string GetLocalizedString(string name)
        {
            throw new NotImplementedException("Localization retrieval is not yet implemented.");
        }

        public Dictionary<string, string> GetAllLocalizedStrings()
        {
            throw new NotImplementedException("Localization retrieval is not yet implemented.");
        }
    }

}
