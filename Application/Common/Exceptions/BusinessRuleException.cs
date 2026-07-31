using Application.Common.Models;
namespace Application.Common
{
    public class BusinessRuleException : AppException
    {
        public BusinessRuleException(string message, string errorCode = "BUSINESS_RULE_VIOLATION")
            : base(message, errorCode, 422, null) { }
    }
}
