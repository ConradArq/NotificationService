using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Dtos.SmtpConfig
{
    public class SearchPaginatedSmtpConfigDto : PaginationRequestDto
    {
        public string? Server { get; set; }
        [DefaultValue(587)]
        public int? Port { get; set; }
        public string? FromAddress { get; set; }
        public string? FromName { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }

        [DefaultValue(true)]
        public bool? EnableSsl { get; set; }
    }
}
