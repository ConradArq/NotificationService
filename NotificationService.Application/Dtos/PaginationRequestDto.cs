using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Dtos
{
    public class PaginationRequestDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "The page number must be greater than 0.")]
        [DefaultValue(1)]
        public int PageNumber { get; set; } = 1;
        [Range(1, int.MaxValue, ErrorMessage = "The page number must be greater than 0.")]
        [DefaultValue(10)]
        public int PageSize { get; set; } = 10;
    }
}
