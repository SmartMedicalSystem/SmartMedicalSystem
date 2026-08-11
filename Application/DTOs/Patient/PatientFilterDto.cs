using Domain.Filters;

namespace Application.DTOs.Patients
{
    /// <summary>
    /// الـ DTO اللي الكونترولر بيستقبله من الـ query string. وارث من
    /// PatientFilterParams عشان يتبعت زي ما هو مباشرة للـ Repository من غير
    /// أي mapping يدوي (Polymorphism بدل AutoMapper هنا).
    /// </summary>
    public class PatientFilterDto : PatientFilterParams
    {
        // لو محتاج تضيف حقول خاصة بالـ API بس (مش موجودة في الـ Domain filter)
        // زي HasPendingReports مستقبلاً، تضاف هنا.
    }
}
