using Microsoft.AspNetCore.Http;

namespace Application.Validators;

internal static class ValidationRules
{
    internal const string NamePattern = @"^[a-zA-Z\u0600-\u06FF]+(?:\s+[a-zA-Z\u0600-\u06FF]+)*$";
    internal const string EgyptianPhonePattern = @"^01[0125][0-9]{8}$";
    internal const string NationalIdPattern = @"^[0-9]{14}$";
    internal static readonly string[] AllowedPhotoExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    internal const long MaxPhotoSizeBytes = 2 * 1024 * 1024;

    internal static bool BeAtLeast18YearsOld(DateTime dateOfBirth)
    {
        var today = DateTime.UtcNow.Date;
        var age = today.Year - dateOfBirth.Date.Year;

        if (dateOfBirth.Date > today.AddYears(-age))
        {
            age--;
        }

        return age >= 18;
    }

    internal static bool HasAllowedPhotoExtension(IFormFile? file)
    {
        if (file is null)
        {
            return true;
        }

        var extension = Path.GetExtension(file.FileName);
        return AllowedPhotoExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }

    internal static bool HasAllowedPhotoSize(IFormFile? file)
    {
        return file is null || file.Length <= MaxPhotoSizeBytes;
    }
}
