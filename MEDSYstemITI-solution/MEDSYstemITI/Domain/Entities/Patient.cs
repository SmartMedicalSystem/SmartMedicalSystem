using System;
using System.Collections.Generic;
using Domain.Common;
using Domain.Enums;
using Domain.Entities.Baseperson;
namespace Domain.Entities
{
    public class Patient : BasePerson
    {
        // Now exposed with public getters (readable by mapping/services) while
        // keeping setters private so the only way to change them is through the
        // validated constructor / UpdateProfile method below.
        public string FirstName { get;  set; } = string.Empty;
        public string LastName { get;  set; } = string.Empty;
        public int Age { get; private set; }

        public DateTime DateOfBirth { get;  set; }

        public Gender Gender { get;  set; }

        public int MobileNumber { get;  set; }

        public string Address { get;  set; } = string.Empty;

        public BloodType BloodType { get;  set; }

        public ICollection<Session> Sessions { get; set; } = new List<Session>();

        private Patient() { }

        public Patient(
       string firstName,
       string lastName,
       int nationalId,
       DateTime dateOfBirth,
       Gender gender,
       int mobileNumber,
       string address,
       BloodType bloodType)
        {
            FirstName = Guard.NotNullOrWhiteSpace(firstName, nameof(firstName), 100);
            LastName = Guard.NotNullOrWhiteSpace(lastName, nameof(lastName), 100);
            EncryptedSSN = nationalId.ToString(); // Assuming nationalId is the SSN
            DateOfBirth = Guard.NotInFuture(
                Guard.NotDefault(dateOfBirth, nameof(dateOfBirth)),
                nameof(dateOfBirth));

            Gender = gender;

            MobileNumber = mobileNumber;

            Address = Guard.NotNullOrWhiteSpace(address, nameof(address), 250);

            BloodType = bloodType;

            Age = CalculateAge(DateOfBirth);
        }

        public void UpdateProfile(
            string firstName,
            string lastName,
            int nationalId,
            DateTime dateOfBirth,
            Gender gender,
            int mobileNumber,
            string address,
            BloodType bloodType)
        {
            FirstName = Guard.NotNullOrWhiteSpace(firstName, nameof(firstName), 100);
            LastName = Guard.NotNullOrWhiteSpace(lastName, nameof(lastName), 100);
            EncryptedSSN = nationalId.ToString(); // Assuming nationalId is the SSN

            DateOfBirth = Guard.NotInFuture(
                Guard.NotDefault(dateOfBirth, nameof(dateOfBirth)),
                nameof(dateOfBirth));

            Gender = gender;

            MobileNumber = mobileNumber;

            Address = Guard.NotNullOrWhiteSpace(address, nameof(address), 250);

            BloodType = bloodType;

            Age = CalculateAge(DateOfBirth);
        }

        private static int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.UtcNow.Date;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
