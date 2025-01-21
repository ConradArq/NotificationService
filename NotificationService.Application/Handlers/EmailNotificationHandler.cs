using NotificationService.Application.Dtos.Notification.Email;
using NotificationService.Application.Exceptions;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Interfaces.Infrastructure.Providers;
using NotificationService.Domain.Models.Entities.External;
using NotificationService.Domain.Models.Entities;
using NotificationService.Domain.Models;
using AutoMapper;
using NotificationService.Domain.Interfaces.Infrastructure.Persistence.Repositories;
using NotificationService.Application.Interfaces.Handlers;
using NotificationService.Application.Interfaces.Factories;

namespace NotificationService.Application.Handlers
{
    public class EmailNotificationHandler : INotificationHandler<CreateEmailNotificationDto>
    {
        private readonly INotificationProvider _emailProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmailNotificationHandler(INotificationProviderFactory providerFactory, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _emailProvider = providerFactory.Create(typeof(EmailNotification));
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Notification> HandleAsync(CreateEmailNotificationDto createEmailNotificationDto)
        {
            var users = await FetchUsersAsync(createEmailNotificationDto);

            if(users.Count == 0)
            {
                throw new InvalidOperationException("The specified users could not be found to complete this operation.");
            }

            var emailNotification = _mapper.Map<EmailNotification>(createEmailNotificationDto);

            //If subject or body is not provided we get it from email template.
            if (string.IsNullOrEmpty(createEmailNotificationDto.Subject) || string.IsNullOrEmpty(createEmailNotificationDto.Body))
            {
                var emailTemplate = await _unitOfWork.EmailTemplateRepository.GetSingleAsync(createEmailNotificationDto.EmailTemplateId!.Value);

                if (emailTemplate == null)
                {
                    throw new InvalidOperationException("The requested template does not exist.");
                }

                foreach (var user in users)
                {
                    var placeholderReplacements = new Dictionary<EmailTemplatePlaceholders, string>
                        {
                            { EmailTemplatePlaceholders.Date, DateTime.Now.ToString("dd/MM/yyyy") },
                            { EmailTemplatePlaceholders.User, $"{user.FirstName} {user.LastName}" },
                        };

                    var userEmailNotification = new EmailNotification()
                    {
                        Subject = emailNotification.Subject,
                        Body = emailNotification.Body,
                        CC = emailNotification.CC,
                        BCC = emailNotification.BCC,
                        FileAttachments = emailNotification.FileAttachments
                    };

                    userEmailNotification.Subject ??= ReplaceEmailTemplatePlaceholders(emailTemplate.Subject, placeholderReplacements);
                    userEmailNotification.Body ??= ReplaceEmailTemplatePlaceholders(emailTemplate.Body, placeholderReplacements);
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

        //If emails are provided, that takes precedence over RoleId and UserId, and email is sent to corresponding email addresses.
        //If email addresses are found in the system we retrieve user's first and last names and add it to the object.
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

        private string ReplaceEmailTemplatePlaceholders(string textHtml, Dictionary<EmailTemplatePlaceholders, string> replacements)
        {
            foreach (var placeholder in Enum.GetValues<EmailTemplatePlaceholders>())
            {
                var placeholderText = $"{{{placeholder}}}";

                if (replacements.TryGetValue(placeholder, out string? replacementValue))
                {
                    textHtml = textHtml.Replace(placeholderText, replacementValue);
                }
            }

            return textHtml;
        }
    }
}
