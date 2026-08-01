using Domain.Common;
using Domain.Enums;
using Domain.Identity;
using System.ComponentModel.DataAnnotations.Schema;
namespace Domain.Entities.Person;

public abstract class BasePerson : BaseEntity
{
    public string EncryptedNationalId { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    public Gender Gender { get; set; }

    public DateTime DateOfBirth { get; set; }

    [NotMapped]
    public int Age
    {
        get
        {
            var today = DateTime.Today;
            var age = today.Year - DateOfBirth.Year;

            if (DateOfBirth.Date > today.AddYears(-age))
                age--;

            return age;
        }
    }

    public string Nationality { get; set; } = string.Empty;

    // Navigation only
    public ApplicationUser? User { get; set; }

    public string PhoneNumber { get; set; } = string.Empty;

    public string? AlternativePhone { get; set; }

    public string? Email { get; set; }

    public string Address { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string? PostalCode { get; set; }

    public string Name
    {
        get => FullName;
        set
        {
            var clean = Guard.NotNullOrWhiteSpace(
                value,
                nameof(Name),
                200);

            var parts = clean.Split(' ', 2);

            FirstName = parts[0];

            LastName =
                parts.Length > 1
                    ? parts[1]
                    : string.Empty;
        }
    }

    public bool AllowLogin { get; set; }

    public bool AccountActive { get; set; }

    public bool ReceiveNotifications { get; set; }
    public ProfileStatus Status { get; set; } = ProfileStatus.Active;
    public string? PhotoUrl { get; set; }

    protected BasePerson() { }

    public BasePerson(
        string firstName,
        string lastName,
        DateTime dateOfBirth)
    {
        FirstName = Guard.NotNullOrWhiteSpace(
            firstName,
            nameof(firstName),
            100);

        LastName = Guard.NotNullOrWhiteSpace(
            lastName,
            nameof(lastName),
            100);

        DateOfBirth =
            Guard.NotInFuture(
                Guard.NotDefault(
                    dateOfBirth,
                    nameof(dateOfBirth)),
                nameof(dateOfBirth));
    }

    public void UpdateProfile(
        string firstName,
        string lastName,
        DateTime dateOfBirth)
    {
        FirstName = Guard.NotNullOrWhiteSpace(
            firstName,
            nameof(firstName),
            100);

        LastName = Guard.NotNullOrWhiteSpace(
            lastName,
            nameof(lastName),
            100);

        DateOfBirth =
            Guard.NotInFuture(
                Guard.NotDefault(
                    dateOfBirth,
                    nameof(dateOfBirth)),
                nameof(dateOfBirth));
    }
}