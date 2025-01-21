using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Dtos.EmailTemplate
{
    public class SearchPaginatedEmailTemplateDto : PaginationRequestDto
    {
        public string? Name { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }

        [DefaultValue((int)Domain.Enums.Status.Active)]
        public int? StatusId { get; set; }
    }
}
