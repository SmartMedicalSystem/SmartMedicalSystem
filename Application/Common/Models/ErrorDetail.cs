using System;

namespace Application.Common.Models
{
    public class ErrorDetail
    {
        public string? Field { get; set; }
        public required string Message { get; set; }
    }
}
