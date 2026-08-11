using Application.DTOs.LabTechnician;
using FluentValidation;

namespace Application.Validators.LabTechnician;

public class LabTechnicianCreateDtoValidator : AbstractValidator<LabTechnicianCreateDto>
{
    public LabTechnicianCreateDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MinimumLength(2).WithMessage("First name must be at least 2 characters long.")
            .MaximumLength(25).WithMessage("First name must not exceed 25 characters.")
            .Matches(ValidationRules.NamePattern).WithMessage("First name can contain only Arabic or English letters and spaces.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MinimumLength(2).WithMessage("Last name must be at least 2 characters long.")
            .MaximumLength(25).WithMessage("Last name must not exceed 25 characters.")
            .Matches(ValidationRules.NamePattern).WithMessage("Last name can contain only Arabic or English letters and spaces.");

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Gender must be a valid value.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.")
            .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past.")
            .Must(ValidationRules.BeAtLeast18YearsOld).WithMessage("The lab technician must be at least 18 years old.");

        RuleFor(x => x.Nationality)
            .NotEmpty().WithMessage("Nationality is required.")
            .MinimumLength(2).WithMessage("Nationality must be at least 2 characters long.")
            .MaximumLength(50).WithMessage("Nationality must not exceed 50 characters.")
            .Matches(ValidationRules.NamePattern).WithMessage("Nationality can contain only Arabic or English letters and spaces.");

        RuleFor(x => x.NationalId)
            .NotEmpty().WithMessage("National ID is required.")
            .Matches(ValidationRules.NationalIdPattern).WithMessage("National ID must contain exactly 14 digits.");

        RuleFor(x => x.LaboratoryId)
            .GreaterThan(0).WithMessage("LaboratoryId must be greater than zero.");

        RuleFor(x => x.JobTitle)
            .NotEmpty().WithMessage("Job title is required.")
            .MinimumLength(2).WithMessage("Job title must be at least 2 characters long.")
            .MaximumLength(100).WithMessage("Job title must not exceed 100 characters.");

        RuleFor(x => x.EmploymentStatus)
            .IsInEnum().WithMessage("Employment status must be a valid value.");

        RuleFor(x => x.WorkShift)
            .IsInEnum().WithMessage("Work shift must be a valid value.");

        RuleFor(x => x.JoiningDate)
            .NotEmpty().WithMessage("Joining date is required.");

        RuleFor(x => x.YearsOfExperience)
            .GreaterThanOrEqualTo(0).WithMessage("Years of experience cannot be negative.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(ValidationRules.EgyptianPhonePattern).WithMessage("Phone number must match the Egyptian mobile format.");

        RuleFor(x => x.AlternativePhone)
            .Must(phone => string.IsNullOrWhiteSpace(phone) || System.Text.RegularExpressions.Regex.IsMatch(phone, ValidationRules.EgyptianPhonePattern))
            .WithMessage("Alternative phone must match the Egyptian mobile format.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email address is not valid.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MinimumLength(5).WithMessage("Address must be at least 5 characters long.")
            .MaximumLength(200).WithMessage("Address must not exceed 200 characters.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.")
            .MinimumLength(2).WithMessage("City must be at least 2 characters long.")
            .MaximumLength(50).WithMessage("City must not exceed 50 characters.")
            .Matches(ValidationRules.NamePattern).WithMessage("City can contain only Arabic or English letters and spaces.");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required.")
            .MinimumLength(2).WithMessage("Country must be at least 2 characters long.")
            .MaximumLength(50).WithMessage("Country must not exceed 50 characters.")
            .Matches(ValidationRules.NamePattern).WithMessage("Country can contain only Arabic or English letters and spaces.");

        RuleFor(x => x.PostalCode)
            .Must(code => string.IsNullOrWhiteSpace(code) || code.Length <= 20)
            .WithMessage("Postal code must not exceed 20 characters.");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
            .MaximumLength(25).WithMessage("Username must not exceed 25 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .MaximumLength(100).WithMessage("Password must not exceed 100 characters.");

        RuleFor(x => x.PhotoUrl)
            .Must(ValidationRules.HasAllowedPhotoExtension).WithMessage("Photo must be a JPG, JPEG, PNG, or WEBP file.")
            .Must(ValidationRules.HasAllowedPhotoSize).WithMessage("Photo size must not exceed 2 MB.");
    }
}
