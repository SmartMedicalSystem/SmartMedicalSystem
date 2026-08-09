using Application.DTOs.Patient;
using FluentValidation;

namespace Application.Validators.Patient;

public class PatientCreateDtoValidator : AbstractValidator<PatientCreateDto>
{
    public PatientCreateDtoValidator()
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

        RuleFor(x => x.NationalId)
            .NotEmpty().WithMessage("National ID is required.")
            .Matches(ValidationRules.NationalIdPattern).WithMessage("National ID must contain exactly 14 digits.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.")
            .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past.");
            
        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Gender must be a valid value.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email address is not valid.");

        RuleFor(x => x.MobileNumber)
            .NotEmpty().WithMessage("Mobile number is required.")
            .Matches(ValidationRules.EgyptianPhonePattern).WithMessage("Mobile number must match the Egyptian mobile format.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MinimumLength(5).WithMessage("Address must be at least 5 characters long.")
            .MaximumLength(200).WithMessage("Address must not exceed 200 characters.");

        RuleFor(x => x.BloodType)
            .IsInEnum().WithMessage("Blood type must be a valid value.");
    }
}
