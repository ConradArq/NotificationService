using NotificationService.Application.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Dtos.EmailTemplate
{
    public class UpdateEmailTemplateDto
    {
        [LocalizedStringLength(100)]
        public string? Name { get; set; }

        [LocalizedStringLength(100)]
        public string? Subject { get; set; }

        [LocalizedStringLength(10000)]
        public string? Body { get; set; }

        [EnumValueValidation(typeof(Domain.Enums.Status))]
        [DefaultValue((int)Domain.Enums.Status.Active)]
        public int? StatusId { get; set; }
    }
}