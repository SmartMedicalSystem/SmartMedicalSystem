using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.RequestLabTests
{
    public class RequestLabTestStatusUpdateDto
    {
        [Required]
        public RequestLabTestStatus Status { get; set; }
    }
}
