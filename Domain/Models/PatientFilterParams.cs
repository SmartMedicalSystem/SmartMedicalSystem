using Domain.Enums;
using Domain.Models;

namespace Domain.Filters
{
    /// <summary>
    /// معايير فلترة المرضى على مستوى الـ Domain (مسموح للـ Repository يعرفها).
    /// وارثة من PaginationParams عشان تاخد PageNumber/PageSize/CalculateSkip() جاهزين.
    /// </summary>
    public class PatientFilterParams : PaginationParams
    {
        /// <summary>
        /// بيدور في الاسم الأول + اللقب + الـ National ID (SSN) مع بعض.
        /// null أو فاضي = من غير فلترة بحث.
        /// </summary>
        public string? Search { get; set; }

        public Gender? Gender { get; set; }

        public int? MinAge { get; set; }

        public int? MaxAge { get; set; }
    }
}
