using System.Collections.Generic;

namespace Application.Common.Models
{
    public class ErrorResponse
    {
        public bool Success { get; set; } = false;
        public int StatusCode { get; set; }
        public required string Message { get; set; }
        public required string ErrorCode { get; set; }
        public required string TraceId { get; set; }
        public List<ErrorDetail> Errors { get; set; } = new List<ErrorDetail>();
    }
}
