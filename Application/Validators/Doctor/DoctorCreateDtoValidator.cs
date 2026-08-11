using Application.DTOs.Doctor;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Application.Validators.Doctor;

public class DoctorCreateDtoValidator : AbstractValidator<DoctorCreateDto>
{
    public DoctorCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters long.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.")
            .Matches(ValidationRules.NamePattern).WithMessage("Name can contain only Arabic or English letters and spaces.");

        RuleFor(x => x.Specialization)
            .NotEmpty().WithMessage("Specialization is required.")
            .MinimumLength(2).WithMessage("Specialization must be at least 2 characters long.")
            .MaximumLength(100).WithMessage("Specialization must not exceed 100 characters.")
            .Matches(ValidationRules.NamePattern).WithMessage("Specialization can contain only Arabic or English letters and spaces.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.")
            .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past.")
            .Must(ValidationRules.BeAtLeast18YearsOld).WithMessage("The doctor must be at least 18 years old.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email address is not valid.");

        RuleFor(x => x.MobileNumber)
            .NotEmpty().WithMessage("Mobile number is required.")
            .Matches(ValidationRules.EgyptianPhonePattern).WithMessage("Mobile number must match the Egyptian mobile format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .MaximumLength(100).WithMessage("Password must not exceed 100 characters.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MinimumLength(5).WithMessage("Address must be at least 5 characters long.")
            .MaximumLength(200).WithMessage("Address must not exceed 200 characters.");

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Gender must be a valid value.");

        RuleFor(x => x.NationalId)
            .NotEmpty().WithMessage("National ID is required.")
            .Matches(ValidationRules.NationalIdPattern).WithMessage("National ID must contain exactly 14 digits.");

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("DepartmentId must be greater than zero.");

        RuleFor(x => x.PhotoUrl)
            .Must(ValidationRules.HasAllowedPhotoExtension).WithMessage("Photo must be a JPG, JPEG, PNG, or WEBP file.")
            .Must(ValidationRules.HasAllowedPhotoSize).WithMessage("Photo size must not exceed 2 MB.");
    }
}
