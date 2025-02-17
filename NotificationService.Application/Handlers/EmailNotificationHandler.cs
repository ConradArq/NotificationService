using NotificationService.Application.Dtos.Notification.Email;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Models.Entities.External;
using NotificationService.Domain.Models.Entities;
using NotificationService.Domain.Models;
using AutoMapper;
using NotificationService.Application.Interfaces.Handlers;
using NotificationService.Application.Interfaces.Factories;
using NotificationService.Domain.Interfaces.Repositories;
using System.Text.Json;

namespace NotificationService.Application.Handlers
{
    public class EmailNotificationHandler : INotificationHandler<CreateEmailNotificationDto>
    {
        private readonly INotificationProviderFactory _notificationProviderFactory;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmailNotificationHandler(INotificationProviderFactory notificationProviderFactory, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _notificationProviderFactory = notificationProviderFactory;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Notification> HandleAsync(CreateEmailNotificationDto createEmailNotificationDto)
        {
            var _emailProvider = _notificationProviderFactory.Create(NotificationType.Email, createEmailNotificationDto.RoleId);

            var users = await FetchUsersAsync(createEmailNotificationDto);

            if (users.Count == 0)
            {
                throw new InvalidOperationException("The specified users could not be found to complete this operation.");
            }

            var emailNotification = _mapper.Map<EmailNotification>(createEmailNotificationDto);

            // If subject or body is not provided we get it from email template.
            if (string.IsNullOrEmpty(createEmailNotificationDto.Subject) || string.IsNullOrEmpty(createEmailNotificationDto.Body))
            {
                var emailTemplate = await _unitOfWork.EmailTemplateRepository.GetSingleAsync(createEmailNotificationDto.TemplateId!.Value);

                if (emailTemplate == null)
                {
                    throw new InvalidOperationException("The requested template does not exist.");
                }

                foreach (var user in users)
                {
                    var placeholderReplacements = GetPlaceholderReplacements(createEmailNotificationDto.TemplatePlaceholderMappings, user);

                    var userEmailNotification = new EmailNotification()
                    {
                        Subject = emailNotification.Subject,
                        Body = emailNotification.Body,
                        CC = emailNotification.CC,
                        BCC = emailNotification.BCC,
                        FileAttachments = emailNotification.FileAttachments
                    };

                    userEmailNotification.Subject ??= ReplaceTemplatePlaceholders(emailTemplate.Subject, placeholderReplacements);
                    userEmailNotification.Body ??= ReplaceTemplatePlaceholders(emailTemplate.Body, placeholderReplacements);
                    userEmailNotification.To.Add(user.Email);

                    await _emailProvider.SendNotificationAsync(userEmailNotification);
                }
            }
            else
            {
                foreach (var user in users)
                {
                    var userEmailNotification = new EmailNotification()
                    {
                        Subject = emailNotification.Subject,
                        Body = emailNotification.Body,
                        CC = emailNotification.CC,
                        BCC = emailNotification.BCC,
                        FileAttachments = emailNotification.FileAttachments
                    };

                    userEmailNotification.AddRecipient(user.Email);
                    await _emailProvider.SendNotificationAsync(userEmailNotification);
                }
            }

            emailNotification.Subject ??= "Custom text from the template";
            emailNotification.Body ??= "Custom text from the template";

            return emailNotification;
        }

        // If emails are provided, that takes precedence over RoleId and UserId, and email is sent to corresponding email addresses.
        // If email addresses are found in the system we retrieve user's first and last names and add it to the object.
        private async Task<List<AspNetUser>> FetchUsersAsync(CreateEmailNotificationDto createEmailNotificationDto)
        {
            List<AspNetUser> users = new List<AspNetUser>();

            if (createEmailNotificationDto.To != null && createEmailNotificationDto.To.Any())
            {
                foreach (var email in createEmailNotificationDto.To)
                {
                    var user = await _unitOfWork.ExternalRepository.GetAspNetUserByAsync(null, email);
                    users.Add(new AspNetUser
                    {
                        Email = email,
                        FirstName = user?.FirstName ?? email,
                        LastName = user?.LastName ?? string.Empty
                    });
                }
            }
            else if (createEmailNotificationDto.RoleId != null)
            {
                users = await _unitOfWork.ExternalRepository.GetAspNetUsersByAsync(createEmailNotificationDto.RoleId);
            }
            else if (createEmailNotificationDto.UserId != null)
            {
                var user = await _unitOfWork.ExternalRepository.GetAspNetUserByAsync(createEmailNotificationDto.UserId, null);
                if (user != null)
                {
                    users.Add(user);
                }
            }

            return users;
        }

        private Dictionary<string, string> GetPlaceholderReplacements(string? templatePlaceholderMappings, AspNetUser user)
        {
            var placeholderReplacements = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (!string.IsNullOrEmpty(templatePlaceholderMappings))
            {
                var mappings = JsonSerializer.Deserialize<Dictionary<string, string>>(templatePlaceholderMappings,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (mappings is not null)
                {
                    foreach (var kvp in mappings)
                    {
                        placeholderReplacements[kvp.Key] = kvp.Value;
                    }
                }
            }

            // Add default placeholders **only if they are not already present**
            if (!placeholderReplacements.ContainsKey(nameof(EmailTemplatePlaceholders.Date)))
            {
                placeholderReplacements[nameof(EmailTemplatePlaceholders.Date)] = DateTime.Now.ToString("dd/MM/yyyy");
            }

            if (!placeholderReplacements.ContainsKey(nameof(EmailTemplatePlaceholders.User)))
            {
                placeholderReplacements[nameof(EmailTemplatePlaceholders.User)] = $"{user.FirstName} {user.LastName}";
            }

            return placeholderReplacements;
        }

        private string ReplaceTemplatePlaceholders(string textHtml, Dictionary<string, string> replacements)
        {
            foreach (var replacement in replacements)
            {
                textHtml = textHtml.Replace($"{{{replacement.Key}}}", replacement.Value);
            }

            return textHtml;
        }
    }
}
