using NotificationService.Application.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Dtos
{
    public class PaginationRequestDto : RequestDto
    {
        [GreaterThanZero]
        [DefaultValue(1)]
        public int PageNumber { get; set; } = 1;

        [GreaterThanZero]
        [DefaultValue(10)]
        public int PageSize { get; set; } = 10;
    }
}
